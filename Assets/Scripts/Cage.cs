using UnityEngine;

//Script para la jaula que cuando la rompes sale un aliado para la horda
public class Cage : MonoBehaviour
{
    public float health = 10f;
    public GameObject orcPrefab;
    private bool isBroken = false;

    public void TakeDamage(float amount)
    {
        if (isBroken) return;
        health -= amount;
        Debug.Log("La jaula recibe dano. Vida restante: " + health);

        if (health <= 0) BreakCage();
    }

    void BreakCage()
    {
        isBroken = true;
        if (orcPrefab != null)
        {
            GameObject newOrc = Instantiate(orcPrefab, transform.position, Quaternion.identity);
            if (HordeManager.Instance != null) HordeManager.Instance.AddOrc(newOrc.GetComponent<OrcUnit>());
        }
        Destroy(gameObject);
    }
}