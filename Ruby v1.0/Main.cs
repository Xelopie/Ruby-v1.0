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

		//private GameWorld activeGameWorld;
		//private float gameWorldUpdateInterval = 10f;
		//private float gameWorldNextUpdate;

		private IEnumerable<Player> playerList;
		private IEnumerable<LootItem> itemList;
		private IEnumerable<LootableContainer> containerList;

		private Vector3 localPlayerCamPos;
		private Player localPlayer;

		private float playerUpdateInterval = 5f;
		private float playerNextUpdate;
		private float itemUpdateInterval = 30f;
		private float itemNextUpdate;
		private float containerUpdateInterval = 60f;
		private float containerNextUpdate;

		private float localPlayerUpdateInterval = 180f;
		private float localPlayerNextUpdate;

		private float playerDistance = 300f;
		private float itemDistance = 300f;
		private float containerDistance = 300f;

		private float keyPressInterval = 0.5f;
		private float keyPressUpdate = 0f;

		private Vector2 scrollPosition = Vector2.zero;

		#region Checkboxes
		// Main switch
		private bool bRuby = false;
		// Menu
		private bool bMenu = false;
		// Main function
		private bool bPlayer = false;
		private bool bItem = false;
		private bool bContainer = false;
		// Advanced options
		private bool bShowCorpse = false;
		private bool bItemFilter = false;
		// Item catagory
		private bool bItem_Barter = false;
		private bool bItem_Loot = false;
		private bool bItem_LabKey = false;
		private bool bItem_Valuable = false;
		private bool bItem_Key = false;
		private bool bItem_Stimulator = false;
		private bool bItem_Silencer = false;
		private bool bItem_Scope = false;
		private bool bItem_Money = false;
		// Item textfield
		private bool bStrItem1 = false;
		private bool bStrItem2 = false;
		private bool bStrItem3 = false;
		#endregion

		// Textfield
		private string strItem1 = "itemNameHere";
		private string strItem2 = "itemNameHere";
		private string strItem3 = "itemNameHere";

		#endregion

		private void Awake()
		{
			Debug.logger.logEnabled = false;
		}

		private void Start()
		{
			Clear();
		}

		private void Clear()
		{
			playerList = null;
			playerNextUpdate = 0f;
			localPlayerNextUpdate = 0f;
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
			GC.Collect();
			Destroy(hGameObj);
			Resources.UnloadUnusedAssets();
			Destroy(this);

		}

		private void OnDisable()
		{
			Clear();
		}

		private void OnDestroy()
		{
			Clear();
		}

		private void Update()
		{
			//if (Time.time >= gameWorldNextUpdate)
			//{
			//	activeGameWorld = FindObjectOfType<GameWorld>();
			//}

			if (Input.GetKeyDown(KeyCode.End))
			{
				Unload();
			}

			if (Input.GetKeyDown(KeyCode.F10) && Time.time >= keyPressUpdate)
			{
				keyPressUpdate = Time.time + keyPressInterval;
				bMenu = !bMenu;
			}

			// Prevent the cheat from running before the game starts
			//if (!activeGameWorld) return;
			if (!bRuby) return;

			try
			{
				if (Time.time >= localPlayerNextUpdate)
				{
					localPlayerNextUpdate = Time.time + localPlayerUpdateInterval;
					UpdateLocalPlayer();
				}

				if (Camera.main.transform.position != null)
				{
					localPlayerCamPos = Camera.main.transform.position;
				}
			}
			catch { }


		}

		private void OnGUI()
		{
			if (bMenu)
			{
				GUIOverlay();
			}

			//if (!activeGameWorld) return;
			if (!bRuby) return;
			try
			{
				if (bPlayer)
				{
					if (Time.time >= playerNextUpdate)
					{
						playerNextUpdate = Time.time + playerUpdateInterval;
						playerList = FindObjectsOfType<Player>();
					}
					DrawPlayer();
				}

				if (bContainer)
				{
					if (Time.time >= containerNextUpdate)
					{
						containerNextUpdate = Time.time + containerUpdateInterval;
						containerList = FindObjectsOfType<LootableContainer>();
					}
					DrawContainer();
				}

				if (bItem)
				{
					if (Time.time >= itemNextUpdate)
					{
						itemList = FindObjectsOfType<LootItem>();
						itemNextUpdate = Time.time + itemUpdateInterval;
					}
					DrawItem();
				}
			}
			catch { }
		}

		private void UpdateLocalPlayer()
		{
			foreach (Player player in FindObjectsOfType<Player>())
			{
				if (player == null) continue;

				if (EPointOfView.FirstPerson == player.PointOfView)
				{
					localPlayer = player;
				}
			}
		}

		private void GUIOverlay()
		{
			GUI.color = Color.gray;
			GUI.Box(new Rect(100f, 100f, 400f, 500f), "");
			GUI.color = Color.white;
			GUI.Label(new Rect(110f, 110f, 150f, 20f), "Settings: ");

			bRuby = GUI.Toggle(new Rect(110f, 150f, 150f, 20f), bRuby, "SWITCH ON/OFF");

			bPlayer = GUI.Toggle(new Rect(110f, 220f, 120f, 20f), bPlayer, "Show Players");
			if (bPlayer)
			{
				GUI.Label(new Rect(110f, 240f, 150f, 20f), "Players Distance");
				playerDistance = GUI.HorizontalSlider(new Rect(280f, 245f, 120f, 20f), playerDistance, 0.0F, 1500.0F);
				GUI.Label(new Rect(405, 240, 50, 20), playerDistance.ToString());

				bShowCorpse = GUI.Toggle(new Rect(250, 220, 120, 20), bShowCorpse, "Show Corpses");
			}

			bItem = GUI.Toggle(new Rect(110f, 280f, 120f, 20f), bItem, "Show Items");
			if (bItem)
			{
				GUI.Label(new Rect(110f, 300f, 150f, 20f), "Items Distance");
				itemDistance = GUI.HorizontalSlider(new Rect(280f, 305f, 120f, 20f), itemDistance, 0.0F, 1500.0F);
				GUI.Label(new Rect(405, 300, 50, 20), itemDistance.ToString());

				bItemFilter = GUI.Toggle(new Rect(250, 280, 120, 20), bItemFilter, "Item Filter");
				if (bItemFilter)
				{
					GUI.color = Color.gray;
					GUI.Box(new Rect(500, 100, 200, 300), "");
					GUI.color = Color.white;

					// An absolute-positioned example: We make a scrollview that has a really large client
					// rect and put it in a small rect on the screen.
					scrollPosition = GUI.BeginScrollView(new Rect(500, 100, 200, 300), scrollPosition, new Rect(0, 0, 200, 300), false, false);

					// Make some conditions
					bItem_Barter = GUI.Toggle(new Rect(10, 10, 120, 20), bItem_Barter, "Barter");
					bItem_Loot = GUI.Toggle(new Rect(10, 30, 120, 20), bItem_Loot, "Loot");
					bItem_LabKey = GUI.Toggle(new Rect(10, 50, 120, 20), bItem_LabKey, "Lab Key");
					bItem_Valuable = GUI.Toggle(new Rect(10, 70, 120, 20), bItem_Valuable, "Valuable");
					bItem_Key = GUI.Toggle(new Rect(10, 90, 120, 20), bItem_Key, "Key");
					bItem_Stimulator = GUI.Toggle(new Rect(10, 110, 120, 20), bItem_Stimulator, "Stimulator");
					bItem_Silencer = GUI.Toggle(new Rect(10, 130, 120, 20), bItem_Silencer, "Silencer");
					bItem_Scope = GUI.Toggle(new Rect(10, 150, 120, 20), bItem_Scope, "Scope");
					bItem_Money = GUI.Toggle(new Rect(10, 170, 120, 20), bItem_Money, "Money");

					// End the scroll view that we began above.
					GUI.EndScrollView();

					GUI.color = Color.gray;
					GUI.Box(new Rect(700, 100, 200, 300), "");
					GUI.color = Color.white;

					// An absolute-positioned example: We make a scrollview that has a really large client
					// rect and put it in a small rect on the screen.
					scrollPosition = GUI.BeginScrollView(new Rect(700, 100, 200, 300), scrollPosition, new Rect(500, 0, 200, 300), false, false);

					// Make text fields that modify conditions
					bStrItem1 = GUI.Toggle(new Rect(510, 10, 10, 20), bStrItem1, "");
					strItem1 = GUI.TextField(new Rect(530, 10, 150, 20), strItem1, 25);
					bStrItem2 = GUI.Toggle(new Rect(510, 30, 10, 20), bStrItem2, "");
					strItem2 = GUI.TextField(new Rect(530, 30, 150, 20), strItem2, 25);
					bStrItem3 = GUI.Toggle(new Rect(510, 50, 10, 20), bStrItem3, "");
					strItem3 = GUI.TextField(new Rect(530, 50, 150, 20), strItem3, 25);

					// End the scroll view that we began above.
					GUI.EndScrollView();
				}
			}

			bContainer = GUI.Toggle(new Rect(110f, 340f, 120f, 20f), bContainer, "Show Containers");
			if (bContainer)
			{
				GUI.Label(new Rect(110f, 360f, 150f, 20f), "Containers Distance");
				containerDistance = GUI.HorizontalSlider(new Rect(280f, 365f, 120f, 20f), containerDistance, 0.0F, 1500.0F);
				GUI.Label(new Rect(405, 360, 50, 20), containerDistance.ToString());
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

		#region Drawing Functions
		private void DrawPlayer()
		{
			foreach (var player in playerList)
			{
				// If the player is invalid
				if (player == null || player == localPlayer || !player.IsVisible || player.Profile.Info.Nickname == string.Empty) continue;

				// Get the player's position
				Vector3 playerPos = player.Transform.position;

				// Calculate the distance between player and localPlayer
				float distance = Vector3.Distance(localPlayerCamPos, playerPos);

				// 3D -> 2D
				Vector3 playerMainCoor = Camera.main.WorldToScreenPoint(playerPos);

				if (distance <= playerDistance && playerMainCoor.z > 0.01)
				{
					Vector3 playerHeadCoor = Camera.main.WorldToScreenPoint(player.PlayerBones.Head.position);

					float boxVectorX = playerMainCoor.x;
					float boxVectorY = playerHeadCoor.y + 10f;
					float boxHeight = Math.Abs(playerHeadCoor.y - playerMainCoor.y) + 10f;
					float boxWidth = boxHeight * 0.65f;

					if (player.HealthController.IsAlive)
					{
						var playerColor = GetPlayerColor(player.Side);
						Utility.DrawBox(boxVectorX - boxWidth / 2f, Screen.height - boxVectorY, boxWidth, boxHeight, playerColor);
						Utility.DrawLine(new Vector2(playerHeadCoor.x - 5f, Screen.height - playerHeadCoor.y - 5f), new Vector2(playerHeadCoor.x + 5f, Screen.height - playerHeadCoor.y + 5f), Color.green);
						Utility.DrawLine(new Vector2(playerHeadCoor.x + 5f, Screen.height - playerHeadCoor.y - 5f), new Vector2(playerHeadCoor.x - 5f, Screen.height - playerHeadCoor.y + 5f), Color.green);

					}
					else
					{
						// If ShowCorpse is OFF
						if (!bShowCorpse) continue;

						var playerColor = Color.gray;
						Utility.DrawBox(playerMainCoor.x - 10f, Screen.height - playerMainCoor.y, 20f, 5f, playerColor);
					}

					var isAI = player.Profile.Info.RegistrationDate <= 0;
					var playerName = isAI ? "AI" : player.Profile.Info.Nickname;
					string playerText = player.HealthController.IsAlive ? playerName : (playerName + " (DEAD)");
					string playerTextDraw = string.Format("{0} [{1}]", playerText, (int)distance);
					var playerTextVector = GUI.skin.GetStyle(playerText).CalcSize(new GUIContent(playerText));
					GUI.Label(new Rect(playerMainCoor.x - playerTextVector.x / 2f, Screen.height - boxVectorY - 20f, 300f, 50f), playerTextDraw);

				}
			}
		}

		private void DrawContainer()
		{
			foreach (var contain in containerList)
			{
				if (contain != null)
				{
					float distance = Vector3.Distance(localPlayerCamPos, contain.transform.position);
					var containerCoor = Camera.main.WorldToScreenPoint(contain.transform.position);
					if (containerCoor.z > 0.01 && distance <= containerDistance)
					{
						GUI.color = Color.cyan;
						string boxText = $"{contain.name} - [{(int)distance}]m";
						GUI.Label(new Rect(containerCoor.x - 50f, (float)Screen.height - containerCoor.y, 100f, 50f), boxText);
					}
				}
			}
		}

		private void DrawItem()
		{
			foreach (var item in itemList)
			{
				if (item == null) continue;

				bool bShowItem = false;

				/* Item Filter */
				if (bItemFilter)
				{
					if (bItem_Barter)
						if (item.name.Contains("barter"))
							bShowItem = true;
					if (bItem_Loot)
						if (item.name.Contains("loot"))
							bShowItem = true;
					if (bItem_LabKey)
						if (item.name.Contains("keycard"))
							bShowItem = true;
					if (bItem_Valuable)
						if (item.name.Contains("valuable"))
							bShowItem = true;
					if (bItem_Key)
						if (item.name.Contains("key"))
							bShowItem = true;
					if (bItem_Stimulator)
						if (item.name.Contains("stimulator"))
							bShowItem = true;
					if (bItem_Silencer)
						if (item.name.Contains("silencer"))
							bShowItem = true;
					if (bItem_Scope)
						if (item.name.Contains("scope"))
							bShowItem = true;
					if (bItem_Money)
						if (item.name.Contains("money"))
							bShowItem = true;
					
					if (bStrItem1)
						if (item.name.Contains(strItem1))
							bShowItem = true;
					if (bStrItem2)
						if (item.name.Contains(strItem2))
							bShowItem = true;
					if (bStrItem3)
						if (item.name.Contains(strItem3))
							bShowItem = true;

					if (!bShowItem) continue;
				}
				

				float distance = Vector3.Distance(localPlayerCamPos, item.transform.position);
				Vector3 itemCoor = Camera.main.WorldToScreenPoint(item.transform.position);

				if (itemCoor.z > 0.01 && distance <= itemDistance)
				{
					string text = $"{item.name} - [{(int)distance}]m";
					GUI.color = Color.magenta;
					GUI.Label(new Rect(itemCoor.x - 50f, Screen.height - itemCoor.y, 100f, 50f), text);
				}
			}
		}
		#endregion
	}
}
