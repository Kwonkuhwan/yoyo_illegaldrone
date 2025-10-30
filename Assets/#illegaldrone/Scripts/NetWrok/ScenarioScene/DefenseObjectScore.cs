using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenseObjectScore : MonoBehaviour
{
    [SerializeField] private GameObject go_Score;
    
    [SerializeField] private float coolTime = 0.0f;
    [SerializeField] private float hideTime = 3.0f;
    [SerializeField] private bool isShow = false;

    private void Update()
    {
        if (isShow)
        {
            coolTime += Time.deltaTime;

            if (hideTime < coolTime)
            {
                UIHide();
            }
        }
    }

    
    public void UIShow(float hideTime, float defenseObjectHP)
    {
        isShow = true;
        coolTime = 0.0f;
        this.hideTime = hideTime;

        go_Score.GetComponentInChildren<TMP_Text>().text = $"{defenseObjectHP}";
        go_Score.SetActive(true);
    }

    private void UIHide()
    {
        isShow = false;
        go_Score.SetActive(false);
    }
}
