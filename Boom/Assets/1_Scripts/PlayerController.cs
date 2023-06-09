using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public bool HaveFood { get { return getFood != null; } }
    public Food GetHaveFood { get { return getFood; } }

    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    private Food getFood = null;
    private Rigidbody rigid;
    private float walkSpeed = 10.0f;

    private bool delay = false;
    public bool Delay { get { return delay; } }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        getFood = null;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        int forwardAction = 0;
        int turnAction = 0;

        if (Input.GetKey(KeyCode.W))
            forwardAction = 1;
        else if (Input.GetKey(KeyCode.S))
            forwardAction = -1;
        if (Input.GetKey(KeyCode.A))
            turnAction = -1;
        else if (Input.GetKey(KeyCode.D))
            turnAction = 1;

        rigid.MovePosition(transform.position + transform.forward * forwardAction * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAction * turnSpeed * Time.fixedDeltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Food"))
        {
            if (getFood != null) return;
            Food food = collision.transform.GetComponent<Food>();
            if (food != null)
            {
                getFood = food;
                GetFood();
            }
        }
        else if (collision.transform.CompareTag("Goal"))
        {
            if (getFood)
            {
                GoalIn();
            }
        }
        
        if (collision.transform.CompareTag("Enemy"))
        {
            if (HaveFood) return;
            Enemy enemy = collision.transform.GetComponent<Enemy>();
            if (enemy.Delay) return;
            if (enemy.HaveFood)
            {
                getFood = enemy.GetHaveFood;
                enemy.LostFood();
                GetFood();
            }
        }
    }
    private IEnumerator DelayGet()
    {
        delay = true;
        yield return new WaitForSeconds(1f);
        delay = false;
    }

    private void GoalIn()
    {
        getFood.ResetObject();
        GameManagement.Instance.AddPlayerScore(1);
        getFood = null;
    }

    private void GetFood()
    {
        StartCoroutine(DelayGet());
        getFood.Get();
        Transform parent = transform.Find("Attach");
        getFood.transform.SetParent(parent);
        getFood.transform.localPosition = Vector3.zero;
        getFood.transform.localRotation = Quaternion.identity;
    }

    public void LostFood()
    {
        getFood.Lost();
        getFood = null;
    }
}