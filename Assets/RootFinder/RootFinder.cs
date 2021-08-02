
using System;
using UnityEngine;

namespace RootChecker
{
    public class RootFinder
    {
        public static Transform FindFirstRoot(Transform aBaseTransform)
        {
            if(aBaseTransform == aBaseTransform.root)
                return aBaseTransform;

            Transform _checkedRoot = aBaseTransform;

            while(_checkedRoot != aBaseTransform.root)
            {
                if(_checkedRoot.parent == null) //Make sure that we can't somehow fall out of the hierarchy and get null errors
                {
                    break;
                }
                _checkedRoot = _checkedRoot.parent;
                if(_checkedRoot.GetComponent<RootAnchor>() != null)
                {
                    return _checkedRoot;
                }

            }
            if(aBaseTransform.parent != aBaseTransform.root)
                Debug.LogWarning($"{aBaseTransform}: Attempted to find first root anchor with no results. This will most likely result in reference errors. Have you added a root anchor to the hierarchy?");
            return aBaseTransform.root;
        }

        public static Transform FindHighestRoot(Transform aBaseTransform)
        {
            if(aBaseTransform == aBaseTransform.root)
                return aBaseTransform;

            Transform _checkedRoot = aBaseTransform;
            Transform _foundRoot = null;
            Transform _returnRoot = null;

            while(_returnRoot == null)
            {
                if(_checkedRoot.parent == null) //Make sure that we can't somehow fall out of the hierarchy and get null errors
                {
                    break;
                }
                _checkedRoot = _checkedRoot.parent;

                if(_checkedRoot.GetComponent<RootAnchor>() != null)
                {
                    _foundRoot = _checkedRoot;
                }

                if(_checkedRoot == aBaseTransform.root)
                {
                    if(_foundRoot == null)
                    {
                        if(aBaseTransform.parent != aBaseTransform.root)
                            Debug.LogWarning($"{aBaseTransform}: Attempted to find root anchor with no results. This will most likely result in reference errors. Have you added a root anchor to the hierarchy ");
                        return _checkedRoot;

                    }
                    else
                    {
                        _returnRoot = _foundRoot;
                    }
                }

            }

            if(_returnRoot != null)
            {
                return _returnRoot;
            }
            else
            {
                return aBaseTransform.root;
            }
        }

        public static Transform LevelRootSearch(Transform aBaseTransform, bool aFindHighestSublevel)
        {
            if(aBaseTransform == aBaseTransform.root)
                return aBaseTransform;

            Transform _checkedRoot = aBaseTransform;
            Transform _foundRoot = null;
            Transform _returnRoot = null;

            while(_returnRoot == null)
            {
                //Maybe null check here
                _checkedRoot = _checkedRoot.parent;

                if(_checkedRoot.parent != null)
                {
                    //Go through all siblings of the _checkedRoot in order and set _checkedRoot to the first one with an anchor
                    int _nrOfSiblings = _checkedRoot.parent.childCount;
                    for(int childIndex = 0; childIndex < _nrOfSiblings; childIndex++)
                    {
                        if(_checkedRoot.parent.GetChild(childIndex) != null)
                        {
                            if(_checkedRoot.parent.GetChild(childIndex).GetComponent<RootAnchor>() != null)
                            {
                                _checkedRoot = _checkedRoot.parent.GetChild(childIndex);
                                break;
                            }
                        }
                    }
                }

                if(_checkedRoot.GetComponent<RootAnchor>() != null)
                {
                    _foundRoot = _checkedRoot;
                    if(aFindHighestSublevel == false) //If we're only looking for the first root anchor, return it
                    {                                                           //note that level search only returns the first level root, we do not allow full length level searches due to unpredictable behaviours on higher levels
                        _returnRoot = _foundRoot;
                    }
                }

                if(_checkedRoot == aBaseTransform.root)
                {
                    if(_foundRoot == null)
                    {
                        if(aBaseTransform.parent != aBaseTransform.root)
                            Debug.LogWarning($"{aBaseTransform}: Attempted to find root anchor with no results during level search. This will most likely result in reference errors. Have you added a root anchor to the hierarchy ");
                        return _checkedRoot;

                    }
                    else
                    {
                        _returnRoot = _foundRoot;
                    }
                }
            }

            return _returnRoot;
        }


        /*
         * This hasn't been tested and is probably a bad idea anyway? 
        public static Component FindComponentInAnchors(Transform aBaseTransform, Type aComponentType)
        {
            //Basically a step by step search of anchors until we reach the highest anchor / base or find the component we're looking for
            if(aBaseTransform == aBaseTransform.root)
            {
                Component aFoundComponent = aBaseTransform.GetComponent(aComponentType);
                return aFoundComponent;
            }

            Transform aParentAnchor = FindFirstRoot(aBaseTransform);

            if(aParentAnchor != null)
            {
                Component aFoundComponent = aBaseTransform.GetComponent(aComponentType);
                if(aFoundComponent != null)
                    return aFoundComponent;
                else
                    return FindComponentInAnchors(aParentAnchor, aComponentType);
            }

            return null;
        }*/
    }

}