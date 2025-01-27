using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public InteractableObject interactableObject;
    public Image buttonPrompt;

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
    public int currentDialogueSequenceIndex = 0;

    private void Start()
    {
        dialoguePanel.SetBool("Open", false);
        if(interactableObject == null)
        {
            interactableObject = GetComponent<InteractableObject>();
        }
    }

    public void Interact()
    {
        if (!isDialogueActive)
        {
            int newIndex = Random.Range(0, dialogueSequences.Count);
            if (newIndex == currentDialogueSequenceIndex)
            {
                currentDialogueSequenceIndex = (currentDialogueSequenceIndex + 1) % dialogueSequences.Count;
            }
            else
            {
                currentDialogueSequenceIndex = newIndex;
            }
            StartCoroutine(StartDialogue(dialogueSequences[currentDialogueSequenceIndex]));
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
        ShowButtonPrompt();
    }

    private void ShowButtonPrompt()
    {
        buttonPrompt.enabled = !isTyping;
        buttonPrompt.sprite = InputTracker.instance.usingMouse ? interactableObject.controlPC : interactableObject.controlGamepad;
    }

    public IEnumerator StartDialogue(DialogueSequence sequence)
    {
        buttonPrompt.enabled = false;
        dialogueText.text = "";
        currentDialogue = sequence.dialogue;
        currentDialogueIndex = 0;
        nameText.text = currentDialogue[currentDialogueIndex].speakerName;
        isDialogueActive = true;
        dialoguePanel.SetBool("Open", true);
        yield return new WaitForSeconds(1f);
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
        CloseDialougeBox();
        currentDialogueIndex = 0;
        if(interactableObject != null)
        {
            interactableObject.EndInteraction();
        }
    }

    public void ForceReset()
    {           
        isDialogueActive = false;
        CloseDialougeBox();
        currentDialogueIndex = 0;
    }

    public void CloseDialougeBox()
    {
        buttonPrompt.enabled = false;
        dialoguePanel.SetBool("Open", false);
    }
}