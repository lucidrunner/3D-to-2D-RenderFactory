using UnityEditor;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    public class EditorPropertyInfoWindow : PopupWindowContent
    {
        private string infoText = "";
        private Vector2 windowSize;

        public void Init(Vector2 aWindowSize, string aInfoText)
        {
            infoText = aInfoText;
            windowSize = aWindowSize;
        }
        
        public override Vector2 GetWindowSize()
        {
            return windowSize;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label(infoText, EditorStyles.textArea);
        }
    }
}