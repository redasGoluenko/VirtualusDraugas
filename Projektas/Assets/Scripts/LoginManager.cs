using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI; // Required for Button

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;   // TMP_InputField for username
    public TMP_InputField passwordInput;   // TMP_InputField for password
    public TextMeshProUGUI errorMessage;   // TextMeshProUGUI for error message
    public Button loginButton;             // Login Button

    private string filePath;

    void Start()
    {
        // Set file path (same location where the users.txt is saved)
        filePath = Application.dataPath + "/users.txt";

        // Assign the button click listener
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    // Called when the Login button is clicked
    void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Debug: Check what data is being entered
        Debug.Log("Login attempt with username: " + username + " and password: " + password);

        // Check if the credentials are correct
        if (IsValidCredentials(username, password))
        {
            errorMessage.text = "Prisijungimas sėkmingas!";
            errorMessage.gameObject.SetActive(true);
            // Proceed to the next scene or perform other actions
        }
        else
        {
            errorMessage.text = "Netinkami prisijungimo duomenys!";
            errorMessage.gameObject.SetActive(true);
        }
    }

    // Checks if the username and password exist in the file
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

            // Debug: Log each line to ensure it's in the correct format
            Debug.Log("Checking line: " + line);

            if (credentials.Length == 3)
            {
                string savedUsername = credentials[0].Trim();
                string savedPassword = credentials[1].Trim();
                string savedEmail = credentials[2].Trim(); // We’re not using email for login

                // Debug: Check what the file contains
                Debug.Log("Saved username: " + savedUsername + ", Saved password: " + savedPassword);

                if (savedUsername == username && savedPassword == password)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
