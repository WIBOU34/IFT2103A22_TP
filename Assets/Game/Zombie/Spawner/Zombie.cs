using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 100;
    private GameObject currentTarget;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
        //this.gameObject.AddComponent<NavMeshAgent>();
        //this.gameObject.GetComponent<NavMeshAgent>().speed = 1f;

        CanRun(false);
        this.GetComponent<NavMeshAgent>().radius = 0.3f;
        this.GetComponent<NavMeshAgent>().height = 1.8f;
        this.GetComponent<NavMeshAgent>().angularSpeed = 360;
        if (Random.Range(0, 10) % 4 == 0)
        {
            CanRun(true);
        }
        this.GetComponent<NavMeshAgent>().destination = CalculatePositionOfClosestTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = CalculatePositionOfClosestTarget();
        if (IsTargetTooFar(targetPos) || this.GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathInvalid)
        {
            TargetLost();
        } else
        {
            this.GetComponent<NavMeshAgent>().destination = targetPos;

            float radius2 = this.GetComponent<NavMeshAgent>().radius * 2;
            if (ZombieController.DistanceSq(currentTarget.transform.position, this.transform.position) <= (radius2 * radius2) * 2)
            {
                TargetHittable();
            }
            else
            {
                TargetNotHittable();
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Killed();
        }
    }

    private Vector3 CalculatePositionOfClosestTarget()
    {
        if (this.currentTarget == null)
        {
            GameObject target = ZombieController.GetTarget(this.transform.position);
            TargetFound(target);
        }
        return currentTarget.transform.position;
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

    private void TargetFound(GameObject target)
    {
        currentTarget = target;
        this.GetComponent<Animator>().SetBool("FoundTarget", true);
    }

    private void TargetLost()
    {
        currentTarget = null;
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
        } else
        {
            this.GetComponent<NavMeshAgent>().speed = 1f;
        }
        //this.gameObject.GetComponent<Animator>().SetBool("CanRun", value);
    }

    private void Killed()
    {
        this.GetComponent<Animator>().SetBool("Killed", true);
    }
}
