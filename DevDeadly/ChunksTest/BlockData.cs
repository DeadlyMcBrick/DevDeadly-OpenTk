using Vector3 = OpenTK.Mathematics.Vector3;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace DevDeadly
{

    public enum BlockType
    {
        DIRT,
        GRASS,
        EMPTY,
        LAVA,
        WOOD,
        LEAVES,
    }

    public enum BiomeType
    {
        FOREST,
        DESERT,
        SNOW,
        JUNGLE
    }


    public enum Faces
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    public struct FaceData
    {
        public List<Vector3> vertices;
        public List<Vector2> uv;
        public List<Vector3> normals;
    }

    public struct FaceDataRaw
    {
        public static readonly Dictionary<Faces, List<Vector3>> rawVertexData = new Dictionary<Faces, List<Vector3>>
        {
            {Faces.FRONT, new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f,-0.5f,0.5f),
                new Vector3(-0.5f,-0.5f,0.5f)
            }},
            //r
             {Faces.BACK, new List<Vector3>()
            {
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f,-0.5f,-0.5f),
                new Vector3(0.5f,-0.5f,-0.5f)
            }},
             //r
             {Faces.LEFT, new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f,-0.5f,0.5f),
                new Vector3(-0.5f,-0.5f,-0.5f)
            }},
             //r
               {Faces.RIGHT, new List<Vector3>()
            {
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f,-0.5f,-0.5f),
                new Vector3(0.5f,-0.5f,0.5f)
            }},
               //r
             {Faces.TOP, new List<Vector3>()
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f,0.5f,0.5f),
                new Vector3(-0.5f,0.5f,0.5f)
            }},

             {Faces.BOTTOM, new List<Vector3>()
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f,-0.5f,0.5f),
                new Vector3(-0.5f,-0.5f,0.5f)
            }},
        };

        public static readonly Dictionary<Faces, Vector3> rawNormalData = new Dictionary<Faces, Vector3>
        {
            {Faces.FRONT,  Vector3.UnitZ},
            {Faces.BACK,  -Vector3.UnitZ},
            {Faces.LEFT,  -Vector3.UnitX},
            {Faces.RIGHT,  Vector3.UnitX},
            {Faces.TOP,    Vector3.UnitY},
            {Faces.BOTTOM,-Vector3.UnitY},
        };
    }
}



