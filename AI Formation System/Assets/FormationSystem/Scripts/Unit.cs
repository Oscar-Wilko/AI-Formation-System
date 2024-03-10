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
        Move();
        Rotate();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private void Rotate()
    {
        Vector3 lookPos = destination-transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
    }

    public void SetDestination(Vector3 dest) => destination = dest;

    public void SetSpeed(float spd) => speed = spd;
}
