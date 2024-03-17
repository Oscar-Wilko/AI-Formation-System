using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseFormation : MonoBehaviour
{
    [SerializeField] protected GameObject unitPrefab;
    [SerializeField] protected float stepSpeed;
    public Vector3 target;
    protected NavMeshAgent agent;
    protected List<Vector2> noiseGrid = new List<Vector2>();
    protected List<GameObject> units = new List<GameObject>();

    virtual protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AdjustRadius();
        MoveToTarget();
    }

    virtual protected void Update()
    {
        
    }

    virtual protected void LateUpdate()
    {
        
    }

    protected void MoveToTarget()
    {
        if (agent.enabled)
            agent.SetDestination(target);
    }

    protected void GizmoDraw(List<Vector2> positions)
    {
        foreach (Vector2 pos in positions)
        {
            float y_pos = Utils.RayDown(new Vector2(pos.x + transform.position.x, pos.y + transform.position.z));
            Gizmos.DrawWireCube(new Vector3(transform.position.x + pos.x, y_pos + 1, transform.position.z + pos.y), new Vector3(1, 2, 1));
        }
    }

    protected void UpdateUnits(List<Vector2> grid, List<GameObject> units)
    {
        for (int index = 0; index < grid.Count && index < units.Count; index++)
        {
            if (!units[index])
                continue;
            Vector2 pos = grid[index];
            pos.x += transform.position.x;
            pos.y += transform.position.z;
            float y_pos = Utils.RayDown(pos);
            units[index].GetComponent<Unit>().SetDestination(new Vector3(pos.x, y_pos + 1, pos.y));
        }
    }

    protected List<GameObject> GenerateUnits(List<Vector2> grid, string formName)
    {
        List<GameObject> units = new List<GameObject>();
        Transform boxParent = new GameObject().transform;
        boxParent.name = formName;
        boxParent.parent = GameObject.FindGameObjectWithTag("UnitHierarchy").transform;

        foreach (Vector2 pos in grid)
        {
            float y_pos = Utils.RayDown(new Vector2(transform.position.x, transform.position.z) + pos);
            GameObject unit = Instantiate(unitPrefab, new Vector3(transform.position.x + pos.x, y_pos + 1, transform.position.z + pos.y), Quaternion.identity, boxParent);
            unit.GetComponent<Unit>().SetSpeed(stepSpeed);
            units.Add(unit);
        }
        return units;
    }

    protected void AdjustRadius()
    {
        float newRadius = 1;
        foreach(GameObject unit in units)
        {
            newRadius = Mathf.Max(newRadius, Vector2.Distance(
                new Vector2(unit.transform.position.x, unit.transform.position.z), 
                new Vector2(transform.position.x, transform.position.z))*0.8f);
        }

        agent.radius = newRadius;
    }
}
