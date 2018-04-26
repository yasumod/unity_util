using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController :  MonoBehaviour
{

    private Animator m_Anim;
    private Rigidbody m_Rigidbody2D;

    [SerializeField]
    private float move_speed = 7.0f;

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody>();
    }

}
