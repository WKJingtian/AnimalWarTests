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
    MapMeshData m_meshData;
    Mesh m_mesh;
    Texture2D m_texture;

    public void SetMap(float[,] m)
    {
        m_map = m;
        m_width = m_map.GetLength(0);
        m_height = m_map.GetLength(1);
        //m_terrain = new();
        //m_terrain.AddTerrain("deep water", -1, 0.2f, 1, new Color(0.1f, 0.1f, 0.4f, 1));
        //m_terrain.AddTerrain("water", 0.15f, 0.35f, 1, new Color(0.2f, 0.2f, 0.8f, 1));
        //m_terrain.AddTerrain("sand", 0.3499f, 0.4f, 1, new Color(0.8f, 0.8f, 0.3f, 1));
        //m_terrain.AddTerrain("grass", 0.3999f, 0.6f, 1, new Color(0.2f, 0.8f, 0.2f, 1));
        //m_terrain.AddTerrain("mountain", 0.5999f, 0.95f, 3, new Color(0.5f, 0.3f, 0.1f, 1));
        //m_terrain.AddTerrain("peak", 0.9f, 10f, 1, new Color(0.95f, 0.95f, 0.95f, 1));
    }
    public void SetMeshData(MapMeshData data)
    {
        m_meshData = data;
    }
    public void SetMesh(Mesh mesh)
    {
        m_mesh = mesh;
        TryDraw();
    }
    public void SetTexture(Texture2D tex)
    {
        m_texture = tex;
        TryDraw();
    }
    // TODO: divide map into tiles and set feature of each tile
    //public void SetTileFeatures(TileFeature[,] features)
    //{
    //
    //}
    public void SetRenderObj(MeshFilter f, Renderer r)
    {
        m_filter = f;
        m_renderer = r;
        TryDraw();
    }

    public void TryDraw()
    {
        if (m_mesh && m_texture &&
            m_filter && m_renderer)
            Draw();
    }
    public void Draw()
    {
        m_filter.sharedMesh = m_mesh;
        m_renderer.sharedMaterial.mainTexture = m_texture;
    }
}
