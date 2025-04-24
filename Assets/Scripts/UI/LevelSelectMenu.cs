using UnityEngine;
using UnityEngine.UIElements;
using Game;

namespace UI
{
   public class LevelSelectMenu: MonoBehaviour
   {
      [SerializeField] private UIDocument LevelSelectDocument;
      private Button _level1Button;
      private Button _backButton;
      
      public delegate void StatEventWithInt(int level);
      public delegate void StatEvent();
      public static event StatEventWithInt levelEvent;
      public static event StatEvent backEvent;

      private void Start()
      {
         VisualElement root = LevelSelectDocument.rootVisualElement;
         
         _level1Button = root.Q<Button>("level-1");
         _backButton = root.Q<Button>("back");
         
         
         _level1Button.clicked += HandleLevel1ButtonClicked;
         _backButton.clicked += HandleBackButtonClicked;
         
       
      }
      
      private void OnDestroy()
      {
         _level1Button.clicked -= HandleLevel1ButtonClicked;
         _backButton.clicked -= HandleBackButtonClicked;
      }
      
      
      private static void HandleLevel1ButtonClicked()
      {
         levelEvent?.Invoke(0);
         Debug.Log("Level 1 button clicked");
      }
      
      private static void HandleBackButtonClicked()
      {
         backEvent?.Invoke();
         Debug.Log("Back button clicked");
      }
   } 
}

    
    


