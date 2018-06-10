﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipeBulletHitAction : MonoBehaviour
{
    [Tooltip("爆発のエフェクト")]public GameObject explosion;
    [Tooltip("墜落の際の煙のエフェクト")]public GameObject breakSmoke;
    [SerializeField,Tooltip("自分のDroneMove2")] private DroneMove2 moveScript;
    [SerializeField,Tooltip("自分のRigidBody")] private Rigidbody m_rigid;

    private List<GameObject> children = new List<GameObject>();
    private Vector3 crashVel;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveScript.enabled)
        {  //墜落処理
            m_rigid.AddTorque(crashVel, ForceMode.Force);
            if (transform.position.y < 0) Destroy(gameObject);
        }
    }

    public void Hit()
    {  //墜落準備
        moveScript.m_collider.enabled = false;
        moveScript.enabled = false;
        m_rigid.useGravity = true;
        m_rigid.isKinematic = false;
        m_rigid.constraints = RigidbodyConstraints.None;
        children.Add(Instantiate(explosion, transform.position, Quaternion.identity));
        GameObject b = Instantiate(breakSmoke, transform.position, Quaternion.identity);
        b.GetComponent<Following>().followTrans = transform;
        gameObject.layer = LayerMask.NameToLayer("StageObject");
        //爆発音再生
        children.Add(b);
        crashVel = new Vector3(Random.Range(-360.0f, 360.0f), 0, Random.Range(-360.0f, 360.0f)).normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Field"))
        {
            Destroy(gameObject);
        }
        HitCheck(other);
    }

    void OnCollisionEnter(Collision other)
    {
        HitCheck(other.collider);
    }

    private void HitCheck(Collider other)
    {
        if (other.CompareTag("SnipeBullet"))
        {
            Hit();
        }
    }

    void OnDestroy()
    {
        foreach (var obj in children)
        {
            Destroy(obj);
        }
    }
}
