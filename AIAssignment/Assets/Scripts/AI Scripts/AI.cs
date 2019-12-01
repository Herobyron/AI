using System.Collections.Generic;
using UnityEngine;

/*****************************************************************************************************************************
 * Write your core AI code in this file here. The private variable 'agentScript' contains all the agents actions which are listed
 * below. Ensure your code it clear and organised and commented.
 *
 * Unity Tags
 * public static class Tags
 * public const string BlueTeam = "Blue Team";	The tag assigned to blue team members.
 * public const string RedTeam = "Red Team";	The tag assigned to red team members.
 * public const string Collectable = "Collectable";	The tag assigned to collectable items (health kit and power up).
 * public const string Flag = "Flag";	The tag assigned to the flags, blue or red.
 * 
 * Unity GameObject names
 * public static class Names
 * public const string PowerUp = "Power Up";	Power up name
 * public const string HealthKit = "Health Kit";	Health kit name.
 * public const string BlueFlag = "Blue Flag";	The blue teams flag name.
 * public const string RedFlag = "Red Flag";	The red teams flag name.
 * public const string RedBase = "Red Base";    The red teams base name.
 * public const string BlueBase = "Blue Base";  The blue teams base name.
 * public const string BlueTeamMember1 = "Blue Team Member 1";	Blue team member 1 name.
 * public const string BlueTeamMember2 = "Blue Team Member 2";	Blue team member 2 name.
 * public const string BlueTeamMember3 = "Blue Team Member 3";	Blue team member 3 name.
 * public const string RedTeamMember1 = "Red Team Member 1";	Red team member 1 name.
 * public const string RedTeamMember2 = "Red Team Member 2";	Red team member 2 name.
 * public const string RedTeamMember3 = "Red Team Member 3";	Red team member 3 name.
 * 
 * _agentData properties and public variables
 * public bool IsPoweredUp;	Have we powered up, true if we’re powered up, false otherwise.
 * public int CurrentHitPoints;	Our current hit points as an integer
 * public bool HasFriendlyFlag;	True if we have collected our own flag
 * public bool HasEnemyFlag;	True if we have collected the enemy flag
 * public GameObject FriendlyBase; The friendly base GameObject
 * public GameObject EnemyBase;    The enemy base GameObject
 * public int FriendlyScore; The friendly teams score
 * public int EnemyScore;       The enemy teams score
 * 
 * _agentActions methods
 * public bool MoveTo(GameObject target)	Move towards a target object. Takes a GameObject representing the target object as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveTo(Vector3 target)	Move towards a target location. Takes a Vector3 representing the destination as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveToRandomLocation()	Move to a random location on the level, returns true if the location is on the navmesh, false otherwise.
 * public void CollectItem(GameObject item)	Pick up an item from the level which is within reach of the agent and put it in the inventory. Takes a GameObject representing the item as a parameter.
 * public void DropItem(GameObject item)	Drop an inventory item or the flag at the agents’ location. Takes a GameObject representing the item as a parameter.
 * public void UseItem(GameObject item)	Use an item in the inventory (currently only health kit or power up). Takes a GameObject representing the item to use as a parameter.
 * public void AttackEnemy(GameObject enemy)	Attack the enemy if they are close enough. ). Takes a GameObject representing the enemy as a parameter.
 * public void Flee(GameObject enemy)	Move in the opposite direction to the enemy. Takes a GameObject representing the enemy as a parameter.
 * 
 * _agentSenses properties and methods
 * public List<GameObject> GetObjectsInViewByTag(string tag)	Return a list of objects with the same tag. Takes a string representing the Unity tag. Returns null if no objects with the specified tag are in view.
 * public GameObject GetObjectInViewByName(string name)	Returns a specific GameObject by name in view range. Takes a string representing the objects Unity name as a parameter. Returns null if named object is not in view.
 * public List<GameObject> GetObjectsInView()	Returns a list of objects within view range. Returns null if no objects are in view.
 * public List<GameObject> GetCollectablesInView()	Returns a list of objects with the tag Collectable within view range. Returns null if no collectable objects are in view.
 * public List<GameObject> GetFriendliesInView()	Returns a list of friendly team AI agents within view range. Returns null if no friendlies are in view.
 * public List<GameObject> GetEnemiesInView()	Returns a list of enemy team AI agents within view range. Returns null if no enemy are in view.
 * public bool IsItemInReach(GameObject item)	Checks if we are close enough to a specific collectible item to pick it up), returns true if the object is close enough, false otherwise.
 * public bool IsInAttackRange(GameObject target)	Check if we're with attacking range of the target), returns true if the target is in range, false otherwise.
 * 
 * _agentInventory properties and methods
 * public bool AddItem(GameObject item)	Adds an item to the inventory if there's enough room (max capacity is 'Constants.InventorySize'), returns true if the item has been successfully added to the inventory, false otherwise.
 * public GameObject GetItem(string itemName)	Retrieves an item from the inventory as a GameObject, returns null if the item is not in the inventory.
 * public bool HasItem(string itemName)	Checks if an item is stored in the inventory, returns true if the item is in the inventory, false otherwise.
 * 
 * You can use the game objects name to access a GameObject from the sensing system. Thereafter all methods require the GameObject as a parameter.
 * 
*****************************************************************************************************************************/

/// <summary>
/// Implement your AI script here, you can put everything in this file, or better still, break your code up into multiple files
/// and call your script here in the Update method. This class includes all the data members you need to control your AI agent
/// get information about the world, manage the AI inventory and access essential information about your AI.
///
/// You may use any AI algorithm you like, but please try to write your code properly and professionaly and don't use code obtained from
/// other sources, including the labs.
///
/// See the assessment brief for more details
/// </summary>
/// 

public enum AIGoals { CaptureFlag, retreat, attack, ProtectFriend, Guard };


public class AI : MonoBehaviour
{
    // Gives access to important data about the AI agent (see above)
    private AgentData _agentData;
    // Gives access to the agent senses
    private Sensing _agentSenses;
    // gives access to the agents inventory
    private InventoryController _agentInventory;
    // This is the script containing the AI agents actions
    // e.g. agentScript.MoveTo(enemy);
    private AgentActions _agentActions;

  
    // refernces to gameobject in the world 
    public GameObject EnemyBase;
    public GameObject HomeBase;

    public int GuardspotNumber = 1;
    public GameObject FriendlyGuardSpotOne;
    public GameObject FreindlyGuardSpotTwo;

    public bool GotEnemyflag = false;

    public bool Startcheck = false;

    public bool Retreated;

    Actions TheActions = new Actions();

    //the Actions for the first Goal GoPickUpFlag
    TheAction A;
    TheAction B;
    TheAction C;
    TheAction D;
    TheAction E;

    //the set of actions required for the second Goal
    TheAction A2;

    //the set of actions for the third goal
    TheAction A3;

    //refernce to calculate the utility value of each goal
    GoalValuefunction Valuefunctions = new GoalValuefunction();

    //the current actions for the AI To Complete
    TheAction Currentaction;

    //refernce to the utiltiy AI
    UtilityAI TheAI = new UtilityAI();
    void Awake()
    {
        if(this.CompareTag("Blue Team"))
        HomeBase = GameObject.FindGameObjectWithTag("BlueBase");
        else
            HomeBase = GameObject.FindGameObjectWithTag("RedBase");


        //goal One go get Flag and Bring it back
        Goals GoPickUpFlag = new Goals(AIGoals.CaptureFlag, 0.0f, 10.0f, 0.0f, Valuefunctions);

        //actions included within the goal
        A = new TheAction(TheActions, "MoveEnemySide", true, true, 0.0f, true);
        A.SetGoalSatisfaction(AIGoals.CaptureFlag, 1);

        B = new TheAction(TheActions, "Attack", false, true, 0.0f, false);
        B.SetGoalSatisfaction(AIGoals.attack, 2);

        C = new TheAction(TheActions, "PickUpFlag", true, true, 0.0f, false);
        C.SetGoalSatisfaction(AIGoals.CaptureFlag, 3);

        D = new TheAction(TheActions, "MoveHome", true, true, 0.0f, false);
        D.SetGoalSatisfaction(AIGoals.CaptureFlag, 4);

        E = new TheAction(TheActions, "DropFlag", true, true, 0.0f, false);
        E.SetGoalSatisfaction(AIGoals.CaptureFlag, 5);

        //the actions sequence for the first goal
        ActionSequence Sequence = new ActionSequence();
        Sequence.AddAction(A);
        Sequence.AddAction(B);
        Sequence.AddAction(C);
        Sequence.AddAction(D);
        Sequence.AddAction(E);

        Sequence.SetGoalSatisfaction(AIGoals.CaptureFlag, 5);

        //goal two protect the team mate with the flag
        Goals ProtectTeamMate = new Goals(AIGoals.ProtectFriend, 0.0f, 1.0f, 0.0f, Valuefunctions);

        // the unique actions for the second goal
        A2 = new TheAction(TheActions, "ProtectTeamMateWithFlag", true, true, 0.0f, true);
        A2.SetGoalSatisfaction(AIGoals.ProtectFriend, 1);
        
        // the action sequence for the second goal
        ActionSequence Sequence2 = new ActionSequence();
        Sequence2.AddAction(A2);
        Sequence2.AddAction(B);
        Sequence2.SetGoalSatisfaction(AIGoals.ProtectFriend, 5);
        
        //goal to guard the base once you have the enemy flag
        Goals GuardBase = new Goals(AIGoals.Guard ,0 ,5 ,0 ,Valuefunctions);

        //the actions unique included within the third goal
        A3 = new TheAction(TheActions, "Guard", true, true, 0.0f, true);
        A3.SetGoalSatisfaction(AIGoals.Guard, 1);

        //the action sequence for the third goal
        ActionSequence Sequence3 = new ActionSequence();
        Sequence3.AddAction(A3);
        Sequence3.AddAction(B);
        Sequence3.SetGoalSatisfaction(AIGoals.Guard, 5);

        //adding the goals and actions to the AI
        TheAI.AddGoal(GoPickUpFlag);
        TheAI.AddAction(Sequence);
        TheAI.AddGoal(ProtectTeamMate);
        TheAI.AddAction(Sequence2);
        TheAI.AddGoal(GuardBase);
        TheAI.AddAction(Sequence3);
    }

    // Use this for initialization
    void Start ()
    {
        // Initialise the accessable script components
        _agentData = GetComponent<AgentData>();
        _agentActions = GetComponent<AgentActions>();
        _agentSenses = GetComponentInChildren<Sensing>();
        _agentInventory = GetComponentInChildren<InventoryController>();

        //actions = GetComponent<Actions>();
        Retreated = false;

        if (_agentData.FriendlyTeamTag == "Blue Team")
        {
            FreindlyGuardSpotTwo = GameObject.FindGameObjectWithTag("BlueGuardSpotTwo");
            FriendlyGuardSpotOne = GameObject.FindGameObjectWithTag("BlueGuardSpotOne");
        }
        else
        {
            FreindlyGuardSpotTwo = GameObject.FindGameObjectWithTag("RedGuardSpotTwo");
            FriendlyGuardSpotOne = GameObject.FindGameObjectWithTag("RedGuardSpotOne");
        }


        //Sequence.AddAction(A);
        //Sequence.AddAction(C);

    }

   

    // Update is called once per frame
    void Update ()
    {
        //everything implemented code
        CheckEnemyflag();

        ActionSequence CurrentAction = TheAI.ChooseAction();
        CurrentAction.Execute(this);
        Startcheck = true;
    }

    public AgentData GetData()
    {
        return _agentData;
    }

    public AgentActions GetActions()
    {
        return _agentActions;
    }

    public Sensing GetSensing()
    {
        return _agentSenses;
    }

    public InventoryController GetInventory()
    {
        return _agentInventory;
    }


    //function checks to see the Ais health. if lower then a certain amount set it so the goal to get health it set
    //this will also lower all other goals value as getting health is more important
    public void CheckHealth()
    {

    }

    //a function that checks the where abouts of the enemies flag.
    //if it is in the enmies base they will attack
    //if it isnt in the enemies base then they will defend
    public void CheckEnemyflag()
    {
        ////else check it its at home base
        //if (TheActions.CheckflagAtBase(_agentSenses, _agentData, HomeBase))
        //{
        //    Debug.Log("finally");
        //    TheAI.UpdateGoalValue(AIGoals.Guard, 3);
        //    TheAI.UpdateGoalValue(AIGoals.attack, 0);
        //}
        ////check if i have the flag first cause if i do then return home
        //else if (_agentData.HasEnemyFlag)
        //{
        //    
        //    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 2);
        //    TheAI.UpdateGoalValue(AIGoals.ProtectFriend, 0);
        //}
        //// is the flag in a team mates possesion
        //else if (!_agentData.HasEnemyFlag)
        //{
        //    
        //    List<GameObject> TeamMembers;
        //    TeamMembers = _agentSenses.GetFriendliesInView();
        //
        //    foreach(GameObject G in TeamMembers)
        //    {
        //        if(G.GetComponent<AgentData>().HasEnemyFlag)
        //        {
        //            
        //            TheAI.UpdateGoalValue(AIGoals.ProtectFriend, 5);
        //            TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 0);
        //
        //        }
        //    }
        //
        //}
        ////if its none of the above then its at the enemy base 
        //else
        //{
        //    
        //    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 2);
        //}

        //testing for blue team if they have their own flag
        if (this.CompareTag("Blue Team"))
        {
            Debug.Log(HomeBase.GetComponent<BaseState>().HasBlueFlag);
            Debug.Log(HomeBase.GetComponent<BaseState>().HasRedFlag);

            if (HomeBase.GetComponent<BaseState>().HasBlueFlag)
            {
                TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 2);
            }
            else if(HomeBase.GetComponent<BaseState>().HasRedFlag)
            {
                Debug.Log("Should be guarding");
                TheAI.UpdateGoalValue(AIGoals.CaptureFlag, -10);
                TheAI.UpdateGoalValue(AIGoals.Guard, 5);
            }

        }

    }


    public void OnTriggerStay(Collider other)
    {
        //if (Startcheck)
        //{
        //
        //    if (other.CompareTag("BlueBase"))
        //    {
        //
        //        if (gameObject.CompareTag("Blue Team"))
        //        {
        //
        //            if (TheActions.FindEnemyflag(_agentSenses, _agentData) || _agentData.HasEnemyFlag)
        //            {
        //                
        //
        //                if (TheAI.goallist.Count != 0)
        //                {
        //                    _agentActions.DropAllItems();
        //                    //TheActions.DropItemAtBase(HomeBase, this, _agentActions, true);
        //                    TheAI.UpdateGoalValue(AIGoals.Guard, 5);
        //                    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 0);
        //                }
        //
        //            }
        //        }
        //    }
        //    else if (other.CompareTag("RedBase"))
        //    {
        //        if (gameObject.CompareTag("Red Team"))
        //        {
        //            if (TheActions.FindEnemyflag(_agentSenses, _agentData))
        //            {
        //                if (TheAI.goallist.Count != 0)
        //                {
        //                    _agentActions.DropAllItems();
        //                    //TheActions.DropItemAtBase(HomeBase,this, _agentActions, true);
        //                    TheAI.UpdateGoalValue(AIGoals.Guard, 5);
        //                    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 0);
        //                }
        //            }
        //        }
        //    }
        //
        //
        //}
    }

}