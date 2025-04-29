using Characters;
using UnityEngine;
using Scene;
using UI;

namespace Audio
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundData soundData;
        
        // Sound Source
        private AudioSource audioSource;
        private AudioClip currentClip;
        private static SoundManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            Hero.OnHeroDeath += PlayWinSound;
            Player.OnPlayerDeath += PlayLoseSound;
            SceneManager.OnDialogueSceneLoaded += PlayBackgroundMusic;
            PauseMenu.UIEnableEvent += PauseMusic;
            PauseMenu.UIDisableEvent += PlayMusic;
        }

        private void OnDisable()
        {
            audioSource = null;
            Hero.OnHeroDeath -= PlayWinSound;
            Player.OnPlayerDeath -= PlayLoseSound;
            SceneManager.OnDialogueSceneLoaded -= PlayBackgroundMusic;
            PauseMenu.UIEnableEvent -= PauseMusic;
            PauseMenu.UIDisableEvent -= PlayMusic;
        }
        
        private void PlayWinSound()
        {
            PauseMusic();
            audioSource.PlayOneShot(soundData.WinSound);
        }
        
        private void PlayLoseSound()
        {
            PauseMusic();
            audioSource.PlayOneShot(soundData.LoseSound);
        }
        
        private void PlayBackgroundMusic(int levelIndex)
        {
            currentClip = soundData.LevelBackgroundMusic[levelIndex];
            PlayMusic();
        }
        
        private void PlayMusic()
        {
            audioSource.clip = currentClip;
            audioSource.Play();
        }
        
        private void PauseMusic()
        {
            audioSource.clip = null;
        }
    }
}