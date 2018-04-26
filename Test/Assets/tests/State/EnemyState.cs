using UnityEngine;

public class EnemyState : State
{

    protected override void PreExecute(StateManager manager)
    {
        Debug.Log(GetType().ToString() + "PreExecute");
    }
    public override void Execute(StateManager manager)
    {
        Debug.Log(GetType().ToString() + " Execute");
    }

    protected override void PostExecute(StateManager manager)
    {
        Debug.Log(GetType().ToString() + " PostExecute");
    }

    protected EnemyCharController GetChar(StateManager manager)
    {
        return manager.Get<EnemyStateManager>().GetChar();
    }
}