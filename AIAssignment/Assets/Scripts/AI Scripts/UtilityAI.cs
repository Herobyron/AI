using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI 
{
    public List<Actions> actionlist = new List<Actions>();

    public List<Goals> goallist = new List<Goals>();

    public void AddGoal(Goals goal)
    {
        goallist.Add(goal);
    }

    public void AddAction(Actions action)
    {
        actionlist.Add(action);
    }
    


}
