using UnityEngine;
using UI;

namespace Game
{
    public class CrossHair : MonoBehaviour
    {
        [SerializeField] private Texture2D crossHairTexture;
        private bool _isUIActive;
        private void Start()
        {
            Cursor.visible = false;
            
        }

        private void OnEnable()
        {
            GameWin.UIEnableEvent += DisableCrossHair;
            GameOver.UIEnableEvent += DisableCrossHair;
            PauseMenu.UIEnableEvent += DisableCrossHair;
            GameWin.UIEnableEvent += DisableCrossHair;
            GameOver.UIDisableEvent += EnableCrossHair;
            PauseMenu.UIDisableEvent += EnableCrossHair;
        }

        private void OnDisable()
        {
            GameWin.UIEnableEvent -= DisableCrossHair;
            GameOver.UIEnableEvent -= DisableCrossHair;
            PauseMenu.UIEnableEvent -= DisableCrossHair;
            GameWin.UIEnableEvent -= DisableCrossHair;
            GameOver.UIDisableEvent -= EnableCrossHair;
            PauseMenu.UIDisableEvent -= EnableCrossHair;
        }

        private void OnGUI()
        {
            if (crossHairTexture != null && !_isUIActive)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                float crosshairSize = 32f; // Adjust size as needed
                GUI.DrawTexture(new Rect(mousePosition.x - crosshairSize / 2, mousePosition.y - crosshairSize / 2, crosshairSize, crosshairSize), crossHairTexture);
            }
        }

        private void DisableCrossHair()
        {
            Cursor.visible = true;
            _isUIActive = true;
        }

        private void EnableCrossHair()
        {
            Cursor.visible = false;
            _isUIActive = false;
        }
    }

}