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

    CameraManager _camManager = new CameraManager();
    // Vector used to store player's last position
    private Vector2 lastPosition;
    // Distance used in enemy detection
    float maxD = 2f;

    private float nextActionTime = 0.0f;
    private float period = 50.0f;

    // Flags
    private bool flagJump;
    private bool flagEnemy;
    private int n_enemies;
    private bool firing;
    private int timer;

    // Raycast variables
    RaycastHit2D hit;
    LayerMask _layerMask;

    public override void OnEpisodeBegin()
    {
        // Initialize player's position at the beginning of the episode
        transform.localPosition = new Vector3(-8.5f,0f,0f);
        //transform.localPosition = new Vector3(6,71f,0.64f,0f);
        //_camManager.AfterFirstHeli;
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        // Flags set to false
        flagJump = false;
        flagEnemy = false;
        n_enemies = 0;
        // Admitted layers
        _layerMask = LayerMask.GetMask("Enemy", "IgnoreRoof", "Building");
    }
        
    private void Update()
    {
        // After a period cumulative reward is printed
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            Debug.Log("Total Reward:    " + GetCumulativeReward());
        }
        timer = timer + 1;
    }

    private void FixedUpdate()
    {
        // if the player goes backwards, a penalty is applied
        if (transform.localPosition.x < (lastPosition.x - 0.25f))
        {
            // player's position update
            lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
            AddReward(-1f);
        }
        else if (transform.localPosition.x > (lastPosition.x))
        {
            // player's position update
            lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        }
        
    }

    // Function called from other classes in order to add a reward/penalty
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
        // int moveV = actions.DiscreteActions[3];
        // float moveH = actions.ContinuousActions[0];

        // Continuous action used to move vertically
        float moveV = actions.ContinuousActions[0];

        // Discrete action used to move horizontally
        int moveH = actions.DiscreteActions[0] - 1;

        // Discrete action used to jump
        int jump = actions.DiscreteActions[2];

        // Check if there are at least 2 enemies around
        flagEnemy = DetectEnemies();
        // Store the number of the enemies around
        n_enemies = FindClosestEnemies();
        // Check if the player is firing
        firing = _playerController.GetFiring();

        // Player moves (-1: left, 0: stop, 1: right)
        _playerController.MoveHorizontally(moveH);

        // Player sees above it only if moveV > 0.5
        if(moveV > 0.5f)
        {
            _playerController.MoveVertically(1);
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

        // The player is going to jump only if he is not already jumping
        if(jump==1 && flagJump == false)
        {

            _playerController.Jump(jump);
            flagJump = true;
        }
        else
        {
            // Set the flag to false as he is not jumping anymore
            flagJump = false;
        }

        if(actions.DiscreteActions[1] == 1)
        {
            _playerController.ThrowGranate(0);
            // Fire
            // If the player was not firing before, then a ray is traced to check if an enemy is above him
            if (moveV > 0.5 && !firing)
            {
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 3.5f, _layerMask);
                
                // If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    // Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up)*3.5f, Color.yellow);
                    // Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    // Since a ray hit an object with tag "Enemy", it means the bullet is going to hit that enemy
                    AddReward(2f);
                    _playerController.Fire(1);  
                }else{
                    if(hit.collider != null && hit.collider.gameObject.tag!="Enemy"){
                        Debug.Log("Penalty for shooting randomly");
                        AddReward(-0.05f);
                    }
                }
            }
            else if(_playerController.GetFacing() == true && !firing)
            {
                // If the player was not firing before, then a ray is traced to check if an enemy is to the right
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 3.5f, _layerMask);
                   
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right)*3.5f, Color.white);
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    // Since a ray hit an object with tag "Enemy", it means the bullet is going to hit that enemy
                    AddReward(2f);
                    _playerController.Fire(1);  
                }else{
                    if(hit.collider != null && hit.collider.gameObject.tag!="Enemy"){
                        Debug.Log("Penalty for shooting randomly");
                        AddReward(-0.05f);
                    }
                }
                
            }
            else if (_playerController.GetFacing() == false && !firing)
            {
                // If the player was not firing before, then a ray is traced to check if an enemy is to the left
                hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), 3.5f, _layerMask);
                
                //If the collider of the object hit is not NUll
                if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
                {
                    //Method to draw the ray in scene for debug purpose
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.left)*3.5f, Color.cyan);
                    //Hit something, print the tag of the object
                    Debug.Log("Ray hit: " + hit.collider.name);
                    
                    // Since a ray hit an object with tag "Enemy", it means the bullet is going to hit that enemy
                    AddReward(2f);
                    _playerController.Fire(1);  
                }else{
                    if(hit.collider != null && hit.collider.gameObject.tag!="Enemy"){
                        Debug.Log("Penalty for shooting randomly");
                        AddReward(-0.05f);
                    }
                }
            }
        }
        else if (actions.DiscreteActions[1] == 0)
        {
            // Player does not anything
            _playerController.ThrowGranate(0);
            _playerController.Fire(0);
        }
        // If the player is free to throw a grenade and at least 2 enemies are around
        else if ((actions.DiscreteActions[1] == 2) && !firing && (flagEnemy == true) && timer > 500)
        {
            // _playerController.ThrowGranate(0);
            flagEnemy = false;
            
            // Player throws a grenade
            _playerController.ThrowGranate(1);
            // _playerController.ThrowGranate(0);
            timer = 0;
        }

        // If at least one enemy was killed, add a reward
        int actual = FindClosestEnemies();
        if (actual < n_enemies)
        {
            AddReward(2f);
        }

    }

    // public bool SetFlag(bool flag)
    // {
    //     return !flag;
    // }

    // Add rewards if the player collide with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Collectible!");
            AddReward(3f);
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint!");
            AddReward(3f);
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
    }

    // Function that returns true if at least 2 enemies are near to the player
    public bool DetectEnemies()
    {
        int n_enemies = FindClosestEnemies();
        if(n_enemies > 1)
        {
            return true;
        }
        return false;
    }

    // Function that returns the number of enemies around 
    public int FindClosestEnemies()
    {
        // GameObject used to store the deteected enemies
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // float position = transform.localPosition.x;
        int n_enemies = 0;

        foreach (GameObject enemy in enemies)
        {
            float currentD = Mathf.Abs(enemy.transform.position.x - transform.localPosition.x);
            if(currentD < maxD)
            {
                // Increase the number of enemies near to the player
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
