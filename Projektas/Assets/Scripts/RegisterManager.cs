using System.Collections;
using System.Collections.Generic;
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

    private string userID;
    private DatabaseReference dbReference;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            app.Options.DatabaseUrl = new System.Uri("https://your-project-id.firebaseio.com/");

            dbReference = FirebaseDatabase.GetInstance(app).RootReference;

            userID = SystemInfo.deviceUniqueIdentifier;

            Debug.Log("Firebase Initialized and Database Reference Set");
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

        User newUser = new User(gmail.text, username.text, password.text);
        string json = JsonUtility.ToJson(newUser);

        DatabaseReference newUserRef = dbReference.Child("users").Push();

        newUserRef.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                SetMessage("Naudotojas priregistruotas sėkmingai!", false);
            }
            else
            {
                SetMessage("Klaida kuriant naudotoją: " + task.Exception.Message, true);
            }
        });
    }

    public void FetchUsers()
    {
        dbReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    foreach (var user in snapshot.Children)
                    {
                        Debug.Log("User ID: " + user.Key);
                        Debug.Log("User Data: " + user.GetRawJsonValue());
                    }
                    SetMessage("Naudotojai pateikti sėkmingai!", false);
                }
                else
                {
                    SetMessage("No users found.", true);
                }
            }
            else
            {
                SetMessage("Error fetching users: " + task.Exception.Message, true);
            }
        });
    }

    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
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
