using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scene
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "Game/Scene Data")]
    public class SceneData : ScriptableObject
    {
        public AssetReference mainMenuScene;
        public AssetReference playerScene;
        public AssetReference gameUIScene;
        public List<AssetReference> levels; // Stores all level scenes
    }
}
