using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    bool gamePaused = false;
    public GameObject pauseMenu;
    private FirstPersonController firstPersonController;
    private Character character;
    
    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonController = GetComponent<FirstPersonController>();
        character = GetComponent<Character>();
    }

    public void PauseGame()
    {
        firstPersonController.enabled = false;
        character.enabled = false;
        gamePaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        firstPersonController.enabled = true;
        character.enabled = true;
        gamePaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused)
        {
            gamePaused = !gamePaused;
            PauseGame();
        }
    }
}
