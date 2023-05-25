using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class PlayerAgent : Agent
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform goalTransform_1;
    [SerializeField] private Transform goalTransform_2;
    [SerializeField] private Transform goalTransform_3;
    [SerializeField] private Transform goalTransform_4;

    private bool flag_1 = true;
    private bool flag_2 = true;
    private bool flag_3 = true;
    private bool flag_4 = true;
    private bool flag_enemy = true;

    GameObject lastEnemy = null;
    private float lastPositionX = 0f;
    // private float lastPositionY = 0f;
    private Vector2 lastPosition;
    float distance_from_enemy = Mathf.Infinity;
    float maxD = 33.0f;
    private float nextActionTime = 0.0f;
    public float period = 100.0f;

    RaycastHit2D hit;

    // private int speed = 5;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-27.53f,1.54f,0f);
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        lastPositionX = transform.localPosition.x;
        // lastPositionY = transform.localPosition.y;
    }
        
    private void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            Debug.Log("Total Reward:    " + GetCumulativeReward());
        }

    }

    private void FixedUpdate()
    {
        hit=Physics2D.Raycast(transform.localPosition, Vector2.right);

        //If the collider of the object hit is not NUll
        if(hit.collider != null && hit.collider.tag == "Enemy")
        {
            //Hit something, print the tag of the object
            Debug.Log("Enemy: " + hit.collider.name);
        }

        //Method to draw the ray in scene for debug purpose
        Debug.DrawRay(transform.localPosition, Vector2.right, Color.red);
        
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(goalTransform_1.localPosition.x);  // goal x reference
        sensor.AddObservation(goalTransform_2.localPosition.x);  // goal x reference
        sensor.AddObservation(goalTransform_3.localPosition.x);  // goal y reference
        sensor.AddObservation(goalTransform_4.localPosition.x);  // goal y reference
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // float moveH = actions.ContinuousActions[0];
        // float moveV = actions.ContinuousActions[1];
        
        int moveH = actions.DiscreteActions[0] - 1;
        int jump = actions.DiscreteActions[2];

        _playerController.MoveHorizontally(moveH);

        if(moveH == 1 && (lastPositionX < transform.localPosition.x))
        {
            lastPositionX = transform.localPosition.x;
            AddReward(1f);
        }
        if (moveH == -1)
        {
            AddReward(-0.1f);
        }
        
        // if(lastPositionX < transform.localPosition.x)
        // {
        //     Debug.Log("Moving on ...");
        //     lastPositionX = transform.localPosition.x;
        //     AddReward(0.01f);
        // } else
        // {
        //     Debug.Log("Wrong direction!");
        //     AddReward(-1f);
        // }
        
        if(actions.DiscreteActions[1] == 0)
        {
            _playerController.Fire();
            // AddReward(0.001f);
        }
    
        // if ((actions.DiscreteActions[1] - Random.Range(0f,1f)) > 0.9f)
        // {
        //     Debug.Log("Bomb");
        //     _playerController.ThrowGranate();
        //     if (DetectEnemy() < previous_enemies)
        //     {
        //         AddReward(5f);
        //     }
        //     else AddReward(-5f);
        // }
    
        if(jump == 1)
        {
            // Debug.Log("Salta");
            _playerController.Jump();
        }

        if(transform.localPosition.x == goalTransform_1.position.x && flag_1 == true)
        {
            Debug.Log("First cartel!");
            AddReward(5f);
            flag_1 = SetFlag(flag_1);
        }
        if(transform.localPosition.x == goalTransform_2.position.x && flag_2 == true)
        {
            Debug.Log("Second cartel!");
            AddReward(10f);
            flag_2 = SetFlag(flag_2);
        }
        if(transform.localPosition.x == goalTransform_3.position.x && flag_3 == true)
        {
            Debug.Log("Boat!");
            AddReward(15f);
            flag_3 = SetFlag(flag_3);
        }
        if(transform.localPosition.x == goalTransform_4.position.x && flag_4 == true)
        {
            Debug.Log("Bridge!");
            AddReward(15f);
            flag_4 = SetFlag(flag_4);
        }

        if(DetectEnemy())
        {           
            GameObject actualEnemy = FindClosestEnemy();
            if (actualEnemy == lastEnemy)
            {
                // Debug.Log("The closest enemy is the same!");
                // AddReward(-0.1f);
            } else
            {
                Debug.Log("Closest enemy: " + actualEnemy);
                // AddReward(5f);
                lastEnemy = actualEnemy;
                flag_enemy = SetFlag(flag_enemy);
            }
        } else if (!DetectEnemy() && flag_enemy)
        {
            Debug.Log("No enemies around!");
            AddReward(5f);
            flag_enemy = SetFlag(flag_enemy);
        }


        // if(transform.localPosition.x == goalTransform_4.localPosition.x && flag_4 == true)
        // {
        //     Debug.Log("Bridge!");
        //     AddReward(20f);
        //     flag_4 = SetFlag(flag_4);
        // }
        // if(actions.DiscreteActions[2] == 0)
        // {
        //     _playerController.Crouch();
        // }

    }

    public bool SetFlag(bool flag)
    {
        return !flag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walkable"))
        {
            if((transform.localPosition.y > (lastPosition.y + 0.1)) && (transform.localPosition.x >= lastPosition.x))
            {
                lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                Debug.Log("Reward for jumping");
                AddReward(0.1f);
            } else
            {
                AddReward(-0.001f);
            }
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Hit!");
            AddReward(-1f);
        }

        if (collision.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Collectible!");
            AddReward(3f);
        }

        if (collision.gameObject.CompareTag("Marco Boat"))
        {
            // Debug.Log("Marco Boat!");
            AddReward(5f);
        }

        if (collision.gameObject.CompareTag("Water Dead"))
        {
            AddReward(-1000f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-10f);
        }

        if (_playerController.GetHealth() <= 0f)
        {
            AddReward(-100);
            EndEpisode();
        }        
    }


    public bool DetectEnemy()
    {
        GameObject enemy = FindClosestEnemy();
        if(enemy != null)
        {
            distance_from_enemy = Mathf.Abs(transform.localPosition.x - enemy.transform.position.x);
            if (distance_from_enemy < maxD)
            {
                // Debug.Log("Distance from the enemy:" + distance_from_enemy);
                return true;
            }
        
        }
        return false;
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = maxD;
        float position = transform.localPosition.x;
        foreach (GameObject enemy in enemies)
        {
            float currentD = Mathf.Abs(enemy.transform.position.x - position);
            if(currentD < distance)
            {
                closest = enemy;
                distance = currentD;
            }
        }
        return closest;
    }


    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //     ActionSegment<int> discreteActions = actionsOut.DiscreteActions;


    //     //Block the player from moving if it's death
    //     if (_playerController.GetHealth()<=0)
    //         return;

    //     continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //     continuousActions[1] = Input.GetAxisRaw("Vertical");

    //     if(Input.GetKey(KeyCode.Mouse0))
    //     {
    //         discreteActions[1] = 1;
    //     }
        
    //     if(Input.GetKey(KeyCode.Space))
    //     {
    //         discreteActions[2] = 1;
    //     }
        
    //     _playerController.FlipShoot();
    // }
}
