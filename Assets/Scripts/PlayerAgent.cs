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
    private float lastPositionY = 0f;
    private float positionYbefore = 0f;
    float distance_from_enemy = Mathf.Infinity;
    float maxD = 30.0f;
    
    // private int speed = 5;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-27.53f,1.54f,0f);
        lastPositionX = transform.localPosition.x;
        lastPositionY = transform.localPosition.y;
        positionYbefore = transform.localPosition.y;
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
        float jump = actions.ContinuousActions[0];
        // float moveH = actions.ContinuousActions[0];
        // float moveV = actions.ContinuousActions[1];
        
        int moveH = actions.DiscreteActions[0] - 1;
        
        _playerController.MoveHorizontally(moveH);
        if(lastPositionX < transform.localPosition.x)
        {
            // Debug.Log("Moving on ...");
            AddReward(1f);
            lastPositionX = transform.localPosition.x;
        } else
        {
            // Debug.Log("Wrong direction!");
            AddReward(-3f);
        }
        

        // if(moveH == 1)
        // {
        //     AddReward(0.3f);
        // }
        // if(moveH == -1)
        // {
        //     AddReward(-1f);
        // }

        // if(moveH > 0f)
        // {
        //     _playerController.MoveHorizontally(1);
        //     AddReward(2f);
        // }
        // if(moveH == 0f)
        // {
        //     _playerController.MoveHorizontally(0);
            
        // }
        // if(moveH < 0f)
        // {
        //     _playerController.MoveHorizontally(-1);
        // }
        
        // if(moveV > 0f)
        // Debug.Log("MoveV:" + moveV);
        // {
        //     _playerController.MoveVertically(1);
        // }
        // if(moveV == 0f)
        // {
        //     _playerController.MoveVertically(0);
        // }
        if(actions.DiscreteActions[1] == 0)
        {
            _playerController.Fire();
            // AddReward(-2f);
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
        
        if(jump > 0.3f)
        {
            // Debug.Log("Salta");
            _playerController.Jump();
        }
        
        // if(transform.localPosition.y == lastPositionY)
        // {
        //     AddReward(-2f);
        // }

        if(transform.localPosition.x == goalTransform_1.localPosition.x && flag_1 == true)
        {
            Debug.Log("First cartel!");
            AddReward(5f);
            flag_1 = SetFlag(flag_1);
        }
        if(transform.localPosition.x == goalTransform_2.localPosition.x && flag_2 == true)
        {
            Debug.Log("Second cartel!");
            AddReward(10f);
            flag_2 = SetFlag(flag_2);
        }
        if(transform.localPosition.x == goalTransform_3.localPosition.x && flag_3 == true)
        {
            Debug.Log("Boat!");
            AddReward(15f);
            flag_3 = SetFlag(flag_3);
        }
        if(transform.localPosition.x == goalTransform_4.localPosition.x && flag_4 == true)
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
                AddReward(-1f);
            } else
            {
                Debug.Log("Closest enemy: " + actualEnemy);
                AddReward(3f);
                lastEnemy = actualEnemy;
                flag_enemy = SetFlag(flag_enemy);
            }
            
        } else if (!DetectEnemy() && flag_enemy)
        {
            Debug.Log("No enemies around! " + GetCumulativeReward());
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

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     LayerMask collectible = LayerMask.GetMask("Collectible");

    //     if(collision.IsTouchingLayers(collectible))
    //     {
    //         Debug.Log("Collectible Layer!");
    //         AddReward(10f);
    //     }
    // }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Walkable"))
        // {
        //     lastPositionY = transform.localPosition.y;
        // }
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
            AddReward(1f);
        }

        if (collision.gameObject.CompareTag("Water Dead"))
        {
            AddReward(-50f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-5f);
        }

        if (_playerController.GetHealth() <= 0f)
        {
            AddReward(-50f);
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
                Debug.Log("Distance from enemy:" + distance_from_enemy);
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