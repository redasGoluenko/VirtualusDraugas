using System;
using System.Security.Cryptography;
using System.Text;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField gmail;
    public TMP_InputField username;
    public TMP_InputField password;

    public TMP_Text messageText;
    public Button returnButton;

    private DatabaseReference dbReference;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.GetInstance(app).RootReference;
        });

        returnButton.onClick.AddListener(ClearFields);
    }

    public void CreateUser()
    {
        if (string.IsNullOrEmpty(gmail.text) || string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text))
        {
            SetMessage("Visi laukai turi būti užpildyti!", true);
            return;
        }

        if (!IsValidEmail(gmail.text))
        {
            SetMessage("Prašome įvesti tinkamą el. pašto formatą!", true);
            return;
        }

        string salt = GenerateSalt();
        string hashedPassword = HashPassword(password.text, salt);
        string createdAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // Get current time in UTC
        string prompt = "unassigned"; // Initial value

        DatabaseReference newUserRef = dbReference.Child("users").Push();
        string userId = newUserRef.Key;

        User newUser = new User(gmail.text, username.text, hashedPassword + ":" + salt);
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                dbReference.Child("users").Child(userId).Child("createdAt").SetValueAsync(createdAt);
                dbReference.Child("users").Child(userId).Child("prompt").SetValueAsync(prompt); // Store the prompt

                SetMessage("Naudotojas priregistruotas sėkmingai!", false);
            }
            else
            {
                SetMessage("Klaida kuriant naudotoją: " + task.Exception.Message, true);
            }
        });
    }

    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private void SetMessage(string message, bool isError)
    {
        messageText.text = message;
    }

    private void ClearFields()
    {
        gmail.text = "";
        username.text = "";
        password.text = "";
        messageText.text = "";
    }
}
