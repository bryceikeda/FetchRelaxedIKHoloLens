using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FetchState { Stop = 0, Go = 1 };

public class ArticulationJointController : MonoBehaviour
{
    public FetchState robotState = FetchState.Stop;
    public float rotationGoal = 0f;
    public float speed = 100.0f;
    public bool goalReached;

    private ArticulationBody articulation;


    // LIFE CYCLE

    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
        goalReached = false; 
    }

    void FixedUpdate()
    {
        if (robotState != FetchState.Stop)
        {
            RotateTo(rotationGoal);
        }
        GoalReached();
    }

    // MOVEMENT HELPERS

    float CurrentPrimaryAxisRotation()
    {
        float currentRotationRads = articulation.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        return currentRotation;
    }

    void GoalReached()
    {
        if (robotState == FetchState.Go) {
            float distance = Mathf.Abs(Mathf.Abs(CurrentPrimaryAxisRotation()) - Mathf.Abs(rotationGoal));
            if (distance < .05)
            {
                goalReached = true; 
            }
            else
            {
                goalReached = false; 
            }
        }
        else
        {
            goalReached = true; 
        }
    }

    void RotateTo(float primaryAxisRotation)
    {
        var drive = articulation.xDrive;
        drive.target = primaryAxisRotation;
        articulation.xDrive = drive;
    }
}
