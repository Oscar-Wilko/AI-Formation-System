using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BaseFormation : MonoBehaviour
{
    [Header("Trackers")]
    public Vector3 endTarget;
    public bool isLeader;
    [Header("References")]
    public GameObject followTarget;
    [SerializeField] protected GameObject unitPrefab;
    [Header("Tweaks")]
    [SerializeField] protected float stepSpeed;
    protected float agentRadius;
    protected Vector3 followShift;
    protected NavMeshAgent agent;
    protected List<Vector2> noiseGrid = new List<Vector2>();
    protected List<GameObject> units = new List<GameObject>();
    protected List<BaseFormation> otherFormations = new List<BaseFormation>();
    protected List<BaseFormation> formationsInFight = new List<BaseFormation>();

    virtual protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AdjustRadius();
        agent.enabled = true;
        if (!isLeader)
        {
            Vector3 shift = followTarget.transform.position - transform.position;
            shift.y = 0;
            followShift = shift;
        }
        foreach(BaseFormation formation in FindObjectsOfType<BaseFormation>())
            if (formation != this)
                otherFormations.Add(formation);
    }

    virtual protected void Update()
    {
        MoveUpdate();
    }

    virtual protected void LateUpdate()
    {
        
    }

    /// <summary>
    /// Update formation based off environment and other formations
    /// </summary>
    protected void MoveUpdate()
    {
        // Get average position of battle
        Vector3 averagePos = Vector3.zero;
        int unitCount = 0;
        foreach(GameObject unit in units)
        {
            if (!unit)
                continue;
            Unit unitComp = unit.GetComponent<Unit>();
            if (unitComp.State() == UnitState.Detected || unitComp.State() == UnitState.Attacking)
            {
                unitCount++;
                averagePos += unit.transform.position;
            }
        }
        // If in battle, go to average position
        if (unitCount != 0)
        {
            averagePos = averagePos / unitCount;
            averagePos.y = Utils.RayDown(new Vector2(averagePos.x, averagePos.z));
            MoveToTarget(averagePos, 1);

            // Get enemy count, and ally count
            int enemyDetectCount = Utils.EnemyCountInRange(12, transform.position, UnitType.Ally);
            int battleCount = TroopCount();
            foreach(BaseFormation formation in formationsInFight)
            {
                battleCount += formation.TroopCount();
            }
            
            // If overpowered, request backup
            if (enemyDetectCount > battleCount)
            {
                RequestBackup();
            }
        }
        // Otherwise, if leader
        else if (isLeader)
        {
            // Halt if other formations are slacking behind
            bool shouldStop = false;
            foreach (BaseFormation formation in otherFormations)
                if (formation.PathDistance() >= 10)
                    shouldStop = true;
            if (shouldStop)
            {
                agent.isStopped = true;
            }
            else
            {
                MoveToTarget(endTarget, 2);
            }
        }
        // Otherwise, if non-leader
        else
        {
            // Move to offset from leader
            MoveToTarget(followTarget.transform.position - followShift, 4);
        }
    }

    protected void RequestBackup()
    {

    }

    public void RetractedFromFight(BaseFormation formation)
    {
        formationsInFight.Remove(formation);
    }

    /// <summary>
    /// Move formation agent to location, with an ignore check
    /// </summary>
    /// <param name="pos">Vector3 of target position</param>
    /// <param name="ignoreDist">float of minimum dist threshold</param>
    protected void MoveToTarget(Vector3 pos, float ignoreDist)
    {
        agent.isStopped = false;
        pos.y = Utils.RayDown(new Vector2(pos.x, pos.z));
        if (agent.enabled && Vector3.Distance(pos, transform.position) >= ignoreDist)
            agent.SetDestination(pos);
    }

    /// <summary>
    /// Draw gizmos at each position inputted
    /// </summary>
    /// <param name="positions">List of Vector2 positions</param>
    protected void GizmoDraw(List<Vector2> positions)
    {
        foreach (Vector2 pos in positions)
        {
            float y_pos = Utils.RayDown(new Vector2(pos.x + transform.position.x, pos.y + transform.position.z));
            Gizmos.DrawWireCube(new Vector3(transform.position.x + pos.x, y_pos + 1, transform.position.z + pos.y), new Vector3(1, 2, 1));
        }
    }

    /// <summary>
    /// Update all units to relocate their destination to a paired position list
    /// </summary>
    /// <param name="grid">List of Vector2 grid positions</param>
    /// <param name="units">List of GameObjects of units to relocate</param>
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

    /// <summary>
    /// Create a group of units based on a grid of positions
    /// </summary>
    /// <param name="grid">List of Vector2 positions to dictate unit grid</param>
    /// <param name="formName">String of formation name to call parent GameObject</param>
    /// <returns>List of GameObjects units</returns>
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

    /// <summary>
    /// Select which agent type to use based on max formation radius
    /// </summary>
    protected void AdjustRadius()
    {
        float newRadius = 1;
        foreach (GameObject unit in units)
        {
            newRadius = Mathf.Max(newRadius, Vector2.Distance(
                new Vector2(unit.transform.position.x, unit.transform.position.z),
                new Vector2(transform.position.x, transform.position.z)) * 0.8f);
        }
        if (newRadius <= 7.5f)
        {
            // 5 Unit Radius Agent
            agentRadius = 5;
            agent.agentTypeID = 287145453;
        }
        else if (newRadius <= 12.5f)
        {
            // 10 Unit Radius Agent
            agentRadius = 10;
            agent.agentTypeID = -334000983;
        }
        else if (newRadius <= 17.5f)
        {
            // 15 Unit Radius Agent
            agentRadius = 15;
            agent.agentTypeID = -1923039037;
        }
        else if (newRadius <= 22.5f)
        {
            // 20 Unit Radius Agent
            agentRadius = 20;
            agent.agentTypeID = -902729914;
        }
        else
        {
            // 20 Unit Radius Agent
            agentRadius = 20;
            agent.agentTypeID = -902729914;
        }
    }

    public float PathDistance() => agent.remainingDistance;
    
    /// <summary>
    /// Get number of alive units
    /// </summary>
    /// <returns>Int of alive unit count</returns>
    public int TroopCount()
    {
        int count = 0;
        foreach (GameObject obj in units)
            if (obj)
                count++;
        return count;
    }
}
