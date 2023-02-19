using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    public static class InspectorTooltips
    {
        #region Overseer

        public const string PlaceModelsInScene = "If set to true, hierarchy setup will place the model in a holding area with their row / position based on the current number of setup models in the scene." +
                                                 "\n\nThis has no effect on rendering but it's easier to visually setup camera sizes on rigs when they're not overlapping with other models.";
        public const string SpaceBetweenModels = "The row / position space, in units, between each model.";
        public const string ModelsPerRow = "The number of models per row.";
        public const string ModelDefaultOffset = "The default X-offset from the center of the holding area.";
        
        #endregion
        
        #region Model Renderer

        public const string SetupFactoryType = "The type of factory we're currently working with. The main animated factory allows for frame-by-frame exports of animated models. The static factory allows for snapshots of selected single frames from an animation.";
        #region Rig Setup

        public const string CameraRig = "The Type of Camera Rig to use.";
        public const string PlacementMode = "The placement mode for the rig cameras. Defaults to Generated, which provides options for automatic setup of a set number of cameras. In contrast Manual will only add a rig of a set type, cameras must then be added via the Camera Rig component on the rig itself.";
        public const string NumberOfCameras = "The number of cameras that will be added to the rig.";
        public const string InitialCameraOffset = "The y-rotation offset that the camera placement will start at. Defaults to -90, which in general provides a side view of the model.";
        public const string InvertCameraRotation = "If checked, cameras will be added by a negative angle compared to the camera before resulting in a ";
        public const string RotationMode = "The way the cameras will be generated around the model. In Auto Wrap the cameras will be evenly placed around the model, in Manual they will follow the set angle.";
        public const string HalfWrap = "If checked, the cameras will be evenly placed according to 180 / number of cameras instead of 360. This can be used to later mirror sprites rather than having unique sides. If checked the Initial Camera Offset should be configured so the cameras are placed front to back rather than starting from the side.";
        #endregion

        #region Options

        

        #endregion

        #region Run

        

        #endregion
        
        #endregion

        #region  Render Settings

        
        #region General

        public const string FPS = "Default: 16";
        public const string DataType = "The file type(s) that the meta data file for a render will be exported as.";
        
        #endregion

       
        
        #region Calculator

        public const string EnableBoundsCalculator = "Before rendering, a set of two calculations are performed to determine the camera viewport size for each frame with the results cached until any settings have been altered. " +
                                                     "The first of these are the bounds calculator, which is quick but can lead to both a too large or small crop." +
                                                     "The second is a very slow pixel perfect edge calculation, where the frame is re-rendered until it's a tight fit around the observed model." + "\n\n" + 
                                                     "If unchecked the factory will perform neither of these scale calculations before rendering, giving the same size and crop for each image." + "\n\n" + "Can only be disabled on individual models.";

        public const string CalculatorMaxSteps = "Maximum amount of steps allowed by the Camera Scale Calculators. " +
                                                   "If this is hit the padding might need to be increased by 1-2 pixels. Default: 10 000";


        #region Bounds

        public const string CalculatorStepPercentage =
            "The percent the camera size is reduced by during each step by the Camera Bounds Scale Calculator. Default: 4%";


        public const string CalculatorMaxPadding =
            "Largest amount of pixels allowed between the bounds & the edge of the camera screen for the Camera Bounds Scale Calculator to finish. Default: 5px";

        #endregion

        #region Edge

        public const string UseEdgeCalculatorTooltip =
            "Edge recalculation guarantees the created frame scale won't lead to any model clipping outside of the image frame during rendering. " +
            "This is achieved by performing a series of renders, stepping up in size until the top and edges of the image is the expected background colour. " + 
            "This operation is very slow though (minimum: #of cameras times slower / frame), so generating scales with it off can be preferable for test renders." +
            "\n\n" +
            "This can't be turned off globally and will be used unless overridden on a model.";

        public const string UseInverseEdgeCalculator = "If checked, performs a tight fit on all renders by also running the edge calculator, but in reverse, when the inital image isn't clipping. "
                                                         + "\n\n" +
                                                         "This is an exponentially slow operation and should be reserved for finals renders. The benefit is a smaller disk storage size as well as a tighter image hitbox, especially for model and animation combinations that produce an overly large bounding box.";

        public const string EdgeCalculatorStepSize =
            "The percent the camera is increased by during each step of the edge recalculation.";

        public const string BypassStandardCalculator = "If checked, doesn't perform the standard bounds calculation outside of the first frame for an animation. Only applied for the omnidirectional edge calculator." +
                                                         "\n\n" + "Greatly speeds up edge calculations on animations with only small movements and animations where the bounding box is much larger than the model.";

        public const string EdgeCalculatorBottomOffset = "The % of the sides (counting from the bottom of the frame) that aren't checked during edge calculations.";

        #endregion

        #endregion

        #region Rendering

        public const string BaselineTooltip =
            "The base orthographic scale we calculate our final texture sizes from. A Texture size of 512, " +
            "camera scale of 1, and base scale set to 0.5f will lead to generated images of 1024px. " +
            "The closer our camera scale or isometric base plate size is to the base scale, the closer the output will be to the set Texture Size.";

        public const string BaselinePopup =
            "Since models from different sources can have varying sizes, setting this to an approximation of what a standard sized model is in relation to " +
            "a larger or smaller model allows us to generate different outputs that are correct in their relative sizes without having to rescale the 3D-models themselves." +
            "\nTo get the relative size of a frame's texture, the % deviation from the standard is calculated via Frame scale / Base Scale. " +
            "This deviation is then applied to the texture size to get the correct output size." +
            "\n" + "\n" + "To easily find a good baseline, play the scene and set a camera orthographic size & y until the model size is the correct relative size compared to the camera viewport (which represents a standard size model / human.)";

        public const string RenderingFormat = "Default: ARGB32. Mismatches between this & the Render Texture Format will often lead to blank images. Beyond that, a lot of options aren't fit for the purpose of this tool so this should probably not be modified.";

        public const string BaseTextureSize = "The base width/height of our rendered textures.";

        public const string OverwriteExisting =
            "Whether or not we write a rendered image if we find one with the same name in the destination folder.";

        public const string RenderBackgroundColor =
            "The solid background color the cameras will be set to during rendering. Defaults to transparent.";

        public const string RenderingLayer =
            "The default user created layer the model will be set to during rendering. The cameras mask out all layers but this during startup.";

        public const string RenderTextureFormat = "Default: ARGB32";

        public const string RenderTextureDepth = "Default: 24";

        public const string RenderTextureReadWrite = "Default: sRGB";

        #endregion

        #region Naming

        public const string UseAnimationName =
            "If checked, adds the Animation Name in place of the index to the name on rendering.";

        public const string AddIdentifiers = "If checked, adds a c / a / f before each Camera / Animation / Frame Index to the name on rendering.";

        public const string IncludeStaticTag = "If checked, adds a _Static identifier to the name on non-animated rendering";


        public const string IncludeRigTag = "If checked, adds the tag of the rig to the file name on rendering.";

        #endregion


        #region Isometric

        public const string IsometricAngle =
            "The initial angle isometric cameras are set to.";

        public const string IsometricStepSize =
            "The y-value the camera will be raised / lowered with during each step when aligning the reference plate with the camera viewport bottom. Default: 0.0001";

        public const string IsometricMaxSteps = "The number of steps allowed during reference plate alignment. Default: 100 000.";


        public const string IsometricBaseline =
            "The initially set size of the model reference plate." +
            "See the Render Output Settings / Baseline Reference Scale popup for more info on baselines and texture size deviation.";


        public const string IsometricDefaultAngle =
            "The default starting angle newly added isometric cameras are set to. When chosing an isometric rig during setup the settings will default to this value.";

        public const string IsometricDefaultBaseline =
            "The default size of the model reference plate, optionally also the baseline from which we calculate the deviated size. When chosing an isometric rig during setup the settings will default to this value. \n \n" +
            "See the Baseline popup for more info on texture deviation.";


        public const string IsometricPreferBasePlate =
            "When calculating texture size, use the deviation between the isometric rig base plate and the Isometric Default Baseline instead of the deviation between the camera scale and the Baseline Scale. " +
            "Note that if this is checked then y-angle shifting between a sidways rig and this will require rescaling of the sprites on the opposite end.  \n \n" +
            "See the Baseline popup for more info on texture deviation.";
        
        #endregion

        #region Animation Settings

        public const string ClampedDetailedMessage =
                    "Clamping can be set for animations where the last recorded frame won't occur (approximately) at the last possible animation time. " +
                    "\n\nFor personally created animations the animation end should optimally be set to match one of the 1 / Animation FPS rendering steps." +
                    " For animations on which this doesn't occur different modes can be selected to line up the animation properly, giving a rendered frame on 1F normalized time." +
                    "\n\nThis can be useful both to align looping animations, as well as providing a correct resolution to a one-shot animation. " +
                    "However, this will impact the animation speed slightly depending on the clamping method used.";

        public const string ClampedPercentage =
            "The maximum time compared to the default step size (1 / FPS) we allow between " +
            "the second to last and last frame when we add an extra frame at the end of the animation. " +
            "If the old last frame is within the default cutoff InsertRemoveLastFrame will be checked by default for an animation when setting up Advanced Animation Settings." +
            "\n\nAdding an extra frame disturbs the overall animation less but can produce some obvious speedup or slowdown at the end of the animation. "+
            "Generally cutting off the last frame is recommended if they're within the 25-30% range of each other, and leaving it in is recommended around the 70-80% range. Otherwise smoothing is recommended.";

        public const string ClampedInsertRemoveLast =  "Adding an extra frame disturbs the overall animation less but can produce some obvious speedup or slowdown at the end of the animation. " +
                                                         "If checked, this removes the old last frame when inserting an extra frame at the end of the animation."+
                                                         "\n\n" + "Generally, checking this is recommended if the new and old last frames are at max 30% of a normal step of each other. Inversely, leaving the frame in is recommended around the 70%+ step time." + 
                                                         "\n\n For values inbetween smoothing is recommended to avoid a noticeable difference on the last step.";

        public const string ClampedStretchFrames =
            "The frame index that we start recording with a smoothed time (Remaining animation time at smooth start / # of smoothed frames) between frames rather than the " +
            "default recording step time (1 / FPS). \n \nSmoothing doesn't add a frame, but instead slightly increases the step time between the existing frames to" +
            "remove the chance of the jarring last frame step that can be seen during Insert-clamping.";

        public const string ClampingTolerance = "The number of decimals we round to on the gap between the calculated time of the last frame and the animation end time when " +
                                                  "deciding if we should offer clamping or not. Generally rounding errors start to appear around 7+ decimals.";

        public const string IgnoreFrameForLooping = "If ticked, will not render the last frame for a looping animation. Useful as looping animations tend to have an identical start / end frame." +
                                                      "\n\nNote that when exporting root motion the new last frame -> first frame delta movement still needs to be queried on the non-existant last frame index (aka the frame count) " +
                                                      "since the 0 frame data is the set starting values rather than a delta.";

        public const string LoopedAnimation = "Adds a flag to the AnimationData that the animation is meant to loop, generally meaning that the start and end frame of the animation is identical."
                                                + "\n\nBy default set to the imported animation clip's IsLooping-flag.";

        #endregion

        #region Root Motion

        public const string RootMotionRecordingsPerFrame =
            "How many times are Root Motion recorded for each rendered frame. Achieved by splitting each animation step in a series of sub-steps and only rendering on each main step.";

        public const string ExportTransform = "Records & exports the delta change for the toggled axes.";

        public const string FollowTransform = "Applies the delta change to the rig, causing the cameras to mimic the models movement & rotation on all toggled axes.";


        public const string PreferEulerAngles = "Is checked, exports Root Motion Rotations as their Euler Vector3-representation rather than a quaternion.";

        public const string RootMotionTolerance = "The number of decimals we allow when exporting the root motion to XML. Vector floating point errors start to crop up around 7+ decimals.";

        public const string DefaultFollow = "If enabled, applies any root motion on the selected axes to the non-isometric rigs causing them to follow the model during recording & rendering.";

        public const string ForceMovementClamp = "If checked, forcibly removes any applied Root Movement to the position from the model during animation if no Root Motion Settings are present or both Enable Export / Default follow are turned off."
                                                   + "\n\n" + "Needed when the rig & meshes are still moved despite Use Root Motion being left unchecked, which is primarily a common mixamo issue. This should have the same effect as downloading an animation with the In-Place modifier ticked.";

        public const string ForceRotationClamp = "Movement clamp, but for rotation.";

        public const string ApplyBaselineDeviation = "Apply the deviation between the globally set baseline to the root position delta - eg for a model whose personal baseline is half of the set global each recorded frame will be doubled to compensate for the difference in model size.";

        #endregion

        #endregion

        public const string ValidateCurrentRigs = "";
        public const string NearestMultiple = "Sets the output size to the nearest multiple of 4, allowing easier use of compression formats.";
        public const string SetToLayer = "If set to true, will set the model to the target rendering layer whenever any factory starts rendering, allowing it to be present as a background object." +
                                         "\nThis can be a bit finicky, as well as possibly breaking the edge calculator, so setting the scale calculators to off and using a static camera size for the run model is recommended.";

        public const string AutomaticDepthCalculation = "If enables, provides a quick and dirty clipping plane calculation for non-isometric cameras that should always result in the model being fully captured.";
        public const string BoundsOverdraw = "The factor the calculated bounding box will be multiplied by.";
        public const string RenderingIgnoreList = "Each added transform in this list won't be included when rendering the model.";
        public const string BoundsIgnoreList = "Each added transform in this list won't be included when calculating the bounds of the model";
        public const string IncludeRenderIgnore = "If true, the ignore list will also include the meshes ignored under the Render Settings.";
        public const string OptionalRenderer = "If set, the latest rendered image will be set as the main texture on the target Renderer. This should preferably be a plane or quad.";
        public const string MoveModelOnStartup = "Moves the model to 0,0 on render startup.";
        public const string FocusModelOnStartup = "Centers the scene camera on the model when starting a new render run.";
        public const string FollowModelOnRender = "Centers the camera on the model on each render step.";
        
    }
}