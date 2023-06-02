using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Enemy : Agent
{
    private Food getFood = null;

    private Rigidbody rigid;
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    public bool HaveFood { get { return getFood != null; } }
    public Food GetHaveFood { get { return getFood; } }

    public override void Initialize()
    {
        rigid= GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        GameManagement.Instance.ResetGame();
        getFood = null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        forwardAmount = actions.DiscreteActions[0];
        float turnAmount = 0f;

        if (actions.DiscreteActions[1] == 1f)
        {
            turnAmount = -1;
        }
        else if (actions.DiscreteActions[1] == 2f)
        {
            turnAmount = 1f;
        }

        rigid.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        if (MaxStep > 0) AddReward(-1f / MaxStep);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;

        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2;
        }
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
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
        else if (collision.transform.CompareTag("Player"))
        {
            if (HaveFood) return;
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            if (player.HaveFood)
            {
                getFood = player.GetHaveFood;
                player.LostFood();
                GetFood();
            }
        }

    }

    private void GoalIn()
    {
        getFood.ResetObject();
        GameManagement.Instance.AddPlayerScore(1);
        getFood = null;
    }

    private void GetFood()
    {
        getFood.Get();
        Transform parent = transform.Find("Attach");
        getFood.transform.SetParent(parent);
        getFood.transform.localPosition = Vector3.zero;
        getFood.transform.localRotation = Quaternion.identity;
    }

    public void LostFood()
    {
        getFood.transform.gameObject.SetActive(false);
        getFood.ResetObject();
        getFood = null;
    }
}
