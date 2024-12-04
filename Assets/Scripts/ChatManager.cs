using UnityEngine;
using UnityEngine.InputSystem;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private DialogflowAgent dialogflowAgent; // Reference to the DialogflowAgent script
    private AudioClip recordingClip;
    private bool isRecording = false;

    private InputAction startRecordingAction;
    private InputAction stopRecordingAction;

    private void Awake()
    {
        // Define and set up the input actions for StartRecording and StopRecording
        startRecordingAction = new InputAction("StartRecording", binding: "<Keyboard>/1");
        stopRecordingAction = new InputAction("StopRecording", binding: "<Keyboard>/2");

        // Enable the actions
        startRecordingAction.Enable();
        stopRecordingAction.Enable();
    }

    private void OnEnable()
    {
        // Bind the actions to their respective methods
        startRecordingAction.performed += context => StartRecording();
        stopRecordingAction.performed += context => StopRecordingAndSend();
    }

    private void OnDisable()
    {
        // Unbind actions when not needed
        startRecordingAction.performed -= context => StartRecording();
        stopRecordingAction.performed -= context => StopRecordingAndSend();
    }

    private void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Recording is already in progress.");
            return;
        }

        Debug.Log("Starting audio recording...");
        isRecording = true;

        // Start recording (up to 10 seconds at 16kHz)
        recordingClip = Microphone.Start(null, false, 10, 16000);
    }

    private void StopRecordingAndSend()
    {
        if (!isRecording)
        {
            Debug.LogWarning("No recording is in progress.");
            return;
        }

        Debug.Log("Stopping audio recording...");
        isRecording = false;

        // Stop recording
        Microphone.End(null);

        if (recordingClip == null)
        {
            Debug.LogError("Failed to capture audio.");
            return;
        }

        Debug.Log("Audio recording finished.");

        // Convert the AudioClip to PCM 16-bit byte array
        byte[] audioData = WavUtility.AudioClipToByteArray(recordingClip);

        // Send the audio data to Dialogflow
        dialogflowAgent.SendAudioToDialogflow(audioData);
    }
}