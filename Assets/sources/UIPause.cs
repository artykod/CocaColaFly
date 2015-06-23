using UnityEngine;
using System.Collections;

public class UIPause : MonoBehaviour {
	private void Awake() {
		Game.OnPause += Game_OnPause;

		gameObject.SetActive(Game.IsPaused);
	}

	private void OnDestroy() {
		Game.OnPause -= Game_OnPause;
	}

	private void Game_OnPause(bool isPaused) {
		gameObject.SetActive(isPaused);
	}
}
