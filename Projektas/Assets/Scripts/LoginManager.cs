using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorMessage;
    public Button loginButton;
    public Button returnButton; // New return button
    public Camera camera;
    public string sceneToLoad;

    private string filePath;

    void Start()
    {
        filePath = Application.dataPath + "/users.txt";
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        returnButton.onClick.AddListener(OnReturnButtonClicked); // Assign event
    }

    void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log("Login attempt with username: " + username + " and password: " + password);

        if (IsValidCredentials(username, password))
        {
            errorMessage.text = "Prisijungimas sėkmingas!";
            errorMessage.gameObject.SetActive(true);

            StartCoroutine(LoadNextScene());
        }
        else
        {
            errorMessage.text = "Netinkami prisijungimo duomenys!";
            errorMessage.gameObject.SetActive(true);
        }
    }

    void OnReturnButtonClicked()
    {
        errorMessage.text = "";
        errorMessage.gameObject.SetActive(false);
    }

    bool IsValidCredentials(string username, string password)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File does not exist at path: " + filePath);
            return false;
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] credentials = line.Split(':');

            Debug.Log("Checking line: " + line);

            if (credentials.Length == 3)
            {
                string savedUsername = credentials[0].Trim();
                string savedPassword = credentials[1].Trim();

                Debug.Log("Saved username: " + savedUsername + ", Saved password: " + savedPassword);

                if (savedUsername == username && savedPassword == password)
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
