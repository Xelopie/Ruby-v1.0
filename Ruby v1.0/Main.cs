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

		private float nextKeyPressAvail;
		private float keyPressCoolDown = 180f;

		private bool bMenu = true;
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

		private void GUIOverlay()
		{
			GUI.color = Color.gray;
			GUI.Box(new Rect(100f, 100f, 400f, 500f), "");
			GUI.color = Color.white;
			GUI.Label(new Rect(180f, 110f, 150f, 20f), "Settings");
			bESP = GUI.Toggle(new Rect(110f, 140f, 120f, 20f), bESP, "Players ESP"); // Display player
		}

		private void OnGUI()
		{
			if (bESP)
			{
				ESP();
			}

			if (bMenu)
			{
				GUIOverlay();
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

				if (Input.GetKeyDown(KeyCode.F11))
				{
					bMenu = !bMenu;
					nextKeyPressAvail = Time.time + keyPressCoolDown;
				}

				if (Input.GetKeyDown(KeyCode.F1) && Time.time >= nextKeyPressAvail)
				{
					bESP = !bESP;
					nextKeyPressAvail = Time.time + keyPressCoolDown;
				}

				if (Time.time >= nextUpdateTime)
				{
					GetLocalPlayer();
					GetPlayerList();
					nextUpdateTime = Time.time + updateInterval;
					camPos = Camera.main.transform.position;
				}

				if (bESP)
				{
					ESP();
				}
			}
			catch
			{

			}

        }

        private void GetPlayerList()
        {
            playerList = FindObjectsOfType<Player>();
        }

		private void GetLocalPlayer()
		{
			try
			{
				foreach (Player player in FindObjectsOfType<Player>())
				{
					if (player == null) continue;

					if (EPointOfView.FirstPerson == player.PointOfView && player != null)
					{
						localPlayer = player;
					}
				}
			}
			catch
			{

			}
		}

		private Color GetPlayerColor(EPlayerSide side)
		{
			switch (side)
			{
				case EPlayerSide.Bear:
					return Color.red;
				case EPlayerSide.Usec:
					return Color.blue;
				case EPlayerSide.Savage:
					return Color.white;
				default:
					return Color.white;

			}
		}

		private void ESP()
		{
			foreach (Player player in playerList)
			{
				try
				{
					if (player == null || player.Profile.Info.Nickname == string.Empty) continue;

					Vector3 playerPos = player.Transform.position;
					Vector3 playerCoor = Camera.main.WorldToScreenPoint(playerPos);

					float distanceToObject = Vector3.Distance(camPos, playerPos);

					if (playerCoor.z > 0.01)
					{
						Vector3 playerHeadVector = Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position);
						Gizmos.DrawCube(playerPos, new Vector3(1, 1, 2));

						float boxVectorX = playerCoor.x;
						float boxVectorY = playerHeadVector.y + 10f;
						float boxHeight = Math.Abs(playerHeadVector.y - playerCoor.y) + 10f;
						float boxWidth = boxHeight * 0.65f;
						bool isAI = (player.Profile.Info.RegistrationDate <= 0);
						var playerColor = player.HealthController.IsAlive ? GetPlayerColor(player.Side) : Color.gray;
						Utility.DrawBox(boxVectorX - boxWidth / 2f, Screen.height - boxVectorY, boxWidth, boxHeight, playerColor);
						Utility.DrawLine(new Vector2(playerHeadVector.x - 2f, Screen.height - playerHeadVector.y), new Vector2(playerHeadVector.x + 2f, Screen.height - playerHeadVector.y), playerColor);
						Utility.DrawLine(new Vector2(playerHeadVector.x, Screen.height - playerHeadVector.y - 2f), new Vector2(playerHeadVector.x, Screen.height - playerHeadVector.y + 2f), playerColor);
						var playerName = isAI ? "AI" : player.Profile.Info.Nickname;
						string playerText = player.HealthController.IsAlive ? playerName : (playerName + " (Dead)");
						string playerTextDraw = string.Format("{0} [{1}]", playerText, (int)distanceToObject);
						var playerTextVector = GUI.skin.GetStyle(playerText).CalcSize(new GUIContent(playerText));
						GUI.Label(new Rect(playerCoor.x - playerTextVector.x / 2f, Screen.height - boxVectorY - 20f, 300f, 50f), playerTextDraw);
					}
				}
				catch
				{
					
				}
			}

		}


	}
}
