using Google.Cloud.Dialogflow.Cx.V3;
using Google.Protobuf;
using System;
using UnityEngine;
using Google.Api.Gax.Grpc.Rest;

public class DialogflowAgent : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to your AudioSource in the Inspector

    private SessionsClient sessionsClient; // Reuse SessionsClient
    private string sessionId; // Maintain a consistent session ID

    private const string projectId = "therapfisi-entrevista-de--nlos";
    private const string location = "us-central1"; // e.g., "us-central1"
    private const string agentId = "099a192a-c879-4a51-a4c4-d8e58602e73f";

    private bool isClientInitialized = false;

    private void Awake()
    {
        sessionId = Guid.NewGuid().ToString(); // Generate a unique session ID

        string credentialsPath = "C:/Users/angel/Documents/GoogleCloudServiceAccount/therapfisi-entrevista-de--nlos-37f963fe34f7.json";
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        Debug.Log(System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
    }

    private void Start()
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
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing SessionsClient: {ex.Message}");
        }
    }

    public async void SendMessageToDialogflow(string userMessage)
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
            if (response.OutputAudio.Length > 0)
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
}
