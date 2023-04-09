using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private MeshFilter m_filter;
    [SerializeField] private Renderer m_renderer;
    float[,] m_map;
    int m_width;
    int m_height;
    MapTerrain m_terrain;

    public void SetMap(float[,] m)
    {
        m_terrain = new();
        m_terrain.AddTerrain("deep water", -1, 0.2f, 1, new Color(0.1f, 0.1f, 0.4f, 1));
        m_terrain.AddTerrain("water", 0.15f, 0.35f, 1, new Color(0.2f, 0.2f, 0.8f, 1));
        m_terrain.AddTerrain("sand", 0.3499f, 0.4f, 1, new Color(0.8f, 0.8f, 0.3f, 1));
        m_terrain.AddTerrain("grass", 0.3999f, 0.6f, 1, new Color(0.2f, 0.8f, 0.2f, 1));
        m_terrain.AddTerrain("mountain", 0.5999f, 0.95f, 3, new Color(0.5f, 0.3f, 0.1f, 1));
        m_terrain.AddTerrain("peak", 0.9f, 10f, 1, new Color(0.95f, 0.95f, 0.95f, 1));
        m_map = m;
        m_width = m_map.GetLength(0);
        m_height = m_map.GetLength(1);
        m_filter.sharedMesh = MapGenerator.GenerateMapMeshData(m).GenerateMesh();
    }
    public void Draw()
    {
        Texture2D tex = new Texture2D(m_width, m_height, TextureFormat.RGBA32, false);
        //tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[m_width * m_height];
        for (int i = 0; i < m_width; i++)
            for (int j = 0; j < m_height; j++)
            {
                colors[i + m_width * j] = m_terrain.GetTerrainOnHeight(m_map[i, j]).m_tColor;
            }
        tex.SetPixels(colors);
        tex.Apply();
        m_renderer.sharedMaterial.mainTexture = tex;
        //m_renderer.transform.localScale = new Vector3(m_width, 1, m_height);
    }

    #region Generate map with given stat
    // for test(inspector) only
    [SerializeField] int m_gWidth = 30;
    [SerializeField] int m_gHeight = 30;
    [SerializeField] int m_gOctaves = 1;
    [SerializeField] float m_gLacunarity = 1;
    [SerializeField] float m_gPersistence = 1;
    [SerializeField] float m_gScale = 1;
    [SerializeField] int m_gSeed = 1;
    [SerializeField] float m_gOffsetX = 1;
    [SerializeField] float m_gOffsetY = 1;
    public void GenerateNew()
    {
        SetMap(MapGenerator.GenerateNoise(
            m_gWidth, m_gHeight, m_gOctaves,
            m_gLacunarity, m_gPersistence, m_gScale,
            m_gSeed, m_gOffsetX, m_gOffsetY));
        Draw();
    }
    private void OnValidate()
    {
        m_gWidth = Mathf.Clamp(m_gWidth, 1, 1000);
        m_gHeight = Mathf.Clamp(m_gHeight, 1, 1000);
        m_gOctaves = Mathf.Clamp(m_gOctaves, 1, 1000);
        m_gLacunarity = Mathf.Clamp(m_gLacunarity, 0, 100);
        m_gPersistence = Mathf.Clamp(m_gPersistence, 0, 100);
        m_gScale = Mathf.Clamp(m_gScale, 0, 100);
    }
    #endregion
}
