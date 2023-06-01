using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class PlayerAgent2 : Agent
{
    [SerializeField] private PlayerController _playerController;

    // GameObject lastEnemy = null;

    // private int countEnemy;
    private float lastPositionX = 0f;
    // private float lastPositionY = 0f;
    private Vector2 lastPosition;
    // float distance_from_enemy = Mathf.Infinity;
    private float nextActionTime = 0.0f;
    private float period = 100.0f;

    RaycastHit2D hit;
    LayerMask _layerMask;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-8.61f,0.13f,0f);
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        lastPositionX = transform.localPosition.x;

        // lastPositionY = transform.localPosition.y;

        _layerMask = LayerMask.GetMask("Enemy", "IgnoreRoof", "Building");
    }
        
    private void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            // flagGrenade = SetFlag(flagGrenade);
            Debug.Log("Total Reward:    " + GetCumulativeReward());
        
        }
    }

    // private void FixedUpdate()
    // {
        // if(lastPositionX < transform.localPosition.x)
        // {
        //     lastPositionX = transform.localPosition.x;
        //     AddReward(10f);
        // }
        // if (lastPositionX > transform.localPosition.x)
        // {
        //     AddReward(-0.1f);
        // }

    //     if (actualCount != countEnemy)
    //     {
    //         Debug.Log("Someone was killed!");
    //         AddReward(10f);
    //         countEnemy = actualCount;
    //     } else
    //     {
    //         AddReward(-0.01f);
    //     }
    // }


    public void registerReward(float rew){
        Debug.Log("Total Reward before:    " + GetCumulativeReward());
        AddReward(rew);
        Debug.Log("Total Reward after:    " + GetCumulativeReward());
    }


    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(goalTransform_2.localPosition.x);  // goal x reference
        // sensor.AddObservation(goalTransform_3.localPosition.x);  // goal y reference
        // sensor.AddObservation(goalTransform_4.localPosition.x);  // goal y reference
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // float moveH = actions.ContinuousActions[0];
        float moveV = actions.ContinuousActions[0];
        
        int moveH = actions.DiscreteActions[0] - 1;
        // int moveV = actions.DiscreteActions[3];
        int jump = actions.DiscreteActions[2];

        _playerController.MoveHorizontally(moveH);
        if(moveV > 0.3)
        {
        _playerController.MoveVertically(1);
        } else
        {
            _playerController.MoveVertically(0);
        }

        // if(moveH == 1 && (lastPositionX < transform.localPosition.x))
        // {
        //     lastPositionX = transform.localPosition.x;
        //     AddReward(0.1f);
        // }
        // if (lastPositionX > transform.localPosition.x)
        // {
        //     AddReward(-1f);
        // }
        
        if(jump == 1)
        {
            // Debug.Log("Salta");
            _playerController.Jump();
        }

        if(actions.DiscreteActions[1] == 1)
        {
            _playerController.Fire(1);
            
            if(_playerController.GetFacing() == true)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 4f, _layerMask);
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Collision with: " + hit.collider.name);
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right)*4f, Color.white);
                
                    AddReward(5f);
                }
            }
            else if (_playerController.GetFacing() == false)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 4f, _layerMask);
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Collision with: " + hit.collider.name);
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left)*4f, Color.cyan);
                
                    AddReward(5f);
                }
            }
            else if (moveV > 0.3)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 4f, _layerMask);
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Collision with: " + hit.collider.name);
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up)*4f, Color.yellow);
                
                    AddReward(5f);
                }
            }

                    // else if(hit.collider != null){
            //         Debug.Log("Collision with: " + hit.collider.tag);
            //         AddReward(-0.01f);
            // }
        }
        else if (actions.DiscreteActions[1] == 0)
        {
            _playerController.Fire(0);
        }
        // if(actions.DiscreteActions[1] == 1 && flagGrenade)
        // {
        //     _playerController.ThrowGranate();
        //     flagGrenade = SetFlag(flagGrenade);
        // }

    }

    public bool SetFlag(bool flag)
    {
        return !flag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Collectible!");
            AddReward(5f);
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint!");
            AddReward(10f);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-5f);
        }
        // if (collision.gameObject.CompareTag("Bullet"))
        // {
        //     Debug.Log("Hit by a bullet!");
        //     AddReward(-1f);
        // }
        // if (collision.gameObject.CompareTag("Marco Boat"))
        // {
        //     Debug.Log("Marco Boat!");
        //     AddReward(5f);
        // }
        // if (collision.gameObject.CompareTag("Water Dead"))
        // {
        //     AddReward(-1000f);
        //     EndEpisode();
        // }
        // if (collision.gameObject.CompareTag("Walkable"))
        // {
        //     if((transform.localPosition.y > (lastPosition.y + 0.1)) && (transform.localPosition.x >= lastPosition.x))
        //     {
        //         lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        //         Debug.Log("Reward for jumping");
        //         AddReward(0.1f);
        //     } else
        //     {
        //         AddReward(-0.001f);
        //     }
        // }
        // if (collision.gameObject.CompareTag("Granate"))
        // {
        //     Debug.Log("Hit!");
        //     AddReward(-50f);
        // }
        // if (_playerController.GetHealth() <= 0f)
        // {
        //    AddReward(-100);
        //    EndEpisode();
        // }        
    }

    // public bool DetectEnemy()
    // {
    //     GameObject enemy = FindClosestEnemy();
    //     if(enemy != null)
    //     {
    //         distance_from_enemy = Mathf.Abs(transform.localPosition.x - enemy.transform.position.x);
    //         if (distance_from_enemy < maxD)
    //         {
    //             // Debug.Log("Distance from the enemy:" + distance_from_enemy);
    //             return true;
    //         }
        
    //     }
    //     return false;
    // }

    // public GameObject FindClosestEnemy()
    // {
    //     GameObject[] enemies;
    //     enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //     GameObject closest = null;
    //     float distance = maxD;
    //     float position = transform.localPosition.x;
    //     foreach (GameObject enemy in enemies)
    //     {
    //         float currentD = Mathf.Abs(enemy.transform.position.x - position);
    //         if(currentD < distance)
    //         {
    //             closest = enemy;
    //             distance = currentD;
    //         }
    //     }
    //     return closest;
    // }
    
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
