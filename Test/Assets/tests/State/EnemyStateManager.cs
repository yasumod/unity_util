using UnityEngine;
using UnityEditor;
using System;

public class EnemyStateManager : StateManager
{
    [SerializeField]
    private EnemyCharController enemyChar;
    private Idle idle;
    private Search search;
    private Chase chase;


    /// <summary>
    /// Start
    /// </summary>
    private void Awake()
    {
        idle = new Idle();
        search = new Search();
        chase = new Chase();
    }
    protected override State GetState()
    {

        if (Input.GetKey("s"))
        {
            return search;
        }
        if (Input.GetKey("c"))
        {
            return chase;
        }

        return idle;
    }

    public EnemyCharController GetChar()
    {
        return enemyChar;
    }
}