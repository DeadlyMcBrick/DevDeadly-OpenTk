using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace DevDeadly.ChunksTest
{
    public static class TextureData
    {
        // ¡IMPORTANTE! El mapeo debe coincidir con el orden en LoadTextureArray
        public static readonly Dictionary<BlockType, int> blockTypeLayers = new Dictionary<BlockType, int>()
        {
            { BlockType.GRASS,  0 },  
            { BlockType.DIRT,   1 },  
            { BlockType.LAVA,   2 },  
            { BlockType.LEAVES,   3 }, 
            { BlockType.WOOD, 4 },  //Lo ignora
        };

        public static readonly List<Vector2> defaultUV = new List<Vector2>()
        {
            new Vector2(0f, 1f), // top-left
            new Vector2(1f, 1f), // top-right
            new Vector2(1f, 0f), // bottom-right
            new Vector2(0f, 0f), // bottom-left
        };

        public static int GetLayer(BlockType type)
        {
              if (blockTypeLayers.TryGetValue(type, out int layer))
    {
        if (type == BlockType.WOOD)
            Console.WriteLine($"🪵 WOOD → Layer {layer}");
        return layer;
    }
    return 0;
        }
    }
}