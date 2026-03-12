using DevDeadly.Shaders;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace DevDeadly
{
    
    public class Game : GameWindow
    {

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

        uint[] indicesCreate = 
        
        {
            0, 1, 2,
            2, 3, 0
        };

        uint[] indicesHUD =

        {
            0, 1, 2,
            2, 3, 0
        };

      
        struct VertexChunk
        {
            public Vector3 Position;   // layout(location=0)
            public Vector3 Normal;     // layout(location=1)
            public float TexLayer;     // layout(location=2)
            public Vector2 TexCoord;   // layout(location=3)
        }


        VertexChunk[] vertices = new VertexChunk[]
           
            {
                new VertexChunk {
                    Position = new Vector3(-0.5f, -0.5f, -0.5f),
                    TexCoord = new Vector2(0.0f, 0.0f),
                    TexLayer = 0.0f,
                    Normal   = new Vector3(0.0f, 0.0f, -1.0f)
                },
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
        };

        float[] backgroundVertices = 
        
        {
            // posX, posY
            -0.8f, -0.6f, // Bottom left
             0.8f, -0.6f, // Bottom right
             0.8f,  0.6f, // Top right
            -0.8f,  0.6f  // Top left
        };

        uint[] backgroundIndices = {
            0, 1, 2,
            2, 3, 0
        };


        //GUI
        ImGuiController _controller;
        private bool _showGui = true;
        private bool _hideInventory = true;
        private bool _hideCreate = true;

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

        //VAO, EBO (INVENTORY SET)
        private int VAOInventory;
        private int EBOInventory;

        //VAO, EBO (TRANSPARENCY)
        private int VAOTransparency;
        private int VBOTransparency;
        private int EBOTransparency;

        //VAO, EBO (CREATIVE HUD SET);
        private int VAOCreate;
        private int EBOCreate;

        //VAO, EBO, VBO (MAIN SET);
        private int VAOMain;
        public int nrAttribute;
        public int width, height;
        public bool OptionCursorState;

        public int VAOItem;
        public int VBOItem;
        public int EBOItem;

        //SHADER SET
        private Stopwatch timer = Stopwatch.StartNew();
        public AudioPlayer Pop;

        Shader lightingShader;
        Shader lampShader;
        Shader cloudShader;
        Shader inventory;
        Shader create;
        Shader rency;
        Shader itemObject;

        private World world;
        private Model modelItem;

        private readonly Vector3 lightPos = new Vector3(2.0f, 4.0f, 2.0f);
        private readonly Vector3 Normal = new Vector3(0.0f, 0.0f, -1.0f);


        //TEXTURE SET
        public Matrix4 projection;
        public Texture createhud;
        public Texture texturehud;
        public Texture itemhud;
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
            world.GenerateInitialChunks(camera.position);

            if (KeyboardState.IsKeyDown(Keys.W)) ;
            if (KeyboardState.IsKeyDown(Keys.A)) ;
            if (KeyboardState.IsKeyDown(Keys.S)) ;
            if (KeyboardState.IsKeyDown(Keys.D)) ;
            if (input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.G))
            {
                _showGui = !_showGui;
            }

            if(input.IsKeyPressed(Keys.Q))
            {
                _hideCreate = !_hideCreate;
                Pop.Play();
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

            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                var hit = world.RaycastBlock(camera.position, camera.front);
                if (hit.HasValue)
                {
                    var (chunk, pos) = hit.Value;

                    chunk.chunkBlocks[pos.X, pos.Y, pos.Z].type = BlockType.EMPTY;
                    chunk.Rebuild();
                }
            }   
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            //Audio Inicialization
            AudioPlayer player = new AudioPlayer("key.wav");
            Pop = new AudioPlayer("Inventory.wav");

            player.Play();
            Console.WriteLine($"Playing sound...{player}");
            Console.WriteLine($"Playing sound...{Pop}");
            Thread.Sleep(3000);

            float offsetX = -1.0f / 90f;
            float offsetY = -1.7f;
            float SlotWidth = 1.6f;
            float hudHeight = 0.2f;

            float[] verticesHUD =
            {
                -SlotWidth / 2 + offsetX, -hudHeight / 2 + offsetY, 0f, 1f,
                 SlotWidth / 2 + offsetX, -hudHeight / 2 + offsetY, 1f, 1f,
                 SlotWidth / 2 + offsetX,  hudHeight / 2 + offsetY, 1f, 0f,
                -SlotWidth / 2 + offsetX,  hudHeight / 2 + offsetY, 0f, 0f,
            };

            float[] backgroundVertices =

            {
                -(SlotWidth * 0.90f) / 2 + offsetX, -hudHeight / 2 + offsetY, 0f, 1f,
                 (SlotWidth * 0.90f) / 2 + offsetX, -hudHeight / 2 + offsetY, 1f, 1f,
                 (SlotWidth * 0.90f) / 2 + offsetX,  hudHeight / 2 + offsetY, 1f, 0f,
                -(SlotWidth * 0.90f) / 2 + offsetX,  hudHeight / 2 + offsetY, 0f, 0f,
            };

            float offsetX2 = 0.0f;       
            float offsetY2 = 0.2f;      
            float CreateWidth = 1.2f;   
            float CreateHeight = 1.6f;

            float[] createHUD =

            {
                -CreateWidth / 2 + offsetX2, -CreateHeight / 2 + offsetY2, 0f, 1f,
                 CreateWidth / 2 + offsetX2, -CreateHeight / 2 + offsetY2, 1f, 1f,
                 CreateWidth / 2 + offsetX2,  CreateHeight / 2 + offsetY2, 1f, 0f,
                -CreateWidth / 2 + offsetX2,  CreateHeight / 2 + offsetY2, 0f, 0f,
            };

            //Chunk inicializated
            chunk = new Chunk(new Vector3(0, 0, 0));
            Title += ": OpenTk Version:" + GL.GetString(StringName.Version);

            world = new World();
            if (world == null)
            {
                Console.WriteLine("world is null");
            }

            else
            {
                world.RenderAll(lightingShader);
            }

            //Controller Inicializated
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            camera = new Camera(100f, 100f, new Vector3(0, 0, 0)); //This is probably the stuff I have to fix.
            //Vector3 playerPos = camera.position;
            //camera = new Camera(Size.X, Size.Y, Vector3.UnitZ * 3);
            //camera = new Camera(Size.X, Size.Y, new Vector3(25, 25, 25));


            //Timer Inicializated.
            timer = Stopwatch.StartNew();
            //camera.SetObstacles(chunk.SolidBlockAABBs);
            camera.SetObstacles(world.GetAllObstacles());
            CursorState = CursorState.Grabbed;

            lightingShader = new Shader(GLSL.vertexShaderSource, GLSL.fragmentShaderSource);
            lampShader = new Shader(GLSL.LampVert, GLSL.LampFrags);
            cloudShader = new Shader(GLSL.CloudVerts, GLSL.CloudFrags);
            inventory = new Shader(GLSL.InventoryVerts, GLSL.InventoryFrags);
            create = new Shader(GLSL.CreationVerts, GLSL.CreationFrags);
            rency = new Shader(GLSL.TransparencyVerts, GLSL.TransparencyFrags);
            itemObject = new Shader(GLSL.ObjectVert,GLSL.ObjectFrag);

            var imagePath = "Asset.png";
            texturehud = new Texture(imagePath);
            texturehud.Use(TextureUnit.Texture10);  

            var imagePath2 = "spritefond.png";
            createhud = new Texture(imagePath2);
            createhud.Use(TextureUnit.Texture11);

            //LAMP
            VAOLamp = GL.GenVertexArray();
            VBOLamp = GL.GenBuffer();

            GL.BindVertexArray(VAOLamp);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOLamp);
            GL.BufferData(BufferTarget.ArrayBuffer, lampVertices.Length * sizeof(float), lampVertices, BufferUsageHint.StaticDraw);

            // Normal en location = 3
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            int lampPosLocation = lampShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(lampPosLocation);
            GL.VertexAttribPointer(lampPosLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int posAttrib = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            int AnormalTesting = lampShader.GetAttribLocation("aNormal");
            //GL.EnableVertexAttribArray(8);
            GL.VertexAttribPointer(AnormalTesting, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(8);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //////CLOUD
            VAOCloud = GL.GenVertexArray();
            int VBOCloud = GL.GenBuffer();

            GL.BindVertexArray(VAOCloud);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCloud);
            GL.BufferData(BufferTarget.ArrayBuffer, CloudsVertices.Length * sizeof(float), CloudsVertices, BufferUsageHint.StaticDraw);

            int posCloud = cloudShader.GetAttribLocation("Cloud");
            GL.VertexAttribPointer(posCloud, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posCloud);

            int posCoord = cloudShader.GetAttribLocation("aTexCoordCloud");
            GL.VertexAttribPointer(posCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(posCoord);
            GL.BindVertexArray(0);

            //INVENTORY
            int VBOInventory = GL.GenBuffer();
            VAOInventory = GL.GenVertexArray();
            EBOInventory = GL.GenBuffer();

            GL.BindVertexArray(VAOInventory);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOInventory);
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

            VAOTransparency = GL.GenVertexArray();
            VBOTransparency = GL.GenBuffer();
            EBOTransparency = GL.GenBuffer();

            GL.BindVertexArray(VAOTransparency);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTransparency);
            GL.BufferData(BufferTarget.ArrayBuffer, backgroundVertices.Length * sizeof(float), backgroundVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOTransparency);
            GL.BufferData(BufferTarget.ElementArrayBuffer, backgroundIndices.Length * sizeof(uint), backgroundIndices, BufferUsageHint.StaticDraw);

            int posTransparency = rency.GetAttribLocation("aPos");
            GL.VertexAttribPointer(posTransparency, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posTransparency);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, VBOItem);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<VertexChunk>(), vertices, BufferUsageHint.StaticDraw);

            ////CREATE
            int VBOCreate = GL.GenBuffer();
            VAOCreate = GL.GenVertexArray();
            EBOCreate = GL.GenBuffer();

            GL.BindVertexArray(VAOCreate);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCreate);
            GL.BufferData(BufferTarget.ArrayBuffer, createHUD.Length * sizeof(float), createHUD, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOCreate);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicesCreate.Length * sizeof(uint), indicesCreate, BufferUsageHint.StaticDraw);

            int CreatePosition = create.GetAttribLocation("Crosition");
            GL.VertexAttribPointer(CreatePosition, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(CreatePosition);

            int CreateCroods = create.GetAttribLocation("aCroods");
            GL.VertexAttribPointer(CreateCroods, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(CreateCroods);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //VAOItem = GL.GenVertexArray();
            //VBOItem = GL.GenBuffer();
            //EBOItem = GL.GenBuffer();

            //GL.BindVertexArray(VAOItem);
            //// Vertex buffer
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VBOItem);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            //// Element buffer
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOItem);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);

            int ObjectPosition = itemObject.GetAttribLocation("aPos");
            GL.VertexAttribPointer(ObjectPosition, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(ObjectPosition);
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

            //Cube Model and rotation
            //Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            //lampModel = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.00f;

            //Matrix4 lampMatrix = Matrix4.Identity;
            Matrix4 lampMatrix = Matrix4.CreateTranslation(0f, 60f, 0f) + Matrix4.CreateScale(7f);

            //Render cube with lighting
            lampShader.Use();
            lampShader.SetMatrix4("model", lampMatrix);
            lampShader.SetMatrix4("view", camera.GetViewMatrix());
            lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            //lampShader.SetVector3("lightPos", new Vector3(5.0f, 5.0f, 0.0f));
            //GL.Uniform3(GL.GetUniformLocation(lampShader.Handle2,"lightPos"),lightPos);
            //GL.Uniform3(GL.GetUniformLocation(lampShader.Handle2, "lightColor"), new Vector3(1.0f, 1.0f, 1.0f));
            //GL.Uniform3(GL.GetUniformLocation(lampShader.Handle2, "viewPos"), camera.position);

            //lampShader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f));
            //lampShader.SetVector3("lightColor", new Vector3(5.0f, 5.0f, 0.0f));
            //lampShader.SetVector3("viewPos", camera.position);
            //lampShader.SetVector3("lightPos", lightPos);

            GL.BindVertexArray(VAOLamp);
            GL.UseProgram(lampShader.Handle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            // Render chunk with texture (no lighting)
            lightingShader.Use();
            Matrix4 model = Matrix4.Identity;
            lightingShader.SetMatrix4("model", model);
            lightingShader.SetMatrix4("view", view);
            lightingShader.SetMatrix4("projection", projection);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "lightPos"), lightPos);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "lightColor"), new Vector3(1.0f, 1.0f, 0.0f));
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "viewPos"), camera.position);

            //chunk.Render(lightingShader);
            world.RenderAll(lightingShader);

            int modelLocation = GL.GetUniformLocation(lightingShader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(lightingShader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(lightingShader.Handle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (_hideCreate)
            {
                create.Use();
                create.SetInt("TextureCreate", 11);
                createhud.Use(TextureUnit.Texture11);
                GL.BindVertexArray(VAOCreate);

                GL.Enable(EnableCap.DepthTest);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }

            rency.Use();
            GL.BindVertexArray(VAOTransparency);

            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            inventory.Use();
            inventory.SetInt("textureHUD", 0);
            texturehud.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAOInventory);

            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            //Matrix4 modelItem = Matrix4.Identity;
            //Matrix4 viewItem = camera.GetViewMatrix();
            //Matrix4 projectionItem = camera.GetProjectionMatrix();

            //itemObject.Use();
            //itemObject.SetMatrix4("model",modelItem);
            //itemObject.SetMatrix4("view", camera.GetViewMatrix());
            //itemObject.SetMatrix4("projection", camera.GetProjectionMatrix());

            //GL.BindVertexArray(VAOItem);
            //GL.UseProgram(itemObject.HandleItem);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.BindVertexArray(0);

            //int modelItemLocation = GL.GetUniformLocation(itemObject.HandleItem, "model");
            //int viewItemLocation = GL.GetUniformLocation(itemObject.HandleItem, "view");
            //int projectionItemLocation = GL.GetUniformLocation(itemObject.HandleItem, "projection");

            int modelLampLocation = GL.GetUniformLocation(lampShader.Handle, "model");
            int viewLampLocation = GL.GetUniformLocation(lampShader.Handle, "view");
            int projectionLampLocation = GL.GetUniformLocation(lampShader.Handle, "projection");

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);

            int modelCloudLocation = GL.GetUniformLocation(cloudShader.Handle, "modelcloud");
            int viewCloudLocation = GL.GetUniformLocation(cloudShader.Handle, "viewcloud");
            int projectionCloudLocation = GL.GetUniformLocation(cloudShader.Handle, "projectioncloud");

            //GL.UniformMatrix4(modelCloudLocation, true, ref modelCloud);
            GL.UniformMatrix4(viewCloudLocation, true, ref viewCloud);
            GL.UniformMatrix4(projectionCloudLocation, true, ref projectionCloud);

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);

            // Enable Docking
            if (_showGui)
            {
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
