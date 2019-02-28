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
        private IEnumerable<Player> playerList;

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
				GC.Collect();
                Destroy(hGameObj);
                Destroy(this);
				Resources.UnloadUnusedAssets();

			}
            catch
            {

            }
        }

        private void Update()
        {
			try
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
			catch
			{

			}
        }

        private void UpdateLocalPlayer()
        {
            playerList = FindObjectsOfType<Player>();
        }


    }
}
