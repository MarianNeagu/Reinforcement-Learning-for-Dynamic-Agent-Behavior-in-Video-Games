using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField]
    private MoveToGoalAgent[] agents;

    private float maxWinRate;

    // show data for the elitist agent

    [SerializeField]
    private TextMeshProUGUI elitistGenerationText;
    [SerializeField]
    private TextMeshProUGUI elitistWinsText;
    [SerializeField]
    private TextMeshProUGUI elitistLossesText;
    [SerializeField]
    private TextMeshProUGUI elitistStepsExceededText;
    [SerializeField]
    private TextMeshProUGUI elitistCurrentCumulativeRewardText;

    [SerializeField]
    private GameObject elitistAgentEnvMarker;

    /*private void Start()
    {
        maxWinRate = 0;
        *//*elitistAgent = agents[0];*//*
        elitistWinsText.text = "Wins: 0";
        elitistLossesText.text = "Out of area: 0";
        elitistStepsExceededText.text = "Steps exceeded: 0";
        elitistCurrentCumulativeRewardText.text = "Cumulative reward: 0";
*//*        StartCoroutine("ElitistUpdateDelay");*//*
    }

    *//*    IEnumerator ElitistUpdateDelay()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                ElitistUpdate();
            }

        }*//*

    private void Update()
    {
        ElitistUpdate();
    }

    private void ElitistUpdate()
    {

        for (int i = 0; i < agents.Length; i++)
        {
            if ((agents[i].winsCounter / (agents[i].episodeCounter - agents[i].winsCounter)) > maxWinRate)
            {
                maxWinRate = agents[i].winsCounter / (agents[i].episodeCounter - agents[i].winsCounter);
            }

            elitistGenerationText.text = "Elitist episode: " + agents[i].episodeCounter;
            elitistCurrentCumulativeRewardText.text = "Elitist's cumulative reward: " + agents[i].GetCumulativeReward();
            elitistWinsText.text = "Wins: " + agents[i].winsCounter;
            elitistLossesText.text = "Out of area: " + agents[i].lossesCounter;
            elitistStepsExceededText.text = "Steps exceeded: " + (agents[i].episodeCounter - agents[i].winsCounter - agents[i].lossesCounter);

            Vector3 envPos = agents[i].GetComponent<Transform>().parent.localPosition;
            elitistAgentEnvMarker.GetComponent<Transform>().position = new Vector3(envPos.x, envPos.y + 5f, envPos.y);

        }
    }
*/


}
