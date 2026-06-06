using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextNodeIndex;
}

[System.Serializable]
public class DialogueNode
{
    public string speakerName;

    [TextArea(2, 5)]
    public string dialogueText;

    public List<DialogueChoice> choices;

    public bool endsDialogue;
    public bool createChoices;
   // public int nextNodeIndex;
}
