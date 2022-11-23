using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private bool isDead = false;
    public float damagePerAttack = 20;
    private GameObject currentTarget;
    private uint nbrTimesPathUpdatedWithObstacleAsTarget = 0;
    private const uint MAX_nbrTimesPathUpdatedWithObstacleAsTarget = 30;

    public Vector3 target;
    public bool isPathStale;
    public NavMeshPathStatus pathStatus;
    public int areaMask;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;

        CanRun(false);
        this.GetComponent<NavMeshAgent>().radius = 0.3f;
        this.GetComponent<NavMeshAgent>().height = 1.8f;
        this.GetComponent<NavMeshAgent>().angularSpeed = 360;
        if (Random.Range(0, 10) % 4 == 0)
        {
            CanRun(true);
        }

        this.AddComponent<CapsuleCollider>();
        this.GetComponent<CapsuleCollider>().height = this.GetComponent<NavMeshAgent>().height;
        this.GetComponent<CapsuleCollider>().radius = this.GetComponent<NavMeshAgent>().radius;
        this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.8f, 0);
        this.GetComponent<CapsuleCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        target = this.GetComponent<NavMeshAgent>().destination;
        isPathStale = this.GetComponent<NavMeshAgent>().isPathStale;
        pathStatus = this.GetComponent<NavMeshAgent>().pathStatus;
        areaMask = this.GetComponent<NavMeshAgent>().areaMask;
    }

    public void PlayerReachableStatusChanged()
    {
        nbrTimesPathUpdatedWithObstacleAsTarget = MAX_nbrTimesPathUpdatedWithObstacleAsTarget + 1;
    }

    void Attack()
    {
        if (ValidateCurrentTargetForAttack())
        {
            Damageable damageable = currentTarget.GetComponent<Damageable>();
            currentTarget.GetComponent<Damageable>().TakeDamage(damagePerAttack);
            //this.GetComponent<Damageable>().TakeDamage(damagePerAttack);
        }
    }

    private bool ValidateCurrentTargetForAttack()
    {
        float radius2 = this.GetComponent<NavMeshAgent>().radius * 2;
        Vector3 target = currentTarget.transform.position;
        if (currentTarget.CompareTag("Destructible"))
        {
            // le point ciblé sur le destructible (le centre peux être trop loin)
            target = this.GetComponent<NavMeshAgent>().destination;
        }
        return ZombieController.DistanceSq(target, this.transform.position) <= (radius2 * radius2) * 2;
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;
        FindBestTarget();
        if (currentTarget == null || IsTargetTooFar(currentTarget.transform.position))
        {
            TargetLost();
        }
        else
        {
            //this.GetComponent<NavMeshAgent>().destination = targetPos;

            float radius2 = this.GetComponent<NavMeshAgent>().radius * 2;
            //if (ZombieController.DistanceSq(currentTarget.transform.position, this.transform.position) <= (radius2 * radius2) * 2)
            if (ValidateCurrentTargetForAttack())
            {
                TargetHittable();
            }
            else
            {
                TargetNotHittable();
            }
        }
    }

    private void FindBestTarget()
    {
        // verify currentTarget is still the best target (if currentTarget is a Destructible)
        if (currentTarget != null)
        {
            if (currentTarget.tag.Equals("Destructible") && nbrTimesPathUpdatedWithObstacleAsTarget++ > MAX_nbrTimesPathUpdatedWithObstacleAsTarget)
            {
                nbrTimesPathUpdatedWithObstacleAsTarget = 0;
                //VerifyCurrentTargetIsStillValid(); // not good enough for objects until we can check if players moved
                CalculatePositionAndPathOfClosestTarget();
            }
            else if (currentTarget.tag.Equals("Player"))
            {
                VerifyCurrentTargetIsStillValid();
                //CalculatePositionAndPathOfClosestTarget();
            }
        }
        else if (currentTarget == null)
        {
            CalculatePositionAndPathOfClosestTarget();
        }
        //else
        //{
        //    //TargetFound(currentTarget.transform.position);
        //    this.GetComponent<NavMeshAgent>().destination = currentTarget.transform.position;
        //}
    }

    private void CalculatePositionAndPathOfClosestTarget()
    {
        NavMeshPath path = ZombieController.GetTarget(this.transform.position, out currentTarget);

        if (path == null || path.corners.Length == 0)
        {
            TargetLost();
            return;
        }
        TargetFound(path);
    }

    private void VerifyCurrentTargetIsStillValid()
    {
        if (ZombieController.ValidateTargetIsStillBestTarget(this.transform.position, currentTarget, out NavMeshPath path))
        {
            TargetFound(path);
        }
        else
        {
            CalculatePositionAndPathOfClosestTarget();
        }
    }

    // Validates if the destination corresponds with the current target position (only use with players to test if they moved)
    private bool ValidatePath()
    {
        if (ZombieController.DistanceSq(this.GetComponent<NavMeshAgent>().destination, currentTarget.transform.position) < 0.5f)
        {
            return true;
        }
        return false;
    }

    private bool IsTargetTooFar(Vector3 target)
    {
        // sqrt(400) = 20 meters
        if (400 < ZombieController.DistanceSq(target, this.transform.position))
        {
            return true;
        }
        return false;
    }

    private void TargetFound(Vector3 position)
    {
        this.GetComponent<NavMeshAgent>().SetDestination(position);
        this.GetComponent<Animator>().SetBool("FoundTarget", true);
    }

    private void TargetFound(NavMeshPath path)
    {
        this.GetComponent<NavMeshAgent>().SetPath(path);
        this.GetComponent<Animator>().SetBool("FoundTarget", true);
    }

    private void TargetLost()
    {
        currentTarget = null;
        this.GetComponent<NavMeshAgent>().ResetPath();
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

    private void CanRun(bool value)
    {
        if (value)
        {
            this.GetComponent<NavMeshAgent>().speed = 3f;
        }
        else
        {
            this.GetComponent<NavMeshAgent>().speed = 1f;
        }
    }

    public void OnKilled()
    {
        this.GetComponent<Animator>().SetBool("Killed", true);
        TargetLost();
        isDead = true;
        this.GetComponent<CapsuleCollider>().enabled = false;
        GameObject.Destroy(this.gameObject, 10);
        // permet de libérer l'espace dans la liste et donc de permettre un nouveau de spawner
        this.transform.parent.gameObject.GetComponent<ZombieSpawner>().ZombieDestroyed(this.gameObject);
    }

    private void OnDestroy()
    {
    }
}
