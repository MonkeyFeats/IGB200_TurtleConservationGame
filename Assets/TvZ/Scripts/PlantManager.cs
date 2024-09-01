using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlantManager : MonoBehaviour
{
    public PlantCardScriptableObject thisSO;
    public Transform shootPoint;
    public GameObject Bullet;
    public float health;
    public float damage;
    public float range;
    public float speed;
    public LayerMask zombieLayer;
    public float fireRate;

    public bool isMine;
    public float growDuration;
    public GameObject explosion;
    public Sprite grownSprite;
    public float blinkingRate;
    public Sprite[] states;
    public int stateCount;
    public bool isGrown;

    public AudioClip damageAudio;

    public GameObject mineParticles;

    public bool isDragging = true;

    public List<PlantObjectVisual> plantObjectsVisual;

    public GameObject plantVisual;

    public string dropedName = "Droped";

    private void Start()
    {
        health = thisSO.health;
        damage = thisSO.damage;
        range = thisSO.range;
        speed = thisSO.speed;
        Bullet = thisSO.Bullet;
        zombieLayer = thisSO.zombieLayer;
        fireRate = thisSO.fireRate;

        isMine = thisSO.isMine;
        growDuration = thisSO.growDuration;
        explosion = thisSO.Explosion;
        grownSprite = thisSO.grownSprite;
        blinkingRate = thisSO.blinkingRate;
        states = thisSO.mineStates;

        foreach (var item in plantObjectsVisual)
        {
            if (item.name.ToLower() == thisSO.name.ToLower())
            {
                plantVisual = item.plantObject;
                plantVisual.SetActive(true);

                break;
            }
        }

        if (isMine)
        {
            StartCoroutine(mineStateUpdate());
            StartCoroutine(blink());
        }

        StartCoroutine(Attack());
        StartCoroutine(WaitUntilDroped());
    }

    private void Update()
    {
        if (health <= 0)
        {
            //Dead
            Destroy(this.gameObject);
        }
    }

    public IEnumerator WaitUntilDroped()
    {
        yield return new WaitUntil(() => !isDragging);

        if (plantVisual != default)
        {
            //plantVisual.GetComponent<Animator>().SetBool(dropedName, true);
        }
    }

    public IEnumerator Attack()
    {
        Debug.Log("Attacking... Plants " + this.gameObject.name);
        yield return new WaitUntil(() => !isDragging);

        if (!isMine)
        {
            if (speed > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, shootPoint.right, range, zombieLayer);

                Debug.DrawRay(shootPoint.position, shootPoint.right, Color.red);

                if (hit)
                {
                    Debug.Log("Attacking Info plant name : " + this.gameObject.name + ", gameObject name : " + hit.collider.name + ", tag : " + hit.collider.tag);

                    if (hit.transform.tag == "Zombie")
                    {
                        Debug.Log("Hit zombie");
                        GameObject bullet = Instantiate(Bullet, shootPoint.transform.position, Quaternion.identity);
                        bullet.GetComponent<PeaManager>().damage = damage;
                        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * speed;
                    }
                }

                yield return new WaitForSeconds(fireRate);
                StartCoroutine(Attack());
            }
        }
        else
        {
            //Attack and destroy
        }
    }

    public IEnumerator mineStateUpdate()
    {
        isGrown = false;
        yield return new WaitUntil(() => !isDragging);
        yield return new WaitForSeconds(growDuration);
        isGrown = true;
    }

    public IEnumerator blink()
    {
        yield return new WaitUntil(() => !isDragging);
        this.GetComponent<SpriteRenderer>().sprite = states[stateCount];
        yield return new WaitForSeconds(blinkingRate);
        stateCount = isGrown ? stateCount == 2 ? 3 : 2 : stateCount == 1 ? 0 : 1;
        StartCoroutine(blink());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                mineParticles.SetActive(true);

                collision.GetComponent<ZombieController>().DealDamage(damage);

                Destroy(this.gameObject, 1.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                Instantiate(explosion, this.transform);
                Destroy(this.gameObject);
            }
        }
    }

    public void Damage(float amnt)
    {
        //Audio to play
        if (!this.GetComponent<AudioSource>().isPlaying)
        {
            this.GetComponent<AudioSource>().Play();
        }

        health -= amnt;
    }
}

[System.Serializable]
public class PlantObjectVisual
{
    public string name;
    public GameObject plantObject;
}
