using UnityEngine;


namespace Audio
{
    [CreateAssetMenu(fileName = "SoundDataSO", menuName = "Scriptable Objects/SoundData")]
    public class SoundData : ScriptableObject
    {
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;
        [SerializeField] private AudioClip[] levelBackgroundMusic;
        [SerializeField] private AudioClip menuMusic;

        public AudioClip WinSound => winSound;
        public AudioClip LoseSound => loseSound;
        public AudioClip[] LevelBackgroundMusic => levelBackgroundMusic;
        public AudioClip MenuMusic => menuMusic;
    }
}

