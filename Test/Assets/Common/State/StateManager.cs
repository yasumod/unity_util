using UnityEngine;

public abstract class StateManager : MonoBehaviour
{
    /// <summary>
    /// アニメーター
    /// </summary>
    [SerializeField]
    protected Animator animator;

    /// <summary>
    /// 前回ステータス
    /// </summary>
    [SerializeField]
    private State beforeState = null;

    /// <summary>
    /// ステータス取得
    /// </summary>
    /// <returns>string:ステータス</returns>
    abstract protected State GetState();

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        beforeState = null;
    }

    /// <summary>
    /// Update
    /// </summary>
    protected virtual void Update()
    {
        UpdateState();
    }

    /// <summary>
    /// ステート更新
    /// </summary>
    protected virtual void UpdateState()
    {

        State state = GetState();


        // ステート不明なら終了
        if (state == null)
        {
            EndBeforeStatus(state);
            return;
        }

        // 初回
        if (beforeState != state)
        {
            // 前回ステータス終了
            EndBeforeStatus(state);

            // 新規ステータス開始
            beforeState = state;
            state.Start(animator,this);
            return;
        }

        // 二回目以降は普通に更新
        state.Execute(this);
    }

    

    /// <summary>
    /// ステータス終了
    /// </summary>
    private void EndBeforeStatus(State cur)
    {
        // 前回データがあれば、前回ステートを終了させる
        if (beforeState != null)
        {
            beforeState.End(animator,this);
        }

        beforeState = null;
    }

    public MGRTYPE Get<MGRTYPE>() where MGRTYPE : StateManager
    {
        return (MGRTYPE)this;
    }
}