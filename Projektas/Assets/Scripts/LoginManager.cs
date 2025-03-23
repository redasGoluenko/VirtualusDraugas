using UnityEngine;
using UnityEngine.SceneManagement;  // To load scenes
using UnityEngine.UI;              // To work with UI elements like Button
using TMPro;                       // For TMP_InputField
using Firebase;
using Firebase.Database;
using Firebase.Extensions;  // To handle async tasks

public class LoginManager : MonoBehaviour
{
    // Assign these in the inspector
    public TMP_InputField nameInput;    // Input field for the name (not surname)
    public TMP_InputField passwordInput; // Input field for the password
    public TMP_Text messageText;         // Text field for displaying error/success messages
    public Button returnButton;          // Button for clearing the fields

    private DatabaseReference dbReference; // Firebase reference

    private void Start()
    {
        // Ensure Firebase is initialized
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.GetInstance(app).RootReference; // Initialize the reference to Firebase           
        });

        // Set the return button to call ClearFields when clicked
        returnButton.onClick.AddListener(ClearFields);
    }

    // Login function
    public void Login()
    {
        // Get the name and password entered by the user
        string name = nameInput.text;
        string password = passwordInput.text;

        // Check if both fields are filled out
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        {
            SetMessage("Vardas ar slaptažodis neužpildyti.", true);  // Show error message
            return;
        }

        // Start the login check
        CheckLogin(name, password);
    }

    // Function to check login credentials against Firebase Database
    private void CheckLogin(string name, string password)
    {
        dbReference.Child("users").OrderByChild("name").EqualTo(name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                // Check if we found a user with this name
                if (snapshot.Exists)
                {
                    bool loginSuccess = false;
                    foreach (var user in snapshot.Children)
                    {
                        // Compare password from the database with the entered password
                        string storedPassword = user.Child("password").Value.ToString();
                        if (storedPassword == password)
                        {
                            // Login successful
                            loginSuccess = true;
                            break;
                        }
                    }

                    // If login is successful, load the Demo scene
                    if (loginSuccess)
                    {
                        SetMessage("Prisijugimas sėkmingas!", false);  // Show success message
                        //SceneManager.LoadScene("Demo");
                    }
                    else
                    {
                        // If password doesn't match, show error
                        SetMessage("Prisijugimas nesėkmingas: neteisingas slaptažodis.", true);  // Show error message
                    }
                }
                else
                {
                    // If no user with that name is found
                    SetMessage("Prisijugimas nesėkmingas: naudotojas nerastas.", true);  // Show error message
                }
            }
            else
            {
                // Handle the case where the task failed (network issue, etc.)
                SetMessage("Klaida teikiant duomenis: " + task.Exception, true);  // Show error message
            }
        });
    }

    // This function sets the message text and controls the color (red for error, green for success)
    private void SetMessage(string message, bool isError)
    {
        messageText.text = message;      
    }

    // Function to clear the fields and error message
    private void ClearFields()
    {
        // Clear the name and password input fields if they have text
        nameInput.text = "";
        passwordInput.text = "";

        // Clear the error message
        SetMessage("", false); // Clears the message (not showing anything)
    }
}
