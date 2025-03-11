using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class VoiceAssistantController : MonoBehaviour
{
    [Header("OpenAI Settings")]
    [Tooltip("Your OpenAI API key (sk-...)")]
    public string openAI_API_Key = "YOUR_OPENAI_API_KEY_HERE";

    [Header("Recording Settings")]
    [Tooltip("Sample rate for microphone recording (e.g., 16000 or 44100)")]
    public int sampleRate = 44100;

    [Tooltip("Key to toggle recording on/off")]
    public KeyCode recordKey = KeyCode.R;

    [Header("Audio")]
    [Tooltip("AudioSource to play TTS audio")]
    public AudioSource audioSource;

    private bool isRecording = false;
    private AudioClip recordedClip;
    private string micDevice;
    private float recordStartTime;

    void Start()
    {
        // Use the first available microphone
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(recordKey))
        {
            if (!isRecording)
                StartRecording();
            else
                StopRecordingAndProcess();
        }
    }

    void StartRecording()
    {
        if (string.IsNullOrEmpty(micDevice))
            return;

        isRecording = true;
        // Start recording; we record for 300 seconds (max) but will stop manually
        recordedClip = Microphone.Start(micDevice, false, 300, sampleRate);
        recordStartTime = Time.time;
        Debug.Log("Recording started.");
    }

    void StopRecordingAndProcess()
    {
        isRecording = false;
        Microphone.End(micDevice);
        Debug.Log("Recording stopped.");

        // Directly use the entire recorded clip without trimming
        byte[] wavData = WavUtility.FromAudioClip(recordedClip);
        if (wavData == null || wavData.Length == 0)
        {
            Debug.LogError("WAV conversion failed.");
            return;
        }

        // Run the AI pipeline (Whisper -> ChatGPT -> TTS)
        StartCoroutine(RunAIPipeline(wavData));
    }

    IEnumerator RunAIPipeline(byte[] wavData)
    {
        // 1. Transcribe using Whisper (language set to Lithuanian "lt")
        string transcription = "";
        yield return StartCoroutine(TranscribeAudio(wavData, result => transcription = result));
        if (string.IsNullOrEmpty(transcription))
        {
            Debug.LogError("Transcription failed.");
            yield break;
        }
        Debug.Log("Transcribed text: " + transcription);

        // 2. Get response from ChatGPT
        string chatResponse = "";
        yield return StartCoroutine(ChatWithGPT(transcription, result => chatResponse = result));
        if (string.IsNullOrEmpty(chatResponse))
        {
            Debug.LogError("ChatGPT response is empty.");
            yield break;
        }
        Debug.Log("ChatGPT response: " + chatResponse);

        // 3. Convert ChatGPT text to speech via TTS
        byte[] ttsAudioData = null;
        yield return StartCoroutine(ConvertTextToSpeech(chatResponse, result => ttsAudioData = result));
        if (ttsAudioData == null || ttsAudioData.Length == 0)
        {
            Debug.LogError("TTS failed to produce audio.");
            yield break;
        }

        // 4. Play the TTS audio
        yield return StartCoroutine(PlayAudioFromBytes(ttsAudioData));
    }

    IEnumerator TranscribeAudio(byte[] audioData, System.Action<string> onComplete)
    {
        string url = "https://api.openai.com/v1/audio/transcriptions";
        WWWForm form = new WWWForm();
        form.AddField("model", "whisper-1");
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");
        // Allow Lithuanian speech recognition:
        form.AddField("language", "lt");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + openAI_API_Key);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Whisper API error: " + www.error);
                onComplete("");
            }
            else
            {
                string json = www.downloadHandler.text;
                WhisperResponse response = JsonConvert.DeserializeObject<WhisperResponse>(json);
                onComplete(response.text);
            }
        }
    }

    IEnumerator ChatWithGPT(string userInput, System.Action<string> onComplete)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        ChatGPTRequest chatRequest = new ChatGPTRequest
        {
            model = "gpt-3.5-turbo",
            messages = new List<Message>()
            {
                new Message { role = "user", content = userInput }
            },
            max_tokens = 200
        };

        string jsonData = JsonConvert.SerializeObject(chatRequest);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + openAI_API_Key);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("ChatGPT API error: " + www.error);
                onComplete("");
            }
            else
            {
                string responseJson = www.downloadHandler.text;
                ChatGPTResponse response = JsonConvert.DeserializeObject<ChatGPTResponse>(responseJson);
                if (response.choices != null && response.choices.Count > 0)
                {
                    onComplete(response.choices[0].message.content);
                }
                else
                {
                    onComplete("");
                }
            }
        }
    }

    IEnumerator ConvertTextToSpeech(string text, System.Action<byte[]> onComplete)
    {
        string url = "https://api.openai.com/v1/audio/speech";
        TTSRequest ttsRequest = new TTSRequest
        {
            model = "tts-1",
            voice = "alloy",
            input = text
        };

        string jsonData = JsonConvert.SerializeObject(ttsRequest);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + openAI_API_Key);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TTS API error: " + www.error);
                onComplete(null);
            }
            else
            {
                onComplete(www.downloadHandler.data);
            }
        }
    }

    IEnumerator PlayAudioFromBytes(byte[] audioData)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "ttsResponse.mp3");
        File.WriteAllBytes(filePath, audioData);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading TTS audio clip: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}

// --- Data Classes ---
[System.Serializable]
public class ChatGPTRequest
{
    public string model;
    public List<Message> messages;
    public int max_tokens;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

public class ChatGPTResponse
{
    public List<ChatGPTChoice> choices;
}

public class ChatGPTChoice
{
    public ChatGPTResponseMessage message;
}

public class ChatGPTResponseMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class TTSRequest
{
    public string model;
    public string voice;
    public string input;
}

public class WhisperResponse
{
    public string text;
}
