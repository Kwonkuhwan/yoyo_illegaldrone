using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class LoginCanvas_PC : MonoBehaviour
    {
        public List<GameObject> panels;
        
        private void Awake()
        {
            foreach (GameObject obj in panels)
            {
                obj.SetActive(false);
            }

            panels[0].SetActive(true);
        }
    }
}
