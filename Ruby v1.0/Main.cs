using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFT;
using UnityEngine;

namespace Ruby
{
    public class Main : MonoBehaviour
    {
        private GameObject hGameObj;
        private Player[] playerList;

        private float nextUpdateTime;
        private float updateInterval = 15f;

        private bool bESP = false;
        
        private Player localPlayer;
        private Vector3 camPos;

        private void Awake()
        {
		
        }

        private void Start()
        {
            Clear();
        }

        private void Clear()
        {
            playerList = null;
            localPlayer = null;
            nextUpdateTime = 0f;
        }

        private void OnEnable()
        {
            Clear();
        }

        private void OnDisable()
        {
            Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Load()
        {
            hGameObj = new GameObject();
            hGameObj.AddComponent<Main>();
            DontDestroyOnLoad(hGameObj);
        }

        public void Unload()
        {
            try
            {
                Clear();
                Destroy(hGameObj);
                Destroy(hGameObj.GetComponent<Main>());
                Destroy(this);
                DestroyObject(this);
            }
            catch
            {

            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                Unload();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                bESP = !bESP;
            }
        }

        private void UpdateLocalPlayer()
        {
            playerList = FindObjectsOfType<Player>();
        }


    }
}
