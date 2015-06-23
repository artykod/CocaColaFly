using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class Config {

	private const string FILE_NAME = "config.json";


	private const string KEY_HOST_NAME = "hostName";
	private const string KEY_HOST_PORT = "hostPort";
	private const string KEY_RESULTS_SHOW_TIME = "resultsShowTime";
	private const string KEY_SCREEN_WIDTH = "screenWidth";
	private const string KEY_SCREEN_HEIGHT = "screenHeight";
	private const string KEY_FULLSCREEN = "fullscreen";
	private const string KEY_GRAVITY = "gravity";
	private const string KEY_JUMP = "jump";
	private const string KEY_LEVEL_SPEED = "levelSpeed";
	private const string KEY_LEVEL_SPEED_ACCELERATION = "levelSpeedAcceleration";
	private const string KEY_LEVEL_SPEED_ACCELERATION_TIME_INTERVAL = "levelSpeedAccelerationTimeInterval";
	private const string KEY_SCORE_ADD_INTERVAL_SECONDS = "scoreAddIntervalSeconds";
	private const string KEY_OBSTACLES_DISTANCE = "obstaclesDistance";
	private const string KEY_OBSTACLES_HEIGHT_MIN = "obstaclesHeightMin";
	private const string KEY_OBSTACLES_HEIGHT_MAX = "obstaclesHeightMax";
	private const string KEY_MAX_PLAYER_SCORE = "maxPlayerScore";
	private const string KEY_IMMORTAL_TIME_AFTER_GROUND_DEATH = "immortalTimeAfterGroundDeath";
	private const string KEY_DEBUG_MODE = "debug";
	private const string KEY_WIFI_NAME = "wifiName";
	private const string KEY_START_GAME_TIMER = "startGameTimer";


	public static string HostName { get; private set; }
	public static int HostPort { get; private set; }
	public static float ResultsShowTime { get; private set; }
	public static int ScreenWidth { get; private set; }
	public static int ScreenHeight { get; private set; }
	public static bool Fullscreen { get; private set; }
	public static float Gravity { get; private set; }
	public static float Jump { get; private set; }
	public static float LevelSpeed { get; private set; }
	public static float LevelSpeedAcceleration { get; private set; }
	public static float LevelSpeedAccelerationTimeInterval { get; private set; }
	public static float ScoreAddIntervalSeconds { get; private set; }
	public static float ObstaclesDistance { get; private set; }
	public static float ObstaclesHeightMin { get; private set; }
	public static float ObstaclesHeightMax { get; private set; }
	public static int MaxPlayerScore { get; private set; }
	public static float ImmortalTimeAfterGroundDeath { get; private set; }
	public static bool DebugMode { get; private set; }
	public static string WiFiName { get; private set; }
	public static float StartGameTimer { get; private set; }


	static Config() {
		DebugMode = true;

		if (Application.isPlaying) {
			Load();
		}
	}


	public static void Load() {

		HostName = "localhost";
		HostPort = 5050;
		ResultsShowTime = 5f;
		ScreenWidth = 1920;
		ScreenHeight = 1080;
		Fullscreen = false;

		Gravity = -18f;
		Jump = 8f;
		LevelSpeed = 4f;
		LevelSpeedAcceleration = 0f;
		LevelSpeedAccelerationTimeInterval = 5f;
		ScoreAddIntervalSeconds = 10f;

		ObstaclesDistance = 5f;
		ObstaclesHeightMin = -3f;
		ObstaclesHeightMax = 3f;

		MaxPlayerScore = 9;
		ImmortalTimeAfterGroundDeath = 3f;

		DebugMode = true;

		WiFiName = "Название сети";

		StartGameTimer = 4f;

		try {
			string configJson = "";

#if !UNITY_IPHONE && !UNITY_ANDROID
			string configPath = Application.dataPath + "/resources/";
#if !UNITY_EDITOR
			configPath += "../../";
#endif
			Debug.Log("Config path: " + configPath + FILE_NAME);
			using (StreamReader sr = new StreamReader(configPath  + FILE_NAME)) {
				configJson = sr.ReadToEnd();
			}
#else
			TextAsset res = Resources.Load<TextAsset>(System.IO.Path.GetFileNameWithoutExtension(FILE_NAME));
			configJson = res.text;

			Application.targetFrameRate = 60;
#endif

			Dictionary<string, object> values = MiniJSON.Json.Deserialize(configJson) as Dictionary<string, object>;

			if (values == null) {
				throw new Exception("Can't parse values from config");
			}

			foreach (var i in values) {
				string key = i.Key;
				object value = i.Value;

				switch (key) {
				case KEY_HOST_NAME:
					HostName = TryParseStringValue(value, HostName);
					break;
				case KEY_HOST_PORT:
					HostPort = (int)TryParseNumberValue(value, HostPort);
					break;
				case KEY_RESULTS_SHOW_TIME:
					ResultsShowTime = TryParseNumberValue(value, ResultsShowTime);
					break;
				case KEY_SCREEN_WIDTH:
					ScreenWidth = (int)TryParseNumberValue(value, ScreenWidth);
					break;
				case KEY_SCREEN_HEIGHT:
					ScreenHeight = (int)TryParseNumberValue(value, ScreenHeight);
					break;
				case KEY_FULLSCREEN:
					Fullscreen = TryParseBooleanValue(value, Fullscreen);
					break;
				case KEY_GRAVITY:
					Gravity = TryParseNumberValue(value, Gravity);
					break;
				case KEY_JUMP:
					Jump = TryParseNumberValue(value, Jump);
					break;
				case KEY_LEVEL_SPEED:
					LevelSpeed = TryParseNumberValue(value, LevelSpeed);
					break;
				case KEY_LEVEL_SPEED_ACCELERATION:
					LevelSpeedAcceleration = TryParseNumberValue(value, LevelSpeedAcceleration);
					break;
				case KEY_LEVEL_SPEED_ACCELERATION_TIME_INTERVAL:
					LevelSpeedAccelerationTimeInterval = TryParseNumberValue(value, LevelSpeedAccelerationTimeInterval);
					break;
				case KEY_SCORE_ADD_INTERVAL_SECONDS:
					ScoreAddIntervalSeconds = TryParseNumberValue(value, ScoreAddIntervalSeconds);
					break;
				case KEY_OBSTACLES_DISTANCE:
					ObstaclesDistance = TryParseNumberValue(value, ObstaclesDistance);
					break;
				case KEY_OBSTACLES_HEIGHT_MIN:
					ObstaclesHeightMin = TryParseNumberValue(value, ObstaclesHeightMin);
					break;
				case KEY_OBSTACLES_HEIGHT_MAX:
					ObstaclesHeightMax = TryParseNumberValue(value, ObstaclesHeightMax);
					break;
				case KEY_MAX_PLAYER_SCORE:
					MaxPlayerScore = (int)TryParseNumberValue(value, MaxPlayerScore);
					break;
				case KEY_IMMORTAL_TIME_AFTER_GROUND_DEATH:
					ImmortalTimeAfterGroundDeath = TryParseNumberValue(value, ImmortalTimeAfterGroundDeath);
					break;
				case KEY_DEBUG_MODE:
					DebugMode = TryParseBooleanValue(value, DebugMode);
					break;
				case KEY_WIFI_NAME:
					WiFiName = TryParseStringValue(value, WiFiName);
					break;
				case KEY_START_GAME_TIMER:
					StartGameTimer = TryParseNumberValue(value, StartGameTimer);
					break;
				default:
					Debug.LogWarningFormat("Unknown config field: {0} with value {1}", key, value);
					break;
				}
			}
		} catch (IOException e) {
			Debug.LogErrorFormat("IO error while loading config: {0}", e.ToString());
		} catch (Exception e) {
			Debug.LogErrorFormat("Error while loading config: {0}", e.ToString());
		} finally {
			Debug.Log("HostName: " + HostName);
			Debug.Log("HostPort: " + HostPort);
		}
	}

	private static float TryParseNumberValue(object value, float defaultValue) {
		float parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		if (value is Double || value is float || value is double) {
			parsedValue = (float)Convert.ToDouble(value);
		} else if (value is Int64 || value is int || value is Int32) {
			parsedValue = (float)Convert.ToInt32(value);
		} else if (value is string) {
			float.TryParse(value as string, out parsedValue);
		}

		return parsedValue;
	}

	private static bool TryParseBooleanValue(object value, bool defaultValue) {
		bool parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		if (value is Boolean || value is bool) {
			parsedValue = Convert.ToBoolean(value);
		} else if (value is string) {
			bool.TryParse(value as string, out parsedValue);
		}

		return parsedValue;
	}

	private static string TryParseStringValue(object value, string defaultValue) {
		string parsedValue = defaultValue;

		if (value == null) {
			return parsedValue;
		}

		parsedValue = value.ToString();

		return parsedValue;
	}
}