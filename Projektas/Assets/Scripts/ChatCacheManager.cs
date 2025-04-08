using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;

public class ChatCacheManager : MonoBehaviour
{
    private List<string> chatCache = new List<string>();
    private DatabaseReference dbReference;
    private VoiceAssistantController voiceAssistant;

    void Awake()
    {
        // Use FindFirstObjectByType (or similar) to get the VoiceAssistantController in the scene
        voiceAssistant = FindFirstObjectByType<VoiceAssistantController>();
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                // Set your project's Firebase Realtime Database URL
                app.Options.DatabaseUrl = new Uri("https://virtualusdraugas-4b8f5-default-rtdb.firebaseio.com/");
                dbReference = FirebaseDatabase.GetInstance(app).RootReference;
                Debug.Log("Firebase Initialized, Database Reference set.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies!");
            }
        });
    }

    /// <summary>
    /// Adds a message (only the STT output) to the conversation cache.
    /// </summary>
    public void AddMessageToCache(string message)
    {
        chatCache.Add(message);
    }

    /// <summary>
    /// This method should be called (e.g., via a key/button press) when the conversation ends.
    /// It retrieves past conversation summaries from the database, combines them with the current cache,
    /// then calls ChatGPT to generate an overall summary and saves it to the database.
    /// </summary>
    public void EndConversation()
    {
        if (chatCache.Count == 0)
        {
            Debug.LogWarning("Chat cache is empty. Nothing to summarize.");
            return;
        }

        // Combine the current conversation messages into a single string.
        string currentConversation = string.Join("\n", chatCache);

        // Start the process of retrieving past summaries, combining with current conversation, summarizing and saving.
        StartCoroutine(GenerateSummaryAndSave(currentConversation));
    }

    /// <summary>
    /// Retrieves past conversation summaries from the database.
    /// </summary>
    private IEnumerator GetPastSummaries(System.Action<string> onComplete)
    {
        string combinedSummaries = "";
        var getTask = dbReference.Child("chatSummaries").GetValueAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        if (getTask.Exception != null)
        {
            Debug.LogError("Error retrieving past summaries: " + getTask.Exception);
            onComplete("");
            yield break;
        }

        DataSnapshot snapshot = getTask.Result;
        if (snapshot.Exists)
        {
            foreach (DataSnapshot child in snapshot.Children)
            {
                if (child.HasChild("summary"))
                {
                    string pastSummary = child.Child("summary").Value.ToString();
                    combinedSummaries += pastSummary + "\n";
                }
            }
        }
        onComplete(combinedSummaries);
    }

    /// <summary>
    /// Combines past summaries (retrieved from the database) with the current conversation,
    /// sends them to ChatGPT to generate a new overall summary, and saves the new summary to the database.
    /// </summary>
    private IEnumerator GenerateSummaryAndSave(string currentConversationText)
    {
        // First, retrieve past conversation summaries from the database.
        string pastConversations = "";
        yield return StartCoroutine(GetPastSummaries(result => pastConversations = result));

        // Combine past conversation summaries with the current conversation.
        string combinedConversation = string.IsNullOrEmpty(pastConversations) ?
                                      currentConversationText :
                                      pastConversations + "\n" + currentConversationText;

        // Create a prompt that instructs ChatGPT to summarize only the user's spoken words.
        string summaryPrompt = 
            "Prašau sukurti aiškią santrauką remiantis šiais pokalbio įrašais (tik vartotojo sakyta, STT tekstai):\n" + 
            combinedConversation;

        // Get the summary from ChatGPT.
        string summaryResult = "";
        if (voiceAssistant == null)
        {
            Debug.LogError("VoiceAssistantController not found in scene.");
            yield break;
        }
        yield return StartCoroutine(
            voiceAssistant.ChatWithGPT(summaryPrompt, (result) => summaryResult = result)
        );

        if (string.IsNullOrEmpty(summaryResult))
        {
            Debug.LogError("Santraukos generavimas nepavyko (gautas tuščias atsakymas).");
            yield break;
        }

        // Create the chat summary object.
        ChatSummary chatSummary = new ChatSummary
        {
            summary = summaryResult,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // Convert the object to JSON.
        string json = JsonConvert.SerializeObject(chatSummary);

        // Print the generated summary JSON for debugging.
        Debug.Log("Generated summary JSON:\n" + json);

        // Save the summary to the Firebase database under the "chatSummaries" node.
        var dbTask = dbReference.Child("chatSummaries").Push().SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError("Klaida įrašant santrauką: " + dbTask.Exception);
        }
        else
        {
            Debug.Log("Santrauka sėkmingai įrašyta į duomenų bazę.");
            Debug.Log("Saved Chat Summary:\nSummary: " + chatSummary.summary + "\nTimestamp: " + chatSummary.timestamp);
            // Clear the current conversation cache.
            chatCache.Clear();
        }
    }
}

[Serializable]
public class ChatSummary
{
    public string summary;
    public string timestamp;
}
