using System;
using UnityEngine;

/// <summary>
/// ステート
/// </summary>
[SerializableAttribute]
public abstract class State
{

    /// <summary>
    /// 開始
    /// </summary>
    abstract protected void PreExecute(StateManager manager);

    /// <summary>
    /// 実行
    /// </summary>
    abstract public void Execute(StateManager manager);

    /// <summary>
    /// 終了
    /// </summary>
    abstract protected void PostExecute(StateManager manager);

    /// <summary>
    /// 開始
    /// </summary>
    /// <param name="animator"></param>
    public void Start(Animator animator, StateManager manager)
    {
        animator.SetBool(GetType().ToString(), true);
        PreExecute(manager);
    }

    /// <summary>
    /// 終了
    /// </summary>
    /// <param name="animator"></param>
    public void End(Animator animator, StateManager manager)
    {
        PostExecute(manager);
        animator.SetBool(GetType().ToString(), false);
    }
}