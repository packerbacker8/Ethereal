using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void SetLayerRecusively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            if(child == null)
            {
                continue;
            }
            SetLayerRecusively(child.gameObject, newLayer);
        }
    }

    public static void SetTagRecusively(GameObject obj, string newTag)
    {
        if (obj == null)
        {
            return;
        }

        obj.tag = newTag;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            SetTagRecusively(child.gameObject, newTag);
        }
    }
}
