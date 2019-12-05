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
    private float Gvalue;

    // this is the class to determine how much value the goal gives towards the utiltiy
    GoalValuefunction UtilityValue;

    // the type of goal this one is
    AIGoals goaltype;

    // the goal to achieve (constructor)
    public Goals(AIGoals type, float goalvaluemin, float goalvaluemax, float currentvalue, GoalValuefunction utilityfunction)
    {
        goaltype = type;
        MinimumGoalValue = goalvaluemin;
        MaximumGoalValue = goalvaluemax;

        Gvalue = currentvalue;
        UtilityValue = utilityfunction;
    }

    //sets the goals utility value
    public void SetValue(float value)
    {
        Gvalue = value;
    }

    //removes a set amount of utility value from the on going goal
    public void SubtractValue(float val)
    {
        Gvalue -= val;
    }

    // returns the goals utilty value
    public float GetValue()
    {
        return Gvalue;
    }

    //returns the goals utlity value 
    public float Utility()
    {
        return UtilityValue.Linear(MinimumGoalValue, MaximumGoalValue, Gvalue);
    }

    //returns the goals type
    public AIGoals TypeReturn()
    {
        return goaltype;
    }

}
