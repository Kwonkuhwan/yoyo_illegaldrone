using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestPhotonInstantiate : MonoBehaviourPun
{
    public string NextSceneName;
    public Button btn;
    public Button btn2;
    [SerializeField] private GameObject prefab;

    private void Awake()
    {
        btn.onClick.AddListener(() => BtnClick());

        btn2.onClick.AddListener(() => Btn2Click());
    }

    private void BtnClick()
    {
        PhotonNetwork.LoadLevel(NextSceneName);
    }

    private void Btn2Click()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
