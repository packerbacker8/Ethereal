using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private Dictionary<string, GameObject> poolObjs;

    // Use this for initialization
    private void Awake()
    {
        poolObjs = new Dictionary<string, GameObject>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject curChild = this.transform.GetChild(i).gameObject;
            if(curChild.GetComponent<ObjectPool>() != null)
            {
                curChild.GetComponent<ObjectPool>().SetupPool();
            }
            poolObjs.Add(curChild.name, curChild);
        }
    }

    /// <summary>
    /// Returns game object associated with that pool name, or null if that name does not exist.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject GetObjectPoolByName(string key)
    {
        return poolObjs[key];
    }
}
