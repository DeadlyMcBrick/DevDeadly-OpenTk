using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = OpenTK.Mathematics.Vector2;


namespace DevDeadly.ChunksTest
{
      class TextureData
    {
        public static readonly Dictionary<BlockType, Dictionary<Faces, List<Vector2>>> blockTypeUVs = new Dictionary<BlockType, Dictionary<Faces, List<Vector2>>>()
        {
            {BlockType.DIRT, new Dictionary<Faces, List<Vector2>>()
            {
                {Faces.FRONT, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }},

                 {Faces.BACK, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }},

                  {Faces.LEFT, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }},

                 {Faces.RIGHT, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }},

                  {Faces.TOP, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }},

                   {Faces.BOTTOM, new List<Vector2>()
                {
                     new Vector2 (2f/16f, 15f/16f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (2f/16f, 1f),
                }}, 
            } },

           {BlockType.GRASS, new Dictionary<Faces, List<Vector2>>()
            {
                {Faces.FRONT, new List<Vector2>()
                {
                     new Vector2 (4f/16f, 1f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (4f/16f, 15f/16f),
                }},

                 {Faces.BACK, new List<Vector2>()
                {
                     new Vector2 (4f/16f, 1f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (4f/16f, 15f/16f),
                }},

                  {Faces.LEFT, new List<Vector2>()
                {
                     new Vector2 (4f/16f, 1f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (4f/16f, 15f/16f),
                }},

                 {Faces.RIGHT, new List<Vector2>()
                {
                     new Vector2 (4f/16f, 1f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (4f/16f, 15f/16f),
                }},

                  {Faces.TOP, new List<Vector2>()
                {
                     new Vector2 (8f/16f, 14f/16f),
                     new Vector2 (7f/16f, 14f/16f),
                     new Vector2 (7f/16f, 13f/16f), //bottom left
                     new Vector2 (8f/16f, 13f/16f), //bottom right
                }},

                   {Faces.BOTTOM, new List<Vector2>()
                {
                     new Vector2 (4f/16f, 1f),
                     new Vector2 (3f/16f, 1f),
                     new Vector2 (3f/16f, 15f/16f),
                     new Vector2 (4f/16f, 15f/16f),
                }},



            }}
        };
    }
}
