using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is used to determine how useful this goal is going to be inorder to satisfy the AI needs so used to help pick which set of actions to complete
public class GoalValuefunction 
{
    private static float LowerUtilityGoalRange = 0.0f;
    private static float HigherUtilityGoalRange = 1.0f;

    // return the linear utility value of the goal 
    // as the values to complete the goal increases the utility incerases lineraly.
    public  float Linear(float LowerGoalRange, float HighGoalRange, float Value)
    {
        return LowerUtilityGoalRange + (HigherUtilityGoalRange - LowerUtilityGoalRange) * ((Value - LowerGoalRange) / (HighGoalRange - LowerGoalRange));
    }

    public  float Exponential(float LowerGoalRange, float HighGoalRange, float value)
    {
        // this will be used to determine the curve of the exponential graph
        const float power = 3f;
        return (HigherUtilityGoalRange - LowerUtilityGoalRange) * Mathf.Pow((value - LowerGoalRange) / (HighGoalRange - LowerGoalRange), power);
    }

    public  float Logarithmic(float LowerGoalRange, float HigherGoalRange, float value)
    {
        //this value determines how steep the logarithmic curve would be
        const float power = 0.3f;
        return LowerUtilityGoalRange + (HigherUtilityGoalRange - LowerUtilityGoalRange) * Mathf.Pow((value = LowerGoalRange) / (HigherGoalRange - LowerGoalRange), power);
    }

}