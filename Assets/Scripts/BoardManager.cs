using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

}

public class Board
{
    public const int LENGTH = 4;
    public int[,] value { get; set; }
    public int score { get; set; }
    public int bombkey { get; set; }
    public bool moved { get; set; }
    public int skillturnCnt { get; set; }
    public int ThanosLine { get; set; }
    public GameObject[] obj_mon;



    private int EnemyTurn=0;
    public int SpawnTurn;

    public string mon_name;
    public int mon_delay;
    public int mon_hp;
    public double hpMul;

    List<MonsterManager.Monster> monsters = new List<MonsterManager.Monster>();

    bool first = true;


    public Board(GameObject[] mon)
    {
        value = new int[LENGTH, LENGTH];
        System.Array.Clear(value, 0, LENGTH*LENGTH);
        score = 0;
        bombkey = -1;
        ThanosLine = -1;
        skillturnCnt = 0;
        mon_name = "";
        mon_delay = 0;
        mon_hp = 0;
        obj_mon = mon;
        hpMul = 1;
    }

    //2048 판에서 빈 칸 반환
    public int[] emptyCell()
    {
        ArrayList list_cell = new ArrayList();
        for (int i = 0; i < LENGTH; i++)
        {
            for (int j = 0; j < LENGTH; j++)
            {
                if (value[i, j] == 0) list_cell.Add(i * LENGTH + j);
            }
        }
        return (int[])list_cell.ToArray(typeof(int));
    }


    //2048 판이 빈 칸을 가지고 있는지
    public bool hasEmptyCell()
    {
        return emptyCell().Length > 0;
    }


    public bool chkActivate()
    {
        bool active = false;
        for(int i = 0; i < obj_mon.Length; i++)
        {
            if (obj_mon[i].activeSelf) active = true;
        }
        return active;
    }
    //2048 판에 새 블럭(숫자)을 넣기
    public void newPut()
    {
        GameManager.instance.Move();
        EnemyTurn++;
        Debug.Log(SpawnTurn);

        if (SpawnTurn == EnemyTurn && !chkActivate())
        {
            if (first)
            {
                EnemySelect(monsters[0]);
                first = false;
            }
            else
            {
                EnemySelect();
            }
            
            switch (mon_name)//몬스터 이름에 따라 다른 몬스터 활성화
            {
                case "SLIME":
                    for (int i = 0; i < obj_mon.Length; i++)
                    {
                        obj_mon[i].SetActive(false);
                    }
                    obj_mon[0].SetActive(true);
                    break;
                case "PIRATE":
                    for (int i = 0; i < obj_mon.Length; i++)
                    {
                        obj_mon[i].SetActive(false);
                    }
                    obj_mon[1].SetActive(true);
                    break;
                case "DEVIL":
                    for (int i = 0; i < obj_mon.Length; i++)
                    {
                        obj_mon[i].SetActive(false);
                    }
                    obj_mon[2].SetActive(true);
                    break;
                default:
                    for (int i = 0; i < obj_mon.Length; i++)
                    {
                        obj_mon[i].SetActive(false);
                    }
                    break;
            }
            EnemyTurn = 0;
        }
        else if (EnemyTurn >= mon_delay && chkActivate())
        {
            switch (mon_name)
            {
                case "SLIME":
                    slime();
                    break;
                case "PIRATE":
                    BombActivate(3);
                    break;
                case "DEVIL":
                    ThanosActivate(3);
                    break;
            }
            EnemyTurn = 0;
        }
        if (hasEmptyCell() == false)
        {
            return;
        }

        if (skillturnCnt > 0)
        {
            skillturnCnt--;
            Debug.Log(skillturnCnt);
        }
        else if (skillturnCnt == 0 && bombkey >= 0)
        {
            BombPop();
        }
        else if (skillturnCnt == 0 && ThanosLine >= 0)
        {
            ThanosFire();
        }

        int[] emptyc_arr = emptyCell();
        int putCell_Idx = emptyc_arr[Random.Range(0, emptyc_arr.Length)];
        value[putCell_Idx / LENGTH, putCell_Idx % LENGTH] = 2;
    }

    public void printBoard()
    {
        Debug.Log("board:" + value);
        Debug.Log("score:" + score);
    }

    //2048 판을 왼쪽 방향에 맞춰 대칭이동 시킨다.
    public void wayMove(char way)
    {
        int[,] tmp = new int[LENGTH, LENGTH];
        for(int i = 0; i < LENGTH; i++)
        {
            for(int j = 0; j < LENGTH; j++)
            {
                switch (way)
                {
                    //오른쪽 ( 좌우 대칭 이동 )
                    case 'd':
                        tmp[i, j] = value[i, LENGTH - 1 - j];
                        break;

                    //아래쪽 ( y = x 대칭 이동 )
                    case 's':
                        tmp[i, j] = value[LENGTH - 1 - j, LENGTH - 1 - i];
                        break;

                    //위쪽 ( y = -x 대칭 이동 )
                    case 'w':
                        tmp[i, j] = value[j, i];
                        break;

                    default:
                        break;
                }
            }
        }

        System.Array.Copy(tmp, value, LENGTH*LENGTH);
    }


    //2048 판을 한 쪽으로 민다
    public void gravity(char way)
    {

        switch (way)
        {
            case 's': case 'd': case 'w':
                wayMove(way);
                break;

            case 'a':
                break;

            default:
                Debug.Log("Input Error");
                return;
        }

        bool[,] added = new bool[4, 4];
        moved = false;

        for (int i= 0; i < LENGTH; i++)
        {
            for(int j = 0; j < LENGTH; j++)
            {
                if(value[i, j] > 0)
                {
                    int k;
                    for (k = 1; j - k >= 0 && value[i, j - k] == 0; k++) { } //0이 아닌 칸을 만날 때까지 중력 작용
                    if(j - k >= 0 && value[i, j] == value[i, j - k] && !added[i, j - k]) //같은 숫자 만나면
                    {
                        value[i, j - k] += value[i, j];
                        score += value[i, j - k];
                        if(mon_hp > 0) mon_hp -= value[i, j - k];
                        value[i, j] = 0;
                        added[i, j - k] = true;
                        moved = true;
                    }
                    else if(j - k >= 0 && value[i, j - k] == -1) //끈끈이
                    {
                        value[i, j - k] = value[i, j];
                        value[i, j] = 0;
                        moved = true;
                    }
                    else if(k > 1)
                    {
                        value[i, j - k + 1] = value[i, j];
                        value[i, j] = 0;
                        moved = true;
                    }
                }
            }
        }
        switch (way)
        {
            case 's':
            case 'd':
            case 'w':
                wayMove(way);
                break;
        }
        if (chkActivate() && mon_hp <= 0)
        {
            EnemyTurnSelect();
        }
    }

    //합칠 수 있는 칸이 있는지
    public bool MergeAble()
    {
        for(int i = 0; i < LENGTH; i++)
        {
            for(int j = 0; j < LENGTH; j++)
            {
                if(value[i,j] == -1)
                {
                    return true;
                }
                if (i + 1 < LENGTH && value[i, j] == value[i + 1, j])
                    return true;

                if (j + 1 < LENGTH && value[i, j] == value[i, j + 1])
                    return true;
            }
        }
        return false;
    }

    //비어있는 칸이 없고 합칠 칸이 없으면 게임 종료(게임 종료시 False 반환)
    public bool GameOver()
    {
        return hasEmptyCell() || MergeAble();
    }

    //해적 스킬 활성화
    public void BombActivate(int cnt)
    {
        bombkey = Random.Range(0, LENGTH * LENGTH);
        skillturnCnt = cnt;
        GameManager.boom_bool = true;

    }

    //해적 스킬 (폭탄)
    public void BombPop()
    {
        value[bombkey / LENGTH, bombkey % LENGTH] = 0;
        bombkey = -1;
    }

    public int boardMax()
    {
        int max = int.MinValue;
        for(int i = 0; i < LENGTH; i++)
        {
            for(int j = 0; j < LENGTH; j++)
            {
                if(max < value[i, j])
                {
                    max = value[i, j];
                }
            }
        }
        return max;
    }

    public System.Tuple<int,int> boardMaxIdx()
    {
        System.Tuple<int, int> maxij = System.Tuple.Create(0, 0);
        for (int i = 0; i < LENGTH; i++)
        {
            for (int j = 0; j < LENGTH; j++)
            {
                if (value[maxij.Item1, maxij.Item2] < value[i, j])
                {
                    maxij = System.Tuple.Create(i, j);
                }
            }
        }
        return maxij;
    }

    //악마 스킬 활성화
    public void ThanosActivate(int cnt)
    {
        System.Tuple<int, int> maxIdx = boardMaxIdx();
        //List<int> idx_arr = new List<int>(new int[] { 0,1,2,3,4,5,6,7});
        //idx_arr.Remove(maxIdx.Item1+LENGTH);
        //idx_arr.Remove(maxIdx.Item2);
        List<int> idx_arr = new List<int>(new int[] { });
        idx_arr.Add(maxIdx.Item1+LENGTH);
        idx_arr.Add(maxIdx.Item2);
        ThanosLine = idx_arr[Random.Range(0, LENGTH * 2 - 2)];
        skillturnCnt = cnt;
    }

    //악마 스킬 (반갈죽, 타노스)
    public void ThanosFire()
    {
        if(ThanosLine < LENGTH)
        {
            for(int i = 0; i < LENGTH; i++)
            {
                if(value[i, ThanosLine] == 2)
                {
                    value[i, ThanosLine] = 0;
                }
                else if(value[i, ThanosLine] > 0)
                {
                    value[i, ThanosLine] /= 2;
                }
            }
        }
        else
        {
            for (int i = 0; i < LENGTH; i++)
            {
                if (value[ThanosLine - LENGTH, i] == 2)
                {
                    value[ThanosLine - LENGTH, i] = 0;
                }
                else if (value[ThanosLine - LENGTH, i] > 0)
                {
                    value[ThanosLine - LENGTH, i] /= 2;
                }
            }
        }
        ThanosLine = -1;
    }

    //슬라임 스킬 활성화
    public void slime()
    {
        int[] arr_cell = emptyCell();
        int slimeIdx = arr_cell[Random.Range(0, arr_cell.Length)];
        value[slimeIdx / LENGTH, slimeIdx % LENGTH] = -1;
    }

    //골렘 스킬 활성화
    public void golem()
    {
        int[] arr_cell = emptyCell();
        int golem = arr_cell[Random.Range(0, arr_cell.Length)];
        value[golem / LENGTH, golem % LENGTH] = -3;
    }


    public void EnemyInit()
    {
        MonsterManager mon_Mana = new MonsterManager();
        mon_Mana.MonInit();
        this.monsters = mon_Mana.monsters;
    }
    public void EnemyTurnSelect()
    {
        EnemyTurn = 0;
        SpawnTurn = Random.Range(20, 40);
        mon_name = "";
        mon_hp = 0;
        mon_delay = 0;
        for (int i = 0; i < obj_mon.Length; i++)
        {
            obj_mon[i].SetActive(false);
        }
        ThanosLine = -1;
        bombkey = -1;
        for (int i = 0; i < LENGTH; i++)
        {
            for(int j = 0; j < LENGTH; j++)
            {
                if(value[i,j] == -1)
                {
                    value[i, j] = 0;
                }
                if(value[i,j] == -3)
                {
                    value[i, j] = 0;
                }
            }
        }
    }

    public void EnemySelect(MonsterManager.Monster monster=null)
    {
        if (monster == null)
        {
            Debug.Log(monsters.Count);
            MonsterManager.Monster mon = monsters[Random.Range(0, monsters.Count)];
            mon_name = mon.monsterType;
            mon_hp = System.Convert.ToInt32(mon.monsterHp*hpMul);
            mon_delay = mon.attackDelay;
            hpMul += 0.02;
        }
        else
        {
            mon_name = monster.monsterType;
            mon_hp = System.Convert.ToInt32(monster.monsterHp*hpMul);
            mon_delay = monster.attackDelay;
        }
        
    }
}
