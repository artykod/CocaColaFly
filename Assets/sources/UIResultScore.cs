using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIResultScore : MonoBehaviour {
	[SerializeField]
	private Text scoreText = null;
	[SerializeField]
	private Text playerNameText = null;

	[SerializeField, Range(1, 3)]
	private int showedPlace = 1;

	private void Awake() {
		List<PlayerInfo> players = new List<PlayerInfo>(GameCore.CurrentPlayers);
		players.Sort((p1, p2) => {
			return p1.TotalScores > p2.TotalScores ? -1 : 1;
		});

		if (players.Count < showedPlace) {
			Refresh(null);
			return;
		}

		Refresh(players[showedPlace - 1]);
	}

	private void Refresh(PlayerInfo p) {
		scoreText.text = p == null ? "" : p.TotalScores.ToString();
		if (playerNameText != null) {
			playerNameText.text = p == null ? "" : p.PlayerName;
		}
	}
}
