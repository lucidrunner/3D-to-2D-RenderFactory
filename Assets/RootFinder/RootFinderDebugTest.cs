using UnityEngine;
using RootChecker;
public class RootFinderDebugTest : MonoBehaviour
{
    public void DebugRoot()
    {
        Debug.Log($"{transform.root}");
        Debug.Log($"{RootFinder.FindFirstRoot(transform)}");
        Debug.Log($"{RootFinder.FindHighestRoot(transform)}");
        Debug.Log($"{RootFinder.LevelRootSearch(transform, false)}");
        Debug.Log($"{RootFinder.LevelRootSearch(transform, true)}");
    }
}
