﻿//using System;
//using System.Collections.Generic;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Mathematics;
//using StbImageSharp;

//namespace DevDeadly
//{
//    internal class Chunk
//    {
//        private List<Vector3> chunkVerts;
//        private List<Vector2> chunkUVs;
//        private List<uint> chunkIndices;

//        private int VAO;
//        const int SIZE = 16;
//        const int HEIGHT = 32;
//        public Vector3 position;

//        private uint indexCount;

//        private int chunkVertexVBO;
//        private int chunkUVVBO;
//        private int chunkIBO;

//        private int textureID;

//        public Chunk(Vector3 position)
//        {
//            this.position = position;

//            chunkVerts = new List<Vector3>();
//            chunkUVs = new List<Vector2>();
//            chunkIndices = new List<uint>();

//            GenBlocks();
//            BuildChunk();
//        }

//        public void GenBlocks()
//        {
//           //Create the blocks chunks
//            for (int i = 0; i < SIZE; i++)
//            {
//                Block block = new Block(new Vector3(i, 0, 0));

//                var frontFaceData = block.GetFace(Faces.FRONT);
//                chunkVerts.AddRange(frontFaceData.vertices);
//                chunkUVs.AddRange(frontFaceData.uv);
//                AddIndices(1);
//            }
//        }

//        public void AddIndices(int amtFaces)
//        {
//            for (int i = 0; i < amtFaces; i++)
//            {
//                chunkIndices.Add(indexCount);
//                chunkIndices.Add(indexCount + 1);
//                chunkIndices.Add(indexCount + 2);
//                chunkIndices.Add(indexCount + 2);
//                chunkIndices.Add(indexCount + 3);
//                chunkIndices.Add(indexCount);

//                indexCount += 4;
//            }
//        }

//        public void BuildChunk()
//        {
            
//            VAO = GL.GenVertexArray();
//            GL.BindVertexArray(VAO);
          
//            chunkVertexVBO = GL.GenBuffer();
//            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVertexVBO);
//            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(chunkVerts.Count * Vector3.SizeInBytes), chunkVerts.ToArray(), BufferUsageHint.StaticDraw);
//            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
//            GL.EnableVertexAttribArray(0);

//            chunkUVVBO = GL.GenBuffer();
//            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkUVVBO);
//            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(chunkUVs.Count * Vector2.SizeInBytes), chunkUVs.ToArray(), BufferUsageHint.StaticDraw);
//            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
//            GL.EnableVertexAttribArray(1);

//            chunkIBO = GL.GenBuffer();
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
//            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(chunkIndices.Count * sizeof(uint)), chunkIndices.ToArray(), BufferUsageHint.StaticDraw);

//            textureID = LoadTexture("Tierra minecraft.jpg");

//            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
//            GL.BindVertexArray(0);
//        }

//        public int LoadTexture(string path)
//        {
           
//            StbImage.stbi_set_flip_vertically_on_load(1);  
//            using (var stream = File.OpenRead(path))
//            {
//                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

//                if (image == null || image.Data == null)
//                    throw new Exception("Can't load the image");

//                int texture = GL.GenTexture();
//                GL.BindTexture(TextureTarget.Texture2D, texture);
//                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
//                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
//                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
//                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

//                return texture;
//            }
//        }


//        public void Render(Shader program)
//        {
//            program.Use();

//            GL.BindVertexArray(VAO);
//            GL.BindTexture(TextureTarget.Texture2D, textureID);

//            // Drawing shit
//            GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
//        }

//        public void Delete()
//        {
//            GL.DeleteVertexArray(VAO);     
//            GL.DeleteBuffer(chunkVertexVBO); 
//            GL.DeleteBuffer(chunkUVVBO);    
//            GL.DeleteBuffer(chunkIBO);     
//            GL.DeleteTexture(textureID);    
//        }
//    }
//}
