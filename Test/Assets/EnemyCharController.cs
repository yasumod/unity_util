
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCharController :  BaseCharController
{

    private bool isRun = false;
    private long spped = 200;

    private int pointIndex = 0;

    private NavMeshAgent navAgent;

    [SerializableAttribute]
    private class StdAction
    {
        public enum STATUS
        {
            SEARCH,
            PATROL,
            IDLE,
        }

        public STATUS status;

        public Transform pointObj;
    }

    [SerializeField]
    private List<StdAction> actionList;
    public BaseCharController player;

    private void Start()
    {
        pointIndex = 0;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = speed;
        navAgent.autoBraking = false;   
        Patrol();
    }



    void Patrol()
    {

        Debug.LogWarning("patrol " + pointIndex + "/" + actionList[pointIndex].pointObj.position);


        navAgent.SetDestination(actionList[pointIndex].pointObj.position);
        pointIndex = (pointIndex + 1) % actionList.Count;
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (navAgent.remainingDistance < 0.5f)
            Patrol();
    }

    public BaseCharController GetPlayer()
    {
        return player;
    }
}