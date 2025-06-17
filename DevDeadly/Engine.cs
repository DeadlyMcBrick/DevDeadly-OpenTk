using System;
using ImGuiNET;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vector3 = OpenTK.Mathematics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static DevDeadly.Chunk;

namespace DevDeadly
{
    public class Game : GameWindow
    {
        //Vertex
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

        //Fragment
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
 
        //Build
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

        private readonly uint[] indices = 
        
        {
             0, 1, 3,  // First 
             1, 2, 3   // Second triangule
        };
        
        private readonly float[] quadVertices = 
        
        {
            // Positions       // Texture Coords
            -0.5f, -0.5f,      0.0f, 0.0f, // Bottom-left
             0.5f, -0.5f,      1.0f, 0.0f, // Bottom-right
             0.5f,  0.5f,      1.0f, 1.0f, // Top-right
            -0.5f,  0.5f,      0.0f, 1.0f  // Top-left
        };

        private readonly uint[] quadIndices = 
        
        {
            0, 1, 2,
            2, 3, 0
        };


        //GUI
        ImGuiController _controller;
        private bool _showGui = true;

        // VAO,EBO,VBO SET
        public int VertexArrayObject;
        int ElementBufferObject;
        int VertexBufferObject;
        int nrAttribute = 0;
        int width, height;

        //SHADER SET
        private Stopwatch timer = Stopwatch.StartNew();
        public Shader lightingShader;
        public int shaderProgram;
        public int state;
        public Shader lampShader;
        public Shader shader;
        //public Tool tool;

        //TEXTURE SET
        public Matrix4 projection;
        public Texture texture;
        public Texture texture2;
        public Texture texture3;
        public Matrix4 model;
        public Matrix4 view;

        Camera camera;
        Chunk chunk;

        //ROT Y
        float yRot;

        //ROT X
        float zRot;

        //WIN CONFIG
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (X: width, Y: height),
            Title = title
        })

        { 
         this.width = width; this.height = height;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            MouseState mouse = MouseState;
            var input = KeyboardState;
            bool IsCursorGrabbed = false;
            camera.Update(input, mouse, e);

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

            if (KeyboardState.IsKeyDown(Keys.F))

            {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))

            {
                Close();
            }

            if (KeyboardState.IsKeyDown(Keys.F));
           
            //This method is going to be added to make current of pressing the key.
            if(KeyboardState.IsKeyDown(Keys.D))
            {
                MakeCurrent();
            }
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            //Inventory inicialization
            InventorySlot[,] HUD = new InventorySlot[10, 4];

            //Audio Inicialization
            AudioPlayer player = new AudioPlayer("wrld.wav");
            player.Play();
            Console.WriteLine("Reproduciendo sonido..." + player);
            Thread.Sleep(3000);

            //Chunk inicializated
            chunk = new Chunk(new Vector3(0, 0, 0));
            Title += ": OpenTk Version:" + GL.GetString(StringName.Version);

            //Controller inicializated
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            camera = new Camera(width, height, Vector3.Zero);

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            ElementBufferObject = GL.GenBuffer();
            GL.Enable(EnableCap.DepthTest);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
            CursorState = CursorState.Grabbed;

            //String Path
            shader = new Shader(vertexShaderSource, fragmentShaderSource);

            string imagePath = "atlas.png"; 
            texture = new Texture(imagePath);
            texture.Use(TextureUnit.Texture0);

            //Replaced by inventory slots
            string imagePath2 = "atlas.png";
            texture2 = new Texture(imagePath2);
            texture2.Use(TextureUnit.Texture1);

            string imagePath3 = "Slots.png";
            texture3 = new Texture(imagePath3);
            texture3.Use(TextureUnit.Texture2);

            //To being able to put both images in the render have to check it if i have another shader.use
            shader.Use();

            //To use the function for the frags
            shader.SetInt("texture0", 0);
            shader.SetInt("texture1", 1);
            shader.SetInt("texture2", 2);

            //Vertices
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //Indices
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

            //Drawing
            int positionLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            //In case I want to change it to the lamp shit, I have to replace it as aNormal and don't forget to change the vert/frag and add the lightingSource
            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof (float));
            GL.EnableVertexAttribArray(texCoordLocation);

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
            _controller.Update(this, (float)args.Time);

            //Cube Model and rotation
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            Matrix4 OrthoProjection = Matrix4.CreateOrthographicOffCenter(0, width, height, 1.0f, 1.0f, 1.0f);
            
            //Matrix4 outlineModel = Matrix4.CreateScale(1.05f) * model;
            //model = Matrix4.CreateRotationZ(zRot) * Matrix4.CreateTranslation(0f, 0f, -5f);
            //zRot += 0.001f;

            //Define here but not as shaderProgram (if it is possible to use shaderProgram should be the correct way), cause otherwise is not going to work.
            int modelLocation = GL.GetUniformLocation(shader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(shader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(shader.Handle, "projection");
            //int OrthoLocation = GL.GetUniformLocation(shader.Handle, "Being taken idk....");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            //GL.UniformMatrix4(OrthoLocation, true, ref OrthoProjection);

            GL.UseProgram(shaderProgram);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // Enable Docking
            if (_showGui)
            {
                //DockSpace made my background dark taking the whole rez of the screen.
                //ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();
                ImGuiController.CheckGLError("End of frame");
            }

            //Don't forget to know the correct order of rendering (The chunk is going to be first than controller).
            chunk.Render(shader);
            _controller.Render();
            GL.BindVertexArray(VertexArrayObject);

            //Being able to not render everything just in case the loop is default // the line border.
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            shader.Use();

            //If I want render 2 images dont use shader here.9
            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            texture3.Use(TextureUnit.Texture2);

            //model = Matrix4.CreateTranslation(new Vector3(cube.Min.X, cube.Min.Y, cube.Min.Z));
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
