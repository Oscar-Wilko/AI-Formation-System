using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitType type;
    private UnitState state = UnitState.Formation;
    private Vector3 destination;
    private MeshRenderer mesh;
    [SerializeField] private Material formationMat;
    [SerializeField] private Material standbyMat;
    [SerializeField] private Material detectedMat;
    [SerializeField] private Material attackingMat;
    [SerializeField] private Material previewMat;
    private NavMeshAgent agent;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate;
    [SerializeField] private float damage;
    [SerializeField] private float maxHp;
    private float hp;
    private float tracker = 0;
    private BaseFormation formation;

    private void Awake()
    {
        hp = maxHp;
        destination = transform.position;
        agent = GetComponent<NavMeshAgent>();
        mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        StateCheck();
        if (Input.GetKeyDown(KeyCode.K) && Random.Range(0, 100) < 10)
            TakeDamage(5);
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
                    state = UnitState.Detected;
                    mesh.material = detectedMat;
                    break;
                }
                // REACHED DESTINATION
                if (Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2(transform.position.x, transform.position.z)) <= 0.2f)
                {
                    state = UnitState.Standby;
                    mesh.material = standbyMat;
                    break;
                }
                // OUT OF DETECT RANGE
                Move(destination);
                mesh.material = formationMat;
                break;
            case UnitState.Standby:
                // IN DETECT RANGE
                if (EnemyInRange(detectRange))
                {
                    state = UnitState.Detected;
                    mesh.material = detectedMat;
                    break;
                }
                // DESTINATION IS FAR AWAY
                if (Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2(transform.position.x, transform.position.z)) > 0.2f)
                {
                    state = UnitState.Formation;
                    mesh.material = formationMat;
                    break;
                }
                // OUT OF DETECT RANGE
                mesh.material = standbyMat;
                break;
            case UnitState.Detected:
                // WITHIN ATTACK RANGE
                enemy = EnemyInRange(attackRange);
                if (enemy)
                {
                    state = UnitState.Attacking;
                    mesh.material = attackingMat;
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
                state = UnitState.Formation;
                mesh.material = formationMat;
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
                    state = UnitState.Detected;
                    mesh.material = detectedMat;
                    break;
                }
                // OUT OF DETECT RANGE
                state = UnitState.Formation;
                mesh.material = formationMat;
                break;
            case UnitState.Preview:
                mesh.material = previewMat;
                break;
        }
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            if (formation)
                formation.LoseUnit(gameObject);
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
    public void SetFormation(BaseFormation form) => formation = form;
}
