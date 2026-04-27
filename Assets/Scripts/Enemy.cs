using UnityEngine;

//Script de enemigo
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 10;
    public int damage = 1;
    public float moveSpeed = 3f;

    [Header("Ranges")]
    public float detectionRange = 10f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Colisiones")]
    public LayerMask wallLayer; 

    private float lastAttackTime;
    private SpriteRenderer spriteRenderer;
    private Animator anim; 
    private AudioSource audioSource; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>(); 
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        
        OrcUnit targetOrc = FindPlayerOrc();

        if (targetOrc != null)
        {
            float distance = Vector2.Distance(transform.position, targetOrc.transform.position);

            if (distance <= attackRange)
            {
                TryAttack(targetOrc);
            }
            else if (distance <= detectionRange)
            {
                MoveTowards(targetOrc.transform.position);
            }
            else
            {
                if (anim != null) anim.SetBool("Walking", false);
            }
        }
        else
        {
            if (anim != null) anim.SetBool("Walking", false);
        }

      
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100) + 15000;
        }
    }

    void MoveTowards(Vector3 targetPos)
    {
        Vector2 direction = (targetPos - transform.position).normalized;
        
        //Rayo para detectar las paredes
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.3f, wallLayer);

        if (hit.collider == null)
        {
            if (anim != null) anim.SetBool("Walking", true);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }
        else 
        {
            
            if (anim != null) anim.SetBool("Walking", false);
        }

        // Girar el sprite para la izquierda o derecha segun movimiento
        if (targetPos.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); 
        else
            transform.localScale = new Vector3(1, 1, 1);  
    }

    void TryAttack(OrcUnit target)
    {
        if (anim != null) anim.SetBool("Walking", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.Play();
            }

            if (anim != null) anim.SetTrigger("Attack");

            target.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
    OrcUnit FindPlayerOrc()
    {
        if (HordeManager.Instance == null)
            return null;

        return HordeManager.Instance.initialPlayerOrc;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(DamageFlash());
        if (health <= 0) Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}