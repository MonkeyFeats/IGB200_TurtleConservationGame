using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class ZombieController : MonoBehaviour
{
    public ZombieScriptableObject thisZombieSO;
    public ZombieAccessoriesManager zombieAccessories;

    public List<GameObject> childObjects;

    public float speed;
    public float health;
    public float handHealth;
    public float currentHealth;
    public GameObject accessory;
    public float accessoryHealth;
    public float damage;
    public float attackInterval;
    GameObject target;
    public bool isAttacking;

    [Tooltip("Index 0 : Normal Zombie, Index 1 : Cone Head Zombie, Index 2 : Bucket Head Zombie")]
    public List<AudioClip> damageAudio;

    public GameObject coinPrefab;

    public GameObject pole;

    public bool isPole = true;

    public float damageDelay = 2f;

    bool isDying;
    bool incremented = false;

    [Header("Animator Parameters")]
    public bool isWalking;

    private void Start()
    {
        isPole = true;
        speed = thisZombieSO.zombieSpeed;
        health = thisZombieSO.zombieHealth;
        accessoryHealth = thisZombieSO.accessoryHealth;
        damage = thisZombieSO.zombieDamage;
        handHealth = thisZombieSO.zombieHandHealth;
        attackInterval = thisZombieSO.attackInterval;
        currentHealth = health;

		if (thisZombieSO.ovveridDefaultSprite)
		{
            this.GetComponent<SpriteRenderer>().sprite = thisZombieSO.newSprite;
		}

		if (!thisZombieSO.useChildObjects)
		{
			foreach (var item in childObjects)
			{
                item.SetActive(false);
			}
		}
    }

    private void Update()
    {
        if (target == null)
        {
            isAttacking = false;
        }

        if (!isAttacking && !isDying)
        {
            //Debug.Log("Walking!");
            isWalking = true;
            this.GetComponent<Animator>().SetBool("IsWalking", isWalking);
            this.transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else
        {
            isWalking = false;
            this.transform.position = this.transform.position;
        }

        if (currentHealth <= handHealth && this.transform.childCount > 1)
        {
            //Add rigidbody 2d to hand
            //Transform hand = this.transform.GetChild(1);

            //hand.gameObject.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            //hand.gameObject.GetComponentInChildren<Rigidbody2D>().gravityScale = 1f;

            //hand.SetParent(null);

            //Destroy(hand.gameObject, 1.5f);
        }

        if (currentHealth <= 0 && this.transform.childCount > 0)
        {
            isDying = true;
			//Dead
			//Add rigidbody 2d to head

			if (!incremented)
			{
                incremented = true; 
                WaveManger.currentZombieKilled++;

                int randomInt = Random.Range(0, 101);

				if (randomInt <= ((thisZombieSO.coinDropPercent / 100) * 100))
				{
                    GameObject coinObj = Instantiate(coinPrefab, this.transform);

                    coinPrefab.transform.SetParent(null);
				}
            }

            Destroy(this.GetComponent<Rigidbody2D>());

            foreach (var item in this.GetComponents<BoxCollider2D>())
            {
                Destroy(item);
            }

            Destroy(this.gameObject, 2f);
        }

        if (accessory == null)
        {
            thisZombieSO.zombieType = ZombieScriptableObject.ZombieType.Normal;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (thisZombieSO.zombieType == ZombieScriptableObject.ZombieType.PoleVaulter && isPole)
        {
            if (collision.gameObject.tag == "Plant" || collision.gameObject.GetComponent<PlantManager>() != null)
            {
                Debug.Log("Jump");
                //Jump and lose pole
                isPole = false;

                pole.SetActive(false);

                StartCoroutine(Jump());
            }
        }

        if (thisZombieSO.zombieType == ZombieScriptableObject.ZombieType.PoleVaulter && isPole)
		{
            Debug.Log("Cannot Attack!");
            return;
		}

        //Debug.Log("Collided with " + collision.gameObject.name);
        //Detect plant collisions
        if (collision.gameObject.tag == "Plant" || collision.gameObject.GetComponent<PlantManager>() != null)
        {
            isAttacking = true;
            target = collision.gameObject;
            StartCoroutine(Attack());
        }
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        return;

		//If we are pole vaulter
		if (thisZombieSO.zombieType != ZombieScriptableObject.ZombieType.PoleVaulter || !isPole)
		{
            return;
		}

        if (collision.gameObject.tag == "Plant")
        {
            Debug.Log("Jump");
            //Jump and lose pole
            isPole = false;

            pole.SetActive(false);

            StartCoroutine(Jump());
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
    {
        target = null;
        isAttacking = false;
    }

    public IEnumerator Attack()
    {
        isWalking = false;
        Debug.Log("Attacking...");
        //Attack Plant
        if (target != null)
        {
            target.GetComponent<PlantManager>().Damage(damage);
        }

        this.GetComponent<Animator>().SetBool("IsWalking", isWalking);
        this.GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(attackInterval);

        if (target != null)
        {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Jump()
	{
        foreach (var item in this.GetComponents<BoxCollider2D>())
        {
            //item.enabled = false;
        }

        this.transform.position += Vector3.up * thisZombieSO.jumpPower * Time.deltaTime;
        yield return new WaitForSeconds(thisZombieSO.jumpDuration);
        this.transform.position -= Vector3.up * thisZombieSO.jumpPower * Time.deltaTime;

        foreach (var item in this.GetComponents<BoxCollider2D>())
        {
            //item.enabled = true;
        }
    }

    public void DealDamage(float amnt)
    {
        //Audio to play
        switch (thisZombieSO.zombieType)
        {
            case ZombieScriptableObject.ZombieType.Normal:
                this.GetComponent<AudioSource>().PlayOneShot(damageAudio[0]);
                break;
            case ZombieScriptableObject.ZombieType.ConeZombie:
                this.GetComponent<AudioSource>().PlayOneShot(damageAudio[1]);
                break;
            case ZombieScriptableObject.ZombieType.BucketZombie:
                this.GetComponent<AudioSource>().PlayOneShot(damageAudio[2]);
                break;
            default:
                break;
        }

        currentHealth -= amnt;

        if (zombieAccessories != null)
        {
            zombieAccessories.TakeDamage(amnt);
        }

        StartCoroutine(DamageColor(this.gameObject.GetComponent<SpriteRenderer>()));

        foreach (Transform item in this.transform.GetComponentInChildren<Transform>())
        {
            StartCoroutine(DamageColor(item.gameObject.GetComponent<SpriteRenderer>()));   
        }
    }

    public IEnumerator DamageColor(SpriteRenderer spriteRenderer)
    {
        for(int i = 0; i <= 255; i+=10)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(i, i, i);
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 255; i <= 0; i-=10)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(i, i, i);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
