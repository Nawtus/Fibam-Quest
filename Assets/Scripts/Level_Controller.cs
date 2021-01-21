using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour
{
    public static Level_Controller instance;

    [Header("In Level Select")]
    public GameObject player;
    private bool isOver;
    public bool unlock;
    [HideInInspector] public int unlockLevels = 1;
    public GameObject[] target;
    public Animator[] animTarget;

    [Header("In Game")]

    public bool inGame;
    public GameObject[] allBoxInScene;
    public bool finishLevel;
    public bool finishAgain;
    public Animator cancelAnim;
    private GameObject cancelObj;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            inGame = true;            
        }

        if (inGame)
        {   
            cancelObj = GameObject.Find("Cancela");
            cancelAnim = cancelObj.GetComponent<Animator>();
            GameObject[] tempBoxs = GameObject.FindGameObjectsWithTag("Box");
            allBoxInScene = new GameObject[tempBoxs.Length];
            allBoxInScene = tempBoxs;
            
        }
        unlock = true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            unlockLevels = PlayerPrefs.GetInt("LevelUnlock");
            for (int i = 1; i < unlockLevels; i++)
            {
                animTarget[i].SetBool("Unlock", unlock);
            }
        }   
    }

    private void Update()
    {       
        if (!inGame)
        {
            Animacao();
        }

        for (int i = 0; i < allBoxInScene.Length; i++)
        {
            if (!allBoxInScene[i].GetComponent<Box_Old>().inTarget)
            {
                finishLevel = false;
                return;
            }
        }
        finishLevel = true;
        if (finishLevel && inGame)
        {
            cancelAnim.SetBool("open",true);            
            cancelObj.GetComponent<BoxCollider2D>().isTrigger = true;   
            cancelObj.tag = "Cancela";
            for (int i = 0; i < allBoxInScene.Length; i++)
            {
                allBoxInScene[i].GetComponent<Box_Old>().lockBox = true;
            }
            if (cancelAnim.GetBool("open") == true)
            {
                PlaySFX("event:/Jogo/Cancela abrindo");
            }
            inGame = false;
        }
        
    }
    void Animacao()
    {
        foreach (var item in target)
        {
            if (Vector2.Distance(item.transform.position, player.transform.position) <= 0.1f)
            {
                isOver = true;
                item.GetComponent<Animator>().SetBool("isOver", isOver);
                if (Input.GetKeyDown(KeyCode.Return) && item.GetComponent<Animator>().GetBool("Unlock") == true)
                {
                    //Nome do Gameobj é o numero da scene + 2 (cenas que não sao levels)
                    Scene_Manager.instance.scene = Scene_Manager.instance.allScenes[int.Parse(item.gameObject.name) + 2];
                    Scene_Manager.instance.gameObject.SetActive(true);
                    PlaySFX("event:/Menu/Botão pressionado");
                }
            }
            else
            {
                isOver = false;
                item.GetComponent<Animator>().SetBool("isOver", isOver);
            }
        }       
    }
    private void PlaySFX(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}
