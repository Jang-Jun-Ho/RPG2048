using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites=new Sprite[15];//0은 숫자 없음, 1~11까지는 2~2048, 12부터는 보스 공격
    [SerializeField] private GameObject obj,pre_boom,pre_thanos, obj_boom;
    GameObject objBoom,objThanos;
    [SerializeField] private GameObject[] obj_mon;
    [SerializeField] private GameObject hpGauge;
    [SerializeField] private Text scoreText;
    public AudioClip move, explosion, back, enemy;
    AudioSource boomsource, backsource, enemysource, movesource;
    [SerializeField] GameObject panel;

    public static GameManager instance;

    Board b;
    int size=4;
    int[,] board;//숫자 배당은 0,2의 배수 : 숫자 타일, 음수 : 보스 공격 패턴
    int score;
    int testVar = 0;
    bool alert = false;
    GameObject[,] board_obj=new GameObject[4,4];
    public static bool boom_bool { get; set; }
    AudioSource audioSource;


    public void Expolision()
    {
        boomsource.PlayOneShot(explosion);
    }
    public void Move()
    {

        movesource.PlayOneShot(move);
    }
    private void Awake()
    {
        Screen.SetResolution(550, 900, false);
        if (GameManager.instance == null)
            GameManager.instance = this;
        boom_bool = false;
        b = new Board(obj_mon);
        board = b.value;
        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                board_obj[i,j] = Instantiate(obj,new Vector3(j*1.1f-0.1f,1.1f*(4-i)-0.7f,0),Quaternion.identity);

            }
        }
        objBoom=Instantiate(pre_boom, new Vector3(-30, -30, -1), Quaternion.identity);
        objThanos= Instantiate(pre_thanos, new Vector3(-30, -30, -1), Quaternion.identity);
    }

    void DrawBoard()
    {
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                switch (board[i, j])
                {
                    case -1:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[12];//슬라임 끈끈이
                        break;
                    case -3:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[14];//골렘 봉인
                        break;
                    case 0:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[0]; break;
                    case 2:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[1]; break;
                    case 4:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[2]; break;
                    case 8:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[3]; break;
                    case 16:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[4]; break;
                    case 32:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[5]; break;
                    case 64:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[6]; break;
                    case 128:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[7]; break;
                    case 256:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[8]; break;
                    case 512:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[9]; break;
                    case 1024:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[10]; break;
                    case 2048:
                        board_obj[i, j].GetComponent<SpriteRenderer>().sprite = sprites[11]; break;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
        boomsource = GetComponent<AudioSource>();
        backsource = GetComponent<AudioSource>();
        enemysource = GetComponent<AudioSource>();
        movesource = GetComponent<AudioSource>();

        //backsource.clip = back;
        //backsource.Play();
        b.newPut();
        b.newPut();
        DrawBoard();
        b.EnemyTurnSelect();
        b.EnemyInit();
        hpGauge.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    int[,] value = new int[4, 4];
        //    for (int i = 0; i < 4; i++)
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //            value[i, j] = 16;
        //        }
        //    }
        //}
        keyInput();
        score=b.score;
        scoreText.text = "score : " + score;
        DrawBoom(b.bombkey);
        DrawThanos(b.ThanosLine);
    }

    void keyInput()
    {
        bool past_mon_activate = b.chkActivate();
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (b.GameOver())
            {
                b.gravity('w');
                if (b.moved) b.newPut();
            }
            else
            {
                panel.SetActive(true);
                Debug.Log("Game Over");
            }

            DrawBoard();

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (b.GameOver())
            {
                b.gravity('a');
                if (b.moved) b.newPut();
            }
            else
            {
                panel.SetActive(true);
                Debug.Log("Game Over");
            }

            DrawBoard();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (b.GameOver())
            {
                b.gravity('s');
                if (b.moved) b.newPut();
            }
            else
            {
                panel.SetActive(true);
                Debug.Log("Game Over");
            }
            DrawBoard();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (b.GameOver())
            {
                b.gravity('d');
                if (b.moved) b.newPut();
            }
            else
            {
                panel.SetActive(true);
                Debug.Log("Game Over");
            }
            DrawBoard();
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (panel.activeSelf)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
        }

        if(past_mon_activate == false && b.chkActivate() == true)
        {
            
            hpGauge.GetComponent<HpManager>().MyMaxValue = b.mon_hp;
            hpGauge.GetComponent<HpManager>().MyCurrentValue = b.mon_hp;
            hpGauge.SetActive(true);
        }
        else if(b.chkActivate() == true)
        {
            hpGauge.GetComponent<HpManager>().MyCurrentValue = b.mon_hp;
        }
        else if(past_mon_activate == true && b.chkActivate() == false)
        {
            hpGauge.GetComponent<HpManager>().MyCurrentValue = 0;
            hpGauge.SetActive(false);
        }

    }

    void DrawBoom(int key)
    {
        if (key != -1&&!alert)
        {
            int xx = key / size;
            int yy = key % size;
            objBoom.transform.position = board_obj[xx, yy].transform.position + new Vector3(0, 0, -1);
            alert = true;
        }else if (key!=-1&&alert)
        {

        }
        else
        {

            obj_boom.transform.position = objBoom.transform.position;
            objBoom.transform.position = new Vector3(-30, -30, -1);
            if (boom_bool == true && b.chkActivate())
            {
                Instantiate(obj_boom);
                instance.Expolision();
                boom_bool = false;
            }
            alert = false;
        }

    }
    void DrawThanos(int key)
    {
        Vector3 pos;
        int rot;
        switch (key)
        {
            case 0:
                pos = board_obj[0, 0].transform.position+new Vector3(0,0,1);
                rot = -90;
                break;
            case 1:
                pos = board_obj[0, 1].transform.position + new Vector3(0, 0, 1);
                rot = -90;
                break;
            case 2:
                pos = board_obj[0, 2].transform.position + new Vector3(0, 0, 1);
                rot = -90;
                break;
            case 3:
                pos = board_obj[0, 3].transform.position + new Vector3(0, 0, 1);
                rot = -90;
                break;
            case 4:
                pos = board_obj[0, 0].transform.position + new Vector3(0, 0, 1);
                rot = 0;
                break;
            case 5:
                pos = board_obj[1, 0].transform.position + new Vector3(0, 0, 1);
                rot = 0;
                break;
            case 6:
                pos = board_obj[2, 0].transform.position + new Vector3(0, 0, 1);
                rot = 0;
                break;
            case 7:
                pos = board_obj[3, 0].transform.position + new Vector3(0, 0, 1);
                rot = 0;
                break;
            default:

                pos = new Vector3(-30, -30, 0);
                rot = 0;
                break;
        }
        objThanos.transform.position = pos;
        objThanos.transform.rotation = Quaternion.Euler(0, 0, rot);

    }
}
