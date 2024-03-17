using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFormation : BaseFormation
{
    [Header("Tweak Values")]
    public BoxValues boxValues;

    protected override void Start()
    {
        units = GenerateUnits(GenPos(), "Box Formation");
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

    private List<Vector2> GenPos() => GeneratePositions(boxValues, noiseGrid, transform.localEulerAngles.y);

    public static List<Vector2> GeneratePositions(BoxValues val, List<Vector2> noiseGrid, float angle)
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 fullSize = new Vector2(val.nthShift * val.size.y + val.spacing.x * val.size.x, val.spacing.y * val.size.y);
        if (val.refreshNoise || noiseGrid.Count == 0 || noiseGrid.Count != val.size.x*val.size.y)
            noiseGrid = Utils.NoiseArray(val.size.x * val.size.y);
        val.refreshNoise = false;

        for(int x = 0; x < val.size.x; x++)
        {
            for (int y = 0; y < val.size.y; y++)
            {
                if (val.hollow && !(x == 0 || x == val.size.x - 1) && !(y == 0 || y == val.size.y - 1)) continue;
                Vector2 pos = new Vector2(0,0);
                pos.x += val.nthShift * y;      // Nth shift per row
                pos.x += (y + 1) % 2 * val.evenShift; // Even shift per even row
                pos.x += val.spacing.x * x;     // X spacing
                pos.y += val.spacing.y * y;     // Y spacing
                pos += noiseGrid[x + y * val.size.x] * val.noise; // Noise shift
                pos -= fullSize * 0.5f;         // Shift from centre
                pos = Utils.Rotate(pos, Mathf.Deg2Rad * - angle); // Rotate to forward vec
                positions.Add(pos);
            }
        }

        return positions;
    }
}
