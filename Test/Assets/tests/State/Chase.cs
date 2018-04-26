using UnityEngine;
using UnityEngine.AI;

public class Chase : EnemyState
{
    protected override void PreExecute(StateManager manager)
    {
        EnemyCharController enemy = manager.Get<EnemyStateManager>().GetChar();
        NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
        Debug.Log("START ");
    }

    public override void Execute(StateManager manager)
    {
        EnemyCharController enemy = manager.Get<EnemyStateManager>().GetChar();
        NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
        nav.speed = 200;
        nav.SetDestination(enemy.GetPlayer().transform.position);

    }
    protected override void PostExecute(StateManager manager)
    {
        EnemyCharController enemy = manager.Get<EnemyStateManager>().GetChar();
        NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
        nav.speed = 0;
        nav.SetDestination(new Vector3());
        Debug.Log("END ");
    }

    protected virtual Vector3 GetDestination()
    {
        return new Vector3(
            Random.Range(-180f, 180f),
            0,
            Random.Range(-180f, 180f)
        );
    }
}