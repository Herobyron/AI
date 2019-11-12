using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI 
{
    public List<Actions> actionlist = new List<Actions>();

    public List<Goals> goallist = new List<Goals>();

    //adds a goal to the existing list of goals
    public void AddGoal(Goals goal)
    {
        goallist.Add(goal);
    }

    //adds a action to the existing list of actions
    public void AddAction(Actions action)
    {
        actionlist.Add(action);
    }
    
    //takes in two parameters one is the tyoe of goal this is
    //second is the goals value that needs to be updated
    public void UpdateGoalValue(AIGoals Type, float value)
    {
        foreach(Goals goal in goallist)
        {
            if(goal.TypeReturn() == Type)
            {
                goal.SetValue(value);
            }
        }
    }


}
