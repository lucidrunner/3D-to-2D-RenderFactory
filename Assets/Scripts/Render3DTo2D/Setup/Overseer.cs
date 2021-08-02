using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using UnityEngine;

namespace Render3DTo2D.Setup
{
    /// <summary>
    /// Tag Class so we can find the Overseer object if ever needed
    /// </summary>
    public class Overseer : MonoBehaviour
    {
        [SerializeField, Tooltip(InspectorTooltips.PlaceModelsInScene)]
        private bool automaticallyPlaceModelOnSetup = true;

        
        [SerializeField, Tooltip(InspectorTooltips.SpaceBetweenModels)] private Vector2 spaceBetweenModels = new Vector2(5,5);
        [SerializeField, Tooltip(InspectorTooltips.ModelsPerRow)] private int modelsPerRow = 10;
        [SerializeField, Tooltip(InspectorTooltips.ModelDefaultOffset)] private int defaultRowOffset = 15;

        [SerializeField, Tooltip(InspectorTooltips.OptionalRenderer)]
        private Renderer optionalRenderer = null;

        public Renderer OptionalRenderer => optionalRenderer;
        
        public static Vector2? GetUniqueScenePosition()
        {
            var _sceneOverseer = FindObjectOfType<Overseer>();
            if(_sceneOverseer == null || !_sceneOverseer.automaticallyPlaceModelOnSetup)
                return null;
            var _sceneModels = FindObjectsOfType<ModelInfo>();

            (int row, int rowPosition) _place = (_sceneModels.Length / _sceneOverseer.modelsPerRow, _sceneModels.Length % _sceneOverseer.modelsPerRow);
            FLogger.LogMessage(_sceneOverseer, FLogger.Severity.Debug, $"Setup placed model at row {_place.row + 1}, position {_place.rowPosition + 1}.");
            return new Vector2(_sceneOverseer.defaultRowOffset + (_sceneOverseer.spaceBetweenModels.x * _place.row), _sceneOverseer.spaceBetweenModels.y * _place.rowPosition);
        }

        private void OnValidate()
        {
            if (modelsPerRow <= 0)
                modelsPerRow = 1;
        }

        #region Global

        public static Overseer Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<Overseer>();

                return instance;
            }
            
        }

        private static Overseer instance;

        #endregion
    }
}
