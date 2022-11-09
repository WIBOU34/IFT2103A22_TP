using UnityEngine;
using UnityEngine.AI;

// Source: https://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class LocomotionSimpleAgent : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    public Vector2 smoothDeltaPosition = Vector2.zero;
    public Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
        //transform.position = agent.nextPosition;
    }

    void FixedUpdate()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        //bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        // Update animation parameters
        //anim.SetBool("move", shouldMove);
        anim.SetFloat("velx", velocity.x);
        anim.SetFloat("vely", velocity.y);

        if (worldDeltaPosition.magnitude > agent.radius)
        {
            // Pull agent towards character (le joueur passe a travers des murs)
            //agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
            // Pull character towards agent (peux causer des glissements, mais respecte les obstacles)
            transform.position = agent.nextPosition - 0.9f * worldDeltaPosition;
        }

        LookAt lookAt = GetComponent<LookAt>();
        if (lookAt)
            lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

    void OnAnimatorMove()
    {

        // Update position to agent position
        // Sync bien la position, mais empêche de sync l'Animation à la vitesse (glissement galore)
        //transform.position = agent.nextPosition;

        // Update position based on animation movement using navigation surface height
        // Cause l'agent à être décalé à droite du character (pas dramatique), mais permet de sync l'animation à la vitesse
        Vector3 position = anim.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
    }
}
