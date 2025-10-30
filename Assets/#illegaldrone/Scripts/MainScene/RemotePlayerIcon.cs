using UnityEngine;
using Photon.Pun;

public class RemotePlayerIcon : MonoBehaviourPun
{
    [SerializeField] private Transform headTrans;


    private void Start()
    {
        if(photonView.IsMine)
            gameObject.SetActive(false);
    }

    void Update()
    {
        if(headTrans.position.x != transform.position.x || headTrans.position.z != transform.position.z)
        {
            transform.position = new Vector3(headTrans.position.x, 0.92f, headTrans.position.z);
        }
    }
}
