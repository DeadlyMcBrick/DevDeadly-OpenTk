using System.Collections.Generic;
using System.Drawing;
using DevDeadly.Shaders;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DevDeadly
{
    public class World
    {
        Shader shader;

        public Dictionary<Vector2i, Chunk> loadedChunks = new Dictionary<Vector2i, Chunk>();

        public void GenerateInitialChunks(Vector3 cameraPosition)
        {
            int renderDistance = 1;

            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector2i key = new Vector2i(x, z);
                    if (!loadedChunks.ContainsKey(key))
                    {
                        Vector3 chunkPosition = new Vector3(x * Chunk.SIZE, 0, z * Chunk.SIZE);
                        loadedChunks[key] = new Chunk(chunkPosition);
                    }
                }
            }
        }

        public void RenderAll(Shader shader)
        {
            //Console.WriteLine("Chunks visibles: " + loadedChunks.Count);

            foreach (var chunk in loadedChunks.Values)
            {
                Matrix4 model = Matrix4.CreateTranslation(chunk.position);
                shader.SetMatrix4("model", model);
                chunk.Render(shader);
            }
        }

        public List<BoundingBox> GetAllObstacles()
        {
            List<BoundingBox> all = new List<BoundingBox>();
            foreach (var chunk in loadedChunks.Values)
            {
                all.AddRange(chunk.SolidBlockAABBs);
            }
            return all;
        }

        public (Chunk chunk, Vector3i blockPos)? RaycastBlock(Vector3 origin, Vector3 direction, float maxDistance = 10f)
        {
            const float step = 0.1f;
            Vector3 currentPos = origin;

            for (float i = 0; i < maxDistance; i += step)
            {
                currentPos += direction * step;

                Vector3i blockPos = new Vector3i((int)MathF.Floor(currentPos.X), (int)MathF.Floor(currentPos.Y), (int)MathF.Floor(currentPos.Z));

                Vector2i chunkCoord = new Vector2i(
                    (int)MathF.Floor(blockPos.X / (float)Chunk.SIZE),
                    (int)MathF.Floor(blockPos.Z / (float)Chunk.SIZE)
                );

                if (loadedChunks.TryGetValue(chunkCoord, out var chunk))
                {
                    Vector3 localPos = blockPos - chunk.position;
                    int lx = (int)localPos.X;
                    int ly = (int)localPos.Y;
                    int lz = (int)localPos.Z;

                    if (lx >= 0 && lx < Chunk.SIZE && ly >= 0 && ly < Chunk.HEIGHT && lz >= 0 && lz < Chunk.SIZE)
                    {
                        var block = chunk.chunkBlocks[lx, ly, lz];
                        if (block != null && block.type != BlockType.EMPTY)
                        {
                            return (chunk, new Vector3i(lx, ly, lz));
                        }
                    }
                }
            }

            return null;
        }

        public void TryPlaceBlock(Camera camera)
        {
            Console.WriteLine("Trying to hit the block...");

            var result = RaycastBlock(camera.position, camera.front);
            if (result != null)
            {
                var (chunk, localBlockPos) = result.Value;
                Console.WriteLine($"Block has impact the chunk {chunk.position}, local: {localBlockPos}");

                Vector3 blockGlobalPos = chunk.position + new Vector3(localBlockPos.X, localBlockPos.Y, localBlockPos.Z);
                Vector3 placePos = blockGlobalPos + Vector3.Normalize(camera.front);

                Vector3i placePosInt = new Vector3i(
                    (int)MathF.Floor(placePos.X),
                    (int)MathF.Floor(placePos.Y),
                    (int)MathF.Floor(placePos.Z)
                );

                Vector2i chunkCoord = new Vector2i(
                    (int)MathF.Floor(placePosInt.X / (float)Chunk.SIZE),
                    (int)MathF.Floor(placePosInt.Z / (float)Chunk.SIZE)
                );

                if (loadedChunks.TryGetValue(chunkCoord, out Chunk targetChunk))
                {
                    Vector3 localPos = placePosInt - targetChunk.position;
                    int lx = (int)localPos.X;
                    int ly = (int)localPos.Y;
                    int lz = (int)localPos.Z;

                    if (lx >= 0 && lx < Chunk.SIZE &&
                        ly >= 0 && ly < Chunk.HEIGHT &&
                        lz >= 0 && lz < Chunk.SIZE)
                    {
                        var currentBlock = targetChunk.chunkBlocks[lx, ly, lz];
                        if (currentBlock == null || currentBlock.type == BlockType.EMPTY)
                        {
                            targetChunk.chunkBlocks[lx, ly, lz] = new Block(placePosInt, BlockType.DIRT);
                            Console.WriteLine("DIRT BlockType being added at: " + placePosInt);
                            targetChunk.Rebuild();
                        }
                        else
                        {
                            Console.WriteLine($"No more blocks at... {lx},{ly},{lz}: {currentBlock.type}");
                        }
                    }
                }
            }
        }

        public void IntegrateFace(Block block, Faces face)
        {
            FaceData data = block.GetFace(face);
            Console.WriteLine($"Face Integrated {face} from block in:  {block.position}");
        }
    }
}
