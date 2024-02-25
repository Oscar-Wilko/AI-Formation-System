using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFormation : MonoBehaviour
{
    [SerializeField] protected GameObject unitPrefab;

    virtual protected void Awake()
    {
        
    }

    virtual protected void Update()
    {
        
    }

    virtual protected void LateUpdate()
    {
        
    }

    protected List<Vector2> NoiseGrid(int width, int height)
    {
        List<Vector2> grid = new List<Vector2>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            }
        }
        return grid;
    }
}
