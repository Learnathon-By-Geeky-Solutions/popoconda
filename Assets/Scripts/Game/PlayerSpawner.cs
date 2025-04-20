using Characters;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject _playerInstance;
        public PlayableDirector timelineDirector;
        
        public delegate void StatEvent();
        public static event StatEvent OnPlayerSpawn;


        private void Awake()
        {
            SpawnPlayer();
            Enemy.OnBossDeath += SpawnPlayer;
        }
        
        private void OnDestroy()
        {
            Enemy.OnBossDeath -= SpawnPlayer;
        }

        private void SpawnPlayer()
        {
            _playerInstance = Instantiate(playerPrefab, transform.position, transform.rotation);
            OnPlayerSpawn?.Invoke();
            BindTimelineAnimation();
        }

        private void BindTimelineAnimation()
        {
            var timeline = timelineDirector.playableAsset as TimelineAsset;

            if (timeline != null)
            {
                var track = timeline.GetOutputTrack(0);
                timelineDirector.SetGenericBinding(track, _playerInstance);
            }
        }
    }
}