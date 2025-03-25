//using OpenTK.Audio.OpenAL;
//using System;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Windowing.GraphicsLibraryFramework;
//using System.Diagnostics;
//using OpenTK.Mathematics;
//using Vector3 = OpenTK.Mathematics.Vector3;
//using ImGuiNET;
//using System.IO;
//using static OpenTK.Audio.OpenAL.AL; // Agregado
//using static OpenTK.Audio.OpenAL.Alc; // Agregado

//public class AudioPlayer
//{
//    private int device;
//    private int context;
//    private int buffer;
//    private int source;

//    public void Initialize()
//    {
//        device = OpenDevice(null); 
//        context = CreateContext(device, (int[])null); 
//        MakeContextCurrent(context); 

//        buffer = GenBuffer(); 
//        source = GenSource(); 
//    }

//    public void LoadAudio(string filePath)
//    {
//        byte[] audioData = File.ReadAllBytes(filePath);
//        int format = (int)ALFormat.Mono16;
//        int frequency = 44100;

//        BufferData(buffer, (ALFormat)format, audioData, frequency); // Corregido
//        Source(source, ALSourcei.Buffer, buffer); // Corregido
//    }

//    public void Play()
//    {
//        SourcePlay(source); // Corregido
//    }

//    public void SetListenerPosition(Vector3 position)
//    {
//        Listener(ALListener3f.Position, position.X, position.Y, position.Z); // Corregido
//    }

//    public void Cleanup()
//    {
//        DeleteSource(source); // Corregido
//        DeleteBuffer(buffer); // Corregido
//        MakeContextCurrent(0); // Corregido
//        DestroyContext(context); // Corregido
//        CloseDevice(device); // Corregido
//    }
//}





