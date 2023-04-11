using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationController : MonoBehaviour
{
    [Serializable]
    public struct MapGenerationData
    {
        // used when generating the noise map (height map)
        [Range(1, 1000)] public int m_gWidth;
        [Range(1, 1000)] public int m_gHeight;
        [Range(0, 100)] public int m_gOctaves;
        [Range(0, 100)] public float m_gLacunarity;
        [Range(0, 100)] public float m_gPersistence;
        [Range(0, 100)] public float m_gScale;
        public int m_gSeed;
        public float m_gOffsetX;
        public float m_gOffsetY;

        // used when generating the mesh data
        [Range(0, 100)] public float m_heightMult;
        [Range(0, 10)] public int m_detailLevel;
        public AnimationCurve m_heightCurve;

        // used when generating the mesh
        /* NONE */

        // used whem generating texture
        public MapTerrain m_terrain;
        public MapGenerationData(MapGenerationData copyFrom)
        {
            m_gWidth = copyFrom.m_gWidth;
            m_gHeight = copyFrom.m_gHeight;
            m_gOctaves = copyFrom.m_gOctaves;
            m_gLacunarity = copyFrom.m_gLacunarity;
            m_gPersistence = copyFrom.m_gPersistence;
            m_gScale = copyFrom.m_gScale;
            m_gSeed = copyFrom.m_gSeed;
            m_gOffsetX = copyFrom.m_gOffsetX;
            m_gOffsetY = copyFrom.m_gOffsetY;
            m_heightMult = copyFrom.m_heightMult;
            m_detailLevel = copyFrom.m_detailLevel;
            m_heightCurve = new AnimationCurve(copyFrom.m_heightCurve.keys);
            m_terrain = copyFrom.m_terrain;
        }
    }
    public MapGenerationData m_data;

    // used after generation process is done
    public Transform m_parent;
    public Vector3 m_worldPos;
    public Material m_material;

    // multi-thread creation, after setting all the stuff, call generation here
    public void GenerateNewMap(Action<Map> callback, Map result)
    { // generate a whole new map from beginning
        MapGenerationData dataCopy = new MapGenerationData(m_data);
        //ThreadStart job = delegate
        {
            float[,] ret = MapGenerator.GenerateNoise(
                dataCopy.m_gWidth, dataCopy.m_gHeight, dataCopy.m_gOctaves,
                dataCopy.m_gLacunarity, dataCopy.m_gPersistence,
                dataCopy.m_gScale, dataCopy.m_gSeed,
                dataCopy.m_gOffsetX, dataCopy.m_gOffsetY);
            result.SetMap(ret);
            GenerateMeshData(dataCopy, ret, callback, result);
        };
    }
    private void GenerateMeshData(MapGenerationData data, float[,] map, Action<Map> callback, Map result)
    {
        //ThreadStart job = delegate
        {
            MapMeshData ret = MapGenerator.GenerateMapMeshData(
                map, data.m_heightMult, data.m_heightCurve, data.m_detailLevel);
            result.SetMeshData(ret);
            //ret.Report();
            GenerateTexture(data, map, null, result);
            GenerateMesh(data, ret, callback, result);
        };
    }
    public void GenerateMeshData(float[,] map, Action<Map> callback, Map result)
    { // regenerate mesh data of existing map
        MapGenerationData dataCopy = new MapGenerationData(m_data);
        //ThreadStart job = delegate
        {
            MapMeshData ret = MapGenerator.GenerateMapMeshData(
                map, dataCopy.m_heightMult, dataCopy.m_heightCurve, dataCopy.m_detailLevel);
            result.SetMeshData(ret);
            //ret.Report();
            GenerateTexture(dataCopy, map, null, result);
            GenerateMesh(dataCopy, ret, callback, result);
        };
    }
    private void GenerateMesh(MapGenerationData data, MapMeshData map, Action<Map> callback, Map result)
    {
        Mesh ret = MapGenerator.GenerateMapMesh(map);
        result.SetMesh(ret);
        if (callback != null) callback(result);
    }
    public void GenerateMesh(MapMeshData map, Action<Map> callback, Map result)
    {
        Mesh ret = MapGenerator.GenerateMapMesh(map);
        result.SetMesh(ret);
        if (callback != null) callback(result);
    }
    private void GenerateTexture(MapGenerationData data, float[,] map, Action<Map> callback, Map result)
    {
        Texture2D tex = new Texture2D(data.m_gWidth, data.m_gHeight, TextureFormat.RGBA32, false);
        //tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[data.m_gWidth * data.m_gHeight];
        for (int i = 0; i < data.m_gWidth; i++)
            for (int j = 0; j < data.m_gHeight; j++)
            {
                colors[i + data.m_gWidth * j] = data.m_terrain.GetTerrainOnHeight(map[i, j]).m_tColor;
            }
        tex.SetPixels(colors);
        tex.Apply();
        result.SetTexture(tex);
        if (callback != null) callback(result);
    }
    public void GenerateTexture(float[,] map, Action<Map> callback, Map result)
    {
        Texture2D tex = new Texture2D(m_data.m_gWidth, m_data.m_gHeight, TextureFormat.RGBA32, false);
        //tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[m_data.m_gWidth * m_data.m_gHeight];
        for (int i = 0; i < m_data.m_gWidth; i++)
            for (int j = 0; j < m_data.m_gHeight; j++)
            {
                colors[i + m_data.m_gWidth * j] = m_data.m_terrain.GetTerrainOnHeight(map[i, j]).m_tColor;
            }
        tex.SetPixels(colors);
        tex.Apply();
        result.SetTexture(tex);
        if (callback != null) callback(result);
    }

    public void OnGenerateBtnPressed()
    {
        foreach (var child in transform.GetComponentsInChildren<Map>())
            DestroyImmediate(child.gameObject);

        // copy the data that will be used after generation
        var obj = new GameObject("map");
        Transform t = m_parent;
        Vector3 p = m_worldPos;
        Material m = m_material;
        MeshFilter f = obj.AddComponent<MeshFilter>();
        Renderer r = obj.AddComponent<MeshRenderer>();

        GenerateNewMap((Map map) =>
        {
            map.transform.SetParent(t);
            map.transform.position = p;
            r.material = m;
            map.SetRenderObj(f, r);
        }, obj.AddComponent<Map>());
    }
}
