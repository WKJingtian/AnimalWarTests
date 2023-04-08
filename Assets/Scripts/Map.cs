using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Renderer m_renderer;
    float[,] m_map;
    int m_width;
    int m_height;
    public void SetMap(float[,] m)
    {
        m_map = m;
        m_width = m_map.GetLength(0);
        m_height = m_map.GetLength(1);
    }
    public void Draw()
    {
        Texture2D tex = new Texture2D(m_width, m_height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[m_width * m_height];
        for (int i = 0; i < m_width; i++)
            for (int j = 0; j < m_height; j++)
            {
                float col = m_map[i, j];
                colors[i + m_width * j] = Color.Lerp(Color.black, Color.white, col);
            }
        tex.SetPixels(colors);
        tex.Apply();
        m_renderer.sharedMaterial.mainTexture = tex;
        m_renderer.transform.localScale = new Vector3(m_width, 1, m_height);
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
    [SerializeField] float m_gOffsetYe = 1;
    public void GenerateNew()
    {
        SetMap(MapGenerator.GenerateNoise(
            m_gWidth, m_gHeight, m_gOctaves,
            m_gLacunarity, m_gPersistence, m_gScale,
            m_gSeed, m_gOffsetX, m_gOffsetYe));
        Draw();
    }
    private void OnValidate()
    {
        m_gWidth = Mathf.Clamp(m_gWidth, 1, 1000);
        m_gHeight = Mathf.Clamp(m_gHeight, 1, 1000);
        m_gOctaves = Mathf.Clamp(m_gOctaves, 1, 1000);
        m_gLacunarity = Mathf.Clamp(m_gLacunarity, -100, 100);
        m_gPersistence = Mathf.Clamp(m_gPersistence, -100, 100);
        m_gScale = Mathf.Clamp(m_gScale, -100, 100);
    }
    #endregion
}
