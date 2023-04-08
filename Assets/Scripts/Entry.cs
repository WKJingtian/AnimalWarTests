using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// debug entry class
public class Entry : MonoBehaviour
{
    [SerializeField] private Map m_map;

    public void GnerateAndDisplayNewMap()
    {
        m_map.SetMap(MapGenerator.GenerateNoise(100, 100));
        m_map.Draw();
    }
}
