using UnityEngine;

namespace Game
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject _playerInstance;

        private void Awake()
        {
            if (_playerInstance == null)
            {
                SpawnPlayer();
            }
        }

        private void SpawnPlayer()
        {
            _playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }
    }
} 