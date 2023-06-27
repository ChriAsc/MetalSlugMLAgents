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
    float maxD = 1.5f;
    private float nextActionTime = 0.0f;
    private float period = 10.0f;
    private bool flag;
    private bool flagGrenade;
    private bool flagEnemy;

    RaycastHit2D hit;
    LayerMask _layerMask;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-8.61f,0.13f,0f);
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        lastPositionX = transform.localPosition.x;
        flag = false;
        flagGrenade = false;
        flagEnemy = false;

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

        flagEnemy = DetectEnemy();

        _playerController.MoveHorizontally(moveH);
        if ((moveH == -1) && (transform.localPosition.y == lastPosition.y))
        {
            flagEnemy = false;
        }
        else
        {
            lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
            flagEnemy = true;
        }

        if(moveV > 0.5f)
        {
            _playerController.MoveVertically(1);
            flagGrenade = false;
        } 
        else if (moveV < 0.5f)
        {
            _playerController.MoveVertically(0);
        }
        // else if (moveV < -0.5f)
        // {
        //     _playerController.MoveVertically(0);
        //     _playerController.Crouch(jump,1);
        //     flagGrenade = false;
        // }

        _playerController.Jump(jump);
        if(jump==1)
        {
            flag = SetFlag(flag);
        }

        if(actions.DiscreteActions[1] == 1)
        {
            // _playerController.ThrowGranate(0);
            _playerController.Fire(1);
            // flagGrenade = false;

            if (moveV > 0.5)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 3.5f, _layerMask);
                
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up)*3.5f, Color.yellow);
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    AddReward(0.5f);
                }
            }
            else if(_playerController.GetFacing() == true)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 3.5f, _layerMask);
                   
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right)*3.5f, Color.white);
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    flagGrenade = true;
                    
                    AddReward(0.5f);
                }
                
            }
            else if (_playerController.GetFacing() == false)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 3.5f, _layerMask);
                
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left)*3.5f, Color.cyan);
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    flagGrenade = true;
                    
                    AddReward(0.5f);
                }
            }
        }
        else if (actions.DiscreteActions[1] == 0)
        {
            // _playerController.ThrowGranate(0);
            flagGrenade = false;
            _playerController.Fire(0);
        }
        else if ((actions.DiscreteActions[1] == 2) && (flagGrenade == true) && (flagEnemy == true))
        {
            // _playerController.Fire(0);
            for (int i = 0; i < 1000000; i++) 
            {
            }
            flagGrenade = SetFlag(flagGrenade);
            flagEnemy = SetFlag(flagEnemy);

            _playerController.ThrowGranate(1);
            for (int i = 0; i < 1000000; i++) 
            {
            }
            // _playerController.ThrowGranate(0);
        }

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
            AddReward(-10f);
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
        //     // if((transform.localPosition.y > (lastPosition.y + 0.5f)) && (transform.localPosition.x >= lastPosition.x))
        //     if((transform.localPosition.y > (lastPosition.y + 0.5f)))
        //     {
        //         lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        //         Debug.Log("Reward for jumping");
        //         AddReward(1f);
        //         flag = SetFlag(flag);
        //     } else if (flag)
        //     {
        //         Debug.Log("This jump was not necessary");
        //         AddReward(-1f);
        //         flag = SetFlag(flag);
        //     }
        // }
        // if (collision.gameObject.CompareTag("Walkable") && flag==true)
        // {
        //     if((transform.localPosition.y > (lastPosition.y + 0.1f)))
        //     {
        //         lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        //         Debug.Log("Reward for jumping");
        //         AddReward(1f);
        //     } else
        //     {
        //         Debug.Log("This jump was not necessary");
        //         AddReward(-0.5f);
        //     }
        //     flag = SetFlag(flag);
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

    public bool DetectEnemy()
    {
        int n_enemies = FindClosestEnemies();
        if(n_enemies > 1)
        {
            return true;
        }
        return false;
    }

    public int FindClosestEnemies()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // GameObject closest = null;
        float distance = maxD;
        float position = transform.localPosition.x;
        int n_enemies = 0;
        foreach (GameObject enemy in enemies)
        {
            float currentD = Mathf.Abs(enemy.transform.position.x - position);
            if(currentD < distance)
            {
                n_enemies = n_enemies + 1;
            }
        }
        return n_enemies;
    }
    
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
        else if(Input.GetKey(KeyCode.Z))
        {
            discreteActions[1] = 2;
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
