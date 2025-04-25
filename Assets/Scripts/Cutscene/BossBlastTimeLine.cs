using Characters;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class BossBlastTimeLine : MonoBehaviour
    {
        private PlayableDirector cutsceneObject;
        [SerializeField] private ParticleSystem blastObject1;
        [SerializeField] private ParticleSystem blastObject2;
        
        private void Awake()
        {
            cutsceneObject = GetComponent<PlayableDirector>();
            PlayerController.OnBossStateChange += StartCutscene;
            CutsceneManager.OnBlastEvent += PlayBlast;
        }
        
        private void OnDestroy()
        {
            PlayerController.OnBossStateChange -= StartCutscene;
            CutsceneManager.OnBlastEvent -= PlayBlast;
        }
        
        private void StartCutscene(int state)
        {
            if (state == 3)
            {
                cutsceneObject.Play();
                Debug.Log("Playing boss blast cutscene");
            }
        }
        
        private void PlayBlast()
        {
            if (blastObject1 == null || blastObject2 == null)
            {
                Debug.LogError("Blast objects are not assigned.");
                return;
            }
            blastObject1.Play();
            blastObject2.Play();
            Debug.Log("Playing blast effect");
        }
        

        

    }
    
}