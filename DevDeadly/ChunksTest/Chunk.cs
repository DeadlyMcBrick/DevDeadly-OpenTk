using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using DevDeadly.Shaders;
using DevDeadly.ChunksTest;

namespace DevDeadly
{
    public class Chunk
    {
        private List<Vector3> chunkVerts;
        private List<Vector2> chunkUVs;
        private List<uint> chunkIndices;
        private List<float> chunkLayers;
        private List<Vector3> chunkNormals;
        private List<float> chunkAO;

        private int VAO;
        private int chunkVertexVBO;
        private int chunkUVVBO;
        private int chunkIBO;
        private int textureID;
        private uint indexCount;

        public const int SIZE = 70;
        public const int HEIGHT = 70;
        public Vector3 position;
        public Block[,,] chunkBlocks = new Block[SIZE, HEIGHT, SIZE];
        public List<BoundingBox> SolidBlockAABBs = new List<BoundingBox>();

        Random rand = new Random();

        public Chunk(Vector3 position)
        {
            this.position = position;
            chunkVerts = new List<Vector3>();
            chunkUVs = new List<Vector2>();
            chunkIndices = new List<uint>();
            chunkLayers = new List<float>();
            chunkNormals = new List<Vector3>();
            chunkAO = new List<float>();
            float[,] heightmap = GetChunk();
            GenBlocks(heightmap);
            GenFaces();
            BuildChunk();
        }

        private float[,] GetChunk()
        {
            float[,] heightmap = new float[SIZE, SIZE];
            SimplexNoise.Noise.Seed = 123456;
            for (int x = 0; x < SIZE; x++)
                for (int z = 0; z < SIZE; z++)
                {
                    float worldX = x + position.X;
                    float worldZ = z + position.Z;
                    heightmap[x, z] = SimplexNoise.Noise.CalcPixel2D((int)worldX, (int)worldZ, 0.005F);
                }
            return heightmap;
        }

        private void GenBlocks(float[,] heightmap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int columnHeight = (int)(heightmap[x, z] / 10);
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        BlockType type = BlockType.EMPTY;
                        if (y < columnHeight - 1) type = BlockType.DIRT;
                        if (y == columnHeight - 1) type = BlockType.GRASS;
                        if (y <= columnHeight - 13) type = BlockType.LAVA;
                        if (type != BlockType.EMPTY)
                        {
                            var block = new Block(new Vector3(x, y, z), type);
                            chunkBlocks[x, y, z] = block;
                            SolidBlockAABBs.Add(block.AABB);
                        }
                        else
                            chunkBlocks[x, y, z] = null;
                    }
                    if (columnHeight > 0 && columnHeight < HEIGHT - 8 && rand.NextDouble() < 0.02)
                        GenerateTree(x, columnHeight, z);
                }
            }
        }

        private void GenerateTree(int x, int groundY, int z)
        {
            int trunkHeight = rand.Next(5, 8);

            for (int y = 0; y < trunkHeight; y++)
            {
                int ty = groundY + y;
                if (ty < HEIGHT)
                    SetBlock(x, ty, z, BlockType.WOOD);
            }

            int topY = groundY + trunkHeight;
            int leafRadius = 3;

            for (int dx = -leafRadius; dx <= leafRadius; dx++)
            {
                for (int dz = -leafRadius; dz <= leafRadius; dz++)
                {
                    for (int dy = -1; dy <= 3; dy++)
                    {
                        int nx = x + dx;
                        int ny = topY + dy;
                        int nz = z + dz;

                        if (nx < 0 || nx >= SIZE || ny < 0 || ny >= HEIGHT || nz < 0 || nz >= SIZE) continue;

                        float dist = MathF.Sqrt(dx * dx + dy * dy * 2 + dz * dz);
                        if (dist <= leafRadius && chunkBlocks[nx, ny, nz] == null)
                            SetBlock(nx, ny, nz, BlockType.LEAVES);
                    }
                }
            }

            for (int dy = 0; dy <= 2; dy++)
            {
                int capY = topY + 3 + dy;
                int capRadius = 1 - dy;
                for (int dx = -capRadius; dx <= capRadius; dx++)
                    for (int dz = -capRadius; dz <= capRadius; dz++)
                    {
                        int nx = x + dx;
                        int nz2 = z + dz;
                        if (nx >= 0 && nx < SIZE && capY < HEIGHT && nz2 >= 0 && nz2 < SIZE && chunkBlocks[nx, capY, nz2] == null)
                            SetBlock(nx, capY, nz2, BlockType.LEAVES);
                    }
            }
        }

        private void SetBlock(int x, int y, int z, BlockType type)
        {
            var block = new Block(new Vector3(x, y, z), type);
            chunkBlocks[x, y, z] = block;
            SolidBlockAABBs.Add(block.AABB);
        }

        private void GenFaces()
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        var block = chunkBlocks[x, y, z];
                        if (block == null || block.type == BlockType.EMPTY) continue;
                        if (x == 0 || chunkBlocks[x - 1, y, z] == null) IntegrateFace(block, Faces.LEFT, x, y, z);
                        if (x == SIZE - 1 || chunkBlocks[x + 1, y, z] == null) IntegrateFace(block, Faces.RIGHT, x, y, z);
                        if (y == 0 || chunkBlocks[x, y - 1, z] == null) IntegrateFace(block, Faces.BOTTOM, x, y, z);
                        if (y == HEIGHT - 1 || chunkBlocks[x, y + 1, z] == null) IntegrateFace(block, Faces.TOP, x, y, z);
                        if (z == SIZE - 1 || chunkBlocks[x, y, z + 1] == null) IntegrateFace(block, Faces.FRONT, x, y, z);
                        if (z == 0 || chunkBlocks[x, y, z - 1] == null) IntegrateFace(block, Faces.BACK, x, y, z);
                    }
                }
            }
        }

        private static readonly Dictionary<Faces, (Vector3i s1, Vector3i s2, Vector3i corner)[]> aoOffsets =
        new Dictionary<Faces, (Vector3i, Vector3i, Vector3i)[]>
{
            { Faces.FRONT, new[] {
                (new Vector3i(-1, 0, 1),  new Vector3i( 0,  1, 1),  new Vector3i(-1,  1, 1)),
                (new Vector3i( 1, 0, 1),  new Vector3i( 0,  1, 1),  new Vector3i( 1,  1, 1)),
                (new Vector3i( 1, 0, 1),  new Vector3i( 0, -1, 1),  new Vector3i( 1, -1, 1)),
                (new Vector3i(-1, 0, 1),  new Vector3i( 0, -1, 1),  new Vector3i(-1, -1, 1)),
            }},
            { Faces.BACK, new[] {
                (new Vector3i( 1, 0, -1), new Vector3i( 0,  1, -1), new Vector3i( 1,  1, -1)),
                (new Vector3i(-1, 0, -1), new Vector3i( 0,  1, -1), new Vector3i(-1,  1, -1)),
                (new Vector3i(-1, 0, -1), new Vector3i( 0, -1, -1), new Vector3i(-1, -1, -1)),
                (new Vector3i( 1, 0, -1), new Vector3i( 0, -1, -1), new Vector3i( 1, -1, -1)),
            }},
            { Faces.LEFT, new[] {
                (new Vector3i(-1, 0, -1), new Vector3i(-1,  1, 0),  new Vector3i(-1,  1, -1)),
                (new Vector3i(-1, 0,  1), new Vector3i(-1,  1, 0),  new Vector3i(-1,  1,  1)),
                (new Vector3i(-1, 0,  1), new Vector3i(-1, -1, 0),  new Vector3i(-1, -1,  1)),
                (new Vector3i(-1, 0, -1), new Vector3i(-1, -1, 0),  new Vector3i(-1, -1, -1)),
            }},
            { Faces.RIGHT, new[] {
                (new Vector3i(1, 0,  1),  new Vector3i(1,  1, 0),   new Vector3i(1,  1,  1)),
                (new Vector3i(1, 0, -1),  new Vector3i(1,  1, 0),   new Vector3i(1,  1, -1)),
                (new Vector3i(1, 0, -1),  new Vector3i(1, -1, 0),   new Vector3i(1, -1, -1)),
                (new Vector3i(1, 0,  1),  new Vector3i(1, -1, 0),   new Vector3i(1, -1,  1)),
            }},
            { Faces.TOP, new[] {
                (new Vector3i(-1, 1, 0),  new Vector3i(0, 1, -1),   new Vector3i(-1, 1, -1)),
                (new Vector3i( 1, 1, 0),  new Vector3i(0, 1, -1),   new Vector3i( 1, 1, -1)),
                (new Vector3i( 1, 1, 0),  new Vector3i(0, 1,  1),   new Vector3i( 1, 1,  1)),
                (new Vector3i(-1, 1, 0),  new Vector3i(0, 1,  1),   new Vector3i(-1, 1,  1)),
            }},
            { Faces.BOTTOM, new[] {
                (new Vector3i(-1, -1, 0), new Vector3i(0, -1, -1),  new Vector3i(-1, -1, -1)),
                (new Vector3i( 1, -1, 0), new Vector3i(0, -1, -1),  new Vector3i( 1, -1, -1)),
                (new Vector3i( 1, -1, 0), new Vector3i(0, -1,  1),  new Vector3i( 1, -1,  1)),
                (new Vector3i(-1, -1, 0), new Vector3i(0, -1,  1),  new Vector3i(-1, -1,  1)),
            }},
        };

        private bool IsSolid(int x, int y, int z)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= HEIGHT || z < 0 || z >= SIZE) return false;
            var b = chunkBlocks[x, y, z];
            return b != null && b.type != BlockType.EMPTY;
        }

        private float ComputeAO(bool s1, bool s2, bool corner)
        {
            if (s1 && s2) return 0.5f;
            int occ = (s1 ? 1 : 0) + (s2 ? 1 : 0) + (corner ? 1 : 0);
            return 1.0f - occ * 0.17f;
        }

        private void IntegrateFace(Block block, Faces face, int bx, int by, int bz)
        {
            FaceData data = block.GetFace(face);

            chunkVerts.AddRange(data.vertices);
            chunkUVs.AddRange(data.uv);

            int layer = TextureData.GetLayer(block.type);
            for (int i = 0; i < 4; i++) chunkLayers.Add(layer);

            Vector3 normal = GetFaceNormal(face);
            for (int i = 0; i < data.vertices.Count; i++) chunkNormals.Add(normal);

            var offsets = aoOffsets[face];
            for (int i = 0; i < 4; i++)
            {
                var (s1off, s2off, coff) = offsets[i];
                bool s1 = IsSolid(bx + s1off.X, by + s1off.Y, bz + s1off.Z);
                bool s2 = IsSolid(bx + s2off.X, by + s2off.Y, bz + s2off.Z);
                bool c = IsSolid(bx + coff.X, by + coff.Y, bz + coff.Z);
                chunkAO.Add(ComputeAO(s1, s2, c));
            }

            int baseIndex = chunkVerts.Count - data.vertices.Count;
            chunkIndices.Add((uint)(baseIndex + 0));
            chunkIndices.Add((uint)(baseIndex + 1));
            chunkIndices.Add((uint)(baseIndex + 2));
            chunkIndices.Add((uint)(baseIndex + 2));
            chunkIndices.Add((uint)(baseIndex + 3));
            chunkIndices.Add((uint)(baseIndex + 0));
        }

        private Vector3 GetFaceNormal(Faces face)
        {
            return face switch
            {
                Faces.TOP => new Vector3(0, 1, 0),
                Faces.BOTTOM => new Vector3(0, -1, 0),
                Faces.LEFT => new Vector3(-1, 0, 0),
                Faces.RIGHT => new Vector3(1, 0, 0),
                Faces.FRONT => new Vector3(0, 0, 1),
                Faces.BACK => new Vector3(0, 0, -1),
                _ => new Vector3(0, 1, 0),
            };
        }

        private void BuildChunk()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            chunkVertexVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkVertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkVerts.Count * Vector3.SizeInBytes, chunkVerts.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            chunkUVVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkUVVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkUVs.Count * Vector2.SizeInBytes, chunkUVs.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            int chunkNormalVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkNormalVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkNormals.Count * Vector3.SizeInBytes, chunkNormals.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);

            int chunkLayerVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, chunkLayerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkLayers.Count * sizeof(float), chunkLayers.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(3);

            chunkIBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, chunkIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, chunkIndices.Count * sizeof(uint), chunkIndices.ToArray(), BufferUsageHint.StaticDraw);

            int ChunkAOVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ChunkAOVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, chunkAO.Count * sizeof(float), chunkAO.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(4,1, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(4);

            textureID = LoadTextureArray(new string[] {
                "Grass.png",
                "graves.jpg",
                "Wood.jpg",
                "plo.png",
                "trunk.png",
            });

            indexCount = (uint)chunkIndices.Count;
            GL.BindVertexArray(0);
        }

        public static int LoadTextureArray(string[] filePaths)
        {
            int textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, textureID);
            using (var img = new System.Drawing.Bitmap(filePaths[0]))
            {
                int width = img.Width;
                int height = img.Height;
                GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba, width, height, filePaths.Length, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    using (var bmp = new System.Drawing.Bitmap(filePaths[i]))
                    {
                        if (bmp.Width != width || bmp.Height != height)
                            throw new Exception($"La textura {filePaths[i]} debe tener el mismo tamaño ({width}x{height})");
                        var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, width, height, 1, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                        bmp.UnlockBits(data);
                    }
                }
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
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
            chunkNormals.Clear();
            SolidBlockAABBs.Clear();
            chunkLayers.Clear();
            chunkAO.Clear();
            GenFaces();
            BuildChunk();
        }
    }
}