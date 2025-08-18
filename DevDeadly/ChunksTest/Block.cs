using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector2 = OpenTK.Mathematics.Vector2;
using DevDeadly.ChunksTest;

namespace DevDeadly
{
    public class Block
    {
        public BoundingBox AABB { get; private set; }
        public Vector3 position;
        public BlockType type;

        private Dictionary<Faces, FaceData> faces;

        public Dictionary<Faces, List<Vector2>> blockUV = new Dictionary<Faces, List<Vector2>>()
        {
            {Faces.FRONT, new List<Vector2>()},
            {Faces.BACK, new List<Vector2>()},
            {Faces.LEFT, new List<Vector2>()},
            {Faces.RIGHT, new List<Vector2>()},
            {Faces.TOP, new List<Vector2>()},
            {Faces.BOTTOM, new List<Vector2>()},
        };

        public Block(Vector3 position, BlockType blockType = BlockType.EMPTY)
        {
            type = blockType;
            this.position = position;

            if (blockType != BlockType.EMPTY)
            {
                Vector3 min = position - new Vector3(0.5f);
                Vector3 max = position + new Vector3(0.5f);
                AABB = new BoundingBox(min, max);
            }

            faces = new Dictionary<Faces, FaceData>
    {
        { Faces.FRONT,  new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.FRONT]),  uv = TextureData.defaultUV } },
        { Faces.BACK,   new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BACK]),   uv = TextureData.defaultUV } },
        { Faces.LEFT,   new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.LEFT]),   uv = TextureData.defaultUV } },
        { Faces.RIGHT,  new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.RIGHT]),  uv = TextureData.defaultUV } },
        { Faces.TOP,    new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.TOP]),    uv = TextureData.defaultUV } },
        { Faces.BOTTOM, new FaceData { vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.BOTTOM]), uv = TextureData.defaultUV } }
    };
        }

        public int GetTextureLayer()
        {
            return TextureData.blockTypeLayers[type];
        }

        public List<Vector3> AddTransformedVertices(List<Vector3> vertices)
        {
            List<Vector3> transformedVertices = new List<Vector3>();
            foreach (var vert in vertices)
            {
                transformedVertices.Add(vert + position);
            }

            return transformedVertices;
        }

        public FaceData GetFace(Faces face)
        {
            return faces[face];
        }

    }
}
