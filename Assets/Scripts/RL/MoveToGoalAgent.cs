using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using FriedRice;
using TMPro;

// Approach [B]
public class MoveToGoalAgent : Agent
{
    // Intern
    private EnemyFieldOfView fov;
    private float rotationValue = 0;
    private float moveValue = 0;
    private Vector3 lastTargetPosition;
    private float moveSpeedX;
    private float moveSpeedZ;
    private float timeWhenTargetChangeAxis;
    private int axisToMoveTarget; // 1 - X, 2 - Z
    private Transform agentInitialPosition;

    [SerializeField]
    private Transform targetTransform;

    [Header("Configurations")]
    [SerializeField]
    private bool inTraining;
    [SerializeField]
    private bool enableLogs;

    [Header("Training settings")]
    [SerializeField]
    private bool enableMoveOfAgent;
    [SerializeField]
    private bool enableDoorReward;
    [SerializeField]
    private int episodesToChangeDoorReward;
    [SerializeField]
    private bool enableRotationOfAgent;
    [SerializeField]
    private bool targetSpawnRandomly;
    [SerializeField]
    private bool agentSpawnRandomly;
    [Tooltip("When unchecked the agent will be always on the left side")]
    [SerializeField]
    private bool agentAndTargetInRandomSides;
    [SerializeField]
    private bool moveTargetRandomly;
    [SerializeField]
    private bool moveTargetOnOneAxis;
    [Tooltip("Must be checked only if moveTargetOnOneAxis is also checked")]
    [SerializeField]
    private bool moveTargetOnZ;
    [Tooltip("Must be checked only if moveTargetOnOneAxis is also checked")]
    [SerializeField]
    private bool moveTargetOnX;
    [SerializeField]
    private float targetMinSpeed;
    [SerializeField]
    private float targetMaxSpeed;
    [SerializeField]
    private float groundScaleX = 5f;
    [SerializeField]
    private float groundScaleZ = 5f;
    [SerializeField]
    private float distanceFromWall = 2f;


    [Header("Agent general settins")]
    [SerializeField] 
    private float rotationSpeed;
    [SerializeField]
    private float moveSpeed;

    [Header("Heuristic settings")]
    [SerializeField]
    [Range(0,1)]
    private float heuristicMoveSpeed;
    [SerializeField]
    [Range(1, 2)]
    private float heuristicRotationSpeed;

    // Visual output of environment based on certain events
    [SerializeField]
    private MeshRenderer groundMeshRenderer;
    [SerializeField]
    private Material[] groundMats;

    private AgentStatus agentStatus;
    private Animator animator;


    private void Start()
    {
/*        agentStatus.SetEpisodeCounter(0);
        agentStatus.SetWinsCounter(0);
        agentStatus.SetOutsideMapCounter(0);*/

        fov = GetComponent<EnemyFieldOfView>();
        timeWhenTargetChangeAxis = 0;

        if (!inTraining)
        {
            animator = GetComponent<Animator>();
        }
        agentInitialPosition = transform;
    }

    public override void OnEpisodeBegin()
    {
        
        if(inTraining)
        {
            
            SetReward(0);
/*            agentStatus.SetEpisodeCounter(agentStatus.GetEpisodeCounter() + 1);*/
            lastTargetPosition = Vector3.zero;  
            MLAgentsTraining.InitializeEnvironment(transform, agentInitialPosition, targetTransform, groundScaleX: groundScaleX, groundScaleZ: groundScaleZ, distanceFromWall: distanceFromWall, randomSides: agentAndTargetInRandomSides, targetSpawnRandomly: targetSpawnRandomly, agentSpawnRandomly: agentSpawnRandomly, resetAgentRotation: false);
            if (moveTargetRandomly)
            {
                moveSpeedX = Random.Range(targetMinSpeed, targetMaxSpeed);
                moveSpeedZ = Random.Range(targetMinSpeed, targetMaxSpeed);
            }
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Initialized with a number outside of the interval [-1,1] interval
        float dotProduct = -2;
        float distanceBetweenAgentAndTarget = -1f;
        sensor.AddObservation(fov.TargetInFOV());

        if (fov.TargetInFOV())
        { 
            if (enableMoveOfAgent)
            {
                // If agent has the target in sight give him the actual distance between him and the target
                distanceBetweenAgentAndTarget = Vector3.Distance(transform.position, targetTransform.position);
                sensor.AddObservation(distanceBetweenAgentAndTarget);
            }
                
            // Remember the last position of the target, based on the last observation
            lastTargetPosition = targetTransform.localPosition;

            Vector3 a = targetTransform.localPosition - transform.localPosition;
            Vector3 b = transform.right; 
            a.Normalize();

            // 0 center, -1 left, 1 right, 2 - not seen
            dotProduct = Vector3.Dot(a, b);
            // Scale by fow to adapt for any given fow radius
            dotProduct /= (Mathf.Sin(fov.GetViewRadius()) / 2.0f);
            
        }
        else if (enableMoveOfAgent)
        {
            // If agent doesn't have the target in sight give him as the distance between him and taget an imposible value (like -1)
            sensor.AddObservation(-1);
        }

        if (enableRotationOfAgent)
        {
            sensor.AddObservation(dotProduct);
        }
        
        
        if (inTraining)
        {
            // Exponential growth of reward based of how centered is the view
            AddReward(-Mathf.Log(Mathf.Abs(dotProduct), 5)/2.0f);
            if(distanceBetweenAgentAndTarget != -1)
            {
                float logBase = 1.1f;
                float c = 44.6f;
                // The logBase and the constant c should complain to the formula: logBase^c = maxSight + 0.01, where maxSight is the length of the radius
                AddReward(-Mathf.Log(distanceBetweenAgentAndTarget + 0.01f, logBase) + c);
            }
                

        }
        
        Utils.Log("Dot product: " + dotProduct.ToString(), enableLogs);
        Utils.Log("Distance: " + distanceBetweenAgentAndTarget, enableLogs);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        if (enableRotationOfAgent)
        {
            rotationValue = actions.ContinuousActions[0];
        }
        if (enableMoveOfAgent)
        {
            moveValue = actions.ContinuousActions[1];
        }
        

        Actions.MoveAndRotate(transform, rotationValue, rotationSpeed, moveValue, moveSpeed, enableMove: enableMoveOfAgent);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxis("Horizontal");
        continousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-30f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-30f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door") && enableDoorReward)
        {
            AddReward(30f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && inTraining)
        {
            AddReward(10f);
        }
    }

    private void Update()
    {

        if (moveTargetRandomly)
        {
            MoveTargetRandomly(targetTransform, groundScaleX: groundScaleX, groundScaleZ: groundScaleZ, distanceFromWall: distanceFromWall, minMoveSpeed: targetMinSpeed, maxMoveSpeed: targetMaxSpeed);
        }
        if (moveTargetOnOneAxis)
        {
            MoveTargetOnOneAxis(targetTransform, groundScaleX: groundScaleX, groundScaleZ: groundScaleZ, distanceFromWall: distanceFromWall, minMoveSpeed: targetMinSpeed, maxMoveSpeed: targetMaxSpeed);
        }

        if (!inTraining && animator != null)
        {
            AnimateMovement();
        }

        if(transform.rotation.x != 0 || transform.rotation.z != 0)
        {
            transform.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
        }
        
        if(CompletedEpisodes == episodesToChangeDoorReward && episodesToChangeDoorReward != 0)
        {
            enableDoorReward = false;
        }
    }

    private void MoveTargetOnOneAxis(Transform targetTransform, float groundScaleX = 1, float groundScaleZ = 1,
            float distanceFromWall = 1f, float minMoveSpeed = 1f, float maxMoveSpeed = 5f)
    {
        float minPosX = -groundScaleX * 5 + distanceFromWall;
        float minPosZ = -groundScaleZ * 5 + distanceFromWall;
        float maxPosX = groundScaleX * 5 - distanceFromWall;
        float maxPosZ = groundScaleZ * 5 - distanceFromWall;

        if (timeWhenTargetChangeAxis <= 0)
        {
            if (moveTargetOnX)
            {
                axisToMoveTarget = 1;
            } 
            else if (moveTargetOnZ)
            {
                axisToMoveTarget = 2;
            } 
            else if (moveTargetOnX && moveTargetOnZ)
            {
                axisToMoveTarget = Random.Range(1, 3);
            }
            else
            {
                Debug.LogError("moveTargetOnX or/and moveTargetOnZ must be checked when moveTargetOnOneAxis is enabled.");
            }
            
            timeWhenTargetChangeAxis = Random.Range(10f, 120f);
            if(axisToMoveTarget == 1)
            {
                moveSpeedX = Random.Range(minMoveSpeed, maxMoveSpeed);
                moveSpeedZ = 0;
            }
            else
            {
                moveSpeedZ = Random.Range(minMoveSpeed, maxMoveSpeed);
                moveSpeedX = 0;
            }
            
        }
        else
        {
            timeWhenTargetChangeAxis -= Time.deltaTime;
            if (targetTransform.localPosition.x <= minPosX)
            {
                moveSpeedX = Random.Range(minMoveSpeed, maxMoveSpeed);
                moveSpeedZ = 0;
            }
            else if (targetTransform.localPosition.x >= maxPosX)
            {
                moveSpeedX = Random.Range(-minMoveSpeed, -maxMoveSpeed);
                moveSpeedZ = 0;
            }
            if (targetTransform.localPosition.z <= minPosZ)
            {
                moveSpeedZ = Random.Range(minMoveSpeed, maxMoveSpeed);
                moveSpeedX = 0;
            }
            else if (targetTransform.localPosition.z >= maxPosZ)
            {
                moveSpeedZ = Random.Range(-minMoveSpeed, -maxMoveSpeed);
                moveSpeedX = 0;
            }

            targetTransform.Translate(moveSpeedX * Time.deltaTime, 0, moveSpeedZ * Time.deltaTime);
        }

        // Change movement to opposite direction when at the  
        if (targetTransform.localPosition.x <= minPosX)
        {
            moveSpeedX = Random.Range(minMoveSpeed, maxMoveSpeed);

        }
        else if (targetTransform.localPosition.x >= maxPosX)
        {
            moveSpeedX = Random.Range(-minMoveSpeed, -maxMoveSpeed);
        }
        if (targetTransform.localPosition.z <= minPosZ)
        {
            moveSpeedZ = Random.Range(minMoveSpeed, maxMoveSpeed);
        }
        else if (targetTransform.localPosition.z >= maxPosZ)
        {
            moveSpeedZ = Random.Range(-minMoveSpeed, -maxMoveSpeed);
        }

        targetTransform.Translate(moveSpeedX * Time.deltaTime, 0, moveSpeedZ * Time.deltaTime);
    }


    private void MoveTargetRandomly(Transform targetTransform, float groundScaleX = 1, float groundScaleZ = 1,
            float distanceFromWall = 1f, float minMoveSpeed = 1f, float maxMoveSpeed = 5f)
    {
        float minPosX = -groundScaleX * 5 + distanceFromWall;
        float minPosZ = -groundScaleZ * 5 + distanceFromWall;
        float maxPosX = groundScaleX * 5 - distanceFromWall;
        float maxPosZ = groundScaleZ * 5 - distanceFromWall;



        if (targetTransform.localPosition.x <= minPosX)
        {
            moveSpeedX = Random.Range(minMoveSpeed, maxMoveSpeed);

        }
        else if (targetTransform.localPosition.x >= maxPosX)
        {
            moveSpeedX = Random.Range(-minMoveSpeed, -maxMoveSpeed);
        }
        if (targetTransform.localPosition.z <= minPosZ)
        {
            moveSpeedZ = Random.Range(minMoveSpeed, maxMoveSpeed);
        }
        else if (targetTransform.localPosition.z >= maxPosZ)
        {
            moveSpeedZ = Random.Range(-minMoveSpeed, -maxMoveSpeed);
        }

        targetTransform.Translate(moveSpeedX * Time.deltaTime, 0, moveSpeedZ * Time.deltaTime);

    }

    private void AnimateMovement()
    {
        animator.SetFloat("VelocityZ", moveValue);
    }

}
