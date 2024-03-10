using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleFormation : BaseFormation
{
    [Header("Tweak Values")]
    [SerializeField] private TriangleValues triangleValues;
    private List<GameObject> units = new List<GameObject>();

    protected override void Awake()
    {
        units = GenerateUnits(GeneratePositions(triangleValues), "Triangle Formation");
        base.Awake();
    }

    protected override void Update()
    {
        UpdateUnits(GeneratePositions(triangleValues), units);
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void OnDrawGizmos()
    {
        GizmoDraw(GeneratePositions(triangleValues));
    }

    private List<Vector2> GeneratePositions(TriangleValues val)
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 fullSize = new Vector2(0,0);
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != TriangleCount(val))
            noiseGrid = Utils.NoiseArray(TriangleCount(val));
        val.refreshNoise = false;
        int index = 0;

        for(int r = 0; r < val.rows; r ++)
        {
            int rowCount = r * val.incPerRow + 1;
            for(int i = 0; i < rowCount; i++)
            {
                if (val.hollow && !(r == 0 || r == val.rows - 1) && !(i == 0 || i == rowCount - 1)) continue;
                Vector2 pos = new Vector2(0, 0);
                pos.x += val.nthShift * r; // Nth shift per row
                pos.x += (r + 1) % 2 * val.evenShift; // Even shift per even row
                pos.x += val.spacing.x * (i - (rowCount-1) * 0.5f);  // X spacing (with triangle offset)
                pos.y -= val.spacing.y * r; // Y spacing
                pos += noiseGrid[index] * val.noise; // Noise shift
                pos -= fullSize * 0.5f; // Shift from centre
                pos = Utils.Rotate(pos, Mathf.Deg2Rad * -transform.localEulerAngles.y); // Rotate to forward vec
                positions.Add(pos);
                index++;
            }
        }

        return positions;
    }

    private int TriangleCount(TriangleValues val)
    {
        int count = 0;
        for(int i = 0; i < val.rows; i ++)
            count += i * val.incPerRow + 1;
        return count;
    }
}
