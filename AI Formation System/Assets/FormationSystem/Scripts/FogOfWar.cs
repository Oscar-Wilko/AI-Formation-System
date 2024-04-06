using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public Vector2 minBound;
    public Vector2 maxBound;
    public Vector2Int quadrantCount;
    public GameObject fogPrefab;
    public float fogSpread;
    public float fogShift;
    public float fogScale;
    private Dictionary<Vector2Int, List<GameObject>> fogDict = new Dictionary<Vector2Int, List<GameObject>>();

    private void Awake()
    {
        GenerateFog();
    }

    public void Update()
    {
        
    }

    private void GenerateFog()
    {
        
    }

    public Vector2Int PosToQuad(Vector3 position)
    {
        return Vector2Int.zero;
    }
}
