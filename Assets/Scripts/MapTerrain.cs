using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct Terrain
{
    public Color m_tColor;
    public float m_tMinHeight;
    public float m_tMaxHeight;
    public float m_tWeight;
    public string m_tName;
    // TODO: slop can also influence terrain so there should be minSlope and maxSlope attributes

    static public Terrain NULL_TERRAIN()
    {
        return new Terrain {
            m_tColor = Color.black,
            m_tMinHeight = 0,
            m_tMaxHeight = 0,
            m_tWeight = 0,
            m_tName = "NULL",
        };
    }
}

[Serializable]
public class MapTerrain
{
    [SerializeField] List<Terrain> m_terrains = new();
    public void AddTerrain(string n, float hMin, float hMax, float w, Color c)
    {
        m_terrains.Add(new Terrain
        {
            m_tColor = c,
            m_tMinHeight = hMin,
            m_tMaxHeight = hMax,
            m_tWeight = w,
            m_tName = n,
        });
    }
    public void RemoveTerrain(string n)
    {
        for (int i = 0; i < m_terrains.Count; i++)
        {
            if (m_terrains[i].m_tName == n)
            {
                m_terrains.RemoveAt(i);
                return;
            }
        }
    }
    // return idx of the terrain
    public Terrain GetTerrainOnHeight(float h)
    {
        float totalWeight = 0;
        Dictionary<int, float> terrainWeightMap = new();
        int idx = 0;
        foreach (Terrain terrain in m_terrains)
        {
            if (terrain.m_tMinHeight > h ||
                terrain.m_tMaxHeight < h)
            {
                idx++;
                continue;
            }
            totalWeight += terrain.m_tWeight;
            terrainWeightMap[idx] = terrain.m_tWeight;
            idx++;
        }
        float rand = UnityEngine.Random.Range(0, totalWeight);
        foreach (var item in terrainWeightMap)
        {
            rand -= item.Value;
            if (rand <= 0) return this[item.Key];
        }
        return Terrain.NULL_TERRAIN();
    }
    public Terrain this[int i]
    {
        get => m_terrains[i];
        private set => m_terrains[i] = value;
    }
}
