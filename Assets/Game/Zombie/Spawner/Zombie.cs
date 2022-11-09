using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private bool isDead = false;
    public float damagePerAttack = 20;
    private GameObject currentTarget;
    private uint nbrTimesPathUpdatedWithObstacleAsTarget = 0;
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

    }

    void Attack()
    {
        if (ValidateCurrentTargetForAttack())
        {
            currentTarget.GetComponent<Damageable>().TakeDamage(damagePerAttack);
            //this.GetComponent<Damageable>().TakeDamage(damagePerAttack);
        }
    }

    private bool ValidateCurrentTargetForAttack()
    {
        float radius2 = this.GetComponent<NavMeshAgent>().radius * 2;
        if (ZombieController.DistanceSq(currentTarget.transform.position, this.transform.position) <= (radius2 * radius2) * 2)
        {
            return true;
        }
        else
        {
            return false;
        }
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
            if (currentTarget.tag.Equals("Destructible") && nbrTimesPathUpdatedWithObstacleAsTarget++ > 30)
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

    private void TargetFound(NavMeshPath path)
    {
        this.GetComponent<NavMeshAgent>().path = (path);
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
        //this.gameObject.GetComponent<Animator>().SetBool("CanRun", value);
    }

    public void OnKilled()
    {
        this.GetComponent<Animator>().SetBool("Killed", true);
        TargetLost();
        isDead = true;
        this.GetComponent<CapsuleCollider>().enabled = false;
        GameObject.Destroy(this.gameObject, 10);
    }

    private void OnDestroy()
    {
        this.transform.parent.gameObject.GetComponent<ZombieSpawner>().ZombieDestroyed(this.gameObject);
    }
}
