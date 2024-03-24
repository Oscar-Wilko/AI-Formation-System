using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleFormation : BaseFormation
{
    [Header("Tweak Values")]
    public TriangleValues triangleValues;

    protected override void Start()
    {
        units = GenerateUnits(GenPos(), "Triangle Formation");
        base.Start();
    }

    protected override void Update()
    {
        UpdateUnits(GenPos(), units);
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void OnDrawGizmos()
    {
        GizmoDraw(GenPos());
    }

    private List<Vector2> GenPos() => GeneratePositions(triangleValues, transform.localEulerAngles.y);

    /// <summary>
    /// Generate positions based on triangle values with own noise
    /// </summary>
    /// <param name="val">TriangleValues of formation</param>
    /// <param name="angle">float of angle to direct by</param>
    /// <returns>List of Vector2 positions</returns>
    private List<Vector2> GeneratePositions(TriangleValues val, float angle)
    {
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != TriangleCount(val))
            noiseGrid = Utils.NoiseArray(TriangleCount(val));
        val.refreshNoise = false;
        return GeneratePositions(val, noiseGrid, angle, 90);
    }

    /// <summary>
    /// Generate positions based on triangle values
    /// </summary>
    /// <param name="val">TriangleValues of formation</param>
    /// <param name="noiseGrid">List of Vector2 positions of noise shift</param>
    /// <param name="angle">float of angle to direct by</param>
    /// <returns>List of Vector2 positions</returns>
    public static List<Vector2> GeneratePositions(TriangleValues val, List<Vector2> noiseGrid, float angle, float anglePreference)
    {
        List<Vector2> positions = new List<Vector2>();
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != TriangleCount(val))
            noiseGrid = Utils.NoiseArray(TriangleCount(val));
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
                pos.y += val.spacing.y * (val.rows - 1) * 0.5f; // Shift from centre
                pos = Utils.Rotate(pos, Mathf.Deg2Rad * -(anglePreference + (angle - anglePreference) * 0.5f)); // Rotate to forward vec
                positions.Add(pos);
                index++;
            }
        }

        return positions;
    }

    /// <summary>
    /// Get number of units based on triangle values
    /// </summary>
    /// <param name="val">TriangleValues of variables</param>
    /// <returns>Int of unit count</returns>
    private static int TriangleCount(TriangleValues val)
    {
        int count = 0;
        for(int i = 0; i < val.rows; i ++)
            count += i * val.incPerRow + 1;
        return count;
    }
}
