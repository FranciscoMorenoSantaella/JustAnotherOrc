using UnityEngine;

//Script de la llave
public class KeyItem : MonoBehaviour
{
    [Header("Configuración de la Puerta")]
    
    public GameObject doorToOpen; 

   
    public GameObject pickupEffect; 

private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Algo ha tocado la llave: " + other.name + " con el tag: " + other.tag);
    
    if (other.CompareTag("Player") || other.CompareTag("Orc"))
    {
        UseKey();
    }
}

    void UseKey()
    {
        if (doorToOpen != null)
        {
            doorToOpen.SetActive(false);
            Debug.Log("Puerta " + doorToOpen.name + " abierta con " + gameObject.name);
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("¡Ojo! La llave " + gameObject.name + " no tiene ninguna puerta asignada en el Inspector.");
        }
    }


}