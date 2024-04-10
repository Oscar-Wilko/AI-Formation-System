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
    protected bool inBattle = false;
    protected List<Vector2> noiseGrid = new List<Vector2>();
    public List<GameObject> units = new List<GameObject>();
    protected List<BaseFormation> otherFormations = new List<BaseFormation>();
    protected List<BaseFormation> formationsInFight = new List<BaseFormation>();
    protected BaseFormation formationAssisting;
    protected Transform groupParent;
    protected FightState fightState = FightState.Hold;
    [Header("References")]
    protected NavMeshAgent agent;
    public GameObject followTarget;
    [SerializeField] protected GameObject unitPrefab;
    [SerializeField] protected GameObject enemyVariant;
    [Header("Tweaks")]
    public UnitType unitType;
    [SerializeField] protected float stepSpeed;
    protected float agentRadius;
    protected Vector3 followShift;

    virtual protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AdjustRadius();
        agent.enabled = true;
        if (unitType == UnitType.Ally)
        {
            if (!isLeader)
            {
                Vector3 shift = followTarget.transform.position - transform.position;
                shift.y = 0;
                followShift = shift;
            }
            foreach (BaseFormation formation in FindObjectsOfType<BaseFormation>())
                if (formation != this && unitType == formation.unitType)
                    otherFormations.Add(formation);
        }
    }

    virtual protected void Update()
    {
        UnitUpdate();
        MoveUpdate();
    }

    virtual protected void LateUpdate()
    {
        
    }

    protected void UnitUpdate()
    {
        // Check if all units from formation have died
        if (TroopCount() == 0)
            NoUnits();
    }

    /// <summary>
    /// Update all formations that this formation will cease to exist and refresh leader if needed
    /// </summary>
    protected void NoUnits()
    {
        if (unitType == UnitType.Ally)
        {
            // Get closest formation to the end
            float dist = float.MaxValue;
            BaseFormation nextInLine = null;
            foreach (BaseFormation formation in otherFormations)
            {
                // Whilst refreshing their lists
                formation.FormationLost(this);
                if (isLeader)
                {
                    float temp = Vector3.Distance(formation.transform.position, endTarget);
                    if (temp < dist)
                    {
                        dist = temp;
                        nextInLine = formation;
                    }
                }
            }
            // Update the new leader if there are any formations left, and if this was a leader
            if (isLeader && nextInLine)
            {
                nextInLine.isLeader = true;
                foreach (BaseFormation formation in otherFormations)
                {
                    if (formation.transform != nextInLine.transform)
                    {
                        formation.followTarget = nextInLine.gameObject;
                        formation.followShift -= nextInLine.followShift;
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Update formation based off environment and other formations
    /// </summary>
    protected void MoveUpdate()
    {
        if (!agent.isOnNavMesh)
            return;
        if (formationAssisting)
        {
            MoveToTarget(formationAssisting.transform.position, 1);
            return;
        }
        // Get average position of battle
        Vector3 averagePos = new Vector3(0,0,0);
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
            inBattle = true;
            averagePos = averagePos / unitCount;
            averagePos.y = Utils.RayDown(new Vector2(averagePos.x, averagePos.z));
            MoveToTarget(averagePos, 1);
            SendAllUnits(units, averagePos);

            // Get enemy count, and ally count
            int enemyDetectCount = Utils.EnemyCountInRange(25, transform.position, unitType);

            // If overpowered, request backup
            if (enemyDetectCount * 2.0f > FightCount())
            {
                RequestBackup();
            }
        }
        else if (unitType == UnitType.Ally)
        {
            // Otherwise, if leader
            if (inBattle && !formationAssisting)
                EndFight();
            if (isLeader)
            {
                inBattle = false;
                // Halt if other formations are slacking behind
                bool shouldStop = false;
                foreach (BaseFormation formation in otherFormations)
                    if (formation.PathDistance() >= 10)
                        shouldStop = true;
                if (shouldStop || fightState == FightState.Hold)
                {
                    agent.isStopped = true;
                }
                else if (fightState == FightState.Attack)
                {
                    MoveToTarget(endTarget, 2);
                }
                else if (fightState == FightState.Flank)
                {
                    // FUTURE FLANK STUFF
                    MoveToTarget(endTarget, 2);
                }
            }
            else
            {
                // Otherwise, if non-leader, move to offset from leader
                inBattle = false;
                MoveToTarget(followTarget.transform.position - followShift, 4);
            }
        }
        else
        {
            MoveToTarget(endTarget, 2);
        }
    }

    /// <summary>
    /// Request another formation to join the fight
    /// </summary>
    protected void RequestBackup()
    {
        // Get closest formation to self that isn't in a battle
        BaseFormation newGroup = null;
        float dist = float.MaxValue;
        foreach(BaseFormation formation in otherFormations)
        {
            if (formation.TroopCount() == 0 || formation.InBattle())
                continue;
            float temp = Vector3.Distance(endTarget, formation.transform.position);
            if (temp < dist)
            {
                newGroup = formation;
                dist = temp;
            }
        }
        if (!newGroup)
            return;
        // If already assisting a fight, tell the fight innitiator about the new member
        if (formationAssisting)
        {
            formationAssisting.EnterFight(newGroup);
        }
        // Otherwise add to self
        else
        {
            EnterFight(newGroup);
        }
    }

    /// <summary>
    /// Inform that a new formation has entered the current fight
    /// </summary>
    /// <param name="formation">BaseFormation joining formation</param>
    public void EnterFight(BaseFormation formation)
    {
        formation.formationAssisting = this;
        formationsInFight.Add(formation);
    }

    /// <summary>
    /// Inform that the fight ended to all formations involved
    /// </summary>
    public void EndFight()
    {
        foreach (BaseFormation formation in formationsInFight)
        {
            formation.formationAssisting = null;
        }
    }

    /// <summary>
    /// Inform that a formation has left the fight
    /// </summary>
    /// <param name="formation">BaseFormation of leaving formation</param>
    public void FormationLost(BaseFormation formation)
    {
        otherFormations.Remove(formation);
        formationsInFight.Remove(formation);
        if (formation == formationAssisting)
            formationAssisting = null;
    }

    /// <summary>
    /// Get total unit count of formations in same battle as this
    /// </summary>
    /// <returns>int of total units from all fighting formations</returns>
    public int FightCount()
    {
        if (formationAssisting)
            return formationAssisting.FightCount();
        int battleCount = TroopCount();
        foreach (BaseFormation formation in formationsInFight)
        {
            battleCount += formation.TroopCount();
        }
        return battleCount;
    }

    /// <summary>
    /// Move formation agent to location, with an ignore check
    /// </summary>
    /// <param name="pos">Vector3 of target position</param>
    /// <param name="ignoreDist">float of minimum dist threshold</param>
    protected void MoveToTarget(Vector3 pos, float ignoreDist)
    {
        if (!agent.isOnNavMesh)
            return;
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

    public virtual void LoseUnit(GameObject unit, bool request_replacement) { }

    public virtual bool CanSendSupply() => false;

    public virtual void RequestSupply(int unitIndex)
    {
        BaseFormation closestSupplier = null;
        float dist = float.MaxValue;
        foreach(BaseFormation formation in otherFormations)
        {
            if (formation.TroopCount() > 0 && formation.CanSendSupply())
            {
                float temp = Vector3.Distance(transform.position, formation.transform.position);
                if (temp < dist)
                {
                    dist = temp;
                    closestSupplier = formation;
                }
            }
        }
        if (closestSupplier != null)
        {
            units[unitIndex] = closestSupplier.SendSupply();
            units[unitIndex].GetComponent<Unit>().SetFormation(this);
            units[unitIndex].transform.parent = groupParent;
        }
    }

    public virtual GameObject SendSupply()
    {
        for(int i = units.Count - 1; i >= 0; i --)
        {
            if (units[i])
            {
                GameObject sentUnit = units[i];
                LoseUnit(units[i], false);
                return sentUnit;
            }
        }
        return null;
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
    /// Update all units to relocate their destination to a paired position list
    /// </summary>
    /// <param name="grid">List of Vector2 grid positions</param>
    /// <param name="units">List of GameObjects of units to relocate</param>
    protected void SendAllUnits(List<GameObject> units, Vector3 position)
    {
        foreach (GameObject unit in units)
        {
            if (!unit)
                continue;
            position.y = Utils.RayDown(position);
            unit.GetComponent<Unit>().SetDestination(new Vector3(position.x, position.y, position.z));
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
        groupParent = boxParent;
        int index = 0;
        foreach (Vector2 pos in grid)
        {
            float y_pos = Utils.RayDown(new Vector2(transform.position.x, transform.position.z) + pos);
            GameObject unit = Instantiate(unitType == UnitType.Ally ? unitPrefab : enemyVariant, new Vector3(transform.position.x + pos.x, y_pos + 1, transform.position.z + pos.y), Quaternion.identity, boxParent);
            unit.GetComponent<Unit>().SetSpeed(stepSpeed);
            unit.GetComponent<Unit>().SetFormation(this);
            unit.GetComponent<Unit>().SetType(unitType);
            unit.name = formName + " at " + index;
            units.Add(unit);
            index++;
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
    public float PathDistance() => agent.remainingDistance;
    public bool InBattle() => inBattle;

    public void SetFightState(FightState newState)
    {
        if (unitType == UnitType.Ally)
        {
            fightState = newState;
        }
    }
}
