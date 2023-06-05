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
    // [SerializeField] private Transform goalTransform_2;
    // [SerializeField] private Transform goalTransform_3;
    // [SerializeField] private Transform goalTransform_4;

    private bool flag_1 = true;
    // private bool flag_2 = true;
    // private bool flag_3 = true;
    // private bool flag_4 = true;

    // GameObject lastEnemy = null;

    private int countEnemy;
    private float lastPositionX;
    // private float lastPositionY = 0f;
    private Vector2 lastPosition;
    float maxD = 1f;
    // float distance_from_enemy = Mathf.Infinity;
    private float nextActionTime = 0.0f;
    private float period = 50.0f;
    // private bool flagGrenade = false;

    // RaycastHit2D hit;
    // LayerMask _layerMask;

    public override void OnEpisodeBegin()
    {
        
        countEnemy = checkEnemy();
        lastPositionX = transform.localPosition.x;
        transform.localPosition = new Vector3(-27.53f,1.54f,0f);
        lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        lastPositionX = transform.localPosition.x;

        // lastPositionY = transform.localPosition.y;
        // _layerMask = LayerMask.GetMask("Enemy", "Building", "Collectible");
    }
        
    private void Update()
    {
        if(transform.position.x == goalTransform_1.position.x && flag_1 == true)
        {
            Debug.Log("First cartel!");
            AddReward(5f);
            flag_1 = SetFlag(flag_1);
        }

        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            // flagGrenade = SetFlag(flagGrenade);
            Debug.Log("Total Reward:    " + GetCumulativeReward());
        
        }

        if (_playerController.GetHealth() <= 0f)
        {
           AddReward(-100f);
           EndEpisode();
        }      
    }

    private void FixedUpdate()
    {

        int actualCount = checkEnemy();
        if (actualCount != countEnemy)
        {
            Debug.Log("Someone was killed!");
            AddReward(10f);
            countEnemy = actualCount;
        } else
        {
            AddReward(-0.01f);
        }

        if(lastPositionX < transform.localPosition.x)
        {
            lastPositionX = transform.localPosition.x;
            AddReward(10f);
        }
        if(lastPosition.x > transform.localPosition.x)
        {
            AddReward(-10f);
            lastPosition.x = transform.localPosition.x;
        }

        if(checkBoat())
        {
            _playerController.ThrowGranate();
        }
    }

    public int checkEnemy()
    {
        GameObject[] enemies;
        int count;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        count = enemies.Length;
        return count;
    }

    public bool checkBoat()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float position = transform.localPosition.x;
        foreach (GameObject enemy in enemies)
        {
            if(enemy.name == "EnemyBoat")
            {
                if (Mathf.Abs(enemy.transform.position.x - transform.localPosition.x)< maxD)
                return true;
            }
        }
        return false;
    }

    public void registerReward(float rew){
        AddReward(rew);
    }


    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goalTransform_1.localPosition.x);  // goal x reference
        // sensor.AddObservation(goalTransform_2.localPosition.x);  // goal x reference
        // sensor.AddObservation(goalTransform_3.localPosition.x);  // goal y reference
        // sensor.AddObservation(goalTransform_4.localPosition.x);  // goal y reference
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // float moveH = actions.ContinuousActions[0];
        // float moveV = actions.ContinuousActions[1];
        
        int moveH = actions.DiscreteActions[0] - 1;
        int jump = actions.DiscreteActions[2];

        _playerController.MoveHorizontally(moveH);

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

        // if(transform.localPosition.x == goalTransform_2.position.x && flag_2 == true)
        // {
        //     Debug.Log("Second cartel!");
        //     AddReward(10f);
        //     flag_2 = SetFlag(flag_2);
        // }
        // if(transform.localPosition.x == goalTransform_3.position.x && flag_3 == true)
        // {
        //     Debug.Log("Boat!");
        //     AddReward(15f);
        //     flag_3 = SetFlag(flag_3);
        // }
        // if(transform.localPosition.x == goalTransform_4.localPosition.x && flag_4 == true)
        // {
        //     Debug.Log("Fish!");
        //     AddReward(15f);
        //     flag_4 = SetFlag(flag_4);
        // }

        if(actions.DiscreteActions[1] == 0)
        {

            // hit=Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 3.5f, _layerMask);
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right)*3.5f, Color.green);
            
            _playerController.Fire(0);

            //If the collider of the object hit is not NUll
            // if(hit.collider != null && hit.collider.gameObject.tag=="Enemy")
            // {
            //     //Hit something, print the tag of the object
            //     Debug.Log("Collision with: " + hit.collider.name);
            //     // Debug.Log("Position: " + hit.collider.transform.position);
            //     //Method to draw the ray in scene for debug purpose
            //     AddReward(1f);
            // }else if(hit.collider != null){
            //         Debug.Log("Collision with: " + hit.collider.tag);
            //         AddReward(-0.01f);
            // }
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
        if (collision.gameObject.CompareTag("Walkable"))
        {
            if((transform.localPosition.y > (lastPosition.y + 0.1)))
            {
                lastPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                Debug.Log("Reward for jumping");
                AddReward(10f);
            } else
            {
                AddReward(-0.0001f);
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
            AddReward(30f);
        }

        if (collision.gameObject.CompareTag("Water Dead"))
        {
            AddReward(-1000f);
            //EndEpisode();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            AddReward(-30f);
        }
 
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
