# MetalSlug with MLAgents
### Overview
- Objectives and requirements
- Agent description
- Class and sequence diagram
- Problems encountered and solutions
- Hyperparameters and best configuration
- Other configurations tried out
- Future developments and conclusions

### Introduction
Given an ever-increasing interest in Deep Reinforcement Learning in gaming, ML-Agents allows DRL to be implemented in any game developed in Unity.
- Objective
  - Implementation of ML-Agents in "Metal Slug" so that the agent is able to complete the level
- Requests:
  - Code editing to fit with ML-Agents
  - Creation of the agent class
  - Configuration of hyperparameters for training
  - Training

### Agent description
About Reinforcement Learning, is an area of machine learning concerned with how intelligent agents ought to take actions in an environment in order to maximize the notion of cumulative reward. The agent is the entity capable of perceiving its environment and it is anchored to the Player (Marco). 
Deep reinforcement learning combines artificial neural networks with a framework of reinforcement learning that helps software agents learn how to reach their goals. That is, it unites function approximation and target optimization, mapping states and actions to the rewards they lead to.
- Agent: an agent takes actions; for example. The algorithm is the agent. It may be helpful to consider that in life, the agent is you.
- Action: an action is the set of all possible moves the agent can make. An action is almost self-explanatory, but it should be noted that agents usually choose from a list of discrete, possible actions.
- State: a state is a concrete and immediate situation in which the agent finds itself; i.e. a specific place and moment, an instantaneous configuration that puts the agent in relation to other significant things such as tools, obstacles, enemies or prizes. It can the current situation returned by the environment, or any future situation.
- Reward: a reward is the feedback by which we measure the success or failure of an agent’s actions in a given state. From any given state, an agent sends output in the form of actions to the environment, and the environment returns the agent’s new state (which resulted from acting on the previous state) as well as rewards, if there are any. Rewards can be immediate or delayed. They effectively evaluate the agent’s action.
- Policy: The policy is the strategy that the agent employs to determine the next action based on the current state. It maps states to actions, the actions that promise the highest reward.

### Observation:

- Vector observation: agent positio
- Rycast observation: through emitted beams, the agent is able to observe specific elements around

### Actions:

- 1 continuous action: vertical movement (threshold)
- 3 discrete actions:
    - Horizontal movement (0: towards left, 1: rest, 2: towards right)
    - Fire (0: rest, 1: shoot, 2: throw grenade)
    - Jump (0: ground, 1: jump)

### Rewards
- Enemy hit: 1
- Killing an enemy: 10
- Collecting a collectible: 30
- Reaching a checkpoint: 20

### Penalties
- Player hit: - 50
- Game over: - 500
- Direction to the left: - 1
- Blank jump: -0.5
- Contact with the edge of the screen: -0.02

### Problems encountered
- Episode reset management
- Animations (grenade/shooter launch)
- Input system
- Grenade throwing
- Other code bugs (such as helicopters)
- Complex level design

### Solutions
- Code editing: Camera and GameManager
- Input parameter
    - int to indicate left or right movement
       (MoveHorizontally)
    - float to indicate the direction of gaze
       (MoveVertically)
    - int to indicate jump (Jump)
    - int to indicate firing (Fire)
    - int to indicate grenade throw (ThrowGranate)
- Timer and flag for throwing the grenade
- Edit of helicopter spawn code


#### Authors: Ascani Christian, Bedetta Alessandro, Recchi Giovanni

## Related Projects
Original game.

https://gamesareart.itch.io/metal-slug-in-unity

ML Agents Tool Kit

https://github.com/Unity-Technologies/ml-agents
