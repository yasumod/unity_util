
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BaseCharController :  MonoBehaviour
{

    protected Vector3 beforePos;
    protected Animator animator;
    protected Rigidbody rigibody;


    protected float speed = 5000;
    protected float kando = 10;

    protected virtual void Awake()
    {
        rigibody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }


}