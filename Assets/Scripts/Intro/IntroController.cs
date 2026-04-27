using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Script que controla la intro del principio del juego
public class IntroManager : MonoBehaviour
{
    [Header("Referencias de tu Jerarquía")]
    public TextMeshProUGUI deathMessageText; 
    public RectTransform bossImage;      
    public string nextSceneName = "MainGameplay";

    [Header("Ajustes de Movimiento (Suavizado)")]
    public float floatSpeed = 3f;        
    public float floatAmount = 15f;      
    public float talkShakeAmount = 3f;   
    public float smoothTime = 0.1f;      

    [Header("Lore y Secuencia")]
    [TextArea(3, 10)]
    public string[] sentences;
    public float typingSpeed = 0.04f;

    [Header("Modo de Escena")]
    [Tooltip("Si es true, NO cambiará de escena al hacer clic fuera. Deberás usar un botón.")]
    public bool isGameOverMode = false;
    public GameObject pressAnyKeyText; 

    private int index = 0;
    private bool isTyping = false;
    private bool sequenceFinished = false;
    private Vector3 bossOriginalPos;
    private Vector3 currentVelocity;

    void Start()
    {
        if (deathMessageText != null) deathMessageText.text = "";
        if (pressAnyKeyText != null) pressAnyKeyText.SetActive(false);
        
        if (bossImage != null) 
        {
            bossOriginalPos = bossImage.anchoredPosition;
        }

        if (sentences.Length > 0)
        {
            StartCoroutine(TypeSentence(sentences[index]));
        }
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        HandleVisuals();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                deathMessageText.text = sentences[index];
                isTyping = false;
                CheckIfSequenceFinished();
            }
            else if (!sequenceFinished)
            {
                NextSentence();
            }
        }
    }

    void HandleVisuals()
    {
        if (bossImage == null) return;

        float idleMovement = Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmount;
        float talkShake = isTyping ? Random.Range(-talkShakeAmount, talkShakeAmount) : 0;

        Vector3 targetPos = bossOriginalPos + new Vector3(0f, idleMovement + talkShake, 0f);

        bossImage.anchoredPosition = Vector3.SmoothDamp(
            bossImage.anchoredPosition, 
            targetPos, 
            ref currentVelocity, 
            smoothTime
        );
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        deathMessageText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            deathMessageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        CheckIfSequenceFinished();
    }

    void CheckIfSequenceFinished()
    {
        if (index >= sentences.Length - 1)
        {
            sequenceFinished = true;
            
            if (isGameOverMode && pressAnyKeyText != null)
            {
                pressAnyKeyText.SetActive(true);
            }
            
            if (!isGameOverMode)
            {
                Invoke("LoadNextScene", 1.5f);
            }
        }
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            StartCoroutine(TypeSentence(sentences[index]));
        }
    }

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}