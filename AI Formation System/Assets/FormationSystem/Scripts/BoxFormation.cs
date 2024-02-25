using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BoxFormation : BaseFormation
{
    [Header("Tweak Values")]
    [SerializeField] private float noise;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private float nthShift;
    [SerializeField] private float evenShift;
    [SerializeField] private Vector2Int size;
    [SerializeField] private bool hollow;
    [SerializeField] private bool refreshNoise;
    [SerializeField] private float rotation;
    private List<Vector2> noiseGrid = new List<Vector2>();
    private List<GameObject> units = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        GenerateUnits();
    }

    protected override void Update()
    {
        base.Update();
        UpdateUnits();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void GenerateUnits()
    {
        Transform boxParent = new GameObject().transform;
        boxParent.name = "Box Formation";
        boxParent.parent = GameObject.FindGameObjectWithTag("UnitHierarchy").transform;

        foreach(Vector2 pos in GeneratePositions())
        {
            float y_pos = RayDown(pos);
            units.Add(Instantiate(unitPrefab, new Vector3(pos.x,y_pos + 1,pos.y), Quaternion.identity, boxParent));
        }
    }

    private void UpdateUnits()
    {
        List<Vector2> grid = GeneratePositions();
        for(int index = 0; index < grid.Count; index++)
        {
            Vector2 pos = grid[index];
            pos.x += transform.position.x;
            pos.y += transform.position.z;
            float y_pos = RayDown(pos);
            units[index].transform.position = new Vector3(pos.x, y_pos + 1, pos.y);
            units[index].transform.localEulerAngles = new Vector3(0, -rotation, 0);
        }
    }

    private List<Vector2> GeneratePositions()
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 fullSize = new Vector2(nthShift * size.y + spacing.x * size.x, spacing.y * size.y);
        if (refreshNoise || noiseGrid.Count == 0)
            noiseGrid = NoiseGrid(size.x, size.y);
        refreshNoise = false;

        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2 pos = new Vector2(0,0);
                pos.x += nthShift * y;
                pos.x += (y + 1)%2 * evenShift;
                pos.x += spacing.x * x; 
                pos.y += spacing.y * y;
                pos += noiseGrid[x + y * size.x] * noise;
                pos -= fullSize * 0.5f;
                pos = Rotate(pos, Mathf.Deg2Rad * rotation);
                positions.Add(pos);
            }
        }

        return positions;
    }

    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private void OutputPositions(List<Vector2> positions)
    {
        foreach(Vector2 pos in positions)
        {
            Debug.Log($"Position: {pos}.");
        }
    }

    private float RayDown(Vector2 pos)
    {
        Physics.Raycast(new Vector3(pos.x, 100, pos.y), Vector3.down, out RaycastHit hitInfo, 1000, LayerMask.GetMask("Ground"));
        return hitInfo.point.y;
    }
}
