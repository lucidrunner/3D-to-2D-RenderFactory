using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RootChecker;
using UnityEngine;
using UnityEngine.UI;

namespace Render3DTo2D.Model_Settings
{
    

    public class RenderingSettings : MonoBehaviour
    {
        #region Admin

        public enum ShowMode
        {
            Standard, Advanced
        }


        #endregion
        
        #region Settings
        
        //Memory note: c# not a fan of editor only variables being present in the file and accessed through unity's property reflection system so we have to do a couple of suppression pragma
        #pragma warning disable 0414
        [SerializeField, UsedImplicitly] private ShowMode settingsMode = ShowMode.Standard;
        #pragma warning restore 0414

        #region General
        
        public int AnimationFPS => animationFramesPerSecond;
        [SerializeField] protected int animationFramesPerSecond = 16;
        
        
        
        
        #endregion

        #region Calculator

        public bool AutomaticDepthCalculation => automaticDepthCalculation;
        [SerializeField] protected bool automaticDepthCalculation = true;

        #region Bounds

        public bool EnableBoundsCalculator => enableBoundsCalculator;
        [SerializeField] protected bool enableBoundsCalculator = true;

        public float BoundsCalculatorSizeModifier => boundsCalculatorSizeModifier;
        [SerializeField] protected float boundsCalculatorSizeModifier  = 1f;

        public float ScaleCalculatorStepPercentage => scaleCalculatorStepPercentage / 100;
        [SerializeField] protected float scaleCalculatorStepPercentage = 4;

        public float ScaleCalculatorMaxPadding => scaleCalculatorMaxPadding;
        [SerializeField] protected float scaleCalculatorMaxPadding = 5f;

        public int ScaleCalculatorMaxSteps => scaleCalculatorMaxSteps;
        [SerializeField] protected int scaleCalculatorMaxSteps = 10000;
        
        [SerializeField] protected bool includeRenderIgnore = true;
        
        public List<Transform> CalculatorIgnoreList
        {
            get
            {
                if (scaleCalculatorIgnoreMeshes == null)
                    scaleCalculatorIgnoreMeshes = new List<Transform>();
                var _toReturn = new List<Transform>(scaleCalculatorIgnoreMeshes);
                if(includeRenderIgnore)
                    _toReturn.AddRange(RenderIgnoreList);
                return _toReturn;
            }
        }

        [SerializeField] protected List<Transform> scaleCalculatorIgnoreMeshes = new List<Transform>();

        #endregion
        #region Edge
        
        public bool UseEdgeCalculator => useEdgeCalculator;
        [SerializeField] private bool useEdgeCalculator = true;

        public bool UseInverseEdgeCalculator => useEdgeCalculator && omnidirectionalEdgeCalculator;
        [SerializeField] private bool omnidirectionalEdgeCalculator = true;

        public bool BypassBoundsCalculator => bypassStandardCalculator && UseInverseEdgeCalculator;
        [SerializeField] private bool bypassStandardCalculator = true;

        public float EdgeStepSize => edgeCalculatorStepSize / 100f;
        [SerializeField] protected float edgeCalculatorStepSize = 2f;

        public double BottomOffset => bottomOffset / 100d;
        [SerializeField] protected float bottomOffset = 3f;

        #endregion
        #endregion
        
        #region Rendering
        
        public Color RenderBackgroundColor => renderBackgroundColor;
        [SerializeField] private Color renderBackgroundColor = Color.clear;

        public int RenderingLayer => LayerMask.NameToLayer(GetLayers[renderingLayer]);
        public string[] RenderingLayerOptions => GetLayers.ToArray();
        [SerializeField] private int renderingLayer;

        public bool SetToLayer => setToLayerWhenNotRendering;
        [SerializeField] private bool setToLayerWhenNotRendering = false;

        public int BaseTextureSize => baseTextureSize;
        [SerializeField] private int baseTextureSize = 256;

        public bool SetToNearestMultiple => setSizeToNearestMultiple;
        [SerializeField] private bool setSizeToNearestMultiple = true;

        public bool OverwriteExistingFrames => overwriteExistingFrames;
        [SerializeField] private bool overwriteExistingFrames = true;

        public float BaselineScale => baselineScale;
        [SerializeField] private float baselineScale = 1f;
        
        public TextureFormat RenderingFormat => renderingFormat;
        [SerializeField] protected TextureFormat renderingFormat = TextureFormat.ARGB32;

        public RenderTextureFormat PresetRenderTextureFormat => renderTextureFormat;
        [SerializeField] protected RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

        public int PresetRenderTextureDepth => renderTextureDepthOptions[renderTextureDepth];
        public string[] RenderTextureDepthOptions => renderTextureDepthOptions.Select(aI => aI.ToString()).ToArray();
        [UsedImplicitly] private int[] renderTextureDepthOptions = {0, 16, 24};
        [SerializeField] protected int renderTextureDepth = 2;

        public RenderTextureReadWrite PresetRenderTextureReadWrite => renderTextureReadWrite;
        [SerializeField] protected RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.sRGB;
        
        public List<Transform> RenderIgnoreList
        {
            get
            {
                if (renderIgnoreList == null)
                    renderIgnoreList = new List<Transform>();
                return renderIgnoreList;
            }
        }

        [SerializeField] protected List<Transform> renderIgnoreList = new List<Transform>();
        
        
        
        #region Naming

        public string ExampleOutput
        {
            get
            {
                StringBuilder _stringBuilder = new StringBuilder();
                foreach (string _element in renderNameFormat)
                {
                    //Slightly ugly label text matching but w/e
                    switch (_element)
                    {
                        case "Model Name":
                            _stringBuilder.Append("BuddyModels_");
                            break;
                        case "Rig Tag":
                            _stringBuilder.Append("ISO4_");
                            break;
                        case "Camera Index":
                            _stringBuilder.Append((includeFormatIdentifier ? "c" : "") + "0_");
                            break;
                        case "Animation Index":
                            _stringBuilder.Append(UseAnimationName ? "Walk_" : (includeFormatIdentifier ? "a" : "") + "3_");
                            break;
                        case "Frame Index":
                            _stringBuilder.Append((includeFormatIdentifier ? "f" : "") + "31_");
                            break;
                        case "Static":
                            _stringBuilder.Append("Static_");
                            break;
                    }
                }

                //Remove the last _
                if(_stringBuilder.Length > 0)
                    _stringBuilder.Remove(_stringBuilder.Length - 1, 1);
                else
                {
                    //Otherwise, If we've done any changes to this class there's a chance we've cleared the format list, requiring a reset
                    ResetFormat();
                }
                return _stringBuilder.ToString();
            }
        }

        public List<char> RenderNameFormat => renderNameFormat.Select(aElement => aElement[0]).ToList();

        public bool UseAnimationName => useAnimationName;
        [SerializeField] protected bool useAnimationName = false;

        [SerializeField] protected bool includeRigTag = true;
        [SerializeField] protected bool includeStaticTag = false;

        public bool IncludeFormatIdentifier => includeFormatIdentifier;
        [SerializeField] protected bool includeFormatIdentifier = false;

        [SerializeField]
        private List<string> renderNameFormat = new List<string>
        {
            "Model Name", "Rig Tag", "Camera Index", "Animation Index", "Frame Index"
        };

        public void ResetFormat()
        {
            renderNameFormat = new List<string>
            {
                "Model Name", "Camera Index", "Animation Index", "Frame Index"
            };
            if (includeRigTag)
            {
                renderNameFormat.Insert(1, "Rig Tag");
            }
            if(includeStaticTag)
            {
                renderNameFormat.Add("Static");
            }
        }

        private void ApplyRigTagChange()
        {
            if(includeRigTag == (renderNameFormat.FirstOrDefault(aElement => aElement.Equals("Rig Tag")) == null))
                ApplyTagChange(includeRigTag, "Rig Tag", renderNameFormat.IndexOf("Model Name") + 1); //We add the rig tag after the model name by default
        }


        private void ApplyStaticTagChange()
        {
            if(includeStaticTag == (renderNameFormat.FirstOrDefault(aElement => aElement.Equals("Static")) == null))
                ApplyTagChange(includeStaticTag, "Static", renderNameFormat.Count); //We add the static tag at the end of the name
        }

        private void ApplyTagChange(bool aToggleState, string aLabelText, int aInsertIndex)
        {
            if (aToggleState == false)
            {
                var _element = renderNameFormat.FirstOrDefault(aElement => aElement.Equals(aLabelText));
                if (_element != null)
                    renderNameFormat.Remove(_element);
            }

            else
            {
                renderNameFormat.Insert(aInsertIndex, aLabelText);
            }
        }

        private void Reset()
        {
            renderingLayer = GlobalRenderingSettings.Instance.renderingLayer;
        }

        #endregion
        #endregion
        
        #region Isometric

        
        public float IsometricDefaultAngle => isometricDefaultAngle;
        [SerializeField] private float isometricDefaultAngle = 32f;

        public float IsometricBaseline => isometricBaseline; 
        [SerializeField] private float isometricBaseline = 1f;

        public bool PreferBasePlateDeviation => preferBasePlateDeviation;
        [SerializeField] private bool preferBasePlateDeviation = false;

        public float IsometricStepSize => isometricStepSize;
        [SerializeField] private float isometricStepSize = 0.0001f;

        public int IsometricMaxSteps => isometricMaxSteps;
        [SerializeField] private int isometricMaxSteps = 100000;

        #endregion

        #region RootMotion

        public int RecordingsPerFrame => recordingsPerFrame;
        [SerializeField] private int recordingsPerFrame = 1;

        public bool ApplyBaselineDeviation => applyBaselineDeviation;
        [SerializeField] private bool applyBaselineDeviation = true;

        public int RootMotionTolerance => rootMotionTolerance;
        [SerializeField] private int rootMotionTolerance = 6;

        #endregion

        #endregion

        #region Methods

        public static RenderingSettings GetFor(Transform aTransform)
        {
            RenderingSettings _localSettings = RootFinder.FindHighestRoot(aTransform).GetComponent<RenderingSettings>();
            return _localSettings == null ? GlobalRenderingSettings.Instance : _localSettings;
        }
        
        private List<string> GetLayers
        {
            get
            {

                //Get all user created layers
                List<string> _layers = new List<string>();
                for (int _i = 8; _i < 31; _i++)
                {
                    string _layer = LayerMask.LayerToName(_i);
                    if(_layer.Length > 0)
                        _layers.Add(_layer);
                }
                return _layers;
            }
        }

        //Used for comparisons to the saved settings in the file
        //Bastardised generated GetHashCode()
        internal int GetRenderingValueHash()
        {
            unchecked
            {
                int _hashCode = 100;
                _hashCode = (_hashCode * 397) ^ animationFramesPerSecond;
                _hashCode = (_hashCode * 397) ^ scaleCalculatorStepPercentage.GetHashCode();
                _hashCode = (_hashCode * 397) ^ scaleCalculatorMaxPadding.GetHashCode();
                _hashCode = (_hashCode * 397) ^ scaleCalculatorMaxSteps;
                _hashCode = (_hashCode * 397) ^ enableBoundsCalculator.GetHashCode();
                _hashCode = (_hashCode * 397) ^ useEdgeCalculator.GetHashCode();
                _hashCode = (_hashCode * 397) ^ omnidirectionalEdgeCalculator.GetHashCode();
                _hashCode = (_hashCode * 397) ^ bypassStandardCalculator.GetHashCode();
                _hashCode = (_hashCode * 397) ^ edgeCalculatorStepSize.GetHashCode();
                _hashCode = (_hashCode * 397) ^ baseTextureSize;
                _hashCode = (_hashCode * 397) ^ baselineScale.GetHashCode();
                return _hashCode;
            }
        }
        #endregion


        public void MoveFormatLeft(int aIndex)
        {
            if (aIndex > 0)
            {
                var _element = renderNameFormat[aIndex];
                renderNameFormat.Insert(aIndex - 1, _element);
                renderNameFormat.RemoveAt(aIndex + 1);
            }
        }

        public void MoveFormatRight(int aIndex)
        {
            if (aIndex < renderNameFormat.Count - 1)
            {
                var _element = renderNameFormat[aIndex];
                renderNameFormat.Remove(_element);
                renderNameFormat.Insert(aIndex + 1, _element);
            }
        }

        private void OnValidate()
        {
            ApplyRigTagChange();
            ApplyStaticTagChange();
            if (baselineScale < 0.0001f)
                baselineScale = 0.0001f;
        }

        public void ResetToGlobal()
        {
            var _global = GlobalRenderingSettings.Instance;
            if (_global == null)
                return;

            CopySettings(_global);
        }

        private void CopySettings(RenderingSettings aRenderingSettings)
        {
            baselineScale = aRenderingSettings.baselineScale;
            bottomOffset = aRenderingSettings.bottomOffset;
            isometricBaseline = aRenderingSettings.isometricBaseline;
            renderingFormat = aRenderingSettings.renderingFormat;
            renderingLayer = aRenderingSettings.renderingLayer;
            applyBaselineDeviation = aRenderingSettings.applyBaselineDeviation;
            automaticDepthCalculation = aRenderingSettings.automaticDepthCalculation;
            baseTextureSize = aRenderingSettings.baseTextureSize;
            bypassStandardCalculator = aRenderingSettings.bypassStandardCalculator;
            enableBoundsCalculator = aRenderingSettings.enableBoundsCalculator;
            includeFormatIdentifier = aRenderingSettings.includeFormatIdentifier;
            includeRenderIgnore = aRenderingSettings.includeRenderIgnore;
            includeRigTag = aRenderingSettings.includeRigTag;
            includeStaticTag = aRenderingSettings.includeStaticTag;
            isometricDefaultAngle = aRenderingSettings.isometricDefaultAngle;
            isometricMaxSteps = aRenderingSettings.isometricMaxSteps;
            isometricStepSize = aRenderingSettings.isometricStepSize;
            omnidirectionalEdgeCalculator = aRenderingSettings.omnidirectionalEdgeCalculator;
            overwriteExistingFrames = aRenderingSettings.overwriteExistingFrames;
            recordingsPerFrame = aRenderingSettings.recordingsPerFrame;
            renderBackgroundColor = aRenderingSettings.renderBackgroundColor;
            renderNameFormat = new List<string>(aRenderingSettings.renderNameFormat);
            renderTextureDepth = aRenderingSettings.renderTextureDepth;
            renderTextureFormat = aRenderingSettings.renderTextureFormat;
            rootMotionTolerance = aRenderingSettings.rootMotionTolerance;
            useAnimationName = aRenderingSettings.useAnimationName;
            useEdgeCalculator = aRenderingSettings.useEdgeCalculator;
            animationFramesPerSecond = aRenderingSettings.animationFramesPerSecond;
            boundsCalculatorSizeModifier = aRenderingSettings.boundsCalculatorSizeModifier;
            edgeCalculatorStepSize = aRenderingSettings.edgeCalculatorStepSize;
            preferBasePlateDeviation = aRenderingSettings.preferBasePlateDeviation;
            renderTextureReadWrite = aRenderingSettings.renderTextureReadWrite;
            scaleCalculatorMaxPadding = aRenderingSettings.scaleCalculatorMaxPadding;
            scaleCalculatorMaxSteps = aRenderingSettings.scaleCalculatorMaxSteps;
            scaleCalculatorStepPercentage = aRenderingSettings.scaleCalculatorStepPercentage;
            setSizeToNearestMultiple = aRenderingSettings.setSizeToNearestMultiple;
            setToLayerWhenNotRendering = aRenderingSettings.setToLayerWhenNotRendering;
        }
    }
}