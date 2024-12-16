using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Animator dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isDialogueActive = false;
    private int currentDialogueIndex = 0;
    private List<DialogueEntry> currentDialogue;
    private bool isTyping = false;
    private string currentText = "";
    private float typeTimer = 0f;
    private int typeIndex = 0;

    [System.Serializable]
    public class DialogueEntry
    {
        public string speakerName;
        public string dialogueText;
    }

    [System.Serializable]
    public class DialogueSequence
    {
        public string sequenceName;
        public List<DialogueEntry> dialogue;
    }

    public List<DialogueSequence> dialogueSequences;

    private void Start()
    {
        dialoguePanel.SetBool("Open", false);
    }

    public void Interact()
    {
        if (!isDialogueActive)
        {
            StartDialogue(dialogueSequences[0]);
        }
        else if (!isTyping)
        {
            AdvanceDialogue();
        }
        else
        {
            CompleteTyping();
        }
    }

    private void Update()
    {
        if (isTyping)
        {
            typeTimer += Time.deltaTime;
            if (typeTimer >= typingSpeed)
            {
                typeTimer = 0f;
                if (typeIndex < currentText.Length)
                {
                    dialogueText.text += currentText[typeIndex];
                    typeIndex++;
                }
                else
                {
                    isTyping = false;
                }
            }
        }
    }

    public void StartDialogue(DialogueSequence sequence)
    {
        currentDialogue = sequence.dialogue;
        currentDialogueIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetBool("Open", true);
        DisplayCurrentDialogue();
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < currentDialogue.Count)
        {
            DisplayCurrentDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentDialogue()
    {
        DialogueEntry entry = currentDialogue[currentDialogueIndex];
        nameText.text = entry.speakerName;
        StartTyping(entry.dialogueText);
    }

    private void StartTyping(string text)
    {
        isTyping = true;
        currentText = text;
        dialogueText.text = "";
        typeIndex = 0;
    }

    private void CompleteTyping()
    {
        isTyping = false;
        dialogueText.text = currentText;
        typeIndex = currentText.Length;
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetBool("Open", false);
        currentDialogueIndex = 0;
    }
}