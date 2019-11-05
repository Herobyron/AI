using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Goals 
{
    // the minimum value needed to start the goal
    private float MinimumGoalValue;

    // the hieghts the value can be for the goal (ie. the miner as a hunger range between 0 and 5 (min and max))
    private float MaximumGoalValue;

    // the value of the goal at the moment (ie. the miners hunger value is at 3 atm)
    // private float value;
    private float value;

    GoalValuefunctions UtilityFunction;

    AIGoals goaltype;

    // the goal to achieve
    public Goals(AIGoals type, float goalvaluemin, float goalvaluemax, float currentvalue, GoalValuefunctions utilityfunction)
    {
        goaltype = type;
        MinimumGoalValue = goalvaluemin;
        MaximumGoalValue = goalvaluemax;

        value = currentvalue;
        UtilityFunction = utilityfunction;
    }





}
