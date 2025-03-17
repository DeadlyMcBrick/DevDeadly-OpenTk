using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using StbImageSharp;

namespace DevDeadly
{
    internal class Chunk
    {
        private List<Vector3> chunkVerts;
        private List<Vector2> chunkUVs;
        private List<uint> chunkIndices;

        private int VAO;
        const int SIZE = 16;
        const int HEIGHT = 32;
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
                    heightmap[x, z] = SimplexNoise.Noise.CalcPixel2D(x, z, 0.01F);
                }
            }

            return heightmap;
        }

        public void GenBlocks(float[,] heightmap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int columnHeight = (int)(heightmap[x, z] / 10);
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
                        int numFaces = 0;

                        if (chunkBlocks[x, y, z].type != BlockType.EMPTY)
                        {
                            //Left Faces 
                            if (x > 0)
                            {
                                if (chunkBlocks[x - 1, y, z].type == BlockType.EMPTY)
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);
                                    numFaces++;
                                }
                                else
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);
                                    numFaces++;
                                }

                                //RIght Faces

                                if (x < SIZE - 1)
                                {
                                    if (chunkBlocks[x - 1, y, z].type == BlockType.EMPTY)
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);
                                        numFaces++;
                                    }
                                }

                                else
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);
                                    numFaces++;
                                }

                                //Top Faces

                                if (y < HEIGHT - 1)
                                {
                                    if (chunkBlocks[x, y + 1, z].type == BlockType.EMPTY)
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);
                                        numFaces++;
                                    }
                                }

                                else
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);
                                    numFaces++;
                                }

                                //Bottom Face
                                if (y > 0)
                                {
                                    if (chunkBlocks[x, y - 1, z].type == BlockType.EMPTY)
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);
                                        numFaces++;
                                    }
                                }

                                else
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);
                                    numFaces++;
                                }

                                //Front Face 

                                if (z < 0)
                                {
                                    if (chunkBlocks[x, y, z - 1].type == BlockType.EMPTY)
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);
                                        numFaces++;
                                    }
                                }

                                else
                                {
                                    IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);
                                    numFaces++;
                                }

                                //Back face
                                if (z > 0)
                                {
                                    if (chunkBlocks[x, y, z - 1].type == BlockType.EMPTY)
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.BACK);
                                        numFaces++;
                                    }

                                    else
                                    {
                                        IntegrateFace(chunkBlocks[x, y, z], Faces.BACK);
                                        numFaces++;
                                    }
                                }


                                AddIndices(numFaces);
                            }
                        }
                    }
                }
            }
        }

        public void IntegrateFace(Block block, Faces face)
        {
            var faceData = block.GetFace(face);
            chunkVerts.AddRange(faceData.vertices);
            chunkUVs.AddRange(faceData.uv);
            Console.WriteLine("Integrating face: " + face.ToString());
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

            chunkVertexVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(chunkVerts.Count * Vector3.SizeInBytes), chunkVerts.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            chunkUVVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkUVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(chunkUVs.Count * Vector2.SizeInBytes), chunkUVs.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            chunkIBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(chunkIndices.Count * sizeof(uint)), chunkIndices.ToArray(), BufferUsageHint.StaticDraw);

            textureID = LoadTexture("atlas.png");

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
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
    }
}
