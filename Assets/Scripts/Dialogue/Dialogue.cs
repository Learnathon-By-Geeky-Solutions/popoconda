using UnityEngine;
using System;
using UnityEngine.Localization;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [Serializable]
        public class DialogueEntry
        {
            public string speakerID; // Key to identify the speaker
            public LocalizedString dialogueText; // Localized dialogue text
        }

        public string[] characterName;
        public Color[] characterColor;

        public DialogueEntry[] dialogues; // Sequence of dialogues
    }
}

