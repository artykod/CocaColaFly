using UnityEngine;
using System.Collections;

public class UIResults : MonoBehaviour {
	[SerializeField]
	private GameObject[] resultsScreens = null;

	private void Awake() {
		foreach (var i in resultsScreens) {
			i.SetActive(false);
		}
		int index = Mathf.Max(0, GameCore.CurrentPlayersCount - 1);
		resultsScreens[index].SetActive(true);
	}
}
