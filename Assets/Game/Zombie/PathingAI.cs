using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PathingAI : MonoBehaviour
{
    private static readonly object padlock = new object();
    public static bool isInUse = false;
    private static int agentTypeIdAvoidDestructibles = 0;
    private static int agentTypeIdIgnoreDestructibles = 0;
    private static uint nbrThreadsInMethod = 0;
    private const uint MAX_THREADS_IN_METHOD = 5;

    public GameObject currentTarget = null;
    public Vector3 currentTargetPos = Vector3.zero;
    private uint nbrTimes = 120;
    private const uint NBR_TIMES_MAX = 120;
    private bool isDead = false;
    public Difficulty difficulty = Difficulty.INTERMEDIATE;

    private const int layerMaskForCarvingDisabled = (1 << 0)
        | (1 << 1)
        | (1 << 2)
        | (1 << 4)
        | (1 << 5);

    private NavMeshAgent agent;

    public static void Init()
    {
        isInUse = false;
        agentTypeIdAvoidDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_AVOID_DESTRUCTIBLE);
        agentTypeIdIgnoreDestructibles = ZombieController.GetAgenTypeIDByName(ZombieController.AGENT_TYPE_NAME_IGNORE_DESTRUCTIBLE);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead)
            return;

        if (nbrTimes++ > NBR_TIMES_MAX)
        {
            if (GetTarget())
                nbrTimes = 0;
            else
                nbrTimes = NBR_TIMES_MAX - 1;
        }

        if (currentTarget != null) // update pathing destination if target exists
        {
            // Only the player target can move
            if (currentTarget.CompareTag(ZombieController.TAG_PLAYER))
            {
                agent.destination = currentTarget.transform.position;
            }
        }
    }

    // Validates if the destination corresponds with the current target position (only use with players to test if they moved)
    private bool ValidatePath()
    {
        if (ZombieController.DistanceSqNoY(agent.destination, currentTarget.transform.position) < 1)
        {
            return true;
        }
        return false;
    }

    private bool GetTarget()
    {
        // max nbr path search per frame in worst case
        // Easy = MAX_THREADS_IN_METHOD^nbrPlayerAlive
        // Intermediate = MAX_THREADS_IN_METHOD^(nbrPlayerAlive * 2)
        if (nbrThreadsInMethod > MAX_THREADS_IN_METHOD)
            return false;
        lock (padlock)
        {
            nbrThreadsInMethod++;
        }

        bool result = false;
        if (difficulty == Difficulty.EASY)
        {
            result = GetTargetDifficultyEasy();
        }
        else if (difficulty == Difficulty.INTERMEDIATE)
        {
            result = GetTargetDifficultyMedium();
        }
        //else if (difficulty == Difficulty.Hard)
        //{
        //    // principle of hordes maybe?
        //    // keeping track of what horde is taking what path to not take the same one
        //}

        lock (padlock)
        {
            nbrThreadsInMethod--;
        }
        return result;
    }

    // Zombies know to take the path without destructibles first, but destroy them if there is no alternative
    private bool GetTargetDifficultyMedium()
    {
        if (agent.agentTypeID == agentTypeIdIgnoreDestructibles)
        {
            agent.agentTypeID = agentTypeIdAvoidDestructibles;
        }

        if (TargetClosestReachablePlayerTarget())
        {
            return true;
        }
        agent.agentTypeID = agentTypeIdIgnoreDestructibles;

        if (TargetClosestDestructibleRequiredToReachClosestPlayer())
        {
            return true;
        }
        return false;
    }

    // Zombies are dumb and just start hitting whatever destructible they cross tor each the closest player
    private bool GetTargetDifficultyEasy()
    {
        if (agent.agentTypeID == agentTypeIdAvoidDestructibles)
        {
            agent.agentTypeID = agentTypeIdIgnoreDestructibles;
        }

        return TargetClosestPlayerOrDestructibleInItsPath();
    }

    // =========================================
    // Player
    // =========================================

    private bool TargetClosestReachablePlayerTarget()
    {
        NavMeshPath path = new NavMeshPath();
        GameObject target = GetClosestReachablePlayerTarget(ref path);

        if (target != null)
        {
            TargetFound(target, path);
        }
        return target != null;
    }

    private GameObject GetClosestReachablePlayerTarget(ref NavMeshPath closestPath)
    {
        GameObject closest = null;
        float distanceClosest = 0;
        NavMeshPath path;
        foreach (GameObject target in ZombieController.zombiePlayerTargets)
        {
            if (VerifyCompletePathingPossible(target, out path))
            {
                float distance = CalculatePathDistance(path);
                if (closest == null || distanceClosest > distance)
                {
                    closestPath = path;
                    closest = target;
                    distanceClosest = distance;
                }
            }
        }
        return closest;
    }

    // =========================================
    // Destructibles
    // =========================================

    private bool TargetClosestDestructibleRequiredToReachClosestPlayer()
    {
        NavMeshPath pathClosest = new NavMeshPath();
        // In here, carving is disabled
        // GetClosestReachablePlayerTarget's result will ignore obstacles
        GameObject closest = GetClosestReachablePlayerTarget(ref pathClosest);
        if (closest != null)
        {
            return TargetDestructibleInPath(pathClosest);
        }

        return false;
    }

    private bool TargetClosestPlayerOrDestructibleInItsPath()
    {
        NavMeshPath pathClosest = new NavMeshPath();
        // In here, carving is disabled
        // GetClosestReachablePlayerTarget's result will ignore obstacles
        GameObject closest = GetClosestReachablePlayerTarget(ref pathClosest);
        if (closest != null)
        {
            if (!TargetDestructibleInPath(pathClosest))
            {
                TargetFound(closest, pathClosest);
            }
            return true;
        }

        return false;
    }

    private bool TargetDestructibleInPath(NavMeshPath path)
    {
        GameObject closest;
        RaycastHit hit;
        Vector3 lastCorner = Vector3.zero;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (i == 0)
            {
                lastCorner = path.corners[i];
                continue;
            }
            Vector3 direction = Vector3.Normalize(path.corners[i] - lastCorner);
            float distance = Vector3.Distance(path.corners[i], lastCorner);
            // lance un rayon sur le path trouvé et vérifie si l'objet frappé est un destructible
            if (Physics.Raycast(lastCorner, direction, out hit, distance, layerMaskForCarvingDisabled))
            {
                if (hit.collider.gameObject.CompareTag("Destructible"))
                {
                    closest = hit.transform.gameObject;
                    TargetFound(closest, hit.point);
                    return true;
                }
            }
            lastCorner = path.corners[i];
        }
        return false;
    }

    // =========================================
    // Utils
    // =========================================

    private bool VerifyCompletePathingPossible(GameObject target, out NavMeshPath path)
    {
        path = new NavMeshPath();
        bool result = agent.CalculatePath(target.transform.position, path);

        // Vérifie si le pathing est possible
        return result && path.status == NavMeshPathStatus.PathComplete;
    }

    public static float CalculatePathDistance(NavMeshPath path)
    {
        uint i = 0;
        var lastCorner = Vector3.zero;
        float totalDistance = 0;
        foreach (Vector3 corner in path.corners)
        {
            if (i++ == 0)
            {
                lastCorner = corner;
                continue;
            }
            totalDistance += Vector3.Distance(lastCorner, corner);
            lastCorner = corner;
        }
        return totalDistance;
    }

    // Preffered since the path will not need to be recalculated
    private void TargetFound(GameObject target, NavMeshPath path)
    {
        currentTarget = target;
        currentTargetPos = path.corners.Last();
        agent.SetPath(path);
        TargetFound();
    }

    // if path was already calculated, use the other one
    private void TargetFound(GameObject target, Vector3 targetPos)
    {
        currentTarget = target;
        currentTargetPos = targetPos;
        agent.SetDestination(targetPos);
        TargetFound();
    }

    private void TargetFound()
    {
        this.GetComponent<Animator>().SetBool("FoundTarget", true);
    }

    private void TargetLost()
    {
        currentTarget = null;
        currentTargetPos = Vector3.zero;
        agent.ResetPath();
        this.GetComponent<Animator>().SetBool("FoundTarget", false);
        TargetNotHittable();
    }

    private void TargetHittable()
    {
        this.GetComponent<Animator>().SetBool("TargetHittable", true);
    }

    private void TargetNotHittable()
    {
        this.GetComponent<Animator>().SetBool("TargetHittable", false);
    }

    public void OnKilled()
    {
        TargetLost();
        isDead = true;
        //if (isInUse && oneUsing == this.gameObject)
        //{
        //    isInUse = false;
        //}
    }

    void OnDestroy()
    {
        //if (isInUse && oneUsing == this.gameObject)
        //{
        //    isInUse = false;
        //}
    }

    //private enum GetTargetResult
    //{
    //    WAIT,
    //    FOUND,
    //    NOT_FOUND
    //}
}
