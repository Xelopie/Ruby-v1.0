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
		#region Variables
		private GameObject hGameObj;

        private IEnumerable<Player> playerList;
		private IEnumerable<LootItem> itemList;
		private IEnumerable<LootableContainer> containerList;

		private Vector3 localPlayerCam;
		private Player localPlayer;

		private bool bMenu = false;
		private bool bPlayer = false;
		private bool bItem = false;
		private bool bContainer = false;

		private float playerDistance = 300f;
		private float itemDistance = 300f;
		private float containerDistance = 300f;

		private float keyPressInterval = 0.5f;
		private float keyPressUpdate = 0f;
		#endregion

		private void Awake()
		{
			// Change the 4-digit chars
		}

		private void Clear()
		{
			playerList = null;
			itemList = null;
			containerList = null;

			localPlayerCam = new Vector3();
			localPlayer = null;

			bMenu = false;
			bPlayer = false;
			bItem = false;
			bContainer = false;

			playerDistance = 0f;
			itemDistance = 0f;
			containerDistance = 0f;
		}

		public void Load()
        {
            hGameObj = new GameObject();
            hGameObj.AddComponent<Main>();
            DontDestroyOnLoad(hGameObj);
        }

		public void Unload()
        {
            Clear();
            Destroy(hGameObj);
            Destroy(this);
        }

        private void Update()
        {
			if (Input.GetKeyDown(KeyCode.End) && Time.time >= keyPressUpdate)
			{
				Unload();
				keyPressUpdate = Time.time + keyPressInterval;
			}
			if (Input.GetKeyDown(KeyCode.F10) && Time.time >= keyPressUpdate)
			{
				bMenu = !bMenu;
				keyPressUpdate = Time.time + keyPressInterval;
			}
        }

		private void OnGUI()
		{
			if (bMenu)
			{
				GUIOverlay();
			}
		}

		private void GUIOverlay()
		{
			GUI.color = Color.gray;
			GUI.Box(new Rect(100f, 100f, 400f, 500f), "");
			GUI.color = Color.white;
			GUI.Label(new Rect(180f, 110f, 150f, 20f), "Settings");

			bPlayer = GUI.Toggle(new Rect(110f, 340f, 120f, 20f), bPlayer, "Show Players"); // Show containers on map
			if (bPlayer)
			{
				GUI.Label(new Rect(110f, 360f, 150f, 20f), "Players Distance");
				playerDistance = GUI.HorizontalSlider(new Rect(210f, 360f, 120f, 20f), playerDistance, 0.0F, 1500.0F);
			}

			bItem = GUI.Toggle(new Rect(110f, 280f, 120f, 20f), bItem, "Show Items"); //Show items on map
			if (bItem)
			{
				GUI.Label(new Rect(110f, 320f, 150f, 20f), "Items Distance");
				itemDistance = GUI.HorizontalSlider(new Rect(210f, 320f, 120f, 20f), itemDistance, 0.0F, 1500.0F);
			}

			bContainer = GUI.Toggle(new Rect(110f, 340f, 120f, 20f), bContainer, "Show Containers"); // Show containers on map
			if (bContainer)
			{
				GUI.Label(new Rect(110f, 360f, 150f, 20f), "Containers Distance");
				containerDistance = GUI.HorizontalSlider(new Rect(210f, 360f, 120f, 20f), containerDistance, 0.0F, 1500.0F);
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

		private void DrawPlayers()
		{
			foreach (var player in playerList)
			{

				if (player == null || player == localPlayer|| !player.IsVisible || player.Profile.Info.Nickname == string.Empty) continue;
				Vector3 playerPos = player.Transform.position;
				float distanceToObject = Vector3.Distance(localPlayerCam, player.Transform.position);
				Vector3 playerBoundingVector = Camera.main.WorldToScreenPoint(playerPos);
				if (distanceToObject <= playerDistance && playerBoundingVector.z > 0.01)
				{
					Vector3 playerHeadVector = Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position);
					Gizmos.DrawCube(playerPos, new Vector3(1, 1, 2));
					float boxVectorX = playerBoundingVector.x;
					float boxVectorY = playerHeadVector.y + 10f;
					float boxHeight = Math.Abs(playerHeadVector.y - playerBoundingVector.y) + 10f;
					float boxWidth = boxHeight * 0.65f;
					var IsAI = player.Profile.Info.RegistrationDate <= 0;
					var playerColor = player.HealthController.IsAlive ? GetPlayerColor(player.Side) : Color.gray;
					Utility.DrawBox(boxVectorX - boxWidth / 2f, Screen.height - boxVectorY, boxWidth, boxHeight, playerColor);
					Utility.DrawLine(new Vector2(playerHeadVector.x - 2f, Screen.height - playerHeadVector.y), new Vector2(playerHeadVector.x + 2f, Screen.height - playerHeadVector.y), playerColor);
					Utility.DrawLine(new Vector2(playerHeadVector.x, Screen.height - playerHeadVector.y - 2f), new Vector2(playerHeadVector.x, Screen.height - playerHeadVector.y + 2f), playerColor);
					var playerName = IsAI ? "AI" : player.Profile.Info.Nickname;
					string playerText = player.HealthController.IsAlive ? playerName : (playerName + " (Dead)");
					string playerTextDraw = string.Format("{0} [{1}]", playerText, (int)distanceToObject);
					var playerTextVector = GUI.skin.GetStyle(playerText).CalcSize(new GUIContent(playerText));
					GUI.Label(new Rect(playerBoundingVector.x - playerTextVector.x / 2f, Screen.height - boxVectorY - 20f, 300f, 50f), playerTextDraw);
				}
			}

		}
	}
}
