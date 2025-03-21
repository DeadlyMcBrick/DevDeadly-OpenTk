//using System;
//using System.Collections.Generic;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Mathematics;
//using StbImageSharp;
//using Vector3 = OpenTK.Mathematics.Vector3;

//////this is going to be for the colition test to being able to not traspase the chunks 
//using DevDeadly;
//using System.Drawing;

//namespace DevDeadly
//{
//    public bool CheckCollision(AABB playerAABB)
//    {
//        Vector3 min = playerAABB.Min;
//        Vector3 max = playerAABB.Max;

//        int startX = Math.Max((int)Math.Floor(min.X), 0);
//        int endX = Math.Min((int)Math.Ceiling(max.X), SIZE - 1);

//        int startY = Math.Max((int)Math.Floor(min.Y), 0);
//        int endY = Math.Min((int)Math.Ceiling(max.Y), HEIGHT - 1);

//        int startZ = Math.Max((int)Math.Floor(min.Z), 0);
//        int endZ = Math.Min((int)Math.Ceiling(max.Z), SIZE - 1);

//        for (int x = startX; x <= endX; x++)
//        {
//            for (int y = startY; y <= endY; y++)
//            {
//                for (int z = startZ; z <= endZ; z++)
//                {
//                    if (chunkBlocks[x, y, z].type != BlockType.EMPTY)
//                    {
//                        if (playerAABB.Intersects(blockAABBs[x, y, z]))
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//        }

//        return false;
//    } 
//}

