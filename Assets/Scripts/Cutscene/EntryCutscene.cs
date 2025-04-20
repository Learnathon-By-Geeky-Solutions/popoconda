using Game;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class EntryCutscene : MonoBehaviour
    {
        private PlayableDirector _entryCutsceneDirector;
        
        private void Awake()
        {
            _entryCutsceneDirector = GetComponent<PlayableDirector>();
            PlayerSpawner.OnPlayerSpawn += StartEntryCutscene;
        }
        
        private void OnDestroy()
        {
            PlayerSpawner.OnPlayerSpawn -= StartEntryCutscene;
        }
        
        private void StartEntryCutscene()
        {
            _entryCutsceneDirector.time = 0;
            _entryCutsceneDirector.Evaluate(); 
            _entryCutsceneDirector.Play();
            Debug.Log("Entry cutscene started");
        }
    }
    
}
