using UnityEngine;
using UnityEngine.SceneManagement;  // Required for scene management

public class RedirectToScene : MonoBehaviour
{  
    public string sceneName;
    public void LoadScene() { SceneManager.LoadScene(sceneName); }   
}
