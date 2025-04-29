using Characters;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class TransitionCutscene : MonoBehaviour
    {
        [SerializeField] private GameObject cutsceneObject;
        private PlayableDirector _playableDirector;
        private bool _onVerticalPlatform;

        private void Awake()
        {
            _playableDirector = cutsceneObject.GetComponent<PlayableDirector>();
            cutsceneObject.SetActive(false);
            _onVerticalPlatform = false;
        }
        
        private void OnEnable()
        {
            Hero.OnHeroDeath += StartCutscene;
            CutsceneManager.OnVerticalPlatformEvent += () => _onVerticalPlatform = true;
        }
        
        private void OnDisable()
        {
            Hero.OnHeroDeath -= StartCutscene;
        }
        
        private void StartCutscene()
        {
            if (_onVerticalPlatform) return;
            
            cutsceneObject.SetActive(true);
            _playableDirector.Play();
        }
        
    }
    
}
