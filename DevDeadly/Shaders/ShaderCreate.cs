using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using DevDeadly;

namespace DevDeadly.Shaders
{
    public class Creation
    {
        public int Handle4;
        private bool disposedValue = false;
        public int programID;
        Creation creation;
        private Stopwatch timer = Stopwatch.StartNew();

        public Creation(string CreationVerts, string CreationFrags)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, CreationVerts);
            CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, CreationFrags);
            CompileShader(fragmentShader);


            Handle4 = GL.CreateProgram();
            GL.AttachShader(Handle4, vertexShader);
            GL.AttachShader(Handle4, fragmentShader);
            GL.LinkProgram(Handle4);

            //Debuggin shit
            GL.GetProgram(Handle4, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle4);
                Console.WriteLine("Program link error: " + infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine("Shader compile error: " + infoLog);
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle4);
        }

        public void Dispose()
        {
            if (disposedValue)
                return;

            GL.DeleteProgram(Handle4);
            disposedValue = true;
            GC.SuppressFinalize(this);
        }

        public int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(Handle4, name);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle4, name);
            GL.Uniform1(location, value);


        }

        public void VertLocation(string name, int value)
        {
            int vertexColorLocation = GL.GetUniformLocation(creation.Handle4, "aCroods");
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
        }

        public void SetVector2(string name, Vector2 vector)
        {
            int location = GL.GetUniformLocation(Handle4, name);
            if (location == -1)
            {
                Console.WriteLine($"Uniform '{name}' not found or unused in shader.");
            }
            else
            {
                GL.Uniform2(location, vector);
            }
        }


        //internal void SetMatrix(string name, Matrix4 matrix)
        //{
        //    // Obtener la ubicación del uniforme en el shader
        //    int location = GL.GetUniformLocation(Handle, name);

        ////    // Verificar si el uniforme existe en el shader
        ////    if (location != -1)
        ////    {
        ////        // Pasar la matriz al shader
        ////        GL.UniformMatrix4(location, false, ref matrix);
        ////    }
        ////    else
        ////    {
        ////        // Si no se encuentra el uniforme, mostrar un mensaje de error
        ////        Console.WriteLine($"Uniform '{name}' no encontrado en el shader.");
        ////    }
        //}

        //Destructor to detect potential memory leaks. Well for my CPU performance
        ~Creation()
        {
            if (!disposedValue)
            {
                Console.WriteLine("GPU Resource Leak, Yoo man! Did I forget to call Dispose?");
            }
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            GL.UseProgram(Handle4);
            int location = GL.GetUniformLocation(Handle4, name);
            GL.UniformMatrix4(location, true, ref matrix);
            int loc = GL.GetUniformLocation(Handle4, "objectColor");
            //Console.WriteLine($"objectColor location: {loc}");

        }
        public void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(Handle4, name), value ? 1 : 0);
        }


        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle4, name);
            if (location != -1)
            {
                GL.Uniform3(location, vector);
            }
            else
            {
                Console.WriteLine($"Uniform '{name}' is not being found.");
            }
        }
    }
}