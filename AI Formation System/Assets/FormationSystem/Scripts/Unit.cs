using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private UnitState state = UnitState.Formation;
    private Vector3 destination;
    private float speed;

    private void Update()
    {
        StateCheck();
    }

    private void StateCheck()
    {
        switch (state)
        {
            case UnitState.None:
                break;
            case UnitState.Formation:
                Move(destination);
                Rotate(destination);
                break;
            case UnitState.Standby:
                break;
            case UnitState.Detected:
                break;
            case UnitState.Attacking:
                break;
            case UnitState.Preview:
                break;
        }
    }

    private void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void Rotate(Vector3 target)
    {
        Vector3 lookPos = target-transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
    }

    public void SetDestination(Vector3 dest) => destination = dest;

    public void SetSpeed(float spd) => speed = spd;

    public void SetPreview() => state = UnitState.Preview;
}
