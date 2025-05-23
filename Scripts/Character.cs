using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected int health;   
    [SerializeField]
    protected Slider healthBar;
    [SerializeField]
    protected TMPro.TextMeshProUGUI healthText;

    protected int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        healthText.text = health.ToString() + "/" + maxHealth.ToString();
    }

    public IEnumerator showParticles(GameObject particles)
    {
        GameObject instance = Instantiate(particles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1);
        Destroy(instance, 1);
    }

    public void ChangeHealth(int difference)
    {
        //show hurt or heal particles based on difference
        if (difference < 0)
        {
            transform.DOShakePosition(0.5f, difference, 10, 90, false, true);
            GameObject hurtParticles = Resources.Load<GameObject>("HurtEffect");
            StartCoroutine(showParticles(hurtParticles));
        }else if (difference > 0)
        {
            transform.DOJump(transform.position, difference, 1, 0.5f);
            GameObject healParticles = Resources.Load<GameObject>("HealEffect");
            StartCoroutine(showParticles(healParticles));
        }

        
        health += difference;
        healthBar.value = health;
        healthText.text = health.ToString() + "/" + maxHealth.ToString();


        if (health <= 0)
        {
            Die();
        }
    }

    public void ChangeDamage(int difference)
    {
        damage += difference;
        GameObject alterParticles = Resources.Load<GameObject>("AlterEffect");
        StartCoroutine(showParticles(alterParticles));
        transform.DOShakePosition(0.5f, difference, 1, 90, false, true);
        if (damage < 0)
        {
            damage = 0;
        }
    }

    public void SetDamage(int n_damage)
    {
        damage = n_damage;
    }

    //used when instantiated
    public void SetHealth(int n_health)
    {
        health = n_health;
        if (health > maxHealth)
        {
            maxHealth = health;
        }

        healthBar.value = health;
        healthText.text = health.ToString() + "/" + maxHealth.ToString();
    }
    public int GetHealth()
    {
        return health;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public abstract void Attack();
    public abstract void Die();
}
