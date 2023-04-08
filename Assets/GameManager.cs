using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // load all my stuff here
    private Entry m_entry;
    private void Awake()
    {
        m_entry = GetComponent<Entry>();
    }
    private void Start()
    {
        m_entry.GnerateAndDisplayNewMap();
    }
}
