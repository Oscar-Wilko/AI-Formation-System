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

    public override bool CanSendSupply() => triangleValues.supplier;

    public override void LoseUnit(GameObject unit)
    {
        int index = units.IndexOf(unit);
        int row = RowOfIndex(index, triangleValues);
        int prevIndex = index;
        Vector3 prevPos = units[prevIndex].transform.position;
        for (int i = row; i < triangleValues.rows - 1; i ++)
        {
            int tempIndex = 0;
            for(int r = 0; r <= i; r++)
            {
                tempIndex += r * triangleValues.incPerRow + 1;
            }
            int closestShift = -1;
            float closestDist = float.MaxValue;
            for(int a = 0; a < (i+1) * triangleValues.incPerRow; a ++)
            {
                if (!units[tempIndex + a])
                    continue;
                float dist = Vector3.Distance(prevPos, units[tempIndex + a].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestShift = a;
                }
            }
            if (closestShift == -1)
                break;
            tempIndex += closestShift;
            units[prevIndex] = units[tempIndex];
            prevPos = units[tempIndex].transform.position;
            prevIndex = tempIndex;
            if (units[tempIndex])
                index = tempIndex;
            units[tempIndex] = null;
        }
        RequestSupply(index);
        base.LoseUnit(unit);
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
        return GeneratePositions(val, noiseGrid, angle, unitType == UnitType.Ally ? 90 : -90);
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
                float angleShift = angle - anglePreference;
                while (angleShift > 180)
                    angleShift -= 360;
                pos = Utils.Rotate(pos, Mathf.Deg2Rad * -(anglePreference + (angleShift) * 0.25f)); // Rotate to forward vec
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

    private static int RowOfIndex(int index, TriangleValues val)
    {
        int count = 0;
        for (int i = 0; i < val.rows; i++)
        {
            count += i * val.incPerRow + 1;
            if (index < count)
                return i;
        }
        return val.rows;
    }
}
