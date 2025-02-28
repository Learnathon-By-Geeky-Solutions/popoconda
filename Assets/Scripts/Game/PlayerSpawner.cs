using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cysharp.Threading.Tasks;  // Import UniTask

namespace Game
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject _playerInstance;
        public PlayableDirector timelineDirector;
        
        public delegate void StatEventWithFloat(float direction);
        public delegate void StatEvent();
        public static event StatEventWithFloat OnPlayerMove;
        public static event StatEvent OnCutsceneEnd;

        private void Awake()
        {
            SpawnPlayer();
            RunPlayerCutscene();
        }
        
        private void OnDestroy()
        {
            OnPlayerMove = null;
            OnCutsceneEnd = null;
        }

        private void SpawnPlayer()
        {
            _playerInstance = Instantiate(playerPrefab, transform.position, transform.rotation);
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

        private async Task RunPlayerCutscene()
        {
            if (timelineDirector != null)
            {
                timelineDirector.Play();
                await MovePlayerDuringTimeline();
                
                // After timeline stops, invoke OnPlayerMove(0) 50 times
                for (int i = 0; i < 50; i++)
                {
                    OnPlayerMove?.Invoke(0);
                    await UniTask.Yield();
                }
                
                OnCutsceneEnd?.Invoke();
                
                // Destroy the timeline director
                Destroy(timelineDirector.gameObject);
            }
        }

        private async UniTask MovePlayerDuringTimeline()
        {
            // Continuously invoke OnPlayerMove(1) while the timeline is playing
            while (timelineDirector.state == PlayState.Playing)
            {
                OnPlayerMove?.Invoke(1);
                await UniTask.Delay(50);
            }
        }
    }
}