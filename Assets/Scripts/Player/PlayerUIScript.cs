using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIScript : MonoBehaviour
{
    public KeyCode pauseGameKey = KeyCode.Escape;

    [SerializeField]
    private RectTransform healthFill;

    [SerializeField]
    private GameObject pauseMenuObj;

    // Use this for initialization
    void Start()
    {
        healthFill.localScale = Vector3.one;
        PauseMenu.isPaused = false;
        pauseMenuObj.SetActive(false);
    }

    private void Update()
    {
        bool pause = Input.GetKeyDown(pauseGameKey);
        if (pause)
        {
            TogglePauseMenu();
        }
    }

    public void SetHealth(float amount)
    {
        healthFill.localScale = new Vector3(amount, 1, 1);
    }

    private void TogglePauseMenu()
    {
        pauseMenuObj.SetActive(!pauseMenuObj.activeInHierarchy);
        PauseMenu.isPaused = pauseMenuObj.activeInHierarchy;
    }
}
