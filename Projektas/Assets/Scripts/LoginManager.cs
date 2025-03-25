using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    public UIElementMover UIElementMover;
    public TMP_InputField nameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public Button returnButton;

    private DatabaseReference dbReference;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.GetInstance(app).RootReference;
        });

        returnButton.onClick.AddListener(ClearFields);
    }

    public void Login()
    {
        string name = nameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        {
            SetMessage("Vardas ar slaptažodis neužpildyti.", true);
            return;
        }

        CheckLogin(name, password);
    }

    private void CheckLogin(string name, string password)
    {
        dbReference.Child("users").OrderByChild("name").EqualTo(name).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    bool loginSuccess = false;
                    string accountCreatedDate = "";

                    foreach (var user in snapshot.Children)
                    {
                        string storedData = user.Child("password").Value.ToString();
                        string[] parts = storedData.Split(':');

                        if (parts.Length == 2)
                        {
                            string storedHashedPassword = parts[0];
                            string storedSalt = parts[1];

                            if (storedHashedPassword == HashPassword(password, storedSalt))
                            {
                                loginSuccess = true;

                                // Fetch account creation date
                                if (user.HasChild("createdAt"))
                                {
                                    accountCreatedDate = user.Child("createdAt").Value.ToString();
                                }

                                break;
                            }
                        }
                    }

                    if (loginSuccess)
                    {
                        SetMessage($"Prisijugimas sėkmingas!\nPaskyra sukurta: {accountCreatedDate}", false);
                        UIElementMover.MoveUILeft();
                        //SceneManager.LoadScene("Demo");
                    }
                    else
                    {
                        SetMessage("Prisijugimas nesėkmingas: neteisingas slaptažodis.", true);
                    }
                }
                else
                {
                    SetMessage("Prisijugimas nesėkmingas: naudotojas nerastas.", true);
                }
            }
            else
            {
                SetMessage("Klaida teikiant duomenis: " + task.Exception, true);
            }
        });
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
        nameInput.text = "";
        passwordInput.text = "";
        SetMessage("", false);
    }
}
