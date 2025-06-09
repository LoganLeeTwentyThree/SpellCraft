using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ParticleSpawner;
using UnityEditor.Experimental.GraphView;

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
    protected GameManager gameManager;

    protected bool exhausted = false;
    protected int exhaustRounds = 0;

    protected int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        gameManager = GameManager.GetInstance();


        healthBar.maxValue = health;
        healthBar.value = health;
        healthText.text = health.ToString() + "/" + maxHealth.ToString();

        
    }

    

    public void ChangeHealth(int difference)
    {
        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        Vector2 startPos = transform.position;
        GameObject particles;
        //show particles based on difference
        if (difference < 0)
        {
            StartCoroutine(Shake(difference));
            particles = Resources.Load<GameObject>("HurtEffect");
            
        }else if (difference > 0)
        {
            StartCoroutine(Jump(difference));
            particles = Resources.Load<GameObject>("HealEffect");
        }
        else
        {
            particles = Resources.Load<GameObject>("NoDamageEffect");
        }
        StartCoroutine(ps.SpawnParticles(particles, transform.position, Quaternion.identity, 1));

        health += difference;
        healthBar.value = health;
        healthText.text = health.ToString() + "/" + maxHealth.ToString();


        if (health <= 0)
        {
            Die();
        }
    }

    public IEnumerator Shake(float intensity)
    {
        Vector2 startPos = transform.position;
        transform.DOShakePosition(0.5f, intensity, 10, 90, false, true);
        yield return new WaitForSeconds(0.5f);
        transform.position = startPos; //reset position after shake
    }

    public IEnumerator Jump(float intensity)
    {
        Vector2 startPos = transform.position;
        transform.DOJump(transform.position, intensity, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.position = startPos; //reset position after jump
    }


    public void ChangeDamage(int difference)
    {
        damage += difference;
        GameObject particles = difference < 0 ? Resources.Load<GameObject>("AlterEffect") : Resources.Load<GameObject>("BuffEffect");
        ParticleSpawner.ParticleSpawner ps = new ParticleSpawner.ParticleSpawner();
        StartCoroutine(ps.SpawnParticles(particles, transform.position, Quaternion.identity, 1));
        StartCoroutine(Shake(difference * 0.1f));
        //dont want enemies to heal you!
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
    public void Exhaust(int rounds = 1)
    {
        exhaustRounds = rounds;
        exhausted = true;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public void UnExhaust()
    {
        exhausted = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool IsExhausted()
    {
        return exhausted;
    }

    public abstract void Attack();
    public abstract void Die();
    
}
