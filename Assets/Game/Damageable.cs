using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float health = 100;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            health = 0;
            this.gameObject.BroadcastMessage("OnKilled");
        }
    }
}
