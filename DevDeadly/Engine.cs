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

            string LampVert = @"
            #version 330 core
            layout(location = 0) in vec3 aPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
            }";

            string LampFrags = @"
            #version 330 core
            out vec4 FragColor;

            uniform vec3 color;

            void main()
            {
                FragColor = vec4(1.0);
            }
            ";


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
          0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
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
                //GL.Enable(EnableCap.Blend);

                //GL.FrontFace(FrontFaceDirection.Cw);
                //GL.Enable(EnableCap.CullFace);
                //GL.CullFace(CullFaceMode.Back);LampVert, LampFrags
                CursorState = CursorState.Grabbed;

                //String Path, Textures configuration and Lighting settings.
                lightingShader = new Shader(vertexShaderSource, fragmentShaderSource);
                lampShader = new ShaderLamp(LampVert, LampFrags);

                texture = new Texture("atlas.png");

                VAOLamp = GL.GenVertexArray();
                int VBOLamp = GL.GenBuffer();

                GL.BindVertexArray(VAOLamp);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOLamp);
                GL.BufferData(BufferTarget.ArrayBuffer, lampVertices.Length * sizeof(float), lampVertices, BufferUsageHint.StaticDraw);

                int lampPosLocation = lampShader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(lampPosLocation);
                GL.VertexAttribPointer(lampPosLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            //Wraps lines boxes
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                //Background Color
                GL.ClearColor(0.227f, 0.557f, 1.0f, 1.0f);

            /*DRAWING 
            ----------------------------------------------------------------------------------------------------------------------------------------------*/
            ////in case i want to change it to the lamp shit, i have to replace it as anormal and don't forget to change the vert/frag and add the lightingsource
            int positionLocation = GL.GetAttribLocation(lampShader.Handle2, "aPos");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            int posAttrib = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(posAttrib);
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

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


            model = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(50f, 10f, 30f);
            //yRot += 0.01f;
            //model = Matrix4.CreateScale(10f);

            Matrix4 lampMatrix = Matrix4.Identity;
            

            lampShader.Use();            
            lampShader.SetMatrix4("model", lampMatrix);
            lampShader.SetMatrix4("view", camera.GetProjectionMatrix());
            lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            lampShader.SetVector3("color", new Vector3(1.1f, 1.0f, 1.0f)); // fucsia

            //GL.BindVertexArray(VAOLamp);    
            //GL.UseProgram(lampShader.Handle2);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            //chunk.Render(lightingShader); //AQUI SI RENDERIZA SIEMPRE DESPUES DE LIGHTING 

            lightingShader.Use();


            lightingShader.SetMatrix4("model",model);
                lightingShader.SetMatrix4("view", view);
                lightingShader.SetMatrix4("projection", projection);

            //GL.BindVertexArray(VAOLamp);
            //GL.UseProgram(lightingShader.Handle);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(VAOLamp);
            GL.UseProgram(lampShader.Handle2);
            GL.DrawArrays(PrimitiveType.Triangles, 0,36);

            //chunk.Render(lightingShader); //AQUI SI RENDERIZA SIEMPRE DESPUES DE LIGHTING 

            //Don't forget to know the correct order of rendering (The chunk is going to be first than controller).
            //chunk.Render(lightingShader); // no this one

            //Si quiero el cubo futsia tengo que dejar color y el resto de cosas por debajo de lightingshader vaolamp, lampShader.Handle y el triangulo
            //Sin activar el chunk, si activo el chunk desaparece o aparece pero con el cubo rojo
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //chunk.Render(lightingShader); //AQUI SI RENDERIZA SIEMPRE DESPUES DE LIGHTING 

            //GL.UseProgram(lampShader.Handle2);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);


            /*Define here but not as shaderProgram (if it is possible to use shaderProgram should be the correct way),
            cause otherwise is not going to work.*/
            //GL.UseProgram(0);

            //lightingShader.Use();
            int modelLocation = GL.GetUniformLocation(lightingShader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(lightingShader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(lightingShader.Handle, "projection");
            //chunk.Render(lightingShader); //AQUI SI RENDERIZA SIEMPRE DESPUES DE LIGHTING 

                GL.UniformMatrix4(modelLocation, true, ref model);
                GL.UniformMatrix4(viewLocation, true, ref view);
                GL.UniformMatrix4(projectionLocation, true, ref projection);

            int modelLampLocation = GL.GetUniformLocation(lampShader.Handle2, "model");
            int viewLampLocation = GL.GetUniformLocation(lampShader.Handle2, "view");
            int projectionLampLocation = GL.GetUniformLocation(lampShader.Handle2, "projection");
            chunk.Render(lightingShader); //AQUI SI RENDERIZA SIEMPRE DESPUES DE LIGHTING Y DESPUES DE MODELLAMPLOCATION

            GL.UniformMatrix4(modelLampLocation, true, ref lampMatrix);
            GL.UniformMatrix4(viewLampLocation, true, ref view);
            GL.UniformMatrix4(projectionLampLocation, true, ref projection);

            //lampModel = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.00f;

            //model = Matrix4.CreateRotationZ(yRot) * Matrix4.CreateTranslation(10f, 0f, -5f);
            //yRot += 0.001f;

            // Enable Docking
            if (_showGui)
                {
                    //DockSpace made my background dark taking the whole rez of the screen.
                    //ImGui.DockSpaceOverViewport();
                    ImGui.ShowDemoWindow();
                    ImGuiController.CheckGLError("End of frame");
                }
                _controller.Render();

            //White color
            //lightingShader.SetVector3("objectcolor", new Vector3(1.0f, 1.0f, 1.0f));
            //lightingShader.SetVector3("lightcolor", new Vector3(1.0f, 1.0f, 1.0f));

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
