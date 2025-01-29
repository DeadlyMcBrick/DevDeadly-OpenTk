using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using OpenTK.Mathematics;
using NumericsVector3 = System.Numerics.Vector3;
using OpenTKVector3 = OpenTK.Mathematics.Vector3;
using System.Reflection;


namespace DevDeadly
{
    public class Game : GameWindow
    {
        private OpenTKVector3 position = new OpenTKVector3(0.0f, 0.0f, 3.0f);
        private OpenTKVector3 front = new OpenTKVector3(0.0f, 0.0f, -1.0f);
        private OpenTKVector3 up = OpenTKVector3.UnitY;

        private OpenTKVector3 cameraRight;
        private OpenTKVector3 cameraUp;


        //private CameraSettings.Settings cameraSettings;
        private Stopwatch timer = Stopwatch.StartNew();
        public Shader shader;


        public const float speed = 0.100f;

        //int nextTime ups= 0; 


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
            texCoord = aTexCoord;
         gl_Position = vec4(aPosition, 1.0) * model * view * projection;
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

        // ROTATION Y
        float yRot;

        //ROTATION X
        float xRot;


        public Texture texture;
        public Texture texture2;

        public Matrix4 model;
        public Matrix4 view;
        public Matrix4 projection;

        // Configuration for the window
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (width, height),
            Title = title
        })
        { }

        //Keybinds (Being able to drop some menu to being able to change in the future idk)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //Movements make sure to change them

            if (!IsFocused)
            {
                return;
            }
            //being able to change the spped to by a OpenGL settings
            KeyboardState input = KeyboardState;
            float delta = (float)e.Time;

            if (input.IsKeyDown(Keys.W))
                position += front * speed * delta;

            if (input.IsKeyDown(Keys.S))
                position -= front * speed * delta;

            if (input.IsKeyDown(Keys.A))
                position -= cameraRight * speed * delta;

            if (input.IsKeyDown(Keys.D))
                position += cameraRight * speed * delta;

            if (input.IsKeyDown(Keys.Space))
                position += up * speed * delta;

            if (input.IsKeyDown(Keys.LeftShift))
                position -= up * speed * delta;

            if (input.IsKeyDown(Keys.K))
                position += up * speed * delta;

            if (input.IsKeyDown(Keys.C))
                Console.Write("");

            //Matrix4 view = Matrix4.LookAt(position, position + front, up);
            //shader.SetMatrix("view", view);

            //Keybinds to detect if this is actually is being pressed...
            if (KeyboardState.IsKeyDown(Keys.W)) Console.WriteLine("W pressed");
            if (KeyboardState.IsKeyDown(Keys.A)) Console.WriteLine("A pressed");
            if (KeyboardState.IsKeyDown(Keys.S)) Console.WriteLine("S pressed");
            if (KeyboardState.IsKeyDown(Keys.D)) Console.WriteLine("D pressed");
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if(KeyboardState.IsKeyDown(Keys.F))
            {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

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

            //wraps
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //separate to the other buffer and shader info...
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            timer = Stopwatch.StartNew();

            //Drawing shit
            GL.BindVertexArray(VertexArrayObject);

            int positionLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);


            //Structure all the info for the buffer and stuff bla....
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
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


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

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.Identity;                                                            // width / height
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float) 90.1f / (float) 90.1f, 0.9f, 100.0f);

            model = Matrix4.CreateRotationY(yRot);
            yRot += 0.001f;

            view = Matrix4.CreateRotationX(xRot);
            xRot += 0.001f;

            //view = Matrix4.CreateRotationX(xRot);
            //xRot += 0.01f;

           //Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);
           // model *= translation;

            //define here but not as shaderProgram, cause otherwise is not going to work.
            int modelLocation = GL.GetUniformLocation(shader.Handle, "model");
            int viewLocation = GL.GetUniformLocation(shader.Handle, "view");
            int projectionLocation = GL.GetUniformLocation(shader.Handle, "projection");

            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(viewLocation, false, ref view);
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.UseProgram(shaderProgram);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(VertexArrayObject);

            //If I wanna render 2 images dont use shader here.

            shader.Use();

            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);

            GL.Enable(EnableCap.DepthTest);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
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
