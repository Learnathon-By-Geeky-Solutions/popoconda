using Characters;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class TransitionCutscene : MonoBehaviour
    {
        [SerializeField] private GameObject cutsceneObject;
        private PlayableDirector _playableDirector;

        private void Awake()
        {
            _playableDirector = cutsceneObject.GetComponent<PlayableDirector>();
            cutsceneObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            Hero.OnHeroDeath += StartCutscene;
        }
        
        private void OnDisable()
        {
            Hero.OnHeroDeath -= StartCutscene;
        }
        
        private void StartCutscene()
        {
            cutsceneObject.SetActive(true);
            _playableDirector.Play();
        }
        
    }
    
}
