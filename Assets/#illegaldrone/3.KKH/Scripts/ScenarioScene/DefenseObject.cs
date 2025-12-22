using Illegaldrone;
using KKH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseObject : MonoBehaviour
{
    [SerializeField] private DefenseObjectScore defenseObjectScroe;

    [SerializeField] private float defenseObjectHP = 1000.0f;
    public float DefenseObjectHP => defenseObjectHP;

    public SpriteRenderer coreSprite = null;


    private void Awake()
    {
        if(defenseObjectScroe == null)
        {
            defenseObjectScroe = GetComponentInChildren<DefenseObjectScore>();
        }

        if(coreSprite != null)
        {
            if (GameManager.instance.isInstructor)
            {
                Color color = new Color(1.0f, 0.549f, 0.549f);
                coreSprite.color = color;
            }
        }
    }

    public void SetDenfenseOBjectHP(float hp)
    {
        defenseObjectHP = hp;
    }

    public void AttackedDefenseObject(float damege)
    {
        defenseObjectHP -= damege;
        defenseObjectScroe.UIShow(3f, DefenseObjectHP);
    }

    private void SetMainTargetRayCast()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // 레이캐스트 실행, 특정 레이어만 감지
        int layerMask = 1 << LayerMask.NameToLayer("Building"); // "Building" 레이어만 감지

        if (Physics.Raycast(origin, direction, out hit, 10000f, layerMask))
        {
            UTILS.Log("충돌한 Building 오브젝트: " + hit.collider.name);
        }
    }
}
