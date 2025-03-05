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
            public int speakerID; // Key to identify the speaker
            public LocalizedString dialogueText; // Localized dialogue text
        }

        [SerializeField] private string[] characterName;
        [SerializeField] private Color[] characterColor;

        [SerializeField] private DialogueEntry[] dialogues; // Sequence of dialogues
        
        public string[] CharacterName => characterName;
        public Color[] CharacterColor => characterColor;
        public DialogueEntry[] Dialogues => dialogues;
    }
}

