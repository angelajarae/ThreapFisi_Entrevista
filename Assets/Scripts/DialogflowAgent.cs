using Google.Cloud.Dialogflow.Cx.V3;
using Google.Protobuf;
using System;
using UnityEngine;
using Google.Api.Gax.Grpc.Rest;
using System.Threading.Tasks;

public class DialogflowAgent : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to your AudioSource in the Inspector

    private SessionsClient sessionsClient; // Reuse SessionsClient
    private string sessionId; // Maintain a consistent session ID

    private const string projectId = "therapfisi-entrevista-de--nlos";
    private const string location = "us-central1"; // e.g., "us-central1"
    private const string agentId = "099a192a-c879-4a51-a4c4-d8e58602e73f";

    public static readonly string dificultad="dificil";//facil,intermedio o dificil
    private bool isClientInitialized = false;

    private void Awake()
    {
        ResetSession();

        string credentialsPath = "C:/Users/angel/Documents/GoogleCloudServiceAccount/therapfisi-entrevista-de--nlos-37f963fe34f7.json";
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        Debug.Log(System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
    }

private void Start()
{
    InitializeAgent();
}
private async void InitializeAgent()
{
    try
    {
        sessionsClient = new SessionsClientBuilder
        {
            GrpcAdapter = RestGrpcAdapter.Default, // Configure REST transport
            Endpoint = "us-central1-dialogflow.googleapis.com"
        }.Build();

        isClientInitialized = true;
        Debug.Log("SessionsClient initialized successfully.");

        // Optionally, test with an initial message
        await SendMessageToDialogflow("Hola",false);
        await SendMessageToDialogflow(dificultad,true);
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error initializing SessionsClient: {ex.Message}");
    }
}

    public async Task SendMessageToDialogflow(string userMessage, bool returnAudio)
    {
        if (!isClientInitialized)
        {
            Debug.LogError("SessionsClient is not initialized.");
            return;
        }

        SessionName sessionName = SessionName.FromProjectLocationAgentSession(projectId, location, agentId, sessionId);

        // Create the query input with the user's message
        QueryInput queryInput = new QueryInput
        {
            Text = new TextInput
            {
                Text = userMessage,
            },
            LanguageCode = "es" // Specify the language as Spanish
        };
        

        // Optional: Configure audio response
        OutputAudioConfig audioConfig = new OutputAudioConfig
        {
            AudioEncoding = OutputAudioEncoding.Linear16,
            SampleRateHertz = 16000
        };

        // Prepare the DetectIntentRequest
        DetectIntentRequest request = new DetectIntentRequest
        {
            SessionAsSessionName = sessionName,
            QueryInput = queryInput,
            OutputAudioConfig = audioConfig
        };

        try
        {
            // Make the request to Dialogflow CX
            DetectIntentResponse response = await sessionsClient.DetectIntentAsync(request);

            // Get the text response from the bot
            string responseText = string.Join(" ", response.QueryResult.ResponseMessages[0].Text.Text_);
            Debug.Log("Texto de la respuesta: " + responseText);

            // Play the audio response if it's available
            if (response.OutputAudio.Length > 0&&returnAudio)
            {
                byte[] audioBytes = response.OutputAudio.ToByteArray();

                // Convert the audio to Unity AudioClip (you will need to add WavUtility script for this to work)
                AudioClip audioClip = WavUtility.ToAudioClip(audioBytes, audioConfig.SampleRateHertz);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en DetectIntent: {ex.Message}");
        }
    }

    public async void SendAudioToDialogflow(byte[] audioData)
    {
        if (!isClientInitialized)
        {
            Debug.LogError("SessionsClient is not initialized.");
            return;
        }

        // Create the session name
        SessionName sessionName = SessionName.FromProjectLocationAgentSession(projectId, location, agentId, sessionId);

        // Create the query input with audio configuration
        QueryInput queryInput = new QueryInput
        {
            Audio = new AudioInput
            {
                Config = new InputAudioConfig
                {
                    AudioEncoding = AudioEncoding.Linear16, // PCM 16-bit format
                    SampleRateHertz = 16000               // Set to your audio's sample rate
                },
                Audio = Google.Protobuf.ByteString.CopyFrom(audioData)
            },
            LanguageCode = "es"
        };

        // Optional: Configure audio response
        OutputAudioConfig audioConfig = new OutputAudioConfig
        {
            AudioEncoding = OutputAudioEncoding.Linear16,
            SampleRateHertz = 16000
        };

        // Prepare the DetectIntentRequest
        DetectIntentRequest request = new DetectIntentRequest
        {
            SessionAsSessionName = sessionName,
            QueryInput = queryInput,
            OutputAudioConfig = audioConfig
        };

        try
        {
            // Make the request to Dialogflow CX
            DetectIntentResponse response = await sessionsClient.DetectIntentAsync(request);

            // Get the text response from the bot
            string responseText = string.Join(" ", response.QueryResult.ResponseMessages[0].Text.Text_);
            Debug.Log("Texto de la respuesta: " + responseText);

            // Play the audio response if it's available
            if (response.OutputAudio.Length > 0)
            {
                byte[] responseAudioBytes = response.OutputAudio.ToByteArray();

                // Convert the audio to Unity AudioClip (requires a WavUtility script)
                AudioClip audioClip = WavUtility.ToAudioClip(responseAudioBytes, audioConfig.SampleRateHertz);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en DetectIntent: {ex.Message}");
        }
    }

    public void ResetSession()
    {
        // Generate a new session ID for a fresh conversation
        sessionId = Guid.NewGuid().ToString();
        Debug.Log($"Session reset. New session ID: {sessionId}");
    }
}
