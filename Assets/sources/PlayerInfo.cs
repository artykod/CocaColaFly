using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo {
	public delegate void OnJumpDelegate();
	public event OnJumpDelegate OnJump;

	public delegate void OnLifeChangeDelegate(int changeValue);
	public event OnLifeChangeDelegate OnLifeChange;

	public string Guid {
		get;
		private set;
	}

	public string CharacterId {
		get;
		private set;
	}

	public string PlayerName {
		get;
		private set;
	}

	public int Scores {
		get;
		private set;
	}

	public int TotalScores {
		get;
		private set;
	}

	public bool IsFinished {
		get;
		private set;
	}

	public int Lifes {
		get;
		private set;
	}

	public void AddScores(int scores) {
		if (IsFinished) {
			return;
		}

		Scores += scores;
		TotalScores += scores;
		//Debug.LogFormat("Add scores to player {0}: scores add = {1}, scores = {2} total = {3}", Guid, scores, Scores, TotalScores);

		if (Scores > Config.MaxPlayerScore) {
			Scores = 0;
			Lifes++;
			if (OnLifeChange != null) {
				OnLifeChange(+1);
			}
		}
	}

	public void Jump() {
		if (IsFinished) {
			return;
		}

		if (OnJump != null) {
			OnJump();
		}
	}

	public bool Finish() {
		if (!IsFinished) {
			Lifes--;
			if (OnLifeChange != null) {
				OnLifeChange(-1);
			}

			if (Lifes < 1) {
				IsFinished = true;
				GameCore.PlayerFinished(this);

				return true;
			}
		}

		return false;
	}

	public PlayerInfo(string guid, string characterId, string playerName) {
		Guid = guid;
		CharacterId = characterId;
		PlayerName = playerName;
		Lifes = 1;
	}
}

public class PlayersCollection : IEnumerable<PlayerInfo> {
	private Dictionary<string, PlayerInfo> players = null;

	public PlayersCollection(Dictionary<string, PlayerInfo> players) {
		this.players = players;
	}

	public IEnumerator<PlayerInfo> GetEnumerator() {
		foreach (var i in players) {
			yield return i.Value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}