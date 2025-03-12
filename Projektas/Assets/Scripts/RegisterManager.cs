using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI; // Required for Button

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameInput;   // TMP_InputField for username
    public TMP_InputField passwordInput;   // TMP_InputField for password
    public TMP_InputField emailInput;      // TMP_InputField for email
    public TextMeshProUGUI errorMessage;   // TextMeshProUGUI for error message
    public Button registerButton;          // Register Button
    public Button returnButton;            // Return Button

    private string filePath;

    void Start()
    {
        // Set the file path to save the data in the project folder
        filePath = Application.dataPath + "/users.txt";

        // Assign the button click listener
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        returnButton.onClick.AddListener(OnReturnButtonClicked); // Assign event
    }

    // Called when the Register button is clicked
    void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string email = emailInput.text;

        // Check if any of the fields are empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            errorMessage.text = "Visi laukai turi būti užpildyti!";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // Optional: Basic Email Validation (you can use a more advanced regex for email validation)
        if (!IsValidEmail(email))
        {
            errorMessage.text = "Netinkamas elektroninio pašto formatas!";
            errorMessage.gameObject.SetActive(true);
            return;
        }

        // Save the user credentials (username, password, email) to the file
        SaveUserCredentials(username, password, email);
    }

    void OnReturnButtonClicked()
    {
        errorMessage.text = "";
        errorMessage.gameObject.SetActive(false);
    }

    // Simple email validation method (you can extend this)
    bool IsValidEmail(string email)
    {
        // Simple email check (e.g., must contain '@' and a dot)
        return email.Contains("@") && email.Contains(".");
    }

    // Save the username, password, and email to the users.txt file
    void SaveUserCredentials(string username, string password, string email)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close(); // Create file if it doesn't exist
        }

        // Write the username, password, and email to the file (with a delimiter like ':')
        File.AppendAllText(filePath, username + ":" + password + ":" + email + "\n");

        // Provide feedback to the user
        errorMessage.text = "Registracija sėkminga!";
        errorMessage.gameObject.SetActive(true);
    }
}
