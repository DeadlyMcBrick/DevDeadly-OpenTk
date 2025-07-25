using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using StbImageSharp;
using Vector3 = OpenTK.Mathematics.Vector3;
using DevDeadly.Shaders;

namespace DevDeadly
{
    internal class Chunk
    {
        private List<Vector3> chunkVerts;
        private List<Vector2> chunkUVs;
        private List<uint> chunkIndices;

        private int VAO;
        //Don't set the value with more than 200, otherwise ur pc will crash :)
        //TODO: disable the faces while their not being seeing
        const int SIZE = 50;
        const int HEIGHT = 50;
        public Vector3 position;
        private uint indexCount;
        private int chunkVertexVBO;
        private int chunkUVVBO;
        private int chunkIBO;

        private int textureID;
        Block[,,] chunkBlocks = new Block[SIZE, HEIGHT, SIZE];

        public Chunk(Vector3 position)
        {
            this.position = position;

            chunkVerts = new List<Vector3>();
            chunkUVs = new List<Vector2>();
            chunkIndices = new List<uint>();

            float[,] heightmap = GetChunk();
            GenBlocks(heightmap);
            GenFaces(heightmap);
            BuildChunk();
        }

        public float[,] GetChunk()
        {
            float[,] heightmap = new float[SIZE, SIZE];

            SimplexNoise.Noise.Seed = 123456;
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    heightmap[x, z] = SimplexNoise.Noise.CalcPixel2D(x, z, 0.005F);
                }
            }

            return heightmap;
        }

        public List<BoundingBox> SolidBlockAABBs = new List<BoundingBox>();
        public void GenBlocks(float[,] heightmap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int columnHeight = (int)(heightmap[x, z] /10);
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        BlockType type = BlockType.EMPTY;
                        if (y < columnHeight - 1)
                        {
                            type = BlockType.DIRT;
                        }
                        if (y == columnHeight - 1)
                        {
                            type = BlockType.GRASS;
                        }

                        if (type != BlockType.EMPTY)
                        {
                            var block = new Block(new Vector3(x, y, z), type);
                            chunkBlocks[x, y, z] = block;
                            SolidBlockAABBs.Add(block.AABB);
                        }

                        chunkBlocks[x, y, z] = new Block(new Vector3(x, y, z), type);
                    }
                }
            }
        }

        public void GenFaces(float[,] heightmap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        if (chunkBlocks[x, y, z].type != BlockType.EMPTY)
                        {
                            // LEFT
                            if (x == 0 || chunkBlocks[x - 1, y, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);

                            // RIGHT
                            if (x == SIZE - 1 || chunkBlocks[x + 1, y, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);

                            // BOTTOM
                            if (y == 0 || chunkBlocks[x, y - 1, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);

                            // TOP
                            if (y == HEIGHT - 1 || chunkBlocks[x, y + 1, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);

                            // FRONT
                            if (z == SIZE - 1 || chunkBlocks[x, y, z + 1].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);

                            // BACK
                            if (z == 0 || chunkBlocks[x, y, z - 1].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.BACK);
                        }
                    }
                }
            }
        }

        private void IntegrateFace(Block block, Faces face)
        {
            FaceData data = block.GetFace(face);
            foreach (var v in data.vertices)
                chunkVerts.Add(v);
            foreach (var uv in data.uv)
                chunkUVs.Add(uv);

            int baseIndex = chunkVerts.Count - data.vertices.Count;
            for (int i = 0; i < data.vertices.Count; i += 4)
            {
                chunkIndices.Add((uint)(baseIndex + 0));
                chunkIndices.Add((uint)(baseIndex + 1));
                chunkIndices.Add((uint)(baseIndex + 2));
                chunkIndices.Add((uint)(baseIndex + 2));
                chunkIndices.Add((uint)(baseIndex + 3));
                chunkIndices.Add((uint)(baseIndex + 0));
            }
        }


        //Create the blocks chunks
        public void AddIndices(int amtFaces)
        {
            for (int i = 0; i < amtFaces; i++)
            {
                chunkIndices.Add(0 + indexCount);
                chunkIndices.Add(1 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(3 + indexCount);
                chunkIndices.Add(0 + indexCount);
                indexCount += 4;
            }
        }

        public void BuildChunk()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Vertex buffer
            chunkVertexVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkVerts.Count * Vector3.SizeInBytes, chunkVerts.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            // UV buffer
            chunkUVVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkUVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkUVs.Count * Vector2.SizeInBytes, chunkUVs.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            // Index buffer
            chunkIBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, chunkIndices.Count * sizeof(uint), chunkIndices.ToArray(), BufferUsageHint.StaticDraw);

            textureID = LoadTexture("atlas.png");
            indexCount = (uint)chunkIndices.Count;


            GL.BindVertexArray(0);

            Console.WriteLine("Vértices: " + chunkVerts.Count);
            Console.WriteLine("Índices: " + chunkIndices.Count);
            Console.WriteLine("Triángulos: " + chunkIndices.Count / 3);

        }


        public int LoadTexture(string path)
        {

            StbImage.stbi_set_flip_vertically_on_load(1);
            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                if (image == null || image.Data == null)
                    throw new Exception("Can't load the image");

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                return texture;
            }
        }

        public void Render(Shader program)
        {
            program.Use();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Drawing shit
            GL.Enable(EnableCap.DepthTest);
            GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(chunkVertexVBO);
            GL.DeleteBuffer(chunkUVVBO);
            GL.DeleteBuffer(chunkIBO);
            GL.DeleteTexture(textureID);
        }

        public void DrawHUD()
        {
        }
    }
}
