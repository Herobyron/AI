using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI 
{
    public List<ActionBase> actionlist = new List<ActionBase>();

    public List<Goals> goallist = new List<Goals>();

    //adds a goal to the existing list of goals
    public void AddGoal(Goals goal)
    {
        goallist.Add(goal);
    }

    //adds a action to the existing list of actions
    public void AddAction(ActionBase action)
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

    public ActionBase ChooseAction()
    {
        //this will be the goal that has the maximum utility (so is the highest on the list)
        //determines what goal it needs to take
        Goals MaxGoal = goallist[0];
        foreach(Goals G in goallist)
        {
            if(G.Utility() > MaxGoal.Utility())
            {
                MaxGoal = G;
            }
        }

        //find the action which satisfies the goal the most
        ActionBase MaxAction = actionlist[0];
        foreach(ActionBase A in actionlist)
        {
            if (A.EvaluateGoalSatisfaction(MaxGoal.TypeReturn()) > MaxAction.EvaluateGoalSatisfaction(MaxGoal.TypeReturn()))
            {
                MaxAction = A;
            }
        }

        return MaxAction;

    }

}
