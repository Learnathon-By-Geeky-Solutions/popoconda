using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "DialogueList", menuName = "Scriptable Objects/DialogueList")]
    public class DialogueList : ScriptableObject
    {
        [SerializeField] private List<AssetReference> levels;
        
        public List<AssetReference> Levels => levels;
    }
}

