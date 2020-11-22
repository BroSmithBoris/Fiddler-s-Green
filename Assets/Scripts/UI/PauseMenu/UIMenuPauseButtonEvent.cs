using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class UIMenuPauseButtonEvent : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    public void OnGameExit()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void OnContinueGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Player.GetComponent<Control>().PlayerControls.Enable();
    }
}
