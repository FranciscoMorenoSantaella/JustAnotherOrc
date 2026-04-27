using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    [Header("Ajustes Visuales")]
    public Vector3 offset = new Vector3(0f, 1.8f, 0f);
    public float bobbingSpeed = 3f;
    public float bobbingAmount = 0.15f;

    [Header("Suavizado")]
    public float smoothSpeed = 10f;
    public float snappingDistance = 0.05f; 

    private OrcUnit targetOrc;
    private Vector3 currentPos;
    private bool isMovingToNewTarget = false;

    public void SetTarget(OrcUnit orc)
    {
        if (orc == null) return;

        targetOrc = orc;
        isMovingToNewTarget = true; 
    }

    void LateUpdate()
    {
        if (targetOrc == null) return;

        // --- LÓGICA DE GIRO ---
        // Calculamos el offset dinámico. Si el orco mira a la izquierda (x negativo), invertimos el offset X.
        Vector3 dynamicOffset = offset;
        if (targetOrc.transform.localScale.x < 0)
        {
            dynamicOffset.x = -offset.x;
        }

        Vector3 targetPos = targetOrc.transform.position + dynamicOffset;

        if (isMovingToNewTarget)
        {
            currentPos = Vector3.Lerp(currentPos, targetPos, Time.unscaledDeltaTime * smoothSpeed);

            if (Vector3.Distance(currentPos, targetPos) < snappingDistance)
            {
                isMovingToNewTarget = false;
            }
        }
        else
        {
            currentPos = targetPos;
        }

        float bobbing = Mathf.Sin(Time.unscaledTime * bobbingSpeed) * bobbingAmount;
        transform.position = currentPos + new Vector3(0, bobbing, 0);

        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        SpriteRenderer orcSR = targetOrc.GetComponent<SpriteRenderer>();
        if (sr != null && orcSR != null)
        {
            sr.sortingOrder = orcSR.sortingOrder + 10;
        }
    }
}