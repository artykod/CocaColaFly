using UnityEngine;
using System.Collections;

public class UIGameControlsInfo : MonoBehaviour {
	private void Awake() {
		if (!Config.DebugMode) {
			gameObject.SetActive(false);
		}
	}
}
