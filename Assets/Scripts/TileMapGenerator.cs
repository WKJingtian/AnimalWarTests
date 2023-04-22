using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the design goal of this script is to generate tile-based map with given image input
// input could be:
// terrain texture, to generate height map
// resource texture, to generate resources scattered on the map
// feature texture, to create details on maps, like trees, boulders, snow, etc
// rule texture, to define the special spot on the map
// in those textures, each channel of a pixel can refer to a possibility.
// after calculating the possibility of each pixel, lerp them to clean out sharp edges (optional)

public class TileMapGenerator
{
    public float[,] GenerateHeightMapWithGivenTerrainTexture(
        Texture2D texIn,
        System.Func<Texture2D, Texture2D> blurFunc,
        System.Func<Color, float> heightFunc)
    {
        float[,] ret = new float[texIn.width, texIn.height];
        for (int x = 0; x < texIn.width; x++)
            for (int y = 0; y < texIn.height; y++)
                texIn.SetPixel(x, y, 
                    CallculateProbability(texIn.GetPixel(x, y)));
        texIn = blurFunc(texIn);
        for (int x = 0; x < texIn.width; x++)
            for (int y = 0; y < texIn.height; y++)
                ret[x, y] = heightFunc(texIn.GetPixel(x, y));
        return ret;
    }

    #region utils
    private static Color CallculateProbability(Color cin)
    {
        float r = cin.r, g = cin.g, b = cin.b;
        float rand = UnityEngine.Random.Range(0, 1);
        if (r + g + b > 1)
        {
            r = r / (r + g + b);
            g = g / (r + g + b);
            b = b / (r + g + b);
        }
        rand -= r;
        if (rand < 0) return Color.red;
        rand -= g;
        if (rand < 0) return Color.green;
        rand -= b;
        if (rand < 0) return Color.blue;
        return Color.black;
    }
    #endregion
}
