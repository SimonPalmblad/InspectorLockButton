using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="type"></param>
    /// <returns>List of all matching child elements. Empty list if there are none.</returns>
    public static List<T> GetChildrenOfType<T>(this VisualElement elem) where T : VisualElement
    {
        var results = new List<T>();

        if (elem.childCount == 0)
        {
            return results;
        }

        for (int i = 0; i < elem.childCount; i++)
        {
            if (elem[i].childCount > 0)
            {
                results.AddRange(FindInChildrenRecursive<T>(elem[i].Children()
                                                                      .ToList()));
            }

            if (elem[i] is T)
            {
                results.Add(elem[i] as T);
                continue;
            }
        }

        return results;


    }

    private static List<Type> FindInChildrenRecursive<Type>(List<VisualElement> targetList) where Type : VisualElement
    {
        var results = new List<Type>();
        
        for (int i = 0; i < targetList.Count; i++)
        {
            if(targetList[i].childCount > 0)
            {
                results.AddRange( FindInChildrenRecursive<Type>(targetList[i].Children()
                                                                             .ToList()) );
            }

            if (targetList[i] is Type)
            {
                results.Add(targetList[i] as Type);                
                continue;
            }
        }

        return results;
    }
}
