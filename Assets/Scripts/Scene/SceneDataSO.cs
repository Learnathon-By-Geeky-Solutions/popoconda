using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scene
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Game/Scene Data")]
    public class SceneData : ScriptableObject
    {
        [SerializeField] private AssetReference mainMenuScene;
        [SerializeField] private AssetReference levelSelectScene;
        [SerializeField] private AssetReference playerScene;
        [SerializeField] private AssetReference gameUIScene;
        [SerializeField] private List<AssetReference> levels; // Stores all level scenes
        
        public AssetReference MainMenuScene => mainMenuScene;
        public AssetReference LevelSelectScene => levelSelectScene;
        public AssetReference PlayerScene => playerScene;
        public AssetReference GameUIScene => gameUIScene;
        public List<AssetReference> Levels => levels;
        
    }
}
