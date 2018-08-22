using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIScript : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthFill;

    // Use this for initialization
    void Start()
    {
        healthFill.localScale = Vector3.one;
    }

    public void SetHealth(float amount)
    {
        healthFill.localScale = new Vector3(amount, 1, 1);
    }
}
