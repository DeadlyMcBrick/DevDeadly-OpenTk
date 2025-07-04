using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using DevDeadly;

namespace DevDeadly
{ }

public class ShaderLamp
{
    public int Handle2;
    private bool disposedValue = false;
    public int programID;
    int lampProgram;
    ShaderLamp shaderLamp;
    private Stopwatch timer = Stopwatch.StartNew();

    public ShaderLamp (string LampVert, string LampFrags)
    {
        int vertexHUD = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexHUD, LampVert);
        CompileShader(vertexHUD);
        int fragmentHUD = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentHUD, LampFrags);
        CompileShader(fragmentHUD);

        Handle2 = GL.CreateProgram();
        GL.AttachShader(Handle2, vertexHUD);
        GL.AttachShader(Handle2, fragmentHUD);
        GL.LinkProgram(Handle2);

        //Debuggin shit
        GL.GetProgram(Handle2, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle2);
            Console.WriteLine("Program link error: " + infoLog);
        }

        GL.DeleteShader(vertexHUD);
        GL.DeleteShader(fragmentHUD);
    }

    private void CompileShader(int lampShader)
    {
        GL.CompileShader(lampShader);
        GL.GetShader(lampShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(lampShader);
            Console.WriteLine("Shader compile error: " + infoLog);
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle2);
    }

    public void Dispose()
    {
        if (disposedValue)
            return;

        GL.DeleteProgram(Handle2);
        disposedValue = true;
        GC.SuppressFinalize(this);
    }

    public int GetAttribLocation(string name)
    {
        return GL.GetAttribLocation(Handle2, name);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle2, name);
        GL.Uniform1(location, value);
    }



    //public void VertLocation(string name, int value)
    //{
    //    int vertexColorLocation = GL.GetUniformLocation(shaderUI.Handle, "ourColor");
    //    if (vertexColorLocation == -1)
    //    {
    //        //I'll try the no string way to fix the frag/vert...
    //    }
    //    else
    //    {
    //        double timeValue = timer.Elapsed.TotalSeconds;
    //        float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
    //        GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
    //    }
    //}


    internal void SetMatrix(string name, Matrix4 matrix)
    {
        // Obtener la ubicación del uniforme en el shader

        int location = GL.GetUniformLocation(Handle2, name);
        if (location == -1)
        {
            Console.WriteLine($"⚠️ Uniform '{name}' no encontrado en el shader.");
        }

        //    // Verificar si el uniforme existe en el shader
        //    if (location != -1)
        //    {
        //        // Pasar la matriz al shader
        //        GL.UniformMatrix4(location, false, ref matrix);
        //    }
        //    else
        //    {
        //        // Si no se encuentra el uniforme, mostrar un mensaje de error
        //        Console.WriteLine($"Uniform '{name}' no encontrado en el shader.");
        //    }
    }

    //Destructor to detect potential memory leaks. Well for my CPU performance
    ~ShaderLamp()
    {
        if (!disposedValue)
        {
            Console.WriteLine("GPU Resource Leak, Yoo man! Did I forget to call Dispose?");
        }
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        GL.UseProgram(lampProgram);
        int location = GL.GetUniformLocation(Handle2, name);
        GL.UniformMatrix4(location, true, ref matrix);
    }
    public void SetBool(string name, bool value)
    {
        GL.Uniform1(GL.GetUniformLocation(Handle2, name), value ? 1 : 0);
    }


    public void SetVector3(string name, Vector3 vector)
    {
        int location = GL.GetUniformLocation(Handle2, name);
        if (location != -1)
        {
            GL.Uniform3(location, vector);
        }
        else
        {
            Console.WriteLine($"Uniform '{name}' no encontrado en el shader.");
        }
    }
}