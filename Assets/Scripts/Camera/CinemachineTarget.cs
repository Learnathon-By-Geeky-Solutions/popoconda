using Cutscene;
using UnityEngine;
using Unity.Cinemachine;

namespace Camera
{
    public class CinemachineTarget : MonoBehaviour
    {
        private CinemachineTargetGroup _targetGroup;
        private bool _isHeroAdded;
        
        private void Start()
        {
            _targetGroup = GetComponent<CinemachineTargetGroup>();
            CutsceneManager.OnCutsceneEnd += AddHeroToTargetGroup;
        }
        
        private void OnDisable()
        {
            CutsceneManager.OnCutsceneEnd -= AddHeroToTargetGroup;
        }

        private void AddHeroToTargetGroup()
        {
            if (_isHeroAdded) return;
            GameObject hero = GameObject.FindGameObjectWithTag("Enemy");
            if (hero != null)
            {
                _targetGroup.AddMember(hero.transform, 1, 0);
                _isHeroAdded = true;
                Debug.Log("Hero added to target group");
            }
            else
            {
                Debug.LogWarning("Hero not found in the scene");
            }
        }
    }
}
