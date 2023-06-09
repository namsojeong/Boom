using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private Rigidbody rigid;
    private Collider collider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }


    public void ResetObject()
    {
       transform.SetParent(transform.parent);
        transform.gameObject.SetActive(true);

        collider.enabled = true;
        rigid.useGravity = true;
        rigid.isKinematic = false;
    }

    public void Get()
    {
        collider.enabled = false;
        rigid.useGravity = false;
        rigid.isKinematic = true;
    }
    public void Lost()
    {
       transform.SetParent(GameManagement.Instance.transform);
        collider.enabled = true;
        rigid.useGravity = true;
        rigid.isKinematic = false;
    }
}
