using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;
using ImGuiNET;

namespace DevDeadly
{
    public class Game : GameWindow
    {
        private Stopwatch timer = Stopwatch.StartNew();
        public Shader shader;
        public Shader lightingShader;

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

        uniform vec3 objectColor;
        uniform vec3 lightColor;

        void main()
        {
            FragColor = vec4(1.0);
            FragColor = vec4(lightColor * objectColor, 1.0);
        }";

        //   in vec2 texCoord;

        //uniform sampler2D texture0;
        //uniform sampler2D texture1;
        //// Mezclar las texturas
        //vec4 color1 = texture(texture0, texCoord);
        //vec4 color2 = texture(texture1, texCoord);
        //FragColor = mix(color1, color2, 0.4);


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

        private readonly Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);


        private readonly uint[] indices =
        {
             0, 1, 3,  // Primer triángulo (superior derecho)
             1, 2, 3   // Segundo triángulo (inferior izquierdo)
        };

        //do the twiceShaders connect it to one
        private readonly float[] texCoords = {
        0.0f, 0.0f,  // lower-left corner  
        1.0f, 0.0f,  // lower-right corner
        0.5f, 1.0f   // top-center corner
        };

        int VertexArrayObject;
        int VertexBufferObject;
        int ElementBufferObject;
        int nrAttribute = 0;
        int width, height;
        Camera camera;
        int resultado;

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

        //Keybinds (Being able to drop some menu to being able to change in the future idk)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            base.OnUpdateFrame(e);
            camera.Update(input, mouse, e);    

            //Keybinds to detect if this is actually is being pressed...
            if (KeyboardState.IsKeyDown(Keys.W)) Console.WriteLine("W pressed");
            if (KeyboardState.IsKeyDown(Keys.A)) Console.WriteLine("A pressed");
            if (KeyboardState.IsKeyDown(Keys.S)) Console.WriteLine("S pressed");
            if (KeyboardState.IsKeyDown(Keys.D)) Console.WriteLine("D pressed");
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

            ImGui.CreateContext();
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            ImGui.GetIO().Fonts.AddFontDefault();  // Default font for ImGui
            ImGui.StyleColorsDark();  // O

            VertexArrayObject = GL.GenVertexArray();
            VertexBufferObject = GL.GenBuffer();
            ElementBufferObject = GL.GenBuffer();

            //String path
            shader = new Shader(vertexShaderSource, fragmentShaderSource);

            string imagePath = "Images/container.jpg";
            texture = new Texture(imagePath);
            texture.Use(TextureUnit.Texture0);

            string imagePath2 = "Images/container.jpg";
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
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            timer = Stopwatch.StartNew();

            GL.BindVertexArray(VertexArrayObject);

            //Drawing shit
            int positionLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;

            //Structure all the info for the buffer and stuff bla....
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.GetInteger(GetPName.MaxVertexAttribs, out nrAttribute);

            //ImGui.End();
            //ImGui.Render();

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
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            //Change this thing to the Shader.cs
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            if (vertexColorLocation == -1)
            {
                //I'll try the no string way to fix the frag/vert...
            }
            else
            {
                double timeValue = timer.Elapsed.TotalSeconds;
                float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
                GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
            }
            //Cube Model and rotation
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            
            //Lamp configuration 
            Matrix4 LampMatrix = Matrix4.Identity;

            //Scale the size of the box (Not working with negatives for some reason)
            LampMatrix *= Matrix4.CreateScale(0.2f);
            LampMatrix = Matrix4.CreateTranslation(lightPos);

            //Creation of the lights for the shadows blocks
            shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));

            //Color projection 
            Color4 coral = new Color4(1.0f, 0.5f, 0.31f, 1.0f);
            Color4 lightColor = new Color4(0.33f, 0.42f, 0.18f, 1.0f);
            Color4 toyColor = new Color4(1.0f, 0.5f, 0.31f, 1.0f);
            Color4 result = lightColor * toyColor;


            //Ilumination position
            LampMatrix = Matrix4.CreateRotationY(yRot) * Matrix4.CreateTranslation(2f, 4f, -5f);
            yRot += 0.0001f;

            //model = Matrix4.CreateRotationZ(zRot) * Matrix4.CreateTranslation(0f, 0f, -5f);
            //zRot += 0.001f;

            //Define here but not as shaderProgram (if it is possible to use shaderProgram should be the correct way), cause otherwise is not going to work.
            int modelLocation = GL.GetUniformLocation(shader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(shader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(shader.Handle, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            GL.UseProgram(shaderProgram);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(VertexArrayObject);

            shader.Use();

            //If I wanna render 2 images dont use shader here.
            shader.Use();

            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);

            GL.Enable(EnableCap.DepthTest);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            //White color
            shader.SetVector3("objectColor", new Vector3(1.0f, 1.0f, 1.0f)); 
            shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f)); 

            model += Matrix4.CreateTranslation(new Vector3(2f, 0f, 1f));
            GL.UniformMatrix4(modelLocation, true, ref LampMatrix);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            

            Context.SwapBuffers();

        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
