using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private MeshFilter m_filter;
    [SerializeField] private Renderer m_renderer;
    Mesh m_mesh;
    Texture2D m_texture;

    public struct MapData
    {
        public float[,] m_map;
        public int m_width;
        public int m_height;
        public MapTerrain m_terrain;
        public MapMeshData m_meshData;
    }
    MapData m_data;

    public void SetData(MapData d)
    {
        m_data = d;
        SetMap(m_data.m_map);
        SetMeshData(m_data.m_meshData);
    }
    public void SetMap(float[,] m)
    {
        m_data.m_map = m;
        m_data.m_width = m_data.m_map.GetLength(0);
        m_data.m_height = m_data.m_map.GetLength(1);
    }
    public void SetMeshData(MapMeshData data)
    {
        m_data.m_meshData = data;
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
