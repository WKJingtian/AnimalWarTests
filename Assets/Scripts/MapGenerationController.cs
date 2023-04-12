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

    struct ThreadInfo
    {
        public Map.MapData data;
        public Action<Map.MapData> callback;
    }
    private Queue<ThreadInfo> m_threadReturns = new();
    private void Update()
    {
        while (m_threadReturns.Count > 0)
        {
            var info = m_threadReturns.Dequeue();
            info.callback(info.data);
        }
    }

    // multi-thread creation, after setting all the stuff, call generation here
    public void GenerateNewMap(Action<Map.MapData> callback, MapGenerationData generationData)
    { // generate a whole new map from beginning
        ThreadStart job = delegate
        {
            float[,] ret = MapGenerator.GenerateNoise(
                generationData.m_gWidth, generationData.m_gHeight, generationData.m_gOctaves,
                generationData.m_gLacunarity, generationData.m_gPersistence,
                generationData.m_gScale, generationData.m_gSeed,
                generationData.m_gOffsetX, generationData.m_gOffsetY);
            Map.MapData mapData = new Map.MapData();
            mapData.m_map = ret;
            lock (m_threadReturns)
            {
                m_threadReturns.Enqueue(new ThreadInfo { data = mapData, callback = callback });
            }
        };
        new Thread(job).Start();
    }
    public void GenerateMeshData(Action<Map.MapData> callback, MapGenerationData generationData, Map.MapData mapData)
    {
        ThreadStart job = delegate
        {
            MapMeshData ret = MapGenerator.GenerateMapMeshData(
                mapData.m_map, generationData.m_heightMult, generationData.m_heightCurve, generationData.m_detailLevel);
            mapData.m_meshData = ret;
            lock (m_threadReturns)
            {
                m_threadReturns.Enqueue(new ThreadInfo { data = mapData, callback = callback });
            }
        };
        new Thread(job).Start();
    }
    public void GenerateMesh(Map map, Map.MapData mapData)
    {
        Mesh ret = MapGenerator.GenerateMapMesh(mapData.m_meshData);
        map.SetMesh(ret);
    }
    public void GenerateTexture(Map map, Map.MapData mapData)
    {
        Texture2D tex = new Texture2D(m_data.m_gWidth, m_data.m_gHeight, TextureFormat.RGBA32, false);
        //tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[m_data.m_gWidth * m_data.m_gHeight];
        for (int i = 0; i < m_data.m_gWidth; i++)
            for (int j = 0; j < m_data.m_gHeight; j++)
            {
                colors[i + m_data.m_gWidth * j] = m_data.m_terrain.GetTerrainOnHeight(mapData.m_map[i, j]).m_tColor;
            }
        tex.SetPixels(colors);
        tex.Apply();
        map.SetTexture(tex);
    }

    public void OnGenerateBtnPressed()
    {
        foreach (var child in transform.GetComponentsInChildren<Map>())
            Destroy(child.gameObject);

        MapGenerationData dataCopy = new MapGenerationData(m_data);
        GenerateNewMap((Map.MapData map) =>
        {
            GenerateMeshData((Map.MapData map) =>
            {
                var obj = new GameObject("map");
                obj.transform.parent = m_parent;
                obj.transform.position = m_worldPos;
                var m = obj.AddComponent<Map>();
                var f = obj.AddComponent<MeshFilter>();
                var r = obj.AddComponent<MeshRenderer>();
                r.material = m_material;
                GenerateMesh(m, map);
                GenerateTexture(m, map);
                m.SetData(map);
                m.SetRenderObj(f, r);
            }, dataCopy, map);
        }, dataCopy);
    }
}
