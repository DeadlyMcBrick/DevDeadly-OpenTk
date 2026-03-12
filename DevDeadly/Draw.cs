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

        public static float[] CloudsVertices =

        {
            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,   0.0f, 1.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 1.0f,

            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 1.0f,
             0.5f, -0.5f, 0.0f,   1.0f, 0.0f
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

        public static List<Vector3> cloudPositions = new List<Vector3>()
        {
            new Vector3(0.20f, 20.0f, 0.1f),
            new Vector3(10.0f, 22.0f, 5.0f),
            new Vector3(-5.0f, 25.0f, -3.0f),
            new Vector3(15.0f, 21.0f, -10.0f),
            new Vector3(-12.0f, 23.0f, 8.0f),
            new Vector3(8.0f, 24.0f, 2.0f),
            new Vector3(3.0f, 26.0f, 12.0f),
            new Vector3(-7.0f, 22.5f, -6.0f),
            new Vector3(18.0f, 25.0f, -4.0f),
            new Vector3(-15.0f, 21.0f, 9.0f),

            new Vector3(20.0f, 27.0f, 0.0f),
            new Vector3(25.0f, 23.0f, -5.0f),
            new Vector3(30.0f, 24.0f, 6.0f),
            new Vector3(-18.0f, 22.0f, 11.0f),
            new Vector3(5.0f, 21.5f, -8.0f),
            new Vector3(13.0f, 26.5f, 4.0f),
            new Vector3(-20.0f, 25.0f, -9.0f),
            new Vector3(22.0f, 23.5f, 7.0f),
            new Vector3(-10.0f, 20.5f, -12.0f),
            new Vector3(17.0f, 24.0f, -2.0f),
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
}