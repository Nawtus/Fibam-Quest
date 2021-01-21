using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Controller_Old : MonoBehaviour
{
    [Header("Animação")]
    public bool saveBlue;
    public bool saveRed;
    private Animator animatorPlayer;

    [Header("Flip")]
    [SerializeField] private bool isLookLeft;

    [Header("Move")]

    public GameObject fantasma;
    private float tempoDeInicio;
    public float gridSize = 0.32f;
    [SerializeField] private bool seMovendoX, seMovendoY;
    [SerializeField] private float speed;  //Quanto menos tempo, mais rapido.
    public bool moving;
    public bool lockPlayer;
    public bool[] checkWall;

    [Header("Raycast")]
    public float distance;
    public Transform[] rayCastPos;
    public bool[] boxInDirection;
    public GameObject[] boxsInDirection;

    //[Header("Audio")]

    void Start()
    {
        animatorPlayer = GetComponent<Animator>();

        if (SceneManager.GetActiveScene().buildIndex >= 7)
        {
            if (PlayerPrefs.HasKey("NewIdle"))
            {   
                saveBlue = PlayerPrefs.GetInt("NewIdle") == 1;
                animatorPlayer.SetLayerWeight(1, 1);
            }
        }
        CheckLock();
    }
    void Update()
    {
        Raycasting();
        Lerping();
        Animacao();

        // Apenas para fase de testes
        if (Input.GetKey(KeyCode.K))
        {
            PlayerPrefs.DeleteAll();
        }
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene_Manager.instance.RestartScene();
            PlaySFX("event:/Jogo/Reload");
        }
        if (Input.GetKeyDown    (KeyCode.Escape))
        {
            Scene_Manager.instance.BackMenu();
            PlaySFX("event:/Jogo/Reload");
        }
        if (lockPlayer == false)
        {
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !seMovendoY && !moving && !checkWall[0] && !lockPlayer)
            {
                if (boxInDirection[0])
                {
                    boxsInDirection[0].SendMessage("moveBox", "left", SendMessageOptions.DontRequireReceiver);
                    if (boxsInDirection[0].GetComponent<Box_Old>().checkWall[0] || boxsInDirection[0].GetComponent<Box_Old>().lockBox)
                    {
                        return;
                    }
                }
                tempoDeInicio = Time.time;
                fantasma.transform.position = new Vector2(fantasma.transform.position.x - gridSize, fantasma.transform.position.y);
                seMovendoX = true;
            }
            else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !seMovendoY && !moving && !checkWall[1] && !lockPlayer)
            {
                //Sound_Controller.instance.PlaySFX(moveAudio, 0.5f);
                if (boxInDirection[1])
                {
                    boxsInDirection[1].SendMessage("moveBox", "rigth", SendMessageOptions.DontRequireReceiver);
                    if (boxsInDirection[1].GetComponent<Box_Old>().checkWall[1] || boxsInDirection[1].GetComponent<Box_Old>().lockBox)
                    {
                        return;
                    }
                }
                tempoDeInicio = Time.time;
                fantasma.transform.position = new Vector2(fantasma.transform.position.x + gridSize, fantasma.transform.position.y);
                seMovendoX = true;
            }
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && !seMovendoX && !moving && !checkWall[2] && !lockPlayer)
            {
                //Sound_Controller.instance.PlaySFX(moveAudio, 0.5f);
                if (boxInDirection[2])
                {
                    boxsInDirection[2].SendMessage("moveBox", "up", SendMessageOptions.DontRequireReceiver);
                    if (boxsInDirection[2].GetComponent<Box_Old>().checkWall[2] || boxsInDirection[2].GetComponent<Box_Old>().lockBox)
                    {
                        return;
                    }
                }
                tempoDeInicio = Time.time;
                fantasma.transform.position = new Vector2(fantasma.transform.position.x, fantasma.transform.position.y + gridSize);
                seMovendoY = true;
            }
            else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && !seMovendoX && !moving && !checkWall[3] && !lockPlayer)
            {
                //Sound_Controller.instance.PlaySFX(moveAudio, 0.5f);
                if (boxInDirection[3])
                {
                    boxsInDirection[3].SendMessage("moveBox", "down", SendMessageOptions.DontRequireReceiver);
                    if (boxsInDirection[3].GetComponent<Box_Old>().checkWall[3] || boxsInDirection[3].GetComponent<Box_Old>().lockBox)
                    {
                        return;
                    }
                }
                tempoDeInicio = Time.time;
                fantasma.transform.position = new Vector2(fantasma.transform.position.x, fantasma.transform.position.y - gridSize);
                seMovendoY = true;
            }
        }      
        //FLIP
        float h = Input.GetAxis("Horizontal"); // FLIPPAR DIREITA/ESQUERDA
        if (h > 0 && isLookLeft == true)
        {
            Flip();
        }
        else if (h < 0 && isLookLeft == false)
        {
            Flip();
        }
    }

    private void Animacao()
    {
        animatorPlayer.SetBool("Move", moving);
        animatorPlayer.SetBool("SaveBlue", saveBlue);
        animatorPlayer.SetBool("SaveBlueMove", moving);
        animatorPlayer.SetBool("SaveRed", saveRed);
    }

    void Lerping()
    {
        float t = ((Time.time - tempoDeInicio) / speed) * Time.deltaTime; // vai calcular a diferença de tempo para dar um tempo exato. o resultado sempre vai menor que 1.
        if (t < 1.0f)
        {
            transform.position = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), new Vector2(fantasma.transform.position.x, fantasma.transform.position.y), t); //aqui sera aplicado o lerp nas posições
        }

        if (Vector2.Distance(transform.position, fantasma.transform.position) <= 0.02f) //se a distancia do player para o destino final dele foi
        {
            seMovendoX = false;
            seMovendoY = false;
            moving = false;
        }
        else
        {
            moving = true;
        }
    }
    void Flip()
    {
        isLookLeft = !isLookLeft;
        GetComponent<SpriteRenderer>().flipX = isLookLeft;
    }
    public void Raycasting()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(rayCastPos[0].position, rayCastPos[0].up, distance);
        RaycastHit2D hitRight = Physics2D.Raycast(rayCastPos[1].position, rayCastPos[1].up, distance);
        RaycastHit2D hitUp = Physics2D.Raycast(rayCastPos[2].position, rayCastPos[2].up, distance);
        RaycastHit2D hitDown = Physics2D.Raycast(rayCastPos[3].position, rayCastPos[3].up, distance);


        if (hitLeft.collider != null)
        {
            if (hitLeft.collider.gameObject.CompareTag("Box"))
            {
                boxInDirection[0] = true;
                boxsInDirection[0] = hitLeft.collider.gameObject;
                checkWall[0] = false;
            }

            if (hitLeft.collider.gameObject.CompareTag("Tilemap Solid"))
            {
                checkWall[0] = true;
                boxInDirection[0] = false;
                boxsInDirection[0] = null;
            }
            if (hitLeft.collider.gameObject.CompareTag("Cancela"))
            {
                checkWall[0] = false;
            }
        }
        else
        {
            boxInDirection[0] = false;
            boxsInDirection[0] = null;
            checkWall[0] = false;
        }

        if (hitRight.collider != null)
        {
            if (hitRight.collider.gameObject.CompareTag("Box"))
            {
                boxInDirection[1] = true;
                boxsInDirection[1] = hitRight.collider.gameObject;
                checkWall[1] = false;
            }

            if (hitRight.collider.gameObject.CompareTag("Tilemap Solid"))
            {
                checkWall[1] = true;
                boxInDirection[1] = false;
                boxsInDirection[1] = null;
            }
            if (hitRight.collider.gameObject.CompareTag("Cancela"))
            {
                checkWall[1] = false;
            }
        }
        else
        {
            boxInDirection[1] = false;
            boxsInDirection[1] = null;
            checkWall[1] = false;
        }

        if (hitUp.collider != null)
        {
            if (hitUp.collider.gameObject.CompareTag("Box"))
            {
                boxInDirection[2] = true;
                boxsInDirection[2] = hitUp.collider.gameObject;
                checkWall[2] = false;
            }

            if (hitUp.collider.gameObject.CompareTag("Tilemap Solid"))
            {
                checkWall[2] = true;
                boxInDirection[2] = false;
                boxsInDirection[2] = null;
            }
            if (hitUp.collider.gameObject.CompareTag("Cancela"))
            {
                checkWall[2] = false;
            }
        }
        else
        {
            boxInDirection[2] = false;
            boxsInDirection[2] = null;
            checkWall[2] = false;
        }

        if (hitDown.collider != null)
        {
            if (hitDown.collider.gameObject.CompareTag("Box"))
            {
                boxInDirection[3] = true;
                boxsInDirection[3] = hitDown.collider.gameObject;
                checkWall[3] = false;
            }

            if (hitDown.collider.gameObject.CompareTag("Tilemap Solid"))
            {
                checkWall[3] = true;
                boxInDirection[3] = false;
                boxsInDirection[3] = null;
            }
            if (hitDown.collider.gameObject.CompareTag("Cancela"))
            {
                checkWall[3] = false;
            }
        }
        else
        {
            boxInDirection[3] = false;
            boxsInDirection[3] = null;
            checkWall[3] = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "FinishSpot" && Level_Controller.instance.finishAgain == false)
        {           
            lockPlayer = true;
            Level_Controller.instance.finishAgain = true;
            Level_Controller.instance.unlockLevels = PlayerPrefs.GetInt("LevelUnlock");
            PlayerPrefs.SetInt("LevelUnlock", Level_Controller.instance.unlockLevels += 1);
            Scene_Manager.instance.scene = Scene_Manager.instance.allScenes[SceneManager.GetActiveScene().buildIndex + 1];
            Scene_Manager.instance.gameObject.SetActive(true);
            PlayerPrefs.SetInt("FinishAgain", Level_Controller.instance.finishAgain ? 1 : 0);
        }

        else if (collision.gameObject.tag == "Blue")
        {
            saveBlue = true;
            PlayerPrefs.SetInt("NewIdle", saveBlue ? 1 : 0);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Red")
        {
            saveRed = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Win")
        {
            Canvas_Controller.instance.win = true;
            Canvas_Controller.instance.winScene = true;
            lockPlayer = true;
        }
    }
    void CheckLock()
    {
        bool finishTutor = false;
        if (PlayerPrefs.HasKey("FinishTutor"))
        {
            finishTutor = PlayerPrefs.GetInt("FinishTutor") == 1;
        }
        if (finishTutor)
        {
            lockPlayer = false;
        }
        else
        {
            lockPlayer = true;
        }
    }
    private void PlaySFX(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}
