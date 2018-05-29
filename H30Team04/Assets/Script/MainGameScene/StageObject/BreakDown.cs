﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakDown : MonoBehaviour
{
    private Rigidbody m_rigid;
    private Vector3 primaryPos;
    [SerializeField, Range(0.0f, 5.0f)] private float torquePower = 2.0f;

    // Use this for initialization
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        primaryPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = primaryPos;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            BreakDownAction(other.transform);
        }
    }
    public void BreakDownAction(Transform target)
    {  //プレイヤーと当たらないレイヤーに変更
        gameObject.layer = LayerMask.NameToLayer("StageObject");
        Vector3 dir = -target.forward;
        m_rigid.useGravity = true;
        m_rigid.AddTorque(dir * torquePower, ForceMode.VelocityChange);
    }
}
