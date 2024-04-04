using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheredMinimapCompass : MonoBehaviour
{
    public static GatheredMinimapCompass Instance { get; private set; }
    [SerializeField] public Transform minimap_directions;

    [SerializeField] public Transform minimap_north;
    [SerializeField] public Transform minimap_south;
    [SerializeField] public Transform minimap_east;
    [SerializeField] public Transform minimap_west;

    private void Awake()
    {
        Instance = this;
    }
}
