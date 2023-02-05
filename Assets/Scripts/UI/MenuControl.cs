using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XnodeBehaviourTree;

public class MenuControl : MonoBehaviour
{
    [Header("Others")]
    [SerializeField]
    private BossBehaviour bossBehaviour;
    [SerializeField]
    private BehaviourTreeRunner bossBehaviourTreeRunner;
    [SerializeField]
    private PlayerInput playerInput;

    [Header("Menu")]
    [SerializeField]
    private GameObject startMenu;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject creditButton;
    [SerializeField]
    private GameObject creditMenu;
    [SerializeField]
    private GameObject creditClose;

    [SerializeField]
    private GameObject win;
    [SerializeField]
    private GameObject winRestart;

    [SerializeField]
    private GameObject lose;
    [SerializeField]
    private GameObject loseRestart;

    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = true;

        bossBehaviour.enabled = false;
        bossBehaviourTreeRunner.enabled = false;
        playerInput.enabled = false;

        startMenu.SetActive(true);
        win.SetActive(false);
        lose.SetActive(false);

        EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void StartTutorial()
    {
        StartGame();
    }

    public void StartGame()
    {
        _canvas.enabled = false;
        bossBehaviour.enabled = true;
        bossBehaviourTreeRunner.enabled = true;
        playerInput.enabled = true;
    }

    public void OpenCredit()
    {
        creditMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditClose);
    }

    public void CloseCredit()
    {
        creditMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(creditButton);
    }

    public void Win()
    {
        StartCoroutine(DelayopenMenu(win));
    }

    public void Lose()
    {
        StartCoroutine(DelayopenMenu(lose));
    }

    IEnumerator DelayopenMenu(GameObject menu)
    {
        _canvas.enabled = true;
        startMenu.SetActive(false);
        yield return new WaitForSeconds(4f);
        menu.SetActive(true);

        if (win.activeSelf) EventSystem.current.SetSelectedGameObject(winRestart);
        if (lose.activeSelf) EventSystem.current.SetSelectedGameObject(loseRestart);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
