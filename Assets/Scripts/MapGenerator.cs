using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapMeshData
{
    public Vector3[] m_vertices;
    public int[] m_indices;
    public Vector2[] m_uvs;
    public MapMeshData(int w, int h)
    {
        m_triangleCount = 0;
        m_vertices = new Vector3[w * h];
        m_indices = new int[(w - 1) * (h - 1) * 6];
        m_uvs = new Vector2[w * h];
    }
    private int m_triangleCount;
    private int IndexTail() { return 3 * m_triangleCount; }
    public Mesh GenerateMesh()
    {
        Mesh ret = new Mesh();
        ret.SetVertices(m_vertices);
        ret.SetIndices(m_indices, MeshTopology.Triangles, 0);
        ret.SetUVs(0, m_uvs);
        return ret;
    }
    public void AddRectangle(int a, int b, int c, int d)
    {
        m_indices[IndexTail() + 0] = c;
        m_indices[IndexTail() + 1] = b;
        m_indices[IndexTail() + 2] = a;
        m_indices[IndexTail() + 3] = b;
        m_indices[IndexTail() + 4] = c;
        m_indices[IndexTail() + 5] = d;
        m_triangleCount += 2;
    }
    public void AddTriangle(int a, int b, int c)
    {

        m_indices[IndexTail() + 0] = a;
        m_indices[IndexTail() + 1] = b;
        m_indices[IndexTail() + 2] = c;
        m_triangleCount++;
    }
}

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

    static public MapMeshData GenerateMapMeshData(float[,] map)
    {
        int w = map.GetLength(0), h = map.GetLength(1);
        MapMeshData ret = new MapMeshData(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int vertIdx = x + y * w;
                ret.m_vertices[vertIdx] = new Vector3(x, map[x,y] * (w / 10F), y);
                ret.m_uvs[vertIdx] = new Vector2((float)x / (float)(w - 1), (float)y / (float)(h - 1));
                //Debug.LogWarning($"uv set to {(float)x / (float)w},{(float)y / (float)h} on idx {x + y * w}");
                if (x < w - 1 && y < h - 1)
                    ret.AddRectangle(vertIdx,
                        vertIdx + 1,
                        vertIdx + w,
                        vertIdx + w + 1);
            }
        return ret;
    }
}
