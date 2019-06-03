using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour
{
    private Image content;

    [SerializeField]
    private Text statText;

    [SerializeField]
    private float lerpSpeed;

    private float currentFill;
    public float MyMaxValue { get; set; }

    MonsterManager mon;
    // 체력 현재 값 설정
    public float MyCurrentValue
    {
        get
        {
            return currentValue;
        }

        set
        {
            if (value > MyMaxValue) currentValue = MyMaxValue;
            else if (value < 0) currentValue = 0;
            else currentValue = value;

            currentFill = currentValue / MyMaxValue;
            statText.text = currentValue + " / " + MyMaxValue;
        }
    }

    private float currentValue;
    void Start()
    {
        mon = new MonsterManager();
        
        content = GetComponent<Image>();
        MyMaxValue = 400.0f; 
        
        MyCurrentValue = 400.0f;
        //Debug.Log(mon.monsters[0].monsterHp);
    }

    // Update is called once per frame
    void Update()
    {
        statText.text = currentValue + " / " + MyMaxValue;
        if (currentFill != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
        }
    }

    // 체력과 마나 값을 셋팅(현재 값, 최대 값)
    public void Initialize(float currentValue, float maxValue)
    {
        MyMaxValue = maxValue;
        MyCurrentValue = currentValue;
    }
}
