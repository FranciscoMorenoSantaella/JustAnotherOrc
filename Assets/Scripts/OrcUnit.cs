using UnityEngine;
using System.Collections;

//Script de orco
public class OrcUnit : MonoBehaviour
{
    [HideInInspector] public Transform hordeCenter;
    public float followSpeed = 7f;
    public float stopDistance = 0.15f;
    public int hp = 3;

    [Header("Ataque")]
    public float attackDamage = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.0f;
    private float lastAttackTime;

    private Vector3 offset;
    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource audioSource; 

    [Header("Configuración de Colisión")]
    public LayerMask wallLayer; 

void Awake()
{
    sr = GetComponent<SpriteRenderer>();
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
    
 
    audioSource = GetComponent<AudioSource>(); 

    if (rb != null)
    {
        rb.bodyType = RigidbodyType2D.Kinematic; 
        rb.useFullKinematicContacts = true; 
    }
}

    public void SetFormationIndex(int index, int total)
    {
        float radius = (index <= 8) ? 1.6f : 3.0f;
        float angle = index * (360f / Mathf.Max(total, 1)) * Mathf.Deg2Rad;
        offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        if (index == 0) offset = Vector3.zero;
    }

    void Update()
    {
        TryAutoAttack();
        MoveOrc();

        if (sr != null) sr.sortingOrder = Mathf.RoundToInt(transform.position.y * -100) + 15000;
    }

    void MoveOrc()
    {
        if (hordeCenter == null) return;

        Vector3 targetPos = hordeCenter.position + offset;
        float distanceToTarget = Vector2.Distance(transform.position, targetPos);

        if (distanceToTarget > stopDistance)
        {
            Vector2 direction = (targetPos - transform.position).normalized;
            float moveStep = followSpeed * Time.deltaTime;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.3f, wallLayer);

            if (hit.collider == null) 
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, moveStep);
                if (anim != null) anim.SetBool("Walking", true);
            }
            else
            {
                if (anim != null) anim.SetBool("Walking", false);
            }

            if (targetPos.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (anim != null) anim.SetBool("Walking", false);
        }
    }

    void TryAutoAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Cage") || hit.CompareTag("Enemy"))
            {
                ExecuteAttack();
                if (hit.CompareTag("Cage")) hit.GetComponent<Cage>()?.TakeDamage(attackDamage);
                else hit.GetComponent<Enemy>()?.TakeDamage((int)attackDamage);
                return;
            }
        }
    }

void ExecuteAttack()
{
    if (Time.time >= lastAttackTime + attackCooldown) 
    {
        lastAttackTime = Time.time;

        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.Play();
        }

        if (anim != null) 
        { 
            anim.ResetTrigger("Attack"); 
            anim.SetTrigger("Attack"); 
        }
        StartCoroutine(AttackColorFlash());
    }
}

    IEnumerator AttackColorFlash()
    {
        if (sr == null) yield break;
        Color originalColor = sr.color;
        sr.color = new Color(0.7f, 0.7f, 0.7f); 
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        StartCoroutine(FlashRed());
        if (hp <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.white;
    }

    void Die()
    {
        if (HordeManager.Instance != null) HordeManager.Instance.OrcDied(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}