using Characters;
using UnityEngine;

namespace Audio
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundData soundData;
        
        // Sound Source
        private AudioSource audioSource;
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
            Enemy.OnBossDeath += PlayWinSound;
            Player.OnPlayerDeath += PlayLoseSound;
        }

        private void OnDisable()
        {
            audioSource = null;
            Enemy.OnBossDeath -= PlayWinSound;
            Player.OnPlayerDeath -= PlayLoseSound;
        }
        
        private void PlayWinSound()
        {
            audioSource.PlayOneShot(soundData.WinSound);
        }
        
        private void PlayLoseSound()
        {
            audioSource.PlayOneShot(soundData.LoseSound);
        }
    }
}