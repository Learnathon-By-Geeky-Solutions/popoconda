using Characters;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class VerticalPlatformTimeline : MonoBehaviour
    { 
        private PlayableDirector _playableDirector;

        private void OnEnable()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            HeroAI.OnHeroSurvive += StartCutscene;
        }
        
        private void OnDisable()
        {
            HeroAI.OnHeroSurvive -= StartCutscene;
        }
        
        private void StartCutscene()
        {
            _playableDirector.Play();
            Debug.Log("Vertical Platform Timeline started");
        }
    }
    
}
