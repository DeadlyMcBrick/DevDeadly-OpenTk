using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace DevDeadly
{
    public class Draw
    {
        public static float[] lampVertices = {

            // positions          // normals
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f, 0.0f,

             0.5f,  0.5f,  0.5f, 1.0f,  0.0f, 0.0f,
             0.5f,  0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f, 1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f, 1.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f, 1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, 0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
             0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, 0.0f, 1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f, 0.0f, 1.0f,  0.0f,
        };

        public static uint[] indicesCreate =

        {
            0, 1, 2,
            2, 3, 0
        };

        public static uint[] indicesHUD =

        {
            0, 1, 2,
            2, 3, 0
        };


        struct VertexChunk
        {
            public Vector3 Position;   // layout(location=0)
            public Vector3 Normal;     // layout(location=1)
            public float TexLayer;     // layout(location=2)
            public Vector2 TexCoord;   // layout(location=3)
        }

        VertexChunk[] vertices = new VertexChunk[]

            {
                new VertexChunk {
                    Position = new Vector3(-0.5f, -0.5f, -0.5f),
                    TexCoord = new Vector2(0.0f, 0.0f),
                    TexLayer = 0.0f,
                    Normal   = new Vector3(0.0f, 0.0f, -1.0f)
                },
            };

        public static float[] backgroundVertices =

        {
            // posX, posY
            -0.8f, -0.6f, // Bottom left
             0.8f, -0.6f, // Bottom right
             0.8f,  0.6f, // Top right
            -0.8f,  0.6f  // Top left
        };

        public static uint[] backgroundIndices = {
            0, 1, 2,
            2, 3, 0
        };
    }

    public class Slot
    {
        const float offsetX = -1.0f / 90f;
        const float offsetY = -1.7f;
        const float SlotWidth = 1.6f;
        const float hudHeight = 0.2f;
        const float offsetX2 = 0.0f;
        const float offsetY2 = 0.2f;
        const float CreateWidth = 1.2f;
        const float CreateHeight = 1.6f;

        public static float[] verticesHUD =
        {
            -SlotWidth / 2 + offsetX, -hudHeight / 2 + offsetY, 0f, 1f,
             SlotWidth / 2 + offsetX, -hudHeight / 2 + offsetY, 1f, 1f,
             SlotWidth / 2 + offsetX,  hudHeight / 2 + offsetY, 1f, 0f,
            -SlotWidth / 2 + offsetX,  hudHeight / 2 + offsetY, 0f, 0f,
        };

        public static float[] backgroundVertices =

        {
            -(SlotWidth * 0.90f) / 2 + offsetX, -hudHeight / 2 + offsetY, 0f, 1f,
            (SlotWidth * 0.90f) / 2 + offsetX, -hudHeight / 2 + offsetY, 1f, 1f,
            (SlotWidth * 0.90f) / 2 + offsetX,  hudHeight / 2 + offsetY, 1f, 0f,
            -(SlotWidth * 0.90f) / 2 + offsetX,  hudHeight / 2 + offsetY, 0f, 0f,
        };

        public static float[] createHUD =

        {
           -CreateWidth / 2 + offsetX2, -CreateHeight / 2 + offsetY2, 0f, 1f,
           CreateWidth / 2 + offsetX2, -CreateHeight / 2 + offsetY2, 1f, 1f,
           CreateWidth / 2 + offsetX2,  CreateHeight / 2 + offsetY2, 1f, 0f,
           -CreateWidth / 2 + offsetX2,  CreateHeight / 2 + offsetY2, 0f, 0f,
        };
    }
}