using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCore : MonoBehaviour {

	public static GameCore Instance {
		get;
		private set;
	}

	public static bool IsMatchStarted {
		get {
			return isMatchStarted;
		}
	}

	public static int CurrentPlayersCount {
		get {
			return currentPlayers.Count;
		}
	}

	public static PlayersCollection CurrentPlayers {
		get {
			return new PlayersCollection(currentPlayers);
		}
	}

	public static bool MatchIsFake {
		get;
		private set;
	}

	private static Dictionary<string, PlayerInfo> currentPlayers = new Dictionary<string, PlayerInfo>();
	private static bool isMatchStarted = false;

	private static Dictionary<string, int> jumpsDebugCounter = new Dictionary<string, int>();

	public static int GetJumpsDebugForPlayerGuid(string playerGuid) {
		return jumpsDebugCounter.ContainsKey(playerGuid) ? jumpsDebugCounter[playerGuid] : 0;
	}

	[RuntimeInitializeOnLoadMethod]
	public static void Initialize() {
		if (!Application.isPlaying) {
			return;
		}

		if (Instance != null) {
			return;
		}

		DebugConsole.Initialize();

#if !UNITY_IPHONE && !UNITY_ANDROID
		Debug.LogFormat("Set screen resolution: {0}x{1} fullscreen: {2}", Config.ScreenWidth, Config.ScreenHeight, Config.Fullscreen);
		Screen.SetResolution(Config.ScreenWidth, Config.ScreenHeight, Config.Fullscreen, 60);
#endif

		GameObject go = new GameObject("GameCore");
		go.AddComponent<GameCore>();
	}

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		
		Debug.Log("Initialize game core");

		NetworkHandler.Create();

		NetworkHandler.OnReceiveMatch += NetworkHandler_OnReceiveMatch;
		NetworkHandler.OnReceiveJump += NetworkHandler_OnReceiveJump;
	}

	private void Update() {

		if (Config.DebugMode) {
			PlayerInfo[] players = new PlayerInfo[currentPlayers.Count];
			currentPlayers.Values.CopyTo(players, 0);

			if (Input.GetMouseButtonDown(0) && players.Length > 0) {
				NetworkHandler_OnReceiveJump(players[0].Guid);
			}

			if (Input.GetMouseButtonDown(1) && players.Length > 1) {
				NetworkHandler_OnReceiveJump(players[1].Guid);
			}

			if (Input.GetMouseButtonDown(2) && players.Length > 2) {
				NetworkHandler_OnReceiveJump(players[2].Guid);
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
#else
			Game.IsPaused = !Game.IsPaused;
#endif
		}
	}

	private static void NetworkHandler_OnReceiveMatch(System.Collections.Generic.Dictionary<string, NetworkHandler.ServerUserInfo> users) {

		MatchIsFake = false;

		if (isMatchStarted) {
			FinishMatch(false);
		}

		Application.LoadLevel("1_game");

		currentPlayers.Clear();

		Debug.Log("Received match:");
		foreach (var i in users) {
			PlayerInfo playerInfo = new PlayerInfo(i.Key, i.Value.character, i.Value.name);
			currentPlayers[i.Key] = playerInfo;

			Debug.LogFormat("user: {0}, character: {1}, name: {2}", playerInfo.Guid, playerInfo.CharacterId, playerInfo.PlayerName);
		}

		isMatchStarted = true;
	}

	private static void NetworkHandler_OnReceiveJump(string userGuid) {
		if (Config.DebugMode) {
			if (jumpsDebugCounter.ContainsKey(userGuid)) {
				jumpsDebugCounter[userGuid] += 1;
			} else {
				jumpsDebugCounter[userGuid] = 1;
			}
		}

		if (Time.timeScale == 0f) {
			return; // paused
		}

		Debug.LogFormat("Received jump of user {0}", userGuid);

		PlayerInfo playerInfo = null;
		if (currentPlayers.TryGetValue(userGuid, out playerInfo)) {
			playerInfo.Jump();
		}
	}

	public static void PlayerFinished(PlayerInfo playerInfo) {

		Debug.LogFormat("PLAYER FINISHED: {0}", playerInfo.Guid);

		NetworkHandler.SendUserFinished(playerInfo.Guid, playerInfo.TotalScores);

		int activeCount = 0;

		foreach (var i in currentPlayers) {
			if (!i.Value.IsFinished) {
				activeCount++;
			}
		}

		if (activeCount < 1) {
			FinishMatch(true);
		}
	}

	public static void FinishMatch(bool sendMessageToServer) {

		if (!isMatchStarted) {
			return;
		}

		Debug.Log("FINISH MATCH");

		isMatchStarted = false;

		Instance.StartCoroutine(InvokeAfterDelay(1f, () => {
			Application.LoadLevel("2_result");

			if (sendMessageToServer) {
				if (Instance != null && Instance.gameObject.activeSelf) {
					Instance.StartCoroutine(InvokeAfterDelay(Config.ResultsShowTime, () => {

						NetworkHandler.SendFinishMatch();

						Instance.StartCoroutine(InvokeAfterDelay(Config.ResultsShowTime, () => {
							if (!isMatchStarted) {
								WaitNewMatch();
							}
						}));
					}));
				} else {
					NetworkHandler.SendFinishMatch();
				}
			}
		}));
	}

	public static void WaitNewMatch() {

		Debug.Log("WAIT NEW MATCH");

		Application.LoadLevel("0_start");

	}

	public static void StartFakeGame() {
		NetworkHandler_OnReceiveMatch(new Dictionary<string, NetworkHandler.ServerUserInfo>() {
			{"fake_guid_1", new NetworkHandler.ServerUserInfo() {
				character = "1",
				name = "Тест1",
			}},
			{"fake_guid_2", new NetworkHandler.ServerUserInfo() {
				character = "2",
				name = "Тест2",
			}},
			{"fake_guid_3", new NetworkHandler.ServerUserInfo() {
				character = Random.Range(3, 5).ToString(),
				name = "Тест3",
			}},
		});

		MatchIsFake = true;
	}

	private static IEnumerator InvokeAfterDelay(float delay, System.Action action) {
		while (delay > 0f) {
			delay -= Time.deltaTime;
			yield return null;
		}

		action();
	}
}
