using Characters;
using Cutscene;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game
{
    public class HeroSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject HeroPrefab;
        private GameObject _HeroInstance;
        [SerializeField] private PlayableDirector timelineDirector;
        [SerializeField] private PlayableDirector nextLevelDirector;
        private bool _onVerticalPlatform;
        
        public delegate void StatEvent();
        public static event StatEvent OnHeroSpawn;


        private void Awake()
        {
            SpawnHero();
            Hero.OnHeroDeath += RespawnHero;
            _onVerticalPlatform = false;
            CutsceneManager.OnVerticalPlatformEvent += () => _onVerticalPlatform = true;
        }
        
        private void OnDestroy()
        {
            Hero.OnHeroDeath -= RespawnHero;
        }

        private void SpawnHero()
        {
            _HeroInstance = Instantiate(HeroPrefab);

            OnHeroSpawn?.Invoke();
            BindTimelineAnimation();
        }
        
        private void RespawnHero()
        {
            if (_onVerticalPlatform)
            {
                _HeroInstance.SetActive(false);
                return;
            }
            
            _HeroInstance.SetActive(false);
            _HeroInstance.transform.position = new Vector3(0, 0, 0); // Set to spawn position
            _HeroInstance.SetActive(true);
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
                timelineDirector.SetGenericBinding(track, _HeroInstance);
            }
            
            if (nextLevelTimeline != null)
            {
                var nextLevelTrack = nextLevelTimeline.GetOutputTrack(0);
                nextLevelDirector.SetGenericBinding(nextLevelTrack, _HeroInstance);
            }
        }
    }
}