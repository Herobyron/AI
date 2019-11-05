using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is used to determine how sueful this goal is going to be inorder to satisfy the AI needs so used to help pick which set of actions to complete
public class GoalValuefunction 
{
    private float LowerUtilityGoalRange = 0.0f;
    private float HigherUtilityGoalRange = 1.0f;

    // return the linear utility value of the goal 
    // as the values to complete the goal increases the utility incerases lineraly.
    public float Linear(float LowerGoalRange, float HighGoalRange, float Value)
    {
        return LowerGoalRange + (HighGoalRange - LowerGoalRange) * ((Value - LowerGoalRange) / (HighGoalRange - LowerGoalRange));
    }

}
