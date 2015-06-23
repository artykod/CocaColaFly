using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour {
	private class LogInfo {
		public string messsage = "";
		public Color color = Color.white;
	}

	private List<LogInfo> logs = new List<LogInfo>();
	private GUIStyle logStyle;
	private Vector2 scrollPosition = Vector2.zero;
	private bool showDebug = false;

	private float width = 1024f;
	private float height = 768f;

	public static void Initialize() {
		GameObject go = new GameObject("DebugConsole");
		go.AddComponent<DebugConsole>();
		DontDestroyOnLoad(go);
	}

	protected void Awake() {
		Application.logMessageReceived += Application_logMessageReceived;
	}

	void Application_logMessageReceived(string log, string stackTrace, LogType type) {
		Color logColor = Color.white;

		switch (type) {
		case LogType.Warning:
			logColor = Color.yellow; 
			break;
		case LogType.Error:
			logColor = Color.red;
			log += " stack trace: " + stackTrace;
			break;
		case LogType.Exception:
			logColor = Color.magenta;
			log += " stack trace: " + stackTrace;
			break;
		case LogType.Assert:
			logColor = Color.blue;
			log += " stack trace: " + stackTrace;
			break;
		default:
			logColor = Color.white;
			break;
		}

		logs.Add(new LogInfo() {
			messsage = System.DateTime.Now.ToString("HH:mm:ss.ffff : ") + log,
			color = logColor
		});

		scrollPosition.y = float.MaxValue;
	}

	private void OnGUI() {

		if (!Config.DebugMode) {
			Application.logMessageReceived -= Application_logMessageReceived;
			gameObject.SetActive(false);
		}

		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / width, Screen.height / height, 1f));

		if (showDebug) {
			GUI.Box(new Rect(0f, 0f, width, height), new GUIContent(), GUI.skin.box);

			logStyle = GUI.skin.label;
			logStyle.wordWrap = true;

			if (GUILayout.Button("Clear log")) {
				logs.Clear();
			}

			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(width), GUILayout.Height(height - 100f));

			Color temp = GUI.color;

			foreach (var i in logs) {
				GUI.color = i.color;
				GUILayout.Label(i.messsage, logStyle);
			}

			GUI.color = temp;

			GUILayout.EndScrollView();
		}

		if (GUI.Button(new Rect(width - 64f, height - 32f, 64f, 32f), "Console")) {
			showDebug = !showDebug;
		}
	}
}
