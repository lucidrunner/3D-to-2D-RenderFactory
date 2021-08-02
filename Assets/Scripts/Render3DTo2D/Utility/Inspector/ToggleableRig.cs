using System;
using JetBrains.Annotations;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Rigging;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    [Serializable]
    public class ToggleableRig
    {
        #region Fields

        [SerializeField, HideInInspector]
        private RigManager rigManager;

        [SerializeField, HideInInspector]
        private CameraRig rig;

        [SerializeField, UsedImplicitly, HideInInspector]
        private string rigName;

        [SerializeField, HideInInspector]
        private bool toggled;

        #endregion

        #region Properties

        public CameraRig Rig => rig;


        public bool Toggled => toggled;

        #endregion

        #region Constructor

        public ToggleableRig(RigManager aRigManager ,CameraRig aRig, bool aToggled)
        {
            rigManager = aRigManager;
            rig = aRig;
            toggled = aToggled;
            rigName = aRig.gameObject.name;
        }

        #endregion

        #region Inspector

        public void Toggle()
        {
            toggled = !toggled;
        }

        //UsedImplicitly only solves the compiler warning when a value is implicitly assigned, but not when it's implicitly accessed and reused (which is something we need to do when it comes to unity inspector UI code)
        #pragma warning disable 0414
        [SerializeField, UsedImplicitly]
        private bool deletionSafetyToggle = false;
        #pragma warning restore 0414

        private void DeleteRig()
        {
            rigManager.RemoveRig(this);
        }

        #endregion
    }
}