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

            in vec2 texCoord;
            out vec4 FragColor;

            uniform sampler2D texture0;
            uniform sampler2D texture1;

            void main()
            {
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
                float spec = pow(max(dot(viewDir, reflectDir), 0.0f), 36);
                vec3 specular = specularStrength * spec * lightColor;

                // Final color
                vec3 result = (ambient + diffuse + specular) * objectColor;
                FragColor = vec4(result, 1.0f);
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
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f
        };


        //GUI
        ImGuiController _controller;
        private bool _showGui = true;

        // VAO,EBO,VBO (TEXTURE SET)
        public int VertexArrayObject;
        public int ElementBufferObject;
        public int VertexBufferObject;

        //VAO,EBO,EBO (LAMP SET)
        public int VAOLamp;
        public int EBOLamp;
        public int VBOLamp;
        public int VAOModel;

        public int nrAttribute = 0;
        public int width, height;
        public bool OptionCursorState;

        //SHADER SET
        private Stopwatch timer = Stopwatch.StartNew();
        public int shaderProgram;
        Shader lightingShader;
        ShaderLamp lampShader;
        public int state;
        public Shader shader;

        private readonly Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        //TEXTURE SET
        public Matrix4 projection;
        public Texture texture;
        public Texture texture2;
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
            if (KeyboardState.IsKeyDown(Keys.W)) Console.WriteLine("W Is being pressed");
            if (KeyboardState.IsKeyDown(Keys.A)) Console.WriteLine("A Is being pressed");
            if (KeyboardState.IsKeyDown(Keys.S)) Console.WriteLine("S Is being pressed");
            if (KeyboardState.IsKeyDown(Keys.D)) Console.WriteLine("D Is being pressed");
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

            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back); 
            CursorState = CursorState.Grabbed;

            //String Path, Textures configuration and Lighting settings.
            lightingShader = new Shader(vertexShaderSource, fragmentShaderSource);
            lampShader = new ShaderLamp(LampVert, LampFrags);

            VAOLamp = GL.GenVertexArray();
            int VBOLamp = GL.GenBuffer();

            GL.BindVertexArray(VAOLamp);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOLamp);
            GL.BufferData(BufferTarget.ArrayBuffer, lampVertices.Length * sizeof(float), lampVertices, BufferUsageHint.StaticDraw);

            /*DRAWING 
            ----------------------------------------------------------------------------------------------------------------------------------------------*/
            ////in case i want to change it to the lamp shit, i have to replace it as anormal and don't forget to change the vert/frag and add the lightingsource

            //Light
            //int lampPosLocation = lampShader.GetAttribLocation("aPosition");
            //GL.EnableVertexAttribArray(lampPosLocation);
            //GL.VertexAttribPointer(lampPosLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int posAttrib = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            int Anormal = lampShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(Anormal);
            GL.VertexAttribPointer(Anormal, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);


            //Wraps lines boxes
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Background Color
            GL.ClearColor(0.529f, 0.808f, 0.922f, 1.0f);
            GL.GetInteger(GetPName.MaxVertexAttribs, out nrAttribute);

            lightingShader.Use();
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
            lampShader.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _controller.Update(this, (float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);

            //Cube Model and rotation}
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            //lampModel = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.00f;

            //model = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.001f;

            Matrix4 lampMatrix = Matrix4.Identity;

            // Render cube with lighting
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

                                                              //L/F     /U/D       L/F
            lampMatrix = Matrix4.CreateTranslation(new Vector3(-30.0f, -6.0f, -30.0f));

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.UseProgram(lampShader.Handle2);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            int modelLocation = GL.GetUniformLocation(lightingShader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(lightingShader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(lightingShader.Handle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            int modelLampLocation = GL.GetUniformLocation(lampShader.Handle2, "model");
            int viewLampLocation = GL.GetUniformLocation(lampShader.Handle2, "view");
            int projectionLampLocation = GL.GetUniformLocation(lampShader.Handle2, "projection");
            chunk.Render(lightingShader); //Render always after the mvp set.

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);

            // Enable Docking
            if (_showGui)
            {
                //DockSpace made my background dark taking the whole rez of the screen.
                //ImGui.DockSpaceOverViewport();
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

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }
    }
}
