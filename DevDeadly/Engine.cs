using DevDeadly.Shaders;
using ImGuiNET;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace DevDeadly
{
    //AABB Collision next hope...   
    public class BoundingBox
    {
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public bool Intersects(BoundingBox other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }
    public class Game : GameWindow
    {
        //Vertex
        string vertexShaderSource = @"

            #version 330 core

            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 3) in vec3 aNormal;   

            out vec2 texCoord;
            out vec3 FragPos;
            out vec3 Normal;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                FragPos = vec3(model * vec4(aPosition, 1.0));
                Normal = mat3(transpose(inverse(model))) * aNormal;  
                gl_Position =  vec4(aPosition, 1.0) * model * view * projection;        
                texCoord = aTexCoord;
            }";

        //Fragment
        string fragmentShaderSource = @"

            #version 330 core

            in vec2 texCoord;
            in vec3 FragPos;
            in vec3 Normal;

            out vec4 FragColor;

            uniform sampler2D tex;
            uniform vec3 lightPos;
            uniform vec3 viewPos;
            uniform vec3 lightColor;
            uniform vec3 objectColor;

            uniform sampler2D texture0;
            uniform sampler2D texture1;

            void main()
            {
                vec3 texColor = texture(tex, texCoord).rgb;

                // Ambient
                float ambientStrength = 0.1f;
                vec3 ambient = ambientStrength * lightColor;

                // Diffuse
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(lightPos - FragPos);
                float diff = max(dot(norm, lightDir), 0.0f);
                vec3 diffuse = diff * lightColor;

                // Specular
                float specularStrength = 0.50f;
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 256);
                vec3 specular = specularStrength * spec * lightColor;

                // Final color
                vec3 result = (ambient + diffuse + specular);
                FragColor = vec4(result, 1.0f);

                vec4 color1 = texture(texture0, texCoord);
                vec4 color2 = texture(texture1, texCoord);
                FragColor = mix(color1, color2, 0.4);
            }";

        string LampVert = @"

            #version 330 core
            layout(location = 0) in vec3 aPos;
            layout(location = 1) in vec3 aNormal;

            out vec3 FragPos;
            out vec3 Normal;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                FragPos = vec3(model * vec4(aPos, 1.0));
                Normal = mat3(transpose(inverse(model))) * aNormal;  
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
            }";

        string LampFrags = @"

            #version 330 core

            in vec3 FragPos;
            in vec3 Normal;

            out vec4 FragColor;

            uniform vec3 lightPos;
            uniform vec3 viewPos;
            uniform vec3 lightColor;
            uniform vec3 objectColor;

            void main()
            {
                // Ambient
                float ambientStrength = 0.1f;
                vec3 ambient = ambientStrength * lightColor;

                // Diffuse
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(lightPos - FragPos);
                float diff = max(dot(norm, lightDir), 0.0f);
                vec3 diffuse = diff * lightColor;

                // Specular
                float specularStrength = 0.50f;
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0f), 256);
                vec3 specular = specularStrength * spec * lightColor;

                // Final color
                vec3 result = (ambient + diffuse + specular) * objectColor;
                FragColor = vec4(result, 1.0f);
            }";

        string CloudVerts = @"

        #version 330 core
        layout(location = 0) in vec3 Cloud;
        layout(location = 1) in vec2 aTexCoordCloud;

        uniform mat4 modelcloud;
        uniform mat4 viewcloud;
        uniform mat4 projectioncloud;

        out vec2 TexCoord;

        void main()
        {
            gl_Position = vec4(Cloud, 1.0) * modelcloud * viewcloud * projectioncloud;
            TexCoord = aTexCoordCloud;
        }";

        string CloudFrags = @"
        
        #version 330 core

        in vec2 TexCoord;
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(0.95, 0.95, 1.0, 0.4);       
        }";

        string InventoryVerts = @"
        
        #version 330 core

        layout (location = 0) in vec2 IUPosition;
        layout (location = 1) in vec2 IUCoord;

        out vec2 TexCoord;

        uniform mat4 projection; 

        void main()
        {
            gl_Position = projection * vec4(IUPosition.xy, 0.0, 1.0);
            TexCoord = IUCoord;
        }";

        string InventoryFrags = @"
        
        #version 330 core

        in vec2 TexCoord;
        out vec4 FragColor;

        uniform sampler2D textureHUD;

        void main()
        {            
            FragColor = texture(textureHUD, TexCoord);
            //FragColor = vec4(1.0, 1.0, 0.0, 1.0); // Amarillo RGBA
        }";

        //Build
        float[] lampVertices = {

            // positions         // normals
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,

             0.5f,  0.5f,  0.5f, 1.0f,  0.0f, 0.0f,
             0.5f,  0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f, 1.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f, 1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
        };

        float[] CloudsVertices =
        {
            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,   0.0f, 1.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 1.0f,

            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 1.0f,
             0.5f, -0.5f, 0.0f,   1.0f, 0.0f
        };

        uint[] cloudsindices = {
            0, 1, 2,
            2, 3, 0
        };



        float[] verticesHUD =

        {
            0f, 0f,     0f, 1f,
            100f, 0f,   1f, 1f,
            100f, 100f, 1f, 0f,
            0f, 100f,   0f, 0f
        };


        uint[] indicesHUD =
        {
                0, 1, 2,
                2, 3, 0
            };


        List<Vector3> cloudPositions = new List<Vector3>()
        {
            new Vector3(0.20f, 20.0f, 0.1f),
            new Vector3(10.0f, 22.0f, 5.0f),
            new Vector3(-5.0f, 25.0f, -3.0f),
            new Vector3(15.0f, 21.0f, -10.0f),
            new Vector3(-12.0f, 23.0f, 8.0f),
            new Vector3(8.0f, 24.0f, 2.0f),
            new Vector3(3.0f, 26.0f, 12.0f),
            new Vector3(-7.0f, 22.5f, -6.0f),
            new Vector3(18.0f, 25.0f, -4.0f),
            new Vector3(-15.0f, 21.0f, 9.0f),

            new Vector3(20.0f, 27.0f, 0.0f),
            new Vector3(25.0f, 23.0f, -5.0f),
            new Vector3(30.0f, 24.0f, 6.0f),
            new Vector3(-18.0f, 22.0f, 11.0f),
            new Vector3(5.0f, 21.5f, -8.0f),
            new Vector3(13.0f, 26.5f, 4.0f),
            new Vector3(-20.0f, 25.0f, -9.0f),
            new Vector3(22.0f, 23.5f, 7.0f),
            new Vector3(-10.0f, 20.5f, -12.0f),
            new Vector3(17.0f, 24.0f, -2.0f),

            new Vector3(-25.0f, 21.0f, 10.0f),
            new Vector3(12.0f, 25.5f, 14.0f),
            new Vector3(-3.0f, 26.0f, -6.0f),
            new Vector3(6.0f, 23.0f, 16.0f),
            new Vector3(9.0f, 22.0f, -15.0f),
            new Vector3(-14.0f, 27.0f, 3.0f),
            new Vector3(28.0f, 25.5f, -1.0f),
            new Vector3(31.0f, 22.5f, 5.0f),
            new Vector3(-22.0f, 24.0f, -4.0f),
            new Vector3(4.0f, 21.0f, 13.0f),

            new Vector3(-6.0f, 23.0f, -10.0f),
            new Vector3(0.0f, 24.0f, 0.0f),
            new Vector3(11.0f, 26.0f, 9.0f),
            new Vector3(-8.0f, 22.5f, -14.0f),
            new Vector3(7.0f, 25.0f, 18.0f),
            new Vector3(-17.0f, 24.5f, -7.0f),
            new Vector3(19.0f, 23.0f, 10.0f),
            new Vector3(-30.0f, 21.5f, -3.0f),
            new Vector3(2.0f, 26.0f, 15.0f),
            new Vector3(16.0f, 27.0f, -5.0f),

            new Vector3(-9.0f, 20.0f, 6.0f),
            new Vector3(14.0f, 22.0f, -11.0f),
            new Vector3(-13.0f, 23.5f, 1.0f),
            new Vector3(23.0f, 25.0f, -2.0f),
            new Vector3(-4.0f, 26.0f, 8.0f),
            new Vector3(21.0f, 24.5f, -13.0f),
            new Vector3(-11.0f, 22.0f, 12.0f),
            new Vector3(27.0f, 23.0f, -9.0f),
            new Vector3(-19.0f, 21.0f, 4.0f),
            new Vector3(1.0f, 25.0f, -6.0f),
            new Vector3(-35.0f, 26.0f, 12.0f),
            new Vector3(38.0f, 24.0f, -15.0f),
            new Vector3(-28.0f, 22.5f, 7.0f),
            new Vector3(33.0f, 27.5f, -10.0f),
            new Vector3(-40.0f, 25.0f, 18.0f),
            new Vector3(29.0f, 23.5f, -5.0f),
            new Vector3(-17.0f, 21.0f, 15.0f),
            new Vector3(42.0f, 26.0f, -8.0f),
            new Vector3(-22.0f, 24.0f, 10.0f),
            new Vector3(36.0f, 22.0f, 3.0f),
            new Vector3(-14.0f, 27.0f, -17.0f),
            new Vector3(11.0f, 23.0f, 19.0f),
            new Vector3(-6.0f, 25.0f, -22.0f),
            new Vector3(18.0f, 21.5f, 16.0f),
            new Vector3(-26.0f, 24.5f, -14.0f),
            new Vector3(24.0f, 22.0f, 11.0f),
            new Vector3(-32.0f, 23.0f, 6.0f),
            new Vector3(15.0f, 26.5f, -19.0f),
            new Vector3(-10.0f, 25.5f, 13.0f),
            new Vector3(31.0f, 24.0f, -3.0f)
        };


        //GUI
        ImGuiController _controller;
        private bool _showGui = true;

        // VAO,EBO,VBO (TEXTURE SET)
        private int VertexArrayObject;
        private int ElementBufferObject;
        private int VertexBufferObject;

        //VAO,EBO,VBO (LAMP SET)
        private int VAOLamp;
        private int EBOLamp;
        private int VBOLamp;
        private int VAOModel;

        //VAO,EBO,VBO (CLOUD SET)
        private int VAOCloud;

       


        private int VAOInventory;
        private int EBOInventory;

        public int nrAttribute;
        public int width, height;
        public bool OptionCursorState;

        //SHADER SET
        private Stopwatch timer = Stopwatch.StartNew();
        private int shaderProgram;
        private int state;

        Shader lightingShader;
        ShaderLamp lampShader;
        CloudShader cloudShader;
        Inventory inventory;

        private readonly Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        //TEXTURE SET
        public Matrix4 projection;
        //public Texture texture;
        //public Texture texture2;
        public TextureHUD texturehud;
        public Matrix4 model;
        public Matrix4 view;
        public static int TextureID;


        Camera camera;
        Chunk chunk;

        //ROT Y
        float yRot;

        //ROT X
        float zRot;

        /*WIN CONFIG
        ---------------------------------------------------------------*/
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (X: width, Y: height),
            Title = title
        })

        {
            this.width = width; this.height = height;
        }


        /* COLOR CONFIG
        ---------------------------------------------------------------*/
        public struct Color4

        {
            public float R, G, B, A;
            public Color4(float r, float g, float b, float a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }
            public static Color4 operator *(Color4 color1, Color4 color2)
            {
                return new Color4(

                    color1.R * color2.R,
                    color1.G * color2.G,
                    color1.B * color2.B,
                    color1.A * color2.A
                );
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            var IsCursorGrabbed = false;

            MouseState mouse = MouseState;
            camera.Update(input, mouse, e);

            //Keybinds to detect if this is actually is being pressed...
            if (KeyboardState.IsKeyDown(Keys.W)) ;
            if (KeyboardState.IsKeyDown(Keys.A)) ;
            if (KeyboardState.IsKeyDown(Keys.S)) ;
            if (KeyboardState.IsKeyDown(Keys.D)) ;
            if (input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.G))
            {
                _showGui = !_showGui;
            }

            // !!TODO: Only it's being able to hide the cursor but not re activate it idk
            if (input.IsKeyDown(Keys.V))

            {
                IsCursorGrabbed = !IsCursorGrabbed;
                CursorState = IsCursorGrabbed ? CursorState.Grabbed : CursorState.Normal;
                Console.WriteLine($"IsCursorGrabbed: {IsCursorGrabbed}, CursorState: {CursorState}");
            }

            if (KeyboardState.IsKeyDown(Keys.F))

            {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            //Audio Inicialization
            AudioPlayer player = new AudioPlayer("Key.wav");
            player.Play();
            Console.WriteLine("Reproduciendo sonido..." + player);
            Thread.Sleep(3000);

            //Chunk inicializated
            chunk = new Chunk(new Vector3(0, 0, 0));
            Title += ": OpenTk Version:" + GL.GetString(StringName.Version);

            //Controller inicializated
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            camera = new Camera(100f, 100f, new Vector3(0, 0, 0)); //This is probably the stuff I have to fix.

            timer = Stopwatch.StartNew();
            camera.SetObstacles(chunk.SolidBlockAABBs);

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            ElementBufferObject = GL.GenBuffer();

            VAOModel = GL.GenVertexArray();
            VAOLamp = GL.GenVertexArray();
            EBOLamp = GL.GenBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            //Reminder to apply this as a way to optimizate that shit loading specific faces.
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back); 
            CursorState = CursorState.Grabbed;

            //String Path, Textures configuration and Lighting settings.
            lightingShader = new Shader(vertexShaderSource, fragmentShaderSource);
            lampShader = new ShaderLamp(LampVert, LampFrags);
            cloudShader = new CloudShader(CloudVerts, CloudFrags);
            inventory = new Inventory(InventoryVerts, InventoryFrags);

            string imagePath = "Slots.png";
            texturehud = new TextureHUD(imagePath);
            texturehud.Use(TextureUnit.Texture10);

            //LAMP
            VAOLamp = GL.GenVertexArray();
            int VBOLamp = GL.GenBuffer();

            GL.BindVertexArray(VAOLamp);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOLamp);
            GL.BufferData(BufferTarget.ArrayBuffer, lampVertices.Length * sizeof(float), lampVertices, BufferUsageHint.StaticDraw);

            //Light
            int lampPosLocation = lampShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(lampPosLocation);
            GL.VertexAttribPointer(lampPosLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int posAttrib = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            int Anormal = lampShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(Anormal);
            GL.VertexAttribPointer(Anormal, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            ////CLOUD
            //VAOCloud = GL.GenVertexArray();
            //int VBOCloud = GL.GenBuffer();

            //GL.BindVertexArray(VAOCloud);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCloud);
            //GL.BufferData(BufferTarget.ArrayBuffer, CloudsVertices.Length * sizeof(float), CloudsVertices, BufferUsageHint.StaticDraw);

            VAOInventory = GL.GenVertexArray();
            int VBOInventory = GL.GenBuffer();
            int EBOInventory = GL.GenBuffer();

            GL.BindVertexArray(VAOInventory);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOInventory);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesHUD.Length * sizeof(float), verticesHUD, BufferUsageHint.StaticDraw);

            //GL.BindVertexArray(EBOInventory);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, EBOInventory);
            GL.BufferData(BufferTarget.ArrayBuffer, indicesHUD.Length * sizeof(float), indicesHUD, BufferUsageHint.StaticDraw);
            GL.BindVertexArray(0);

            /*DRAWING 
            ----------------------------------------------------------------------------------------------------------------------------------------------*/
            ////in case i want to change it to the lamp shit, i have to replace it as anormal and don't forget to change the vert/frag and add the lightingsource

            //CLOUD
            VAOCloud = GL.GenVertexArray();
            int VBOCloud = GL.GenBuffer();

            GL.BindVertexArray(VAOCloud);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCloud);
            GL.BufferData(BufferTarget.ArrayBuffer, CloudsVertices.Length * sizeof(float), CloudsVertices, BufferUsageHint.StaticDraw);

            //----------------------------------------------------------------------------------------------------------------------------------------------*/
            int posCloud = cloudShader.GetAttribLocation("Cloud");
            GL.VertexAttribPointer(posCloud, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posCloud);

            int posCoord = cloudShader.GetAttribLocation("aTexCoordCloud");
            GL.VertexAttribPointer(posCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(posCoord);
            GL.BindVertexArray(0);

            VAOInventory = GL.GenVertexArray();
            //int VBOInventory = GL.GenBuffer();
            EBOInventory = GL.GenBuffer();             

            GL.BindVertexArray(VAOInventory);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOInventory);

            //float hudWidth = 256f;
            //float hudHeight = 64f;
            //float hudX = (ClientSize.X / 2f) - (hudWidth / 2f);
            //float hudY = ClientSize.Y - hudHeight - 20f;

            //float[] verticesHUD = new float[]
            //{
            //    hudX, hudY,                     0f, 1f, // inferior izquierdo
            //    hudX + hudWidth, hudY,         1f, 1f, // inferior derecho
            //    hudX + hudWidth, hudY + hudHeight, 1f, 0f, // superior derecho
            //    hudX, hudY + hudHeight,        0f, 0f  // superior izquierdo
            //};

            GL.BufferData(BufferTarget.ArrayBuffer, verticesHUD.Length * sizeof(float), verticesHUD, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOInventory);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicesHUD.Length * sizeof(uint), indicesHUD, BufferUsageHint.StaticDraw);

            int posIU = inventory.GetAttribLocation("IUPosition");
            GL.VertexAttribPointer(posIU, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posIU);

            int posIUCoord = inventory.GetAttribLocation("IUCoord");
            GL.VertexAttribPointer(posIUCoord, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(posIUCoord);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //Wraps lines boxes
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Background Color
            GL.ClearColor(0.53f, 0.81f, 0.92f, 1.0f);
            GL.GetInteger(GetPName.MaxVertexAttribs, out nrAttribute);
            Console.WriteLine($"Amount of cores using right now: {nrAttribute}");

            lightingShader.Use();
            lampShader.Use();
            timer.Start();
        }
        protected override void OnUnload()

        {
            base.OnUnload();
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteVertexArray(VAOLamp);
            lightingShader.Dispose();
            cloudShader.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _controller.Update(this, (float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);

            //Matrix4 modelCloud = Matrix4.CreateScale(10f, 10f, 10f) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f)) *  Matrix4.CreateTranslation(30f, 40.0f, 30f);            
            //Matrix4 modelCloud = Matrix4.Identity;
            Matrix4 viewCloud = camera.GetViewMatrix();
            Matrix4 projectionCloud = camera.GetProjectionMatrix();
            Vector3 elevation = new Vector3(0f, 15f, 0f);

            foreach (var pos in cloudPositions)
            {
                float separationFactor = 2.5f;

                Vector3 separatedPos = new Vector3(pos.X * separationFactor, pos.Y, pos.Z * separationFactor);
                Vector3 elevatedPos = separatedPos + elevation;

                Matrix4 modelCloud = Matrix4.CreateScale(20f, 20f, 20f) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f)) * Matrix4.CreateTranslation(elevatedPos);

                cloudShader.Use();
                cloudShader.SetMatrix4("modelcloud", modelCloud);
                cloudShader.SetMatrix4("viewcloud", camera.GetViewMatrix());
                cloudShader.SetMatrix4("projectioncloud", camera.GetProjectionMatrix());

                GL.BindVertexArray(VAOCloud);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

            //cloudShader.Use();
            //cloudShader.SetMatrix4("modelcloud", modelCloud);
            //cloudShader.SetMatrix4("viewcloud", viewCloud);
            //cloudShader.SetMatrix4("projectioncloud", projectionCloud);

            //Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, ClientSize.Y, 0, -1.0f, 1.0f);

            string imagePath = "Slots.png";

            GL.Disable(EnableCap.DepthTest);

            //Set for IU interface
            inventory.Use();
            inventory.SetInt("textureHUD", 10);
            texturehud.Use(TextureUnit.Texture0);
            //GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.BindVertexArray(VAOInventory);

            GL.Enable(EnableCap.DepthTest);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            //GL.Enable(EnableCap.DepthTest);

            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, ClientSize.Y, 0, -1.0f, 1.0f);
            inventory.SetMatrix4("projection", ortho);

            //Cube Model and rotation
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            //lampModel = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.00f;

            //model = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.001f;

            Matrix4 lampMatrix = Matrix4.Identity;

            //Render cube with lighting
            lampShader.Use();
            lampShader.SetMatrix4("model", lampMatrix);
            lampShader.SetMatrix4("view", camera.GetViewMatrix());
            lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            lampShader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
            lampShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            lampShader.SetVector3("viewPos", camera.position);
            lampShader.SetVector3("lightPos", lightPos);

            GL.BindVertexArray(VAOLamp);
            GL.UseProgram(lampShader.Handle2);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            // Render chunk with texture (no lighting)
            lightingShader.Use();
            lightingShader.SetMatrix4("model", model);
            lightingShader.SetMatrix4("view", view);
            lightingShader.SetMatrix4("projection", projection);
            
                                                                  //L/F     /U/D    L/F
            //lampMatrix = Matrix4.CreateTranslation(new Vector3(-30.0f, -6.0f, -30.0f));
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

            int modelLocation = GL.GetUniformLocation(lightingShader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(lightingShader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(lightingShader.Handle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            int modelLampLocation = GL.GetUniformLocation(lampShader.Handle2, "model");
            int viewLampLocation = GL.GetUniformLocation(lampShader.Handle2, "view");
            int projectionLampLocation = GL.GetUniformLocation(lampShader.Handle2, "projection");

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);

            int modelCloudLocation = GL.GetUniformLocation(cloudShader.Handle3, "modelcloud");
            int viewCloudLocation = GL.GetUniformLocation(cloudShader.Handle3, "viewcloud");
            int projectionCloudLocation = GL.GetUniformLocation(cloudShader.Handle3, "projectioncloud");

            //GL.UniformMatrix4(modelCloudLocation, true, ref modelCloud);
            //GL.UniformMatrix4(viewCloudLocation, true, ref viewCloud);
            //GL.UniformMatrix4(projectionCloudLocation, true, ref projectionCloud);
            chunk.Render(lightingShader); //Render always before modelLampLocation the mvp set.

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);


            // Enable Docking
            if (_showGui)
            {
                //DockSpace made my background dark taking the whole rez of the screen.
                //ImGui.DockSpaceOverViewport();
                ImGui.Begin("Debug Info");
                ImGui.Text($"TextureID: {TextureID}");
                ImGui.Text("HUD debería estar activo");
                ImGui.End();
                ImGui.ShowDemoWindow();
                ImGuiController.CheckGLError("End of frame");
            }
            _controller.Render();
            Context.SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _controller.MouseScroll(e.Offset);
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _controller?.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            width = e.Width;
            height = e.Height;

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);

        }
    }
}
