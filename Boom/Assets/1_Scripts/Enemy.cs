using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Enemy : Agent
{
    private Food getFood = null;
    private Rigidbody rigid;
    private Animator anim;
    
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;

    private bool isWalk = false;
    private bool delay = false;

    private void Awake()
    {
    }

    #region Get
    public bool HaveFood { get { return getFood != null; } }
    public Food GetHaveFood { get { return getFood; } }

    public bool Delay { get { return delay; } }
    #endregion

    #region Distance
    private float TargetDistance()
    {
        return Vector3.Distance(GameManagement.Instance.GetPlayerTransform.position, transform.position);
    }
    private float GoalDistance()
    {
        return Vector3.Distance(GameManagement.Instance.GoalTransform.position, transform.position);
    }
    private float FoodDistance()
    {
        return Vector3.Distance(GameManagement.Instance.FoodTransform.position, transform.position);
    }
    #endregion

    #region Agent
    public override void Initialize()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        ResetEnemy();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward); // 3
        sensor.AddObservation(TargetDistance()); // 1
        sensor.AddObservation(GoalDistance()); // 1
        sensor.AddObservation(FoodDistance()); // 1
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        if (actions.DiscreteActions[1] == 1f)
            turnAmount = -1;
        else if (actions.DiscreteActions[1] == 2f)
            turnAmount = 1;
        if (actions.DiscreteActions[0] == 1f)
            forwardAmount = 1;
        else if (actions.DiscreteActions[0] == 2f)
            forwardAmount = -1;

        if (forwardAmount == 0 && turnAmount == 0) WalkAnimation(false);
        else WalkAnimation(true);

        rigid.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        if (HaveFood)
        {
            if (GoalDistance() < 4f)
                AddReward(0.01f);
        }
        else
        {
            if (FoodDistance() < 4f)
                AddReward(0.01f);
        }
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
        if (Input.GetKey(KeyCode.S))
        {
            forwardAction = 2;
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
    #endregion

    #region Animation

    private void WalkAnimation(bool isWalk)
    {
        if (isWalk == this.isWalk) return;
        this.isWalk = isWalk;
        anim.SetBool("IsWalk", this.isWalk);
    }

    private void AnimationTrigger(string str)
    {
        anim.SetTrigger(str);
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Wall"))
        {
            AddReward(-0.01f);
        }

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

        if (collision.transform.CompareTag("Player"))
        {
            if (HaveFood) return;
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            if (player.Delay) return;
            if (player.HaveFood)
            {
                getFood = player.GetHaveFood;
                player.LostFood();
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
        AnimationTrigger("Goal");
        AddReward(4);
        getFood.ResetObject();
        if (gameObject.tag == "Enemy")
        {
            GameManagement.Instance.AddEnemyScore(1);
        }
        else if (gameObject.tag == "Player")
        {
            GameManagement.Instance.AddPlayerScore(1);
        }
        getFood = null;
    }

    private void GetFood()
    {
        AnimationTrigger("Eat");
        StartCoroutine(DelayGet());
        AddReward(2);
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

    public void ResetEnemy()
    {
        getFood = null;
    }
}
