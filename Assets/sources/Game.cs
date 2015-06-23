using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	[SerializeField]
	private LevelGenerator levelGeneratorPrefab = null;
	[SerializeField]
	private Player playerPrefab = null;

	public static Vector3 ScreenSizeWorld {
		get {
			return screenSizeWorld;
		}
	}

	public delegate void OnPauseDelegate(bool isPaused);
	public static event OnPauseDelegate OnPause;
	public static bool IsPaused {
		get {
			return isPaused;
		}
		set {
			Time.timeScale = (isPaused = value) ? 0f : 1f;
			if (OnPause != null) {
				OnPause(isPaused);
			}
		}
	}

	public static float StartTimer {
		get;
		private set;
	}

	public static float LevelSpeed {
		get;
		private set;
	}

	private static bool isPaused = false;
	private static Vector3 screenSizeWorld = Vector3.zero;

	private LevelGenerator levelGenerator = null;
	private Dictionary<string, Player> players = new Dictionary<string, Player>();

	private float levelSpeedAccelerationTime = 0f;


	private void Awake() {
		GameCore.Initialize();

		screenSizeWorld = Camera.main.ViewportToWorldPoint(Vector3.one);

		IsPaused = false;
		StartTimer = Config.StartGameTimer + 1f;

		LevelSpeed = Config.LevelSpeed;

		levelGenerator = Instantiate(levelGeneratorPrefab);
		levelGenerator.OnMoveObstacle += OnMoveObstacle;

		if (!GameCore.IsMatchStarted) {
			GameCore.StartFakeGame();
		}

		float xOffset = -screenSizeWorld.x + 2f;
		foreach (var i in GameCore.CurrentPlayers) {
			Player player = Instantiate(playerPrefab, new Vector3(xOffset, 0f, 0f), Quaternion.identity) as Player;
			player.OnFinish += player_OnFinish;
			player.FetchPlayerInfo(i);
			xOffset += 3f;

			players.Add(i.Guid, player);
		}
	}

	private void Update() {
		StartTimer -= Time.deltaTime;

		levelSpeedAccelerationTime += Time.deltaTime;
		if (levelSpeedAccelerationTime >= Config.LevelSpeedAccelerationTimeInterval) {
			levelSpeedAccelerationTime = 0f;
			LevelSpeed += Config.LevelSpeedAcceleration;
		}
	}

	private void player_OnFinish(Player player) {
		players.Remove(player.SelfPlayerInfo.Guid);
	}

	private void OnMoveObstacle(Obstacle obstacle) {
		foreach (var i in players) {
			Player player = i.Value;

			if (player == null) {
				continue;
			}

			if (player.transform.position.x > obstacle.transform.position.x) {
				player.DoneObstacle(obstacle);
			}
		}
	}
}
