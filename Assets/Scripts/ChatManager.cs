using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private DialogflowAgent dialogflowAgent; // Reference to the DialogflowAgent script

    private void Start()
    {
        dialogflowAgent.SendMessageToDialogflow("Hola");
    }
    public void ProcessUserMessage(string userMessage)
    {
        dialogflowAgent.SendMessageToDialogflow(userMessage);
    }

}