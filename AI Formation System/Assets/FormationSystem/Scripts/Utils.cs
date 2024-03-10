using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static void OutputPositions(List<Vector2> positions)
    {
        foreach (Vector2 pos in positions)
        {
            Debug.Log($"Position: {pos}.");
        }
    }

    public static float RayDown(Vector2 pos)
    {
        Physics.Raycast(new Vector3(pos.x, 100, pos.y), Vector3.down, out RaycastHit hitInfo, 1000, LayerMask.GetMask("Ground"));
        return hitInfo.point.y;
    }

    public static List<Vector2> NoiseArray(int size)
    {
        List<Vector2> grid = new List<Vector2>();
        for (int i = 0; i < size; i++)
        {
            grid.Add(new Vector2(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f)));
        }
        return grid;
    }
}
