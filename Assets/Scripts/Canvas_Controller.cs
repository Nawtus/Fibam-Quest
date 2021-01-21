using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;

public class Canvas_Controller : MonoBehaviour
{
    public static Canvas_Controller instance;

    [Header("Menu")]
    public bool inMenu;
    public Animator scroolView;
    public bool scroolViewMove;
    public Button credits;
    public Button jogar;
    public Button sair;

    [Header("Tutorial")]
    public bool inLevelSelect;
    public bool finishTutor;
    public int secondNextClick;
    public Animator animTuto;
    public GameObject back;
    public GameObject tutorial;
    public GameObject canvas;
    public GameObject eventSystem;

    [Header("In Game")]
    public Text currentTimeTxt;
    public float totalTime;
    public bool needTime;
    public bool win;
    public bool winScene;
    public GameObject bg;
    public GameObject playAgain;
    public GameObject sair2;
    public GameObject Win;
    public Animator animWin;

    [SerializeField]
    [Header("Audio")]
    private StudioEventEmitter studioEvent;

    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        if (!inMenu && !Level_Controller.instance.inGame && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (PlayerPrefs.HasKey("FinishTutor"))
            {
                finishTutor = PlayerPrefs.GetInt("FinishTutor") == 1;
            }
            if (finishTutor)
            {
                canvas.SetActive(false);
                eventSystem.SetActive(false);
            }
        }
    }
    public void Update()
    {
        if (needTime)
        {
            Clock();
        }
        if (inLevelSelect)
        {
            tutorial.SetActive(true);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                BackTutor();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                NextTutor();
                if (secondNextClick >= 2)
                {
                    FinishTutor();
                }
            }
        }
        if (win && winScene)
        {
            Win.SetActive(true);
            animWin.SetBool("Win", win);
        }
    }
    public void PlayGame()
    {        
        Scene_Manager.instance.scene = Scene_Manager.instance.allScenes[SceneManager.GetActiveScene().buildIndex + 1];
        Scene_Manager.instance.gameObject.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Credits()
    {
        scroolViewMove = !scroolViewMove;

        if (scroolViewMove)
        {
            scroolView.SetBool("Move", true);
        }
        credits.interactable = false;
        jogar.interactable = false;
        sair.interactable = false;

    }
    public void Close()
    {
        scroolViewMove = !scroolViewMove;

        if (!scroolViewMove)
        {
            scroolView.SetBool("Move", false);
        }
        credits.interactable = true;
        jogar.interactable = true;
        sair.interactable = true;
    }
    public void Clock()
    {
        totalTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.RoundToInt(totalTime % 60f);

        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }

        currentTimeTxt.text = minutes.ToString("00") + ":" + seconds.ToString("00");       

        if (totalTime < 1)
        {
            bg.SetActive(true);
            playAgain.SetActive(true);
            sair2.SetActive(true);
        }
    }
    public void NextTutor()
    {
        animTuto.SetBool("Next", true);
        secondNextClick ++;
        back.SetActive(true);
        PlaySFX("event:/Menu/Botão tutorial -  página");
    }
    public void FinishTutor()
    {
        if (secondNextClick >= 2)
        {
            canvas.SetActive(false);
            eventSystem.SetActive(false);
            finishTutor = true;
            PlayerPrefs.SetInt("FinishTutor", finishTutor ? 1 : 0);
            GameObject.FindGameObjectWithTag("Player").SendMessage("CheckLock",SendMessageOptions.DontRequireReceiver);
            PlaySFX("event:/Menu/Botão tutorial -  página");
        }
    }
    public void BackTutor()
    {
        animTuto.SetBool("Next", false);
        secondNextClick --;
        back.SetActive(false);
        PlaySFX("event:/Menu/Botão tutorial -  página");
    }
    public void PlayAgain()
    {
        Scene_Manager.instance.RestartScene();
    }
    private void PlaySFX(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}   
