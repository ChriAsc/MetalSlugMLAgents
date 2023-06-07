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

    private float lastPositionX = 0f;
    // private float lastPositionY = 0f;
    private Vector2 lastPosition;
    // float distance_from_enemy = Mathf.Infinity;
    private float nextActionTime = 0.0f;
    private float period = 100.0f;
    private bool flagGrenade;

    RaycastHit2D hit;
    LayerMask _layerMask;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-8.61f,0.13f,0f);
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        lastPositionX = transform.localPosition.x;
        flagGrenade = false;

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
        if(moveV > 0.3f)
        {
            _playerController.MoveVertically(1);
        } 
        else if (moveV < 0.3f)
        {
            _playerController.MoveVertically(0);
        }
        // else if (moveV < -0.5f)
        // {
        //     _playerController.MoveVertically(0);
        //     _playerController.Crouch(jump,moveV);
        // }

        // if(moveH == 1 && (lastPositionX < transform.localPosition.x))
        // {
        //     lastPositionX = transform.localPosition.x;
        //     AddReward(0.1f);
        // }
        // if (lastPositionX > transform.localPosition.x)
        // {
        //     AddReward(-1f);
        // }

        _playerController.Jump(jump);
        // if(jump == 1)
        // {
        //     // Debug.Log("Salta");
        //     _playerController.Jump(1);
        // }
        // else if(jump == 0)
        // {
        //     // Debug.Log("Salta");
        //     _playerController.Jump(0);
        // }

        if(actions.DiscreteActions[1] == 1)
        {
            _playerController.Fire(1);

            if (moveV > 0.3)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 3.5f, _layerMask);
                //Method to draw the ray in scene for debug purpose
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up)*3.5f, Color.yellow);
                
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    AddReward(1f);
                }
            }
            else if(_playerController.GetFacing() == true)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 3.5f, _layerMask);
                //Method to draw the ray in scene for debug purpose
                // Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right)*3.5f, Color.white);
                    
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    AddReward(1f);
                }
                // else
                // {
                //     Vector2 down = new Vector2(1,-1);
                //     hit=Physics2D.Raycast(transform.position, transform.TransformDirection(down), 3.5f, _layerMask);
                //     //If the collider of the object hit is not NUll
                //     if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                //     {
                //         //Hit something, print the tag of the object
                //         Debug.Log("Ray hit: " + hit.collider.name);
                //         flagGrenade = SetFlag(flagGrenade);

                //         if(flagGrenade==true)
                //         {
                //             _playerController.ThrowGranate(1);
                //             flagGrenade = SetFlag(flagGrenade);
                //         }
                //         _playerController.ThrowGranate(0);
                //         // AddReward(1f);
                //         }
                //     //Method to draw the ray in scene for debug purpose
                //     Debug.DrawRay(transform.position, transform.TransformDirection(down)*3.5f, Color.white);
                // }
                
            }
            else if (_playerController.GetFacing() == false)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 3.5f, _layerMask);
                //Method to draw the ray in scene for debug purpose
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left)*3.5f, Color.cyan);
                
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    AddReward(1f);
                }
            }
        }
        else if (actions.DiscreteActions[1] == 0)
        {
            _playerController.Fire(0);
        }
        // else if (actions.DiscreteActions[1] == 2 && flagGrenade==true)
        // {
        //     _playerController.ThrowGranate(1);
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
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;


        //Block the player from moving if it's death
        if (_playerController.GetHealth()<=0)
            return;

        continuousActions[0] = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(KeyCode.Mouse0))
        {
            discreteActions[1] = 1;
        }
        
        if(Input.GetKey(KeyCode.Space))
        {
            discreteActions[2] = 1;
        }

        if(Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 0;
        } else if(Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 2;
        } else
        {
            discreteActions[0] = 1;
        }
    }
}
