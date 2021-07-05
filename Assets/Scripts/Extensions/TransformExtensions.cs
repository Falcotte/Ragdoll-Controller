using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Performs a recursive search to find the child transform by name
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static Transform FindRecursive(this Transform transform, string name, bool includeInactive = false)
    {
        foreach(var child in transform.GetComponentsInChildren<Transform>(includeInactive))
        {
            if(child.name.Equals(name))
            {
                return child;
            }
        }
        return null;
    }
}