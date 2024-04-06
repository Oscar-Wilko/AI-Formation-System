using System.Collections.Generic;
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
    [SerializeField] public bool supplier;
    [SerializeField] public bool refreshNoise;
}

[System.Serializable]
public struct ArrowValues
{
    [SerializeField] public float noise;
    [SerializeField] public Vector2 spacing;
    [SerializeField] public float sharpness;
    [SerializeField] public float nthShift;
    [SerializeField] public float evenShift;
    [SerializeField] public Vector2Int size;
    [SerializeField] public bool hollow;
    [SerializeField] public bool supplier;
    [SerializeField] public bool refreshNoise;
}

[System.Serializable]
public struct TriangleValues
{
    [SerializeField] public float noise;
    [SerializeField] public Vector2 spacing;
    [SerializeField] public float nthShift;
    [SerializeField] public float evenShift;
    [SerializeField] public int rows;
    [SerializeField] public int incPerRow;
    [SerializeField] public bool hollow;
    [SerializeField] public bool supplier;
    [SerializeField] public bool refreshNoise;
}

[System.Serializable]
public struct FormationLog
{
    public FormationType type;
    public Vector2 position;
    public BoxValues boxValues;
    public ArrowValues arrowValues;
    public TriangleValues triangleValues;
}

[System.Serializable]
public class FullFormation
{
    public List<FormationLog> logs;
}