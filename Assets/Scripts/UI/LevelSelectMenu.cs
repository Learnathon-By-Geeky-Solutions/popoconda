using UnityEngine;
using UnityEngine.UIElements;
using Game;

namespace UI
{
   public class LevelSelectMenu: MonoBehaviour
   {
      [SerializeField] private UIDocument LevelSelectDocument;
      private Button _level1Button;
      private Button _level2Button;
      private Button _level3Button;
      private Button _backButton;
      
      public delegate void StatEventWithInt(int level);
      public delegate void StatEvent();
      public static event StatEventWithInt levelEvent;
      public static event StatEvent backEvent;

      private void Start()
      {
         VisualElement root = LevelSelectDocument.rootVisualElement;
         
         _level1Button = root.Q<Button>("level-1");
         _level2Button = root.Q<Button>("level-2");
         _level3Button = root.Q<Button>("level-3");
         _backButton = root.Q<Button>("back");
         
         
         _level1Button.clicked += HandleLevel1ButtonClicked;
         _level2Button.clicked += HandleLevel2ButtonClicked;
         _level3Button.clicked += HandleLevel3ButtonClicked;
         _backButton.clicked += HandleBackButtonClicked;
         
         // Update button by LevelManager.UnlockedLevel
         UpdateLevel();
      }
      
      private void OnDestroy()
      {
         _level1Button.clicked -= HandleLevel1ButtonClicked;
         _level2Button.clicked -= HandleLevel2ButtonClicked;
         _level3Button.clicked -= HandleLevel3ButtonClicked;
         _backButton.clicked -= HandleBackButtonClicked;
      }

      private void UpdateLevel()
      {
         if (LevelManager.UnlockedLevels < 1)
         {
            _level2Button.SetEnabled(false);
         }
         if (LevelManager.UnlockedLevels < 2)
         {
            _level3Button.SetEnabled(false);
         }
      }
      
      private static void HandleLevel1ButtonClicked()
      {
         levelEvent?.Invoke(0);
         Debug.Log("Level 1 button clicked");
      }
      
      private static void HandleLevel2ButtonClicked()
      {
         levelEvent?.Invoke(1);
         Debug.Log("Level 2 button clicked");
      }
      
      private static void HandleLevel3ButtonClicked()
      {
         levelEvent?.Invoke(2);
         Debug.Log("Level 3 button clicked");
      }
      
      private static void HandleBackButtonClicked()
      {
         backEvent?.Invoke();
         Debug.Log("Back button clicked");
      }
   } 
}

    
    


