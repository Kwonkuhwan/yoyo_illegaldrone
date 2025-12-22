using Illegaldrone;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class NetworkDrone : MonoBehaviourPun//, IPunObservable
    {
        public PhotonView pv;
        public DroneNavMeshAgent droneNavMeshAgent;

        // Original
        [Header("Original")]
        public Transform parent;
        public float fObjectScale = 1f;
        //public Transform droneBody;
        //public List<Transform> dronsWings = new List<Transform>();

        // Remote
        //[Header("Remote")]
        //public Transform remote_droneBody;
        //public List<Transform> remote_DroneWings = new List<Transform>();

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }

            if(droneNavMeshAgent == null)
            {
                droneNavMeshAgent = GetComponent<DroneNavMeshAgent>();
                if(!GameManager.instance.isInstructor)
                {
                    droneNavMeshAgent.agent.enabled = false;
                    droneNavMeshAgent.enabled = false;                    
                }
            }
        }

        private void Update()
        {
            if (parent == null) return;

            if (GameManager.instance.isInstructor)
            {
                parent.position = new Vector3(transform.position.x, parent.position.y, transform.position.z);
                //parent.rotation = transform.rotation;
            }
        }

        #region 삭제
        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    // This is our player, send our positions to the other players
        //    if (stream.IsWriting)
        //    {
        //        // Drone Body
        //        stream.SendNext(parent.position);
        //        stream.SendNext(parent.rotation);

        //        stream.SendNext(droneBody.position);
        //        stream.SendNext(droneBody.rotation);

        //        // Drone Wings                
        //        foreach (Transform wing in dronsWings)
        //        {
        //            stream.SendNext(wing.position);
        //            stream.SendNext(wing.rotation);
        //        }
        //    }
        //    else
        //    {
        //        transform.position = (Vector3)stream.ReceiveNext();
        //        transform.rotation = (Quaternion)stream.ReceiveNext();

        //        // Drone Body
        //        remote_droneBody.position = (Vector3)stream.ReceiveNext();
        //        remote_droneBody.rotation = (Quaternion)stream.ReceiveNext();

        //        // Drone Wings                
        //        foreach (Transform wing in remote_DroneWings)
        //        {
        //            wing.position = (Vector3)stream.ReceiveNext();
        //            wing.rotation = (Quaternion)stream.ReceiveNext();
        //        }
        //    }
        //}

        //private List<Transform> GetChildObjects(Transform parent)
        //{
        //    List<Transform> children = new List<Transform>();

        //    foreach (Transform child in parent)
        //    {
        //        children.Add(child);
        //    }

        //    return children;
        //}

        //public void AssignDroneObjects(Transform parent)
        //{
        //    this.parent = parent;
        //    droneBody = parent.gameObject.GetComponentInChildren<DroneMoveMent>().transform;
        //    dronsWings = droneBody.GetComponent<DroneMoveMent>().rotateObjs;
        //}
        #endregion

        public void SetDrone(Transform parent)
        {
            if (!GameManager.instance.isInstructor) return;
            //parent.parent = transform;
            this.parent = parent;
            transform.position = new Vector3(parent.position.x, 150.0f, parent.position.z); // 2025-04-30 RJH 드론 높이 수정
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(fObjectScale, fObjectScale, fObjectScale);
            UTILS.Log($"SetDrone : {transform.position}");
            droneNavMeshAgent.NavMeshReset(); // 2025-04-30 RJH 드론 높이 수정 후 NavMeshAgent 다시실행
        }

        public void SetTarget(Vector3 pos)
        {
            droneNavMeshAgent.target = new Vector3(pos.x, 150.0f, pos.z); // 2025-04-30 RJH 타겟 높이 수정
            UTILS.Log($"SetTarget : {transform.position}");

        }

        public void SetNetWorkDroneName(string droneName)
        {
            pv.RPC("SetNetWorkDroneNameRPC", RpcTarget.AllBuffered, $"{droneName}");
        }

        [PunRPC]
        public void SetNetWorkDroneNameRPC(string name)
        {
            gameObject.name = name;
        }

        private void OnDestroy()
        {
            //if(PhotonNetwork.IsConnected)
            //{
            //    PhotonNetwork.Destroy(parent.gameObject);
            //}
        }
    }
}