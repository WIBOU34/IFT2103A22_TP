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
        this.gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.AnimatePhysics;
        this.gameObject.AddComponent<NavMeshAgent>();
        this.gameObject.GetComponent<NavMeshAgent>().speed = 2f;
        this.gameObject.GetComponent<NavMeshAgent>().radius = 0.3f;
        this.gameObject.GetComponent<NavMeshAgent>().height = 1.8f;
        this.gameObject.GetComponent<NavMeshAgent>().angularSpeed = 360;
        this.gameObject.GetComponent<NavMeshAgent>().autoBraking = true;
        this.gameObject.GetComponent<NavMeshAgent>().updatePosition = true;
        this.gameObject.GetComponent<NavMeshAgent>().destination = CalculatePositionOfClosestTarget();

        if (Random.Range(0, 10) % 5 == 0)
        {
            CanRun(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorMove()
    {
        Vector3 position = this.gameObject.GetComponent<Animator>().rootPosition;
        position.y = this.gameObject.GetComponent<NavMeshAgent>().nextPosition.y;
        transform.position = position;
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = CalculatePositionOfClosestTarget();
        if (!isTargetTooFar(targetPos))
        {
            this.gameObject.GetComponent<NavMeshAgent>().destination = targetPos;

            if (1.5f > ZombieController.DistanceSq(this.gameObject.GetComponent<NavMeshAgent>().destination, this.transform.position))
            {
                TargetHittable();
            } else
            {
                TargetNotHittable();
            }
        } else
        {
            TargetLost();
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

    private bool isTargetTooFar(Vector3 target)
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
        this.gameObject.GetComponent<Animator>().SetBool("FoundTarget", true);
    }

    private void TargetLost()
    {
        currentTarget = null;
        this.gameObject.GetComponent<Animator>().SetBool("FoundTarget", false);
        TargetNotHittable();
    }

    private void TargetHittable()
    {
        this.gameObject.GetComponent<Animator>().SetBool("TargetHittable", true);
    }

    private void TargetNotHittable()
    {
        this.gameObject.GetComponent<Animator>().SetBool("TargetHittable", false);
    }

    private void CanRun(bool value)
    {
        this.gameObject.GetComponent<Animator>().SetBool("CanRun", value);
    }

    private void Killed()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Killed", true);
    }
}
