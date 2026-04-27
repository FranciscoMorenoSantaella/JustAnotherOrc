using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    [Header("Configuración")]
    public Enemy heroEnemy;             // Arrastra aquí al Héroe (que tiene el script Enemy)
    public string victorySceneName = "WinScene";
    public float delayBeforeTransition = 2f; // Un pequeño respiro antes de pasar a los diálogos

    private bool isVictoryTriggered = false;

    void Update()
    {
        // Si el héroe ha sido asignado y su vida llega a 0 (o es destruido)
        if (!isVictoryTriggered && heroEnemy == null)
        {
            // Entramos aquí si el objeto ha sido destruido por el script Enemy.Die()
            StartVictory();
        }
        else if (!isVictoryTriggered && heroEnemy != null && heroEnemy.health <= 0)
        {
            // Entramos aquí justo cuando muere pero antes de ser destruido
            StartVictory();
        }
    }

    void StartVictory()
    {
        isVictoryTriggered = true;
        Debug.Log("¡El Héroe ha caído! Cargando victoria...");
        Invoke("LoadVictoryScene", delayBeforeTransition);
    }

    void LoadVictoryScene()
    {
        SceneManager.LoadScene(victorySceneName);
    }
}