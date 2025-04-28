using Characters;
using Cutscene;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject _playerInstance;
        [SerializeField] private PlayableDirector timelineDirector;
        [SerializeField] private PlayableDirector nextLevelDirector;
        private bool _onVerticalPlatform;
        
        public delegate void StatEvent();
        public static event StatEvent OnPlayerSpawn;


        private void Awake()
        {
            SpawnPlayer();
            Hero.OnHeroDeath += RespawnPlayer;
            _onVerticalPlatform = false;
            CutsceneManager.OnVerticalPlatformEvent += () => _onVerticalPlatform = true;
        }
        
        private void OnDestroy()
        {
            Hero.OnHeroDeath -= RespawnPlayer;
        }

        private void SpawnPlayer()
        {
            _playerInstance = Instantiate(playerPrefab);

            OnPlayerSpawn?.Invoke();
            BindTimelineAnimation();
        }
        
        private void RespawnPlayer()
        {
            if (_onVerticalPlatform)
            {
                _playerInstance.SetActive(false);
                return;
            }
            
            _playerInstance.SetActive(false);
            _playerInstance.transform.position = new Vector3(0, 0, 0); // Set to spawn position
            _playerInstance.SetActive(true);
            timelineDirector.Play();
            BindTimelineAnimation();
        }

        private void BindTimelineAnimation()
        {
            var timeline = timelineDirector.playableAsset as TimelineAsset;
            var nextLevelTimeline = nextLevelDirector.playableAsset as TimelineAsset;

            if (timeline != null)
            {
                var track = timeline.GetOutputTrack(0);
                timelineDirector.SetGenericBinding(track, _playerInstance);
            }
            
            if (nextLevelTimeline != null)
            {
                var nextLevelTrack = nextLevelTimeline.GetOutputTrack(0);
                nextLevelDirector.SetGenericBinding(nextLevelTrack, _playerInstance);
            }
        }
    }
}