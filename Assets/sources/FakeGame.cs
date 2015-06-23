using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FakeGame : MonoBehaviour, IPointerClickHandler {
	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		GameCore.StartFakeGame();
	}

	private void Update() {
		if (!Config.DebugMode) {
			gameObject.SetActive(false);
		}
	}
}
