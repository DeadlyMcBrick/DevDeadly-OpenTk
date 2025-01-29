using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace DevDeadly
{
    public class Shader
    {
        public int Handle;
        private bool disposedValue = false;
        public int programID;
        int shaderProgram;

        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            //Debuggin shit
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
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
            GL.UseProgram(Handle);
        }

        public void Dispose()
        {
            if (disposedValue)
                return;

            GL.DeleteProgram(Handle);
            disposedValue = true;
            GC.SuppressFinalize(this);
        }

        public void GetAttribLocation()
        {
            Console.WriteLine("Is this fucking shit working now??");
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, value);
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
        ~Shader()
        {
            if (!disposedValue)
            {
                Console.WriteLine("GPU Resource Leak, Yoo man! Did I forget to call Dispose?");
            }
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            GL.UseProgram(shaderProgram);
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, true, ref matrix);
        }


    }
}