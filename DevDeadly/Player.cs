//using System;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Windowing.GraphicsLibraryFramework;
//using System.Diagnostics;
//using OpenTK.Mathematics;
//using Vector3 = OpenTK.Mathematics.Vector3;
//using ImGuiNET;

//namespace DevDeadly
//{
//    public class Player
//    {
//        public AABB playerAABB;
//        public Vector3 position;
//        public Vector3 size; 

//        public Player(Vector3 startPos, Vector3 playerSize)
//        {
//            //testing movement to being able to apply AABB
//            position = startPos;
//            size = playerSize;
//            playerAABB = new AABB(position, position + size);
//        }

//        public void Move(Vector3 movement, Chunk chunk)
//        {
//            Vector3 moveX = new Vector3(movement.X, 0, 0);
//            Vector3 moveY = new Vector3(0, movement.Y, 0);
//            Vector3 moveZ = new Vector3(0, 0, movement.Z);

//            AABB testAABBX = new AABB(position + moveX, position + moveX + size);
//            if (!chunk.CheckCollision(testAABBX))
//            {
//                position += moveX;
//                playerAABB = testAABBX;
//            }

//            AABB testAABBY = new AABB(position + moveY, position + moveY + size);
//            if (!chunk.CheckCollision(testAABBY))
//            {
//                position += moveY;
//                playerAABB = testAABBY;
//            }

//            AABB testAABBZ = new AABB(position + moveZ, position + moveZ + size);
//            if (!chunk.CheckCollision(testAABBZ))
//            {
//                position += moveZ;
//                playerAABB = testAABBZ;
//            }
//        }
//    }
//}
