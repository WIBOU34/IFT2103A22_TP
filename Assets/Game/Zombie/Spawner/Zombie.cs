using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public List<GameObject> zombiePlayerTargets = new List<GameObject>();
    private bool isDead = false;
    public float damagePerAttack = 20;
    public ZombieSpawner spawner;
    private NavMeshAgent agent;
    private PathingAI pathingAI;
    private SoundManager soundManager;
    private List<AudioSource> zombieAudioSources = new List<AudioSource>();
    private AudioSource zombieVoiceSoundSource;
    private AudioSource zombieStepsSoundSource;
    private AudioSource zombieDyingSoundSource;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathingAI = GetComponent<PathingAI>();
        this.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;

        CanRun(false);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.radius = 0.3f;
        agent.height = 1.8f;
        agent.angularSpeed = 360;
        agent.agentTypeID = 0;
        if (Random.Range(0, 10) % 6 == 0)
        {
            CanRun(true);
        }

        this.AddComponent<CapsuleCollider>();
        this.GetComponent<CapsuleCollider>().height = agent.height;
        this.GetComponent<CapsuleCollider>().radius = agent.radius;
        this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.8f, 0);
        this.GetComponent<CapsuleCollider>().isTrigger = true;

        zombieVoiceSoundSource = this.AddComponent<AudioSource>();
        soundManager.PlayZombieVoiceSound(zombieVoiceSoundSource);
        zombieAudioSources.Add(zombieVoiceSoundSource);
        zombieStepsSoundSource = this.AddComponent<AudioSource>();
        soundManager.PlayZombieStepsSound(zombieStepsSoundSource);
        zombieAudioSources.Add(zombieStepsSoundSource);
        zombieDyingSoundSource = this.AddComponent<AudioSource>();
        zombieAudioSources.Add(zombieDyingSoundSource);
        AdjustZombieSoundsBasedOnPlayerDistance();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (isDead || soundManager.gameIsPaused)
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

        AdjustZombieSoundsBasedOnPlayerDistance();
    }

    // Called by the animation controller
    void Attack()
    {
        if (ValidateCurrentTargetForAttack())
        {
            pathingAI.currentTarget.GetComponent<Damageable>().TakeDamage(damagePerAttack);
            soundManager.PlayZombieAttackSound(zombieVoiceSoundSource);
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

    public void PrepareForRespawn()
    {
        pathingAI.PrepareForRespawn();
    }

    public void OnKilled()
    {
        this.GetComponent<Animator>().SetBool("Killed", true);
        isDead = true;
        this.GetComponent<CapsuleCollider>().enabled = false;
        GameObject.Destroy(this.gameObject, 5);

        // permet de libérer l'espace dans la liste et donc de permettre un nouveau de spawner
        spawner.ZombieKilled(this);

        foreach (AudioSource audioSource in zombieAudioSources)
        {
            audioSource.Stop();
        }
        soundManager.PlayZombieDyingSound(zombieDyingSoundSource);
    }

    private void OnDestroy()
    {
        if (isDead)
            spawner.ZombieDestroyed();
    }

    private void AdjustZombieSoundsBasedOnPlayerDistance()
    {
        float distanceSq = float.PositiveInfinity;

        foreach (GameObject player in zombiePlayerTargets)
        {
            float distanceBetweenZombieAndPlayer = ZombieController.DistanceSq(player.transform.position, gameObject.transform.position);

            if (distanceBetweenZombieAndPlayer < distanceSq)
            {
                distanceSq = distanceBetweenZombieAndPlayer;
            }
        }

        foreach (AudioSource zombieAudioSource in zombieAudioSources)
        {
            if (distanceSq > 100)
            {
                zombieAudioSource.volume = 0;
            }
            else
            {
                zombieAudioSource.volume = soundManager.foleyVolume * soundManager.foleyVolume - (distanceSq / 100);
            }
        }
    }
}
