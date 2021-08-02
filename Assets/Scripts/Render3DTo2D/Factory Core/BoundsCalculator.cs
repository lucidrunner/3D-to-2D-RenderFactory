using System.Collections;
using System.Collections.Generic;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Factory_Core
{
    public class BoundsCalculator: MonoBehaviour
    {
        
        #region Class Fields

        private Vector3 boundsCenter = Vector3.zero;
        private Bounds calculatedBounds;
        private bool isCached = false;

        #endregion Class Fields

        #region Inspector
        
        [SerializeField]
        private bool drawCalculatedBounds = false;

        [SerializeField]
        private Color boundsColor = Color.cyan;

        [SerializeField]
        private List<Bounds> meshBounds = new List<Bounds>();

        [SerializeField]
        private List<Renderer> meshesInHierarchy = new List<Renderer>();

        #region Toggle Buttons

        #endregion Toggle Buttons
        #endregion Inspector

        #region Public Methods

        private void Reset()
        {
            RecalculateBounds();
        }

        /// <summary>
        /// Calculates the bounds of the model, unless it's already been cached this frame, and returns the updated bounds.
        /// </summary>
        public Bounds CalculateAndReturn()
        {
            if(isCached == false)
            {
                RecalculateBounds();
            }

            return calculatedBounds;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerator ClearBoundsCaching()
        {
            yield return new WaitForEndOfFrame();
            isCached = false;
        }

        private void ClearBounds()
        {
            calculatedBounds = new Bounds();
            boundsCenter = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if(drawCalculatedBounds)
            {
                Gizmos.color = boundsColor;
                Gizmos.DrawCube(new Vector3(RootFinder.FindHighestRoot(transform).position.x, RootFinder.FindHighestRoot(transform).position.y + (calculatedBounds.size.y) / 2, RootFinder.FindHighestRoot(transform).position.z), calculatedBounds.size);
            }
        }

        private void RecalculateBounds()
        {
            //Taken from https://forum.unity.com/threads/find-bounds-of-a-prefab-of-all-the-meshes.443083/
            
            //Clear our current setup and get the new mesh lists
            if(meshesInHierarchy.Count > 0 || meshBounds.Count > 0)
            {
                ClearBounds();
            }
            RefreshMeshLists();

            //Add the bounds & centers
            for(int index = 0; index < meshesInHierarchy.Count; index++)
            {
                meshBounds.Add(meshesInHierarchy[index].bounds);
                boundsCenter += meshesInHierarchy[index].bounds.center;
            }

            //Calculate the mean center
            boundsCenter /= meshesInHierarchy.Count;
            
            //Setup our new calculated bounds
            calculatedBounds = new Bounds(boundsCenter, Vector3.zero);

            //Go through each bound and encapsulate it via our mean bounds
            for(int index = 0; index < meshBounds.Count; index++)
            {
                calculatedBounds.Encapsulate(meshBounds[index]);
            }
            
            //Expand if needed via the overdraw
            var _settings = RenderingSettings.GetFor(transform);
            if (_settings.BoundsCalculatorSizeModifier > 0)
            {
                Vector3 _expandSize = (calculatedBounds.size * _settings.BoundsCalculatorSizeModifier) - calculatedBounds.size;
                calculatedBounds.Expand(_expandSize);
            }

            isCached = true;
            //Start our end of frame coroutine to clear the caching
            StartCoroutine(ClearBoundsCaching());
        }

        private void RefreshMeshLists()
        {
            //Get our render settings
            RenderingSettings _settings = RenderingSettings.GetFor(transform);
            //Clear out lists
            meshesInHierarchy = new List<Renderer>();
            meshBounds = new List<Bounds>();
            //Get all the children of the model (via the Model info holder)
            Transform[] _allChildren = ModelInfo.GetFor(transform).GetComponentsInChildren<Transform>();
            foreach (Transform _child in _allChildren)
            {
                //If it's in the ignore list, bypass it for calculations
                if (_settings.CalculatorIgnoreList.Contains(_child ))
                {
                    continue;
                }

                //If it has a renderer, add it to the list
                if (_child.GetComponent<Renderer>() != null)
                {
                    meshesInHierarchy.Add(_child.GetComponent<Renderer>());
                }
            }
        }

        #endregion Private Methods
    }
}
