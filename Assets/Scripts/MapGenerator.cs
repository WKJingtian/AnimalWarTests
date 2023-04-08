using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    // width, height, octave, lacunarity, persistence, noise scale
    public static float[,] GenerateNoise(
        int w, int h,
        int o = 1, float l = 1,
        float p = 1, float s = 1,
        int seed = 0, float xOffset = 0, float yOffset = 0)
    {
        System.Random rand = new System.Random(seed);
        Vector2 [] noiseOffset = new Vector2[o];
        for (int i = 0; i < o; i++)
            noiseOffset[i] = new Vector2(
                rand.Next(-1000, 1000) + xOffset,
                rand.Next(-1000, 1000) + yOffset);

        float[,] ret = new float[w, h];
        s = Mathf.Max(s, 0.1f);
        float min = float.MaxValue, max = float.MinValue;
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                float frequncy = 1;
                float amplitude = 1;
                float height = 0;

                for (int i = 0; i < o; i++)
                {
                    float sampleX = frequncy * (float)x / s + noiseOffset[i].x;
                    float sampleY = frequncy * (float)y / s + noiseOffset[i].y;
                    float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    height += perlin * amplitude;
                    //Debug.LogWarning($"generate number {perlin} on point {sampleX}, {sampleY}");

                    frequncy *= l;
                    amplitude *= p;
                }

                ret[x, y] = height;
                if (height < min) min = height;
                if (height > max) max = height;
            }

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                ret[x, y] = (ret[x, y] - min) / (max - min);
            }
        return ret;
    }
}
