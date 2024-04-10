using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("References")]
    private Vector3 destination;
    private MeshRenderer mesh;
    private NavMeshAgent agent;
    [Header("Ally Materials")]
    [SerializeField] private Material formationMat;
    [SerializeField] private Material standbyMat;
    [SerializeField] private Material detectedMat;
    [SerializeField] private Material attackingMat;
    [SerializeField] private Material previewMat;
    [Header("Enemy Materials")]
    [SerializeField] private Material formationMatEnemy;
    [SerializeField] private Material standbyMatEnemy;
    [SerializeField] private Material detectedMatEnemy;
    [SerializeField] private Material attackingMatEnemy;
    [SerializeField] private Material previewMatEnemy;
    [Header("Tweak Values")]
    [SerializeField] private UnitType type;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate;
    [SerializeField] private float damage;
    [SerializeField] private float maxHp;
    [Header("Trackers")]
    private UnitState state = UnitState.Formation;
    private float hp;
    private float tracker = 0;
    private BaseFormation formation;
    private bool dead = false;

    private void Awake()
    {
        hp = maxHp;
        destination = transform.position;
        agent = GetComponent<NavMeshAgent>();
        mesh = GetComponent<MeshRenderer>();
        ChangeState(UnitState.Formation);
    }

    private void Update()
    {
        StateCheck();
    }

    private void StateCheck()
    {
        tracker =  Mathf.Clamp(tracker + Time.deltaTime, 0, 1 / attackRate);
        GameObject enemy;
        switch (state)
        {
            case UnitState.None:
                break;
            case UnitState.Formation:
                // IN DETECT RANGE
                if (EnemyInRange(detectRange))
                {
                    ChangeState(UnitState.Detected);
                    break;
                }
                // REACHED DESTINATION
                if (Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2(transform.position.x, transform.position.z)) <= 0.2f)
                {
                    ChangeState(UnitState.Standby);
                    break;
                }
                // OUT OF DETECT RANGE
                Move(destination);
                break;
            case UnitState.Standby:
                // IN DETECT RANGE
                if (EnemyInRange(detectRange))
                {
                    ChangeState(UnitState.Detected);
                    break;
                }
                // DESTINATION IS FAR AWAY
                if (Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2(transform.position.x, transform.position.z)) > 0.2f)
                {
                    ChangeState(UnitState.Formation);
                    break;
                }
                break;
            case UnitState.Detected:
                // WITHIN ATTACK RANGE
                enemy = EnemyInRange(attackRange);
                if (enemy)
                {
                    ChangeState(UnitState.Attacking);
                    break;
                }
                // STILL IN DETECT RANGE
                enemy = EnemyInRange(detectRange);
                if (enemy)
                {
                    Move(enemy.transform.position);
                    break;
                }
                // OUT OF DETECT RANGE
                ChangeState(UnitState.Formation);
                break;
            case UnitState.Attacking:
                // IN ATTACK RANGE
                enemy = EnemyInRange(attackRange);
                if (enemy)
                {
                    agent.isStopped = true;
                    if (tracker >= (1 / attackRate))
                    {
                        enemy.GetComponent<Unit>().TakeDamage(damage);
                        tracker = 0;
                    }
                    break;
                }
                agent.isStopped = false;
                // STILL IN DETECT RANGE
                enemy = EnemyInRange(detectRange);
                if (enemy)
                {
                    ChangeState(UnitState.Detected);
                    break;
                }
                // OUT OF DETECT RANGE
                ChangeState(UnitState.Formation);
                break;
            case UnitState.Preview:
                ChangeState(UnitState.Preview);
                break;
        }
    }

    private void ChangeState(UnitState new_state)
    {
        state = new_state;
        switch (state)
        {
            case UnitState.Formation:
                mesh.material = type == UnitType.Ally ? formationMat : formationMatEnemy;
                break;
            case UnitState.Standby:
                mesh.material = type == UnitType.Ally ? standbyMat : standbyMatEnemy;
                break;
            case UnitState.Detected:
                mesh.material = type == UnitType.Ally ? detectedMat : detectedMatEnemy;
                break;
            case UnitState.Attacking:
                mesh.material = type == UnitType.Ally ? attackingMat : attackingMatEnemy;
                break;
            case UnitState.Preview:
                mesh.material = type == UnitType.Ally ? previewMat : previewMatEnemy;
                break;
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0 && !dead)
        {
            if (formation)
                formation.LoseUnit(gameObject, true);
            dead = true;
            Destroy(gameObject);
        }
    }

    private GameObject EnemyInRange(float range) => Utils.EnemyInRange(range, transform.position, type);
    private void Move(Vector3 target) => agent.SetDestination(target);
    public void SetDestination(Vector3 dest) => destination = dest;
    public void SetSpeed(float spd) => agent.speed = spd;
    public void SetPreview() => state = UnitState.Preview;
    public UnitType Type() => type;
    public UnitState State() => state;
    public void SetType(UnitType n_type) => type = n_type;
    public void SetFormation(BaseFormation form) => formation = form;
}
