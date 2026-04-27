using UnityEngine;

// Script que maneja el centro de la horda
public class HordeCenter : MonoBehaviour
{
    public float speed = 3.5f;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, y, 0f);

        if (input.magnitude > 0.1f)
        {
            transform.position += input.normalized * speed * Time.deltaTime;
        }
    }
}