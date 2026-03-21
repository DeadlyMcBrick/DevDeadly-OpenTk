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
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (X: width, Y: height),
            Title = title
        })
        {
            this.width = width; this.height = height;
        }

        private int VertexArrayObject;
        private int ElementBufferObject;
        private int VertexBufferObject;
        private bool _showGui = true;
        private bool _hideCreate = true;
        private int VAOLamp;
        private int VBOLamp;
        private int VAOCloud;
        private int VAOInventory;
        private int EBOInventory;
        private int VAOTransparency;
        private int VBOTransparency;
        private int EBOTransparency;
        private int VAOCreate;
        private int EBOCreate;
        private int VAOMain;
        private double _fpsTimer;
        private Vector3 light_Pos;
        public int nrAttribute;
        public int width, height;
        public bool OptionCursorState;

        private const int ShadowMapSize = 2048;
        private int _shadowFBO;
        private int _shadowDepthTex;

        private float _timeOfDay = 0.3f;
        private float _dayDuration = 120.0f;
        private int VAOSky;

        private Stopwatch timer = Stopwatch.StartNew();
        public AudioPlayer Pop;

        public Matrix4 projection;
        public Matrix4 model;
        public Matrix4 view;
        private Matrix4 _lightSpaceMatrix;
        public static int TextureID;

        Shader lightingShader;
        Shader lampShader;
        Shader cloudShader;
        Shader inventory;
        Shader create;
        Shader rency;
        Shader itemObject;
        Shader skyShader;
        Shader shadowShader;

        Camera camera;
        Chunk chunk;
        Texture createhud;
        Texture texturehud;
        World world;
        ImGuiController _controller;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            var IsCursorGrabbed = false;

            MouseState mouse = MouseState;
            _timeOfDay += (float)(e.Time / _dayDuration);
            if (_timeOfDay >= 1.0f) _timeOfDay -= 1.0f;

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

            if (input.IsKeyDown(Keys.V))
            {
                IsCursorGrabbed = !IsCursorGrabbed;
                CursorState = IsCursorGrabbed ? CursorState.Grabbed : CursorState.Normal;
                Console.WriteLine($"IsCursorGrabbed: {IsCursorGrabbed}, CursorState: {CursorState}");
            }

            if (KeyboardState.IsKeyPressed(Keys.F))
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

            Title += ": OpenTk Version:" + GL.GetString(StringName.Version);

            chunk = new Chunk(new Vector3(0, 0, 0));
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            camera = new Camera(100f, 100f, new Vector3(0, 0, 0));
            shadowShader = new Shader(GLSL.ShadowVert, GLSL.ShadowFrag);
            skyShader = new Shader(GLSL.SkyVert, GLSL.SkyFrag);
            lightingShader = new Shader(GLSL.vertexShaderSource, GLSL.fragmentShaderSource);
            lampShader = new Shader(GLSL.LampVert, GLSL.LampFrags);
            cloudShader = new Shader(GLSL.CloudVerts, GLSL.CloudFrags);
            inventory = new Shader(GLSL.InventoryVerts, GLSL.InventoryFrags);
            create = new Shader(GLSL.CreationVerts, GLSL.CreationFrags);
            rency = new Shader(GLSL.TransparencyVerts, GLSL.TransparencyFrags);
            itemObject = new Shader(GLSL.ObjectVert, GLSL.ObjectFrag);
            world = new World();
            if (world == null)
            {
                Console.WriteLine("world is null");
            }

            else
            {
                world.RenderAll(lightingShader);
            }

            AudioPlayer player = new AudioPlayer("key.wav");
            Pop = new AudioPlayer("Inventory.wav");
            player.Play();
            Thread.Sleep(3000);

            timer = Stopwatch.StartNew();
            camera.SetObstacles(chunk.SolidBlockAABBs);
            camera.SetObstacles(world.GetAllObstacles());
            CursorState = CursorState.Grabbed;

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
            GL.BufferData(BufferTarget.ArrayBuffer,Draw.lampVertices.Length * sizeof(float), Draw.lampVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(3); // Normal en location = 3
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

            //CLOUD
            VAOCloud = GL.GenVertexArray();
            int VBOCloud = GL.GenBuffer();

            GL.BindVertexArray(VAOCloud);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCloud);
            GL.BufferData(BufferTarget.ArrayBuffer, Draw.CloudsVertices.Length * sizeof(float), Draw.CloudsVertices, BufferUsageHint.StaticDraw);

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
            GL.BufferData(BufferTarget.ArrayBuffer, Slot.verticesHUD.Length * sizeof(float), Slot.verticesHUD, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOInventory);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Draw.indicesHUD.Length * sizeof(uint), Draw.indicesHUD, BufferUsageHint.StaticDraw);

            int posIU = inventory.GetAttribLocation("IUPosition");
            GL.VertexAttribPointer(posIU, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posIU);

            int posIUCoord = inventory.GetAttribLocation("IUCoord");
            GL.VertexAttribPointer(posIUCoord, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(posIUCoord);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //TRANSPARENCY
            VAOTransparency = GL.GenVertexArray();
            VBOTransparency = GL.GenBuffer();
            EBOTransparency = GL.GenBuffer();

            GL.BindVertexArray(VAOTransparency);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTransparency);
            GL.BufferData(BufferTarget.ArrayBuffer, Slot.backgroundVertices.Length * sizeof(float), Slot.backgroundVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOTransparency);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Draw.backgroundIndices.Length * sizeof(uint), Draw.backgroundIndices, BufferUsageHint.StaticDraw);

            int posTransparency = rency.GetAttribLocation("aPos");
            GL.VertexAttribPointer(posTransparency, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posTransparency);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //CREATE
            int VBOCreate = GL.GenBuffer();
            int VAOCreate = GL.GenVertexArray();
            EBOCreate = GL.GenBuffer();

            GL.BindVertexArray(VAOCreate);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOCreate);
            GL.BufferData(BufferTarget.ArrayBuffer, Slot.createHUD.Length * sizeof(float), Slot.createHUD, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOCreate);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Draw.indicesCreate.Length * sizeof(uint), Draw.indicesCreate, BufferUsageHint.StaticDraw);

            int CreatePosition = create.GetAttribLocation("Crosition");
            GL.VertexAttribPointer(CreatePosition, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(CreatePosition);

            int CreateCroods = create.GetAttribLocation("aCroods");
            GL.VertexAttribPointer(CreateCroods, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(CreateCroods);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            int ObjectPosition = itemObject.GetAttribLocation("aPos");
            GL.VertexAttribPointer(ObjectPosition, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(ObjectPosition);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            //Wraps lines boxes
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };
            float[] skyQad = { -1f, -1f, 1f, -1f, 1f, 1f, -1f, -1f, 1f, 1f, -1f, 1f };

            VAOSky = GL.GenVertexArray();
            int VBOSky = GL.GenBuffer();
            GL.BindVertexArray(VAOSky);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOSky);
            GL.BufferData(BufferTarget.ArrayBuffer, skyQad.Length * sizeof(float), skyQad, BufferUsageHint.StaticDraw);
            int skyPosLoc = skyShader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(skyPosLoc, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(skyPosLoc);
            GL.BindVertexArray(0);

            _shadowDepthTex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _shadowDepthTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,
                ShadowMapSize, ShadowMapSize, 0,
                PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] shadowBorder = { 1f, 1f, 1f, 1f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, shadowBorder);

            _shadowFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _shadowFBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, _shadowDepthTex, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.ClearColor(0f, 0f, 0f, 1f);
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

        private Vector3 ComputeSkyHorizonColor(float t)
        {
            Vector3 nightH = new Vector3(0.025f, 0.025f, 0.07f);
            Vector3 dawnH = new Vector3(0.95f, 0.45f, 0.20f);
            Vector3 dayH = new Vector3(0.65f, 0.82f, 0.98f);
            Vector3 duskH = new Vector3(1.00f, 0.35f, 0.05f);
            if (t < 0.2f) return nightH;
            if (t < 0.3f) return Vector3.Lerp(nightH, dawnH, (t - 0.2f) * 10f);
            if (t < 0.4f) return Vector3.Lerp(dawnH, dayH, (t - 0.3f) * 10f);
            if (t < 0.6f) return dayH;
            if (t < 0.7f) return Vector3.Lerp(dayH, duskH, (t - 0.6f) * 10f);
            if (t < 0.8f) return Vector3.Lerp(duskH, nightH, (t - 0.7f) * 10f);
            return nightH;
        }

        private Vector3 ComputeLightColor(float t)
        {
            float day = Math.Clamp((t - 0.25f) / 0.1f, 0f, 1f) * (1f - Math.Clamp((t - 0.7f) / 0.1f, 0f, 1f));
            Vector3 sunrise = new Vector3(1.0f, 0.65f, 0.30f);
            Vector3 noon = new Vector3(1.0f, 0.97f, 0.88f);
            Vector3 night = new Vector3(0.05f, 0.07f, 0.15f);
            float noonFactor = (float)Math.Sin(day * Math.PI);
            return Vector3.Lerp(night, Vector3.Lerp(sunrise, noon, noonFactor), day);
        }

        private float ComputeAmbientStrength(float t)
        {
            float day = Math.Clamp((t - 0.25f) / 0.1f, 0f, 1f) * (1f - Math.Clamp((t - 0.7f) / 0.1f, 0f, 1f));
            return 0.04f + day * 0.22f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _controller.Update(this, (float)args.Time);
            _fpsTimer += args.Time;
            if (_fpsTimer >= 0.5)
            {
                Title = $"DevDeadly | FPS: {(int)(1.0 / args.Time)}";
                _fpsTimer = 0;
            }

            float orthoSize = 160f;
            Vector3 lightDir = Vector3.Normalize(light_Pos);
            Vector3 upVec = MathF.Abs(Vector3.Dot(lightDir, Vector3.UnitY)) > 0.97f ? Vector3.UnitX : Vector3.UnitY;
            Matrix4 lightView = Matrix4.LookAt(camera.position + lightDir * 250f, camera.position, upVec);
            Matrix4 lightProj = Matrix4.CreateOrthographicOffCenter(-orthoSize, orthoSize, -orthoSize, orthoSize, 10f, 600f);
            _lightSpaceMatrix = lightView * lightProj;

            float sunAngle = (_timeOfDay - 0.5f) * MathHelper.TwoPi;
            light_Pos = new Vector3(MathF.Sin(sunAngle), MathF.Cos(sunAngle), 0.1f) * 800f;
            Vector3 sunDir = Vector3.Normalize(light_Pos);
            Vector3 fogColor = ComputeSkyHorizonColor(_timeOfDay);
            Vector3 dynamicLightColor = ComputeLightColor(_timeOfDay);
            float ambientStr = ComputeAmbientStrength(_timeOfDay);

            GL.ClearColor(fogColor.X, fogColor.Y, fogColor.Z, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.DepthMask(false);
            GL.Disable(EnableCap.DepthTest);
            skyShader.Use();
            skyShader.SetFloat("timeOfDay", _timeOfDay);
            skyShader.SetVector3("sunDir", sunDir);
            skyShader.SetVector3("camFront", camera.front);
            skyShader.SetVector3("camRight", camera.right);
            skyShader.SetVector3("camUp", camera.up);
            skyShader.SetFloat("fovTan", MathF.Tan(MathHelper.DegreesToRadians(35.0f)));
            skyShader.SetFloat("aspectRatio", (float)width / height);
            GL.BindVertexArray(VAOSky);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);

            GL.Viewport(0, 0, ShadowMapSize, ShadowMapSize);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _shadowFBO);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            shadowShader.Use();
            shadowShader.SetMatrix4("lightSpaceMatrix", _lightSpaceMatrix);
            world.RenderAll(shadowShader);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.CullFace);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, width, height);

            Matrix4 viewCloud = camera.GetViewMatrix();
            Matrix4 projectionCloud = camera.GetProjectionMatrix();
            Vector3 elevation = new Vector3(0f, 15f, 0f);

            foreach (var pos in Draw.cloudPositions)
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
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "lightPos"), light_Pos);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "lightColor"), dynamicLightColor);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.Handle, "viewPos"), camera.position);
            lightingShader.SetFloat("fogStart", 55f);
            lightingShader.SetFloat("fogEnd", 125f);
            lightingShader.SetVector3("fogColor", fogColor);
            lightingShader.SetFloat("ambientStrength", ambientStr);

            lightingShader.SetMatrix4("lightSpaceMatrix", _lightSpaceMatrix);
            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, _shadowDepthTex);
            lightingShader.SetInt("shadowMap", 5);

            //chunk.Render(lightingShader);  //AABB Collition on
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
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }
    }
}
