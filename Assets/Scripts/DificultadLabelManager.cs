using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class DificultadLabelManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI labelText; // Reference to the TextMeshProUGUI component

    void Start()
    {
        // Set the initial text
        InitLabel();
    }

    // Method to update the label based on the variable
    void InitLabel()
    {
        labelText.text = DialogflowAgent.dificultad;
    }
}