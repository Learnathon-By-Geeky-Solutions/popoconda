using UnityEngine;

namespace Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        public delegate void StatEvent();
        public static event StatEvent OnCutsceneStart;
        public static event StatEvent OnCutsceneEnd;
        
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
        
        
    }
    
}
