using UnityEngine;

namespace FriedRice
{
    public static class Utils
    {
        public static float ReLU(float x)
        {
            return Mathf.Max(0, x);
        }


        public static void Log(string msg, bool enableLogs)
        {
            if(enableLogs)
            {
                Debug.Log(msg);
            }
        }
    }

    public static class Actions
    {
        public static void MoveAndRotate(Transform transform, float rotationValue, float rotationSpeed, float moveValue, float moveSpeed, bool enableMove = true, bool enableRotate = true)
        {
            if (enableRotate)
            {
                float appliedRotation = rotationValue * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, appliedRotation, 0);
            }

            if (enableMove)
            {
                transform.position += moveSpeed * Utils.ReLU(moveValue) * Time.deltaTime * transform.forward;
            }
        }
    }

    public static class MLAgentsTraining
    {
        // if random sides is false then the agent will be always on the left side
        public static void InitializeEnvironment(Transform agentTransform, Transform targetTransform, float groundScaleX = 1, float groundScaleZ = 1, 
            float distanceFromWall = 2f, bool randomSides = true, bool resetAgentRotation = false)
        {

            float maxPosX = groundScaleX * 5 - distanceFromWall;
            float maxPosZ = groundScaleZ * 5 - distanceFromWall;

            int side = randomSides ? Random.Range(0, 2): 0;

            // agent left, target right
            if (side == 0)
            {
                agentTransform.localPosition = new Vector3(Random.Range(-maxPosX, -1f), 1f, Random.Range(-maxPosZ, maxPosZ));
                targetTransform.localPosition = new Vector3(Random.Range(1f, maxPosX), 1f, Random.Range(-maxPosZ, maxPosZ));
            }
            // agent right, target left
            else
            {
                agentTransform.localPosition = new Vector3(Random.Range(1f, maxPosX), 1f, Random.Range(-maxPosZ, maxPosZ));
                targetTransform.localPosition = new Vector3(Random.Range(-maxPosX, -1f), 1f, Random.Range(-maxPosZ, maxPosZ));
            }

            agentTransform.localRotation = resetAgentRotation ? new Quaternion(0f, 0f, 0f, 1f): agentTransform.localRotation;
        }

        
    }

    public class AgentStatus
    {
        private int episodeCounter;
        private int winsCounter;
        private int outsideMapCounter;

        public int GetEpisodeCounter()
        {
            return episodeCounter;
        }

        public void SetEpisodeCounter(int value)
        {
            episodeCounter = value;
        }

        public int GetWinsCounter()
        {
            return winsCounter;
        }

        public void SetWinsCounter(int value)
        {
            winsCounter = value;
        }

        public int GetOutsideMapCounter()
        {
            return outsideMapCounter;
        }

        public void SetOutsideMapCounter(int value)
        {
            outsideMapCounter = value;
        }
    }


}
