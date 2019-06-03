using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
     
    public static MonsterManager instace
    {
        get;set;
    }

    IEnumerator AwaitMakeNote(Monster monster) 

    {
        string monsterType = monster.monsterType;
        int monsterHp = monster.monsterHp;
        int attackDelay = monster.attackDelay;
        
        yield return 0;

    }
    public class Monster
    {
        public string monsterType { get; set; } 
        public int monsterHp { get; set; }
        public int attackDelay { get; set; }
        public Monster(string monsterType, int monsterHp, int attackDelay) // 몬스터타입과 순서를 결정하는 생성자를 만들어주고
        {
            this.monsterType = monsterType;
            this.monsterHp = monsterHp;
            this.attackDelay = attackDelay;
        
        }
    }
    public List<Monster> monsters = new List<Monster>();
    void Start()
    {
        MonInit();
        
    }

    public void MonInit()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("read/" + 1);
        if (textAsset == null)
            Debug.Log(1);
        StringReader reader = new StringReader(textAsset.text);

        string line; // 몬스터 정보
        while ((line = reader.ReadLine()) != null)
        {
            Monster mosnter = new Monster(
                Convert.ToString(line.Split(' ')[0]), // 몬스터타입
                Convert.ToInt32(line.Split(' ')[1]), // 몬스터 피 
                Convert.ToInt32(line.Split(' ')[2])// 지연 시간
            );
            monsters.Add(mosnter);
        }
        Debug.Log(monsters[0].monsterHp);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
