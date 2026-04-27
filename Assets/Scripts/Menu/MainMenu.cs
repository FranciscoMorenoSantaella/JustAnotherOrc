using UnityEngine;
using UnityEngine.SceneManagement;

//Script para cambiar de escena pulsando los botones
public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadSceneAsync(1);
    }
    
    public void PlayGameWithoutIntro(){
        SceneManager.LoadSceneAsync(2);
    }

    public void GoToMenu(){
        SceneManager.LoadSceneAsync(0);
    }
}
