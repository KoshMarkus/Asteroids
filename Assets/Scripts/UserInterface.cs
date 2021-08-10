using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class UserInterface : MonoBehaviour
{
    [SerializeField] GameObject deathPrompt;

    [SerializeField] GameObject playerInfo;
    [SerializeField] GameObject objectSpawner;
    [SerializeField] GameObject player;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject continueButton;

    [SerializeField] GameObject livesCounter;
    [SerializeField] GameObject pointsCounter;
    [SerializeField] GameObject controlsButtonText;

    public enum States { BeforePlay, Play, Pause, Dead };
    public States state;

    private static bool restart;
    public static bool mouseControls;

    public static UserInterface Instance;
    

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        player = GameObject.Find("Player");
        objectSpawner = GameObject.Find("ObjectSpawner");
        menu = GameObject.Find("Menu");
        controlsButtonText = GameObject.Find("Controls Text");

        player.SetActive(false);
        objectSpawner.SetActive(false);

        if (restart)
        {
            ChangeState(States.Play);
            controlsButtonText.GetComponent<TextMeshProUGUI>().text = "Управление: Клавиатура";
            restart = false;
        }
        else
        {
            ChangeState(States.BeforePlay);
        }

        ChangeControlScheme(true);
    }

    public void PauseMenu()
    {
        if (state == States.Play)
        {
            ChangeState(States.Pause);
        }
        else if (state == States.Pause)
        {
            ChangeState(States.Play);
        }
    }

    public void ChangeControlScheme(bool check)
    {
        if (check)
        {
            if (mouseControls == false)
            {
                controlsButtonText.GetComponent<TextMeshProUGUI>().text = "Управление: WAD/Space";
            }
            else
            {
                controlsButtonText.GetComponent<TextMeshProUGUI>().text = "Управление: ЛКМ/ПКМ";
            }
        }
        else
        {
            if (mouseControls == true)
            {
                mouseControls = false;
                controlsButtonText.GetComponent<TextMeshProUGUI>().text = "Управление: WAD/Space";
            }
            else
            {
                mouseControls = true;
                controlsButtonText.GetComponent<TextMeshProUGUI>().text = "Управление: ЛКМ/ПКМ";
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            PauseMenu();
        }

        if (!player && !objectSpawner)
        {
            player = GameObject.Find("Player");
            objectSpawner = GameObject.Find("ObjectSpawner");
            UpdateLivesCounter(Player.Instance.lives);
            UpdatePointsCounter(Player.Instance.currentPoints);
        }  
    }

    public void StartGame()
    {
        if(state == States.BeforePlay)
        {
            ChangeState(States.Play);
        }
        else if(state == States.Pause)
        {
            restart = true;
            state = States.Play;
            Reload();
        }
    }

    public void ContinueGame()
    {
        ChangeState(States.Play);
    }

    public void ShowDeathPrompt()
    {
        ChangeState(States.Dead);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Reload()
    {
        if (state == States.Dead)
        {
            ChangeState(States.Play);
            deathPrompt.SetActive(false);
            restart = true;
        }

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ChangeState(States stateToBe)
    {
        state = stateToBe;

        if (state == States.BeforePlay)
        {
            Time.timeScale = 0;
            deathPrompt.SetActive(false);
            playerInfo.SetActive(false);
            menu.SetActive(true);
        }
        else if (state == States.Play)
        {
            Time.timeScale = 1;
            menu.SetActive(false);
            playerInfo.SetActive(true);
            player.SetActive(true);
            objectSpawner.SetActive(true);
            continueButton.SetActive(true);

            UpdateLivesCounter(Player.Instance.lives);
            UpdatePointsCounter(Player.Instance.currentPoints);
        }
        else if (state == States.Pause)
        {
            Time.timeScale = 0;
            menu.SetActive(true);
        }
        else if (state == States.Dead)
        {
            Time.timeScale = 1;
            deathPrompt.SetActive(true);
        }
    }

    public void UpdatePointsCounter(int points)
    {
        pointsCounter.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }

    public void UpdateLivesCounter(int lives)
    {
        livesCounter.GetComponent<TextMeshProUGUI>().text = lives.ToString();
    }
}
