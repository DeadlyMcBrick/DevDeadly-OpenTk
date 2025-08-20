using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using StbImageSharp;
using Vector3 = OpenTK.Mathematics.Vector3;
using DevDeadly.Shaders;
using System.Drawing.Imaging;
using System.Drawing;
using DevDeadly.ChunksTest;

namespace DevDeadly
{
    public class Chunk
    {
        private List<Vector3> chunkVerts;
        private List<Vector2> chunkUVs;
        private List<uint> chunkIndices;

        private int VAO;
        //Don't set the value with more than 200, otherwise ur pc will crash :)
        //TODO: disable the faces while their not being seeing
        public const int SIZE = 100;
        public const int HEIGHT = 300;
        public Vector3 position;
        private uint indexCount;
        private int chunkVertexVBO;
        private int chunkUVVBO;
        private int chunkIBO;

        private int textureID;
        public Block[,,] chunkBlocks = new Block[SIZE, HEIGHT, SIZE];
        private List<float> chunkLayers;

        public Chunk(Vector3 position)
        {
            this.position = position;

            chunkVerts = new List<Vector3>();
            chunkUVs = new List<Vector2>();
            chunkIndices = new List<uint>();
            chunkLayers = new List<float>();

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
                    float worldX = x + position.X;
                    float worldZ = z + position.Z;

                    heightmap[x, z] = SimplexNoise.Noise.CalcPixel2D((int)worldX, (int)worldZ, 0.005F);
                }
            }

            return heightmap;
        }

        public List<BoundingBox> SolidBlockAABBs = new List<BoundingBox>();
        Random rand = new Random();
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
                        if (type == BlockType.GRASS && rand.NextDouble() < 0.02)
                        {
                            GenerateTree(x, columnHeight, z);
                        }

                        if (y <= columnHeight - 13)
                        {
                            type = BlockType.LAVA;
                        }

                        if (type != BlockType.EMPTY)
                        {
                            var block = new Block(new Vector3(x, y, z), type);
                            chunkBlocks[x, y, z] = block;
                            SolidBlockAABBs.Add(block.AABB);
                        }
                        else
                        {
                            chunkBlocks[x, y, z] = null;
                        }
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
                        if (chunkBlocks[x, y, z] != null && chunkBlocks[x, y, z].type != BlockType.EMPTY)
                        {
                            // LEFT
                            if (x == 0 || chunkBlocks[x - 1, y, z] == null || chunkBlocks[x - 1, y, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);

                            // RIGHT
                            if (x == SIZE - 1 || chunkBlocks[x + 1, y, z] == null || chunkBlocks[x + 1, y, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);

                            // BOTTOM
                            if (y == 0 || chunkBlocks[x, y - 1, z] == null || chunkBlocks[x, y - 1, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);

                            // TOP
                            if (y == HEIGHT - 1 || chunkBlocks[x, y + 1, z] == null || chunkBlocks[x, y + 1, z].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);

                            // FRONT
                            if (z == SIZE - 1 || chunkBlocks[x, y, z + 1] == null || chunkBlocks[x, y, z + 1].type == BlockType.EMPTY)
                                IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);

                            // BACK
                            if (z == 0 || chunkBlocks[x, y, z - 1] == null || chunkBlocks[x, y, z - 1].type == BlockType.EMPTY)
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

            int layer = TextureData.GetLayer(block.type);
            for (int i = 0; i < 4; i++)
                chunkLayers.Add(layer);

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
        private void GenerateTree(int x, int groundY, int z)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int height = rand.Next(4, 7);

            for (int y = 0; y < height; y++)
            {
                int trunkY = groundY + y;
                if (trunkY < HEIGHT)
                    chunkBlocks[x, trunkY, z] = new Block(new Vector3(x, trunkY, z), BlockType.WOOD);
            }

            int radius = 2;
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dz = -radius; dz <= radius; dz++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int nx = x + dx;
                        int ny = groundY + height - 1 + dy;
                        int nz = z + dz;

                        if (nx >= 0 && nx < SIZE &&
                            ny >= 0 && ny < HEIGHT &&
                            nz >= 0 && nz < SIZE)
                        {
                            if (dx * dx + dy * dy + dz * dz <= radius * radius + 1)
                            {
                                if (chunkBlocks[nx, ny, nz] == null ||
                                    chunkBlocks[nx, ny, nz].type == BlockType.EMPTY)
                                {
                                    chunkBlocks[nx, ny, nz] = new Block(new Vector3(nx, ny, nz), BlockType.LEAVES);
                                }
                            }
                        }
                    }
                }
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

            // Layer buffer (atributo 2)
            int chunkLayerVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkLayers.Count * sizeof(float), chunkLayers.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);

            textureID = LoadTextureArray(new string[] {
            "ces.jpg",    
            "graves.jpg",       
            "lavas.jpg" ,     
            "plo.png",
        });

            indexCount = (uint)chunkIndices.Count;
            GL.BindVertexArray(0);

            Console.WriteLine("Vértices: " + chunkVerts.Count);
            Console.WriteLine("Índices: " + chunkIndices.Count);
            Console.WriteLine("Triángulos: " + chunkIndices.Count / 3);
        }

        public static int LoadTextureArray(string[] filePaths)
        {
            int textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, textureID);

            using (var img = new System.Drawing.Bitmap(filePaths[0]))
            {
                int width = img.Width;
                int height = img.Height;

                GL.TexImage3D(TextureTarget.Texture2DArray, 0,PixelInternalFormat.Rgba, width, height, filePaths.Length, 0,OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                PixelType.UnsignedByte, IntPtr.Zero);

                for (int i = 0; i < filePaths.Length; i++)
                {
                    using (var bmp = new System.Drawing.Bitmap(filePaths[i]))
                    {
                        if (bmp.Width != width || bmp.Height != height)
                            throw new Exception($"La textura {filePaths[i]} debe tener el mismo tamaño ({width}x{height})");

                        var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        GL.TexSubImage3D(TextureTarget.Texture2DArray, 0,
                            0, 0, i, width, height, 1,
                            OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                            PixelType.UnsignedByte, data.Scan0);

                        bmp.UnlockBits(data);
                    }
                }

                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

                float maxAnis;
                GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropy, out maxAnis);

                float desiredAnis = Math.Min(16.0f, maxAnis);
                GL.TexParameter(TextureTarget.Texture2DArray,(TextureParameterName)All.TextureMaxAnisotropyExt, desiredAnis);

            }

            return textureID;
        }



        public void Render(Shader program)
        {
            program.Use();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, textureID);
            program.SetInt("atlasArray", 0);

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

        public void Rebuild()
        {
            chunkVerts.Clear();
            chunkUVs.Clear();
            chunkIndices.Clear();
            SolidBlockAABBs.Clear();
            chunkLayers.Clear();         

            GenFaces(null); 
            BuildChunk();
        }
    }
}
