using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; 


//Script que es el que manaje todo lo relacionado con la horda de orcos
public class HordeManager : MonoBehaviour
{
    public static HordeManager Instance;

    [Header("Referencias")]
    public Transform hordeCenter;
    public PlayerMarker playerMarker;
    public OrcUnit initialPlayerOrc;

    [Header("UI Máquina de Escribir")]
    public GameObject deathCanvas;
    public TextMeshProUGUI deathText;
    public float typingSpeed = 0.05f;

    [Header("Escenas")]
    public string gameOverSceneName = "DeathScene"; 

    private List<string> allMessages = new List<string>();
    private List<string> currentPool = new List<string>();
    private bool isTyping = false;

    public List<OrcUnit> orcs = new List<OrcUnit>();

    void Awake()
    {
        Instance = this;
        InitializeMessages();

        if (deathCanvas != null) deathCanvas.SetActive(false);

        OrcUnit[] found = Object.FindObjectsByType<OrcUnit>(FindObjectsSortMode.None);
        foreach (OrcUnit o in found)
        {
            AddOrc(o);
        }

        if (initialPlayerOrc != null)
        {
            if (playerMarker != null) playerMarker.SetTarget(initialPlayerOrc);
        }
        else if (orcs.Count > 0)
        {
            initialPlayerOrc = orcs[0];
            if (playerMarker != null) playerMarker.SetTarget(initialPlayerOrc);
        }
    }

    void InitializeMessages()
    {
        allMessages.Add("You died. No big deal, you weren't the protagonist anyway.");
        allMessages.Add("Next orc, please! This one was totally replaceable.");
        allMessages.Add("Unit lost. Don't worry, there are plenty more where that came from.");
        allMessages.Add("Who was that again? I already forgot his name.");
        allMessages.Add("Just another number in the horde. Keep moving!");
        allMessages.Add("Your death was... insignificant. Try harder with the next one.");
        allMessages.Add("Nothing of value was lost.");
        allMessages.Add("Next!");

        currentPool = new List<string>(allMessages);
    }

    public void AddOrc(OrcUnit o)
    {
        if (!orcs.Contains(o))
        {
            orcs.Add(o);
            o.hordeCenter = this.hordeCenter;
            Refresh();
        }
    }

    public void OrcDied(OrcUnit o)
    {
        orcs.Remove(o); 

        // Si el orco que muere es el que esta señalado por la flecha
        if (o == initialPlayerOrc)
        {
            if (orcs.Count > 0)
            {
                // Si quedan mas orcos
                int randomIndex = Random.Range(0, orcs.Count);
                initialPlayerOrc = orcs[randomIndex];

                if (playerMarker != null) playerMarker.SetTarget(initialPlayerOrc);
                
                ShowDeathMessage();
            }
            else
            {
                // Si no quedan orcos
                initialPlayerOrc = null;
                TriggerGameOver();
            }
        }
        
        // Si muere un orco que no controlamos pero era el ultimo de la horda
        if (orcs.Count == 0 && initialPlayerOrc == null)
        {
            TriggerGameOver();
        }

        Refresh();
    }

    void TriggerGameOver()
    {
        Debug.Log("Horda aniquilada. Cargando Game Over...");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(gameOverSceneName);
    }

    void ShowDeathMessage()
    {
        if (deathCanvas != null && deathText != null && !isTyping)
        {
            if (currentPool.Count == 0)
            {
                currentPool = new List<string>(allMessages);
            }

            string message = "";
            int indexToUse = (currentPool.Count > (allMessages.Count - 3)) ? 0 : Random.Range(0, currentPool.Count);
            
            message = currentPool[indexToUse];
            currentPool.RemoveAt(indexToUse);

            StartCoroutine(TypeTextAndHide(message));
        }
    }

    IEnumerator TypeTextAndHide(string text)
    {
        isTyping = true;
        deathText.text = "";
        deathCanvas.SetActive(true);

        Time.timeScale = 0f;

        foreach (char letter in text.ToCharArray())
        {
            deathText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        deathCanvas.SetActive(false);
        isTyping = false;
    }

    void Refresh()
    {
        for (int i = 0; i < orcs.Count; i++) orcs[i].SetFormationIndex(i, orcs.Count);
    }
}