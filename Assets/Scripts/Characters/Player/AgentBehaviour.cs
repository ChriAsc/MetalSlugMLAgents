using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;



public class AgentBehaviour : Agent
{

    PlayerController playerController;

    public override void Initialize(){
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Debug.Log("Agent initialized");

    }

    public override void OnEpisodeBegin(){
        Debug.Log("Resetting episode...");
        Reset();
    }
    private void Reset(){
        Initialize();
    }
    
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.position.x);
    }
    public override void OnActionReceived(ActionBuffers actions){
        if(actions.DiscreteActions[1]==0){
            Debug.Log("Jump");
            playerController.Jump();
        }else{
            Debug.Log("Crouch");
            playerController.Crouch();

        }
    }
    
}
