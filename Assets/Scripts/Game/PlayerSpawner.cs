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
            Hero.OnHeroDeath += RespawnPlayer;
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
            _playerInstance.SetActive(false);
            _playerInstance.transform.position = new Vector3(0, 0, 0); // Set to spawn position
            _playerInstance.SetActive(true);
            timelineDirector.Play();
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