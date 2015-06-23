using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using MiniJSON;

public class NetworkHandler : MonoBehaviour {

	private const string FINISH_MATCH_MESSAGE = "{\"MATCH_FINISHED\":1}";
	private const string USER_FINISHED_MESSAGE = "{{\"USER_FINISHED\":{{\"{0}\":{1}}}}}";

	private const string SERVER_MESSAGE_MATCH = "MATCH";
	private const string SERVER_MESSAGE_JUMP = "JUMP";

	private const string SERVER_MESSAGE_MATCH_USER_NAME = "name";
	private const string SERVER_MESSAGE_MATCH_USER_CHARACTER = "char";

	public static NetworkHandler Instance {
		get;
		private set;
	}

	private TcpClient client = null;
	private NetworkStream channel = null;

	public delegate void OnReceiveMessageDelegate(string message);
	public delegate void OnReceiveMatchDelegate(Dictionary<string, ServerUserInfo> users);
	public delegate void OnReceiveJumpDelegate(string userGuid);

	public static event OnReceiveMessageDelegate OnReceiveMessage;
	public static event OnReceiveMatchDelegate OnReceiveMatch;
	public static event OnReceiveJumpDelegate OnReceiveJump;

	public static void Create() {
		GameObject go = new GameObject("NetworkHandler");
		go.AddComponent<NetworkHandler>();
	}

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		} else {
			Instance = this;
			DontDestroyOnLoad(gameObject);

			Application.runInBackground = true;
		}

		try {
			client = new TcpClient();
			client.Connect(Config.HostName, Config.HostPort);
			channel = client.GetStream();

			Debug.Log("Net: Connected to server");

		} catch (System.Exception e) {
			Debug.LogErrorFormat("Net: Connect error = {0}", e.ToString());

			if (!GameCore.MatchIsFake) {
				GameCore.WaitNewMatch();
			}
		}
	}

	private void OnDestroy() {
		SendFinishMatch();

		Debug.Log("Net: Disconnect from server");

		Instance = null;

		if (channel != null) {
			channel.Close();
			channel = null;
		}

		if (client != null) {
			client.Close();
			client = null;
		}
	}

	private void Update() {
		if (channel != null && channel.DataAvailable) {
			string message = "";

			try {
				byte[] data = new byte[client.ReceiveBufferSize];
				channel.Read(data, 0, client.ReceiveBufferSize);
				message = Encoding.UTF8.GetString(data);

				Debug.LogFormat("Net: received message = {0}", message);

				if (!string.IsNullOrEmpty(message)) {

					if (OnReceiveMessage != null) {
						OnReceiveMessage(message);
					}

					HandleBaseMessages(message);
				}

			} catch (System.Exception e) {
				Debug.LogErrorFormat("Net: fail while receive message = {0}", e.ToString());

				if (!GameCore.MatchIsFake) {
					GameCore.WaitNewMatch();
				}
			}
		}
	}

	public static void SendMessageToServer(string message) {
		if (Instance == null) {
			Debug.LogError("Net: Not found NetworkHandler instance. Create it in scene.");
			return;
		}

		try {
			Debug.LogFormat("Net: send message to server message = {0}", message);

			byte[] data = Encoding.UTF8.GetBytes(message);
			Instance.client.Client.Send(data);
		} catch (System.Exception e) {
			Debug.LogErrorFormat("Net: fail while send message = {0}", e.ToString());

			if (!GameCore.MatchIsFake) {
				GameCore.WaitNewMatch();
			}
		}
	}

	#region utils

	public class ServerUserInfo {
		public string guid = "";
		public string name = "";
		public string character = "";
	}

	public static void HandleBaseMessages(string message) {
		Dictionary<string, object> dict = Json.Deserialize(message) as Dictionary<string, object>;

		foreach (var i in dict) {
			switch (i.Key) {
			case SERVER_MESSAGE_MATCH:
				Dictionary<string, object> users = i.Value as Dictionary<string, object>;
				try {
					if (users != null) {
						Dictionary<string, ServerUserInfo> parsedUsers = new Dictionary<string, ServerUserInfo>();

						foreach (var u in users) {
							Dictionary<string, object> userInfo = u.Value as Dictionary<string, object>;
							ServerUserInfo user = new ServerUserInfo();

							user.guid = u.Key;
							if (userInfo != null) {
								foreach (var uKey in userInfo) {
									switch (uKey.Key) {
									case SERVER_MESSAGE_MATCH_USER_NAME:
										user.name = uKey.Value as string;
										break;
									case SERVER_MESSAGE_MATCH_USER_CHARACTER:
										user.character = uKey.Value as string;
										break;
									}
								}

								parsedUsers.Add(u.Key, user);
							}
						}

						if (parsedUsers.Count > 0) {
							if (OnReceiveMatch != null) {
								OnReceiveMatch(parsedUsers);
							}
						} else {
							throw new System.Exception();
						}
					} else {
						throw new System.Exception();
					}
				} catch {
					Debug.LogWarningFormat("Net: received users in MATCH message has unknown format, type: {0}", i.Value.GetType());
				}
				break;
			case SERVER_MESSAGE_JUMP:
				string user = i.Value as string;
				if (user != null) {
					if (OnReceiveJump != null) {
						OnReceiveJump(user);
					}
				} else {
					Debug.LogWarningFormat("Net: received user in JUMP message has unknown format, type: {0}", i.Value.GetType());
				}
				break;
			default:
				Debug.LogWarningFormat("Net: Unknown server message {0} with value {1}", i.Key, i.Value);
				break;
			}
		}
	}

	public static void SendFinishMatch() {
		SendMessageToServer(FINISH_MATCH_MESSAGE);
	}

	public static void SendUserFinished(string userGuid, int userScores) {
		SendMessageToServer(string.Format(USER_FINISHED_MESSAGE, userGuid, userScores));
	}

	#endregion
}
