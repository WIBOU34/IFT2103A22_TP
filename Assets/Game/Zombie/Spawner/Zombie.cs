using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private bool isDead = false;
    public float damagePerAttack = 20;
    private NavMeshAgent agent;
    private PathingAI pathingAI;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathingAI = GetComponent<PathingAI>();
        this.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;

        CanRun(false);
        agent.radius = 0.3f;
        agent.height = 1.8f;
        agent.angularSpeed = 360;
        agent.agentTypeID = 0;
        if (Random.Range(0, 10) % 4 == 0)
        {
            CanRun(true);
        }

        this.AddComponent<CapsuleCollider>();
        this.GetComponent<CapsuleCollider>().height = agent.height;
        this.GetComponent<CapsuleCollider>().radius = agent.radius;
        this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.8f, 0);
        this.GetComponent<CapsuleCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (pathingAI.currentTarget != null)
        {
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

    // Called by the animation controller
    void Attack()
    {
        if (ValidateCurrentTargetForAttack())
        {
            pathingAI.currentTarget.GetComponent<Damageable>().TakeDamage(damagePerAttack);
        }
    }

    private bool ValidateCurrentTargetForAttack()
    {
        float radius2 = agent.radius * 2;
        Vector3 target = pathingAI.currentTarget.transform.position;
        if (pathingAI.currentTarget.CompareTag("Destructible"))
        {
            // le point ciblé sur le destructible (le centre peux être trop loin)
            target = agent.destination;
        }
        return ZombieController.DistanceSq(target, this.transform.position) <= (radius2 * radius2) * 2;
    }

    // Validates if the destination corresponds with the current target position (only use with players to test if they moved)
    private bool ValidatePath()
    {
        if (ZombieController.DistanceSq(agent.destination, pathingAI.currentTarget.transform.position) < 0.5f)
        {
            return true;
        }
        return false;
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
            agent.speed = 3f;
        }
        else
        {
            agent.speed = 1f;
        }
    }

    public void OnKilled()
    {
        this.GetComponent<Animator>().SetBool("Killed", true);
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
