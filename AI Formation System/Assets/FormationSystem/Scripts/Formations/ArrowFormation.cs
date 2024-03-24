using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFormation : BaseFormation
{
    [Header("Tweak Values")]
    public ArrowValues arrowValues;

    protected override void Start()
    {
        units = GenerateUnits(GenPos(), "Arrow Formation");
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

    private List<Vector2> GenPos() => GeneratePositions(arrowValues, transform.localEulerAngles.y);

    /// <summary>
    /// Generate positions based on arrow values with own noise grid
    /// </summary>
    /// <param name="val">ArrowValues of formation</param>
    /// <param name="angle">float of angle to direct by</param>
    /// <returns>List of Vector2 positions</returns>
    private List<Vector2> GeneratePositions(ArrowValues val, float angle)
    {
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != val.size.x * val.size.y)
            noiseGrid = Utils.NoiseArray(val.size.x * val.size.y);
        val.refreshNoise = false;
        return GeneratePositions(val, noiseGrid, angle, 90);
    }

    /// <summary>
    /// Generate positions based on arrow values
    /// </summary>
    /// <param name="val">ArrowValues of formation</param>
    /// <param name="noiseGrid">List of Vector2 positions of noise shift</param>
    /// <param name="angle">float of angle to direct by</param>
    /// <returns>List of Vector2 positions</returns>
    public static List<Vector2> GeneratePositions(ArrowValues val, List<Vector2> noiseGrid, float angle, float anglePreference)
    {
        List<Vector2> positions = new List<Vector2>();
        int centreIndex = val.size.x / 2;
        Vector2 fullSize = new Vector2(val.nthShift * val.size.y + val.spacing.x * val.size.x, val.spacing.y * val.size.y - centreIndex * val.sharpness);
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != val.size.x * val.size.y)
            noiseGrid = Utils.NoiseArray(val.size.x * val.size.y);

        for (int x = 0; x < val.size.x; x++)
        {
            for (int y = 0; y < val.size.y; y++)
            {
                if (val.hollow && !(x == 0 || x == val.size.x - 1) && !(y == 0 || y == val.size.y - 1)) continue;
                Vector2 pos = new Vector2(0, 0);
                pos.x += val.nthShift * y; // Nth shift per row
                pos.x += (y + 1) % 2 * val.evenShift; // Even shift per even row
                pos.x += val.spacing.x * x;  // X spacing
                pos.y += val.spacing.y * y; // Y spacing
                pos.y += -Mathf.Abs(x - centreIndex) * val.sharpness; // Arrow shift from centre
                pos += noiseGrid[x + y * val.size.x] * val.noise; // Noise shift
                pos -= fullSize * 0.5f; // Shift from centre
                pos = Utils.Rotate(pos, Mathf.Deg2Rad * -(anglePreference + (angle - anglePreference) * 0.5f)); // Rotate to forward vec
                positions.Add(pos);
            }
        }

        return positions;
    }
}
