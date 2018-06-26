﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDroneHit : MonoBehaviour {

    [Tooltip("爆発のエフェクト")] public GameObject explosion;
    [Tooltip("墜落の際の煙のエフェクト")] public GameObject breakSmoke;
    [SerializeField, Tooltip("自分のRigidBody")] private Rigidbody m_rigid;

    private List<GameObject> children = new List<GameObject>();
    private Vector3 crashVel;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        if (m_rigid.useGravity)
        {  //墜落処理
            m_rigid.AddTorque(crashVel, ForceMode.Force);
            if (transform.position.y < 0)
                Destroy(gameObject);
        }
    }

    public void Hit()
    {  //墜落準備
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
        GetComponent<DroneAudioPlay>().Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        HitCheck(other);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Field"))
        {
            Destroy(gameObject);
        }
        HitCheck(other.collider);
    }

    private void HitCheck(Collider other)
    {
        if (other.CompareTag("SnipeBullet"))
        {
            Hit();
            GetComponent<Collider>().enabled = false;
            m_rigid.velocity = Vector3.zero;
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
