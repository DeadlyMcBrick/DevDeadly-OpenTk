using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;
using ImGuiNET;
using static DevDeadly.Chunk;
using System.Runtime.CompilerServices;


/*
  ⢠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡴⠀
⠀⠀⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠁⠀
⠀⠀⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⡀⠀
⠀⢸⠁⠇⠀⠀⠀⠀⢀⠠⠔⠂⠀⠀⠒⠂⠤⡀⠀⠀⠀⠀⠸⠁⡇⠀
⠀⢸⣀⡌⠀⡀⣠⠞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⠀⡀⢁⣀⡇⠀
⠀⠘⣧⣀⣠⣿⠃⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠘⣿⣦⣀⣼⠃⠀
⢀⠀⠘⢿⣿⣃⡞⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠱⣜⣿⣿⠋⠀⡀
⠈⣧⠀⢸⡟⠿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⢻⡇⠀⣴⠁
⠀⢸⢃⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇⡘⡇⠀
⠀⠸⡇⢻⠀⢠⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⡀⠀⡟⢸⠇⠀
⠀⠀⠱⠀⡂⠈⠉⢏⠓⢄⠀⡀⠀⠀⢀⠀⡠⠚⡩⠉⠁⢸⠁⠈⠀⠀
⠀⠀⢂⢰⣧⠀⠀⠀⠉⠉⠉⢀⠀⠀⡀⠉⠉⠉⠀⠀⠀⣼⡇⡸⠀⠀
⠀⠀⠘⢾⣿⠱⠤⣄⠤⢾⠀⢸⠀⠀⡇⠀⡷⠤⣠⠤⠎⣿⡷⠃⠀⠀
⠀⠀⠀⠀⢻⡆⡇⠠⡆⠢⢈⠙⢤⡤⠃⡁⠔⢲⠄⢸⢠⡟⠀⠀⠀⠀
⠀⠀⠀⠀⠈⣧⢸⢻⣿⣷⣰⣬⣍⣩⣥⣇⣾⣿⡏⡇⣼⠁⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠸⣼⠨⡌⢸⢻⣿⣿⣿⣿⠟⡏⢡⠅⣧⠇⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⢻⠃⠘⣶⣠⡠⣠⣄⢄⣄⣴⠃⠐⡟⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢧⠀⠣⠤⠒⠒⠓⠒⠤⠜⠀⡼⠁⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠈⠳⡀⠀⠀⠀⠀⠀⠀⢀⠜⠁⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠐⠒⠀⠀⠒⠂⠁⠀⠀⠀⠀⠀             
/\  _`\                  /\  _`\                     /\ \ /\_ \               
\ \ \/\ \     __   __  __\ \ \/\ \     __     __     \_\ \\//\ \    __  __    
 \ \ \ \ \  /'__`\/\ \/\ \\ \ \ \ \  /'__`\ /'__`\   /'_` \ \ \ \  /\ \/\ \   
  \ \ \_\ \/\  __/\ \ \_/ |\ \ \_\ \/\  __//\ \L\.\_/\ \L\ \ \_\ \_\ \ \_\ \                      
   \ \____/\ \____\\ \___/  \ \____/\ \____\ \__/.\_\ \___, _\/\____\\/`____ \ 
    \/___/  \/____/ \/__/    \/___/  \/____/\/__/\/_/\/__, _ /\/____/ `/___/> \
                                                                        /\___/
                                                                        \/__/ */


namespace DevDeadly
{
    public class Game : GameWindow
    {
        //GUI
        ImGuiController _controller;
        private bool _showGui = true;
      
        Chunk chunk;
        private Stopwatch timer = Stopwatch.StartNew();
        public Shader shader;
        public Shader lightingShader;
        public Shader lampShader;
        //public List<BoundingBox> cubes;
        //public List<BoundingBox> cubes2;


        //Define separate for the other shader shit
        int shaderProgram;

        // Define the vertex and fragment shader sources
        string vertexShaderSource = @"
        #version 330 core

        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec2 aTexCoord;
        
        out vec2 texCoord;

        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;

        void main()
        {
            gl_Position =  vec4(aPosition, 1.0) * model * view * projection;        
            texCoord = aTexCoord;
        }";

        string fragmentShaderSource = @"
       #version 330 core

        out vec4 FragColor;

        in vec2 texCoord;

        uniform sampler2D texture0;
        uniform sampler2D texture1;

        void main()
        {
            // Mezclar las texturas
            vec4 color1 = texture(texture0, texCoord);
            vec4 color2 = texture(texture1, texCoord);
            FragColor = mix(color1, color2, 0.4);
         }";


        //FragColor = vec4(1.0);
        //FragColor = vec4(lightColor* objectColor, 1.0);

        //in vec2 texCoord;

        //uniform sampler2D texture0;
        //uniform sampler2D texture1;
        //// Mezclar las texturas
        //vec4 color1 = texture(texture0, texCoord);
        //vec4 color2 = texture(texture1, texCoord);
        //FragColor = mix(color1, color2, 0.4);

        //string lightingShaderSource = @"
        //#version 330 core
        //out vec4 FragColor;

        // uniform vec3 objectColor;
        // uniform vec3 lightColor;
        // uniform vec3 lightPos;
        // uniform vec3 viewPos;

        //in vec3 Normal;
        //in vec3 FragPos;

        //void main ()
        //{

        // float ambientStrength = 0.1;
        // vec3 ambient = ambientStrength * lightColor;

        // vec3 norm = normalize(Normal);
        // vec3 lightDir = normalize(lightPos - FragPos);

        // float diff = max(dot(norm, lightDir), 0.0);
        // vec3 diffuse = diff * lightColor;    

        // float specularStrength = 0.5;
        // vec3 viewDir = normalize (viewPos - FragPos);
        // vec3 reflectDir = reflect(-lightDir, norm);

        // float spec = pow(max(dot(viewDir, reflectDir), 0.0), 256);
        // vec3 specular = specularStrength * spec * lightColor;

        // vec3 result = (ambient + diffuse + specular) * objectColor;
        // FragColor = vec4(result, 1.0);

        //}";

         //Replace this one for the Faces.Bottoms to being able to chunks this terrain 
        // Vertex data
        private readonly float[] vertices = {

    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

        //public readonly Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private readonly uint[] indices =
        {
             0, 1, 3,  // First 
             1, 2, 3   // Second triangule
        };

        //do the twiceShaders connect it to one
        private readonly float[] texCoords = 
        {
        0.0f, 0.0f,  // lower-left corner  
        1.0f, 0.0f,  // lower-right corner
        0.5f, 1.0f   // top-center corner
        };

        public int VertexArrayObject;
        int VertexBufferObject;
        int ElementBufferObject;
        int nrAttribute = 0;
        int width, height;
        Camera camera;
        int resultado;
        AABB colition;
        Player player = new Player(new Vector3(5, 35, 5), new Vector3(1, 2, 1));
        Vector3 playerMovement = new Vector3(0, 0, 0);

        // ROTATION Y
        float yRot;

        //ROTATION X
        float zRot;

        public Texture texture;
        public Texture texture2;
        public Matrix4 model;
        public Matrix4 view;
        public Matrix4 projection;


        // Configuration for the window

        public Game() : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(1600, 900), APIVersion = new Version(3, 3) })
        { }
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (X: width, Y: height),
            Title = title
        })

        { 
         this.width = width; this.height = height;
        }


        //public struct Color4
        //{
        //    public float R, G, B, A;
        //    public Color4(float r, float g, float b, float a)
        //    {
        //        R = r;
        //        G = g;
        //        B = b;
        //        A = a;
        //    }
        //    public static Color4 operator *(Color4 color1, Color4 color2)
        //    {
        //        return new Color4(
        //            color1.R * color2.R,
        //            color1.G * color2.G,
        //            color1.B * color2.B,
        //            color1.A * color2.A
        //        );
        //    }
        //}

        //Keybinds (Being able to drop some menu to being able to change in the future idk)


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //private KeyboardState _lastKeyboardState;
            base.OnUpdateFrame(e);

            MouseState mouse = MouseState;
            var input = KeyboardState;
            bool IsCursorGrabbed = false;
            camera.Update(input, mouse, e);

            //if (ImGui.GetIO().WantCaptureMouse || ImGui.GetIO().WantCaptureKeyboard)
            //    return;

            //Keybinds to detect if this is actually is being pressed...
            if (KeyboardState.IsKeyDown(Keys.W)) Console.WriteLine("W pressed");
            if (KeyboardState.IsKeyDown(Keys.A)) Console.WriteLine("A pressed");
            if (KeyboardState.IsKeyDown(Keys.S)) Console.WriteLine("S pressed");
            if (KeyboardState.IsKeyDown(Keys.D)) Console.WriteLine("D pressed");

            if(input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.G))
            {
                _showGui = !_showGui;
            }

            // !! Only it's being able to hide the cursor but not re activate it idk
            if (input.IsKeyDown(Keys.V))

            {
                IsCursorGrabbed = !IsCursorGrabbed;

                CursorState = IsCursorGrabbed ? CursorState.Grabbed : CursorState.Normal;
                Console.WriteLine($"IsCursorGrabbed: {IsCursorGrabbed}, CursorState: {CursorState}");
            }


            //else
            //{
            //    (input)
            //}

            //if (input.IsKeyDown(Keys.Tab) && !_lastKeyboardState.IsKeyDown(Keys.Tab))
            //{
            //    _showGuiMenu = !_showGuiMenu;
            //    Console.WriteLine($"GUI Menu: {(_showGuiMenu ? "Abierto" : "Cerrado")}");
            //}
            //_lastKeyboardState = input; 

            //if (KeyboardState.IsKeyDown(Keys.Tab))
            // {
            //     _showGuiMenu = !_showGuiMenu;
            // }

            if (KeyboardState.IsKeyDown(Keys.F))
            {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            //if (cubes != null)
            //{
            //    Vector3 cameraPosition = camera.position;

            //    foreach (var cube in cubes)
            //    {
            //        if (cube.Intersects(cameraPosition))
            //        {
            //            Console.WriteLine("Colision detected");
            //        }
            //    }
            //}

        }
        protected override void OnLoad()
        {
            base.OnLoad();

            chunk  = new Chunk(new Vector3(0, 0, 0));

            Title += ": OpenTk Version:" + GL.GetString(StringName.Version);
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);


            //_imguiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            //ImGui.GetStyle().WindowRounding = 5.0f;

            //int width = 9;
            //int height = 1;
            //int depth = 3;
            //cubes2 = GenerateTerrain(width, height, depth); 


            //cubes = new List<BoundingBox>()

            //{
            //     new BoundingBox(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f)),
            //     new BoundingBox(new Vector3(3.0f, -1.0f, -1.0f), new Vector3(5.0f, 1.0f, 1.0f))

            //};

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            ElementBufferObject = GL.GenBuffer();
            GL.Enable(EnableCap.DepthTest);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            //String path
            shader = new Shader(vertexShaderSource, fragmentShaderSource);

            string imagePath = "atlas.png"; 
            texture = new Texture(imagePath);
            texture.Use(TextureUnit.Texture0);

            string imagePath2 = "atlas.png";
            texture2 = new Texture(imagePath2);
            texture2.Use(TextureUnit.Texture1);

            //To being able to put both images in the render have to check it if i have another shader.use
            shader.Use();

            //being able to use the function for the frags
            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);

            //vertices
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            //Wraps lines boxes
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdgeSgis);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //separate to the other buffer and shader info...
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Background Color
            GL.ClearColor(3.0f, 3.0f, 3.0f, 2.0f);

            timer = Stopwatch.StartNew();

            GL.BindVertexArray(VertexArrayObject);

            //Drawing shit
            int positionLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            //In case I want to change it to the lamp shit, I have to replace it as aNormal and don't forget to change the vert/frag and add the lightingSource
            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof (float));
            GL.EnableVertexAttribArray(texCoordLocation);

            //camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f);
            camera = new Camera(width, height, Vector3.Zero);

            //CursorState = CursorState.Grabbed;

            //Structure all the info for the buffer and stuff bla....
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.GetInteger(GetPName.MaxVertexAttribs, out nrAttribute);

            shader.Use();
            timer.Start();
        }
        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            shader.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);

        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


            //Cube Model and rotation
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            //Matrix4 outlineModel = Matrix4.CreateScale(1.05f) * model;
            
            ////Lamp configuration 
            //Matrix4 LampMatrix = Matrix4.Identity;

            //Scale the size of the box (Not working with negatives for some reason)
            //model *= Matrix4.CreateScale(0.2f);
            //model = Matrix4.CreateTranslation(lightPos);

            //Creation of the lights for the shadows blocks
          
            //shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            //shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            //shader.SetVector3("lightPos", lightPos);
            //shader.SetVector3("viewPos", camera.position);

            //Color projection 
            //Color4 coral = new Color4(1.0f, 0.5f, 0.31f, 1.0f);
            //Color4 lightColor = new Color4(0.33f, 0.42f, 0.18f, 1.0f);
            //Color4 toyColor = new Color4(1.0f, 0.5f, 0.31f, 1.0f);
            //Color4 result = lightColor * toyColor;

            //Ilumination position
            //model = Matrix4.CreateRotationY(yRot) * Matrix4.CreateTranslation(2f, 4f, -5f);
            //yRot += 0.0001f;

            //model = Matrix4.CreateRotationZ(zRot) * Matrix4.CreateTranslation(0f, 0f, -5f);
            //zRot += 0.001f;

            //Define here but not as shaderProgram (if it is possible to use shaderProgram should be the correct way), cause otherwise is not going to work.
            int modelLocation = GL.GetUniformLocation(shader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(shader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(shader.Handle, "projection");
            //This is going to be separated to the other one, also need to be added in the Frags and Verts
            int testingLocation = GL.GetUniformLocation(shader.Handle, "LocationTesting");
           
            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.UseProgram(shaderProgram);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            _controller.Update(this, (float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // Enable Docking

            if (_showGui)
            {
                //DockSpace made my background dark taking the whole rez of the screen.
                //ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();
                //_controller.Render();
                ImGuiController.CheckGLError("End of frame");
            }

            //Don't forget to know the correct order of rendering.
            chunk.Render(shader);
            _controller.Render();

            GL.BindVertexArray(VertexArrayObject);

            //Being able to not render everything just in case the loop is default.
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //_controller.Render();

            shader.Use();

            //If I wanna render 2 images dont use shader here.9
            shader.Use();

            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);

            //model = Matrix4.CreateTranslation(new Vector3(cube.Min.X, cube.Min.Y, cube.Min.Z));

            //GL.Enable(EnableCap.DepthTest);              // x   y   z 
            //model += Matrix4.CreateTranslation(new Vector3(1f, 0f, 0f));
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            ////White color
            ////shader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f)); 
            ////shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            ////shader.SetVector3("lightPos", lightPos);
            ////shader.SetVector3("viewPos", camera.position);

            //GL.Enable(EnableCap.DepthTest);
            //GL.UniformMatrix4(modelLocation, true, ref model);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            //DrawHUD();
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

    }
}
