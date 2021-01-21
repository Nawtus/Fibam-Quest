using UnityEngine;

public class Box_Old : MonoBehaviour
{
    private Animator box;

    [Header("Move")]
    public GameObject fantasma;
    private float tempoDeInicio;
    public float gridSize = 0.32f;
    [SerializeField] bool seMovendoX, seMovendoY;
    [SerializeField] float speed;  //Quanto menos tempo, mais rapido.
    public bool inTarget;
    public bool lockBox;

    [Header("Raycasting")]
    public Transform[] rayCastPos;
    public float distanceRay;
    public bool[] checkWall;

    void Start()
    {   
        box = GetComponent<Animator>();
    }

    private void Update()
    {
        Lerping();
        CheckWall();
    }
    public void Lerping()
    {
        float t = (Time.time - tempoDeInicio) / speed; // vai calcular a diferença de tempo para dar um tempo exato. o resultado sempre vai menor que 1.
        if (t < 1.0f)
        {
            transform.position = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), new Vector2(fantasma.transform.position.x, fantasma.transform.position.y), t); //aqui sera aplicado o lerp nas posições
        }
        if (Vector2.Distance(transform.position, fantasma.transform.position) <= 0.02f) //se a distancia do player para o destino final dele foi
        {
            seMovendoX = false;
            seMovendoY = false;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void moveBox(string direction)
    {
        
        tempoDeInicio = Time.time;
        switch (direction)
        {
            case "left":
                if (!checkWall[0] && !lockBox)
                {
                    fantasma.transform.position = new Vector2(fantasma.transform.position.x - gridSize, fantasma.transform.position.y);
                    PlaySFX("event:/Jogo/Caixa arrastando");
                }
                break;

            case "rigth":
                if (!checkWall[1] && !lockBox)
                {
                    fantasma.transform.position = new Vector2(fantasma.transform.position.x + gridSize, fantasma.transform.position.y);
                    PlaySFX("event:/Jogo/Caixa arrastando");
                }
                break;

            case "up":
                if (!checkWall[2] && !lockBox)
                {
                    fantasma.transform.position = new Vector2(fantasma.transform.position.x, fantasma.transform.position.y + gridSize);
                    PlaySFX("event:/Jogo/Caixa arrastando");
                }
                break;

            case "down":
                if (!checkWall[3] && !lockBox)
                {
                    fantasma.transform.position = new Vector2(fantasma.transform.position.x, fantasma.transform.position.y - gridSize);
                    PlaySFX("event:/Jogo/Caixa arrastando");
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Target")
        {
            inTarget = true;
            box.SetBool("inTarget", inTarget);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Target")
        {
            inTarget = false;
            box.SetBool("inTarget", inTarget);
        }
    }

    void CheckWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(rayCastPos[0].position, rayCastPos[0].up, distanceRay);
        RaycastHit2D hitRight = Physics2D.Raycast(rayCastPos[1].position, rayCastPos[1].up, distanceRay);
        RaycastHit2D hitUp = Physics2D.Raycast(rayCastPos[2].position, rayCastPos[2].up, distanceRay);
        RaycastHit2D hitDown = Physics2D.Raycast(rayCastPos[3].position, rayCastPos[3].up, distanceRay);

        if (hitLeft.collider != null)
        {
            if (hitLeft.collider.CompareTag("Tilemap Solid") || hitLeft.collider.CompareTag("Box"))
            {
                checkWall[0] = true;
            }
            if (hitLeft.collider.gameObject.CompareTag("Target") || hitLeft.collider.gameObject.CompareTag("Player"))
            {
                checkWall[0] = false;
            }
        }
        else
        {
            checkWall[0] = false;
        }

        if (hitRight.collider != null)
        {
            if (hitRight.collider.CompareTag("Tilemap Solid") || hitRight.collider.CompareTag("Box"))
            {
                checkWall[1] = true;
            }
            if (hitRight.collider.gameObject.CompareTag("Target") || hitRight.collider.gameObject.CompareTag("Player"))
            {
                checkWall[1] = false;
            }
        }
        else
        {
            checkWall[1] = false;
        }

        if (hitUp.collider != null)
        {
            if (hitUp.collider.CompareTag("Tilemap Solid") || hitUp.collider.CompareTag("Box"))
            {
                checkWall[2] = true;
            }
            if (hitUp.collider.gameObject.CompareTag("Target") || hitUp.collider.gameObject.CompareTag("Player"))
            {
                checkWall[2] = false;
            }
        }
        else
        {
            checkWall[2] = false;
        }

        if (hitDown.collider != null)
        {
            if (hitDown.collider.CompareTag("Tilemap Solid") || hitDown.collider.CompareTag("Box"))
            {
                checkWall[3] = true;
            }
            if (hitDown.collider.gameObject.CompareTag("Target") || hitDown.collider.gameObject.CompareTag("Player"))
            {
                checkWall[3] = false;
            }
        }
        else
        {
            checkWall[3] = false;
        }
    }
    private void PlaySFX(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}