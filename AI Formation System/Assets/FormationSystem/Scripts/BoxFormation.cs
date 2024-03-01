using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BoxFormation : BaseFormation
{
    [Header("Tweak Values")]
    [SerializeField] private BoxValues boxValues;
    private List<Vector2> noiseGrid = new List<Vector2>();
    private List<GameObject> units = new List<GameObject>();
    [SerializeField] private float stepSpeed;
    public Vector3 target;

    protected override void Awake()
    {
        base.Awake();
        GenerateUnits();
        MoveToTarget();
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

    private void OnDrawGizmos()
    {
        foreach (Vector2 pos in GeneratePositions(boxValues))
        {
            float y_pos = RayDown(new Vector2(pos.x + transform.position.x, pos.y + transform.position.z));
            Gizmos.DrawWireCube(new Vector3(transform.position.x + pos.x,y_pos +1, transform.position.z + pos.y), new Vector3(1,2,1));
        }
    }

    private void MoveToTarget()
    {
        agent.SetDestination(target);
    }

    private void GenerateUnits()
    {
        Transform boxParent = new GameObject().transform;
        boxParent.name = "Box Formation";
        boxParent.parent = GameObject.FindGameObjectWithTag("UnitHierarchy").transform;

        foreach(Vector2 pos in GeneratePositions(boxValues))
        {
            float y_pos = RayDown(pos);
            units.Add(Instantiate(unitPrefab, new Vector3(transform.position.x + pos.x, y_pos + 1, transform.position.z + pos.y), Quaternion.identity, boxParent));
        }
    }

    private void UpdateUnits()
    {
        List<Vector2> grid = GeneratePositions(boxValues);
        for(int index = 0; index < grid.Count; index++)
        {
            Vector2 pos = grid[index];
            pos.x += transform.position.x;
            pos.y += transform.position.z;
            float y_pos = RayDown(pos);
            units[index].transform.position = Vector3.MoveTowards(units[index].transform.position, new Vector3(pos.x, y_pos + 1, pos.y), stepSpeed * Time.deltaTime);
            units[index].transform.rotation = transform.rotation;
        }
    }

    private List<Vector2> GeneratePositions(BoxValues val)
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 fullSize = new Vector2(val.nthShift * val.size.y + val.spacing.x * val.size.x, val.spacing.y * val.size.y);
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != val.size.x*val.size.y)
            noiseGrid = NoiseGrid(val.size.x, val.size.y);
        val.refreshNoise = false;

        for(int x = 0; x < val.size.x; x++)
        {
            for (int y = 0; y < val.size.y; y++)
            {
                if (val.hollow && !(x == 0 || x == val.size.x - 1) && !(y == 0 || y == val.size.y - 1)) continue;
                Vector2 pos = new Vector2(0,0);
                pos.x += val.nthShift * y;
                pos.x += (y + 1)%2 * val.evenShift;
                pos.x += val.spacing.x * x; 
                pos.y += val.spacing.y * y;
                pos += noiseGrid[x + y * val.size.x] * val.noise;
                pos -= fullSize * 0.5f;
                pos = Rotate(pos, Mathf.Deg2Rad * -transform.localEulerAngles.y);
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
