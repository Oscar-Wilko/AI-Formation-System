using UnityEngine;

[System.Serializable]
public struct BoxValues
{
    [SerializeField] public float noise;
    [SerializeField] public Vector2 spacing;
    [SerializeField] public float nthShift;
    [SerializeField] public float evenShift;
    [SerializeField] public Vector2Int size;
    [SerializeField] public bool hollow;
    [SerializeField] public bool refreshNoise;
}