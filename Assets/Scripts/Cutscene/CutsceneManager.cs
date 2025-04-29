using UnityEngine;

namespace Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        public delegate void StatEvent();
        public static event StatEvent OnCutsceneStart;
        public static event StatEvent OnCutsceneEnd;
        public static event StatEvent OnDialogueStart;
        public static event StatEvent OnBlastEvent;
        public static event StatEvent OnVerticalPlatformEvent;
        
        private bool _isDialogueShown;
        
        
        public static void StartCutscene()
        {
            OnCutsceneStart?.Invoke();
            Debug.Log("Cutscene started");
        }
        
        public static void EndCutscene()
        {
            OnCutsceneEnd?.Invoke();
            Debug.Log("Cutscene ended");
        }
        
        public void LoadDialogueScene()
        {
            if(_isDialogueShown) return;
            
            OnDialogueStart?.Invoke();
            _isDialogueShown = true;
        }
        
        public static void TriggerBlast()
        {
            OnBlastEvent?.Invoke();
            Debug.Log("Blast triggered");
        }
        
        public static void TriggerVerticalPlatform()
        {
            OnVerticalPlatformEvent?.Invoke();
            Debug.Log("Vertical Platform triggered");
        }
        
    }
    
}
