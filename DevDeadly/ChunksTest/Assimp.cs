using System;
using Assimp;
using Assimp.Configs;
using DevDeadly.Shaders;
using ImGuiNET;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Reflection;
using Vector3 = OpenTK.Mathematics.Vector3;
using GLPrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

public class Model
{
    ItemObject itemObject;
    Camera camera;
    private int VAOItem;
    private int VBOItem;
    private int EBOItem;

    public List<float> Vertices { get; private set; } = new();
    public List<float> TexCoords { get; private set; } = new();
    public List<float> Normals { get; private set; } = new();
    public List<uint> Indices { get; private set; } = new();
    public string TexturePath { get; private set; }
    public Model(string path)
    {
        LoadModel(path);
        SetupMesh();
    }

    private void LoadModel(string path)
    {
        var ObjImporter = new AssimpContext();
        var scene = ObjImporter.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

        if (scene == null || scene.MeshCount == 0)
        {
            throw new IOException("No mesh found");
        }

        var mesh = scene.Meshes[0];

        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            var v = mesh.Vertices[i];
            Vertices.Add(v.X);
            Vertices.Add(v.Y);
            Vertices.Add(v.Z);
            Console.WriteLine($"Vertices {i}: {v.X}, {v.Y}, {v.Z}");
        }

        if (mesh.TextureCoordinateChannels[0].Count > 0)
        {
            foreach (var t in mesh.TextureCoordinateChannels[0])
            {
                TexCoords.Add(t.X);
                TexCoords.Add(t.Y);
            }
        }

        foreach (var n in mesh.Normals)
        {
            Normals.Add(n.X);
            Normals.Add(n.Y);
            Normals.Add(n.Z);
        }

        foreach (var face in mesh.Faces)
        {
            foreach (var index in face.Indices)
                Indices.Add((uint)index);
        }

        if (scene.MaterialCount > 0)
        {
            var material = scene.Materials[mesh.MaterialIndex];
            var texSlot = material.TextureDiffuse;
            if (texSlot.FilePath != null)
            {
                TexturePath = Path.Combine(Path.GetDirectoryName(path), texSlot.FilePath);
            }
        }
    }

    private void SetupMesh()
    {
        int vertexCount = Vertices.Count / 3;
        int stride = 8; // 3 pos + 3 normal + 2 uv
        float[] vertexData = new float[vertexCount * stride];

        for (int i = 0; i < vertexCount; i++)
        {
            vertexData[i * stride + 0] = Vertices[i * 3 + 0]; // X
            vertexData[i * stride + 1] = Vertices[i * 3 + 1]; // Y
            vertexData[i * stride + 2] = Vertices[i * 3 + 2]; // Z

            vertexData[i * stride + 3] = Normals.Count > i * 3 ? Normals[i * 3 + 0] : 0;
            vertexData[i * stride + 4] = Normals.Count > i * 3 ? Normals[i * 3 + 1] : 0;
            vertexData[i * stride + 5] = Normals.Count > i * 3 ? Normals[i * 3 + 2] : 0;

            vertexData[i * stride + 6] = TexCoords.Count > i * 2 ? TexCoords[i * 2 + 0] : 0;
            vertexData[i * stride + 7] = TexCoords.Count > i * 2 ? TexCoords[i * 2 + 1] : 0;
        }

        VAOItem = GL.GenVertexArray();
        VBOItem = GL.GenBuffer();
        EBOItem = GL.GenBuffer();

        GL.BindVertexArray(VAOItem);

        // Vertex buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOItem);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

        // Element buffer
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOItem);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);

        // Atributo: posición (location = 0)
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0);

        // Atributo: normales (location = 1)

        // Atributo: UV (location = 2)
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 6 * sizeof(float));
        GL.BindVertexArray(0);
    }

    public void Draw()

    {
        GL.BindVertexArray(VAOItem);
        GL.UseProgram(itemObject.HandleItem);
        GL.DrawElements(GLPrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
}