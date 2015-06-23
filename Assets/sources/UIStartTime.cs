using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIStartTime : MonoBehaviour {

	[SerializeField]
	private Transform rootScreen = null;

	private Text text = null;
	private Image rootImage = null;

	private void Awake() {
		text = GetComponent<Text>();
		rootImage = rootScreen.GetComponent<Image>();
	}

	private void Update() {
		Refresh();
	}

	private void Refresh() {
		int time = (int)Game.StartTimer;

		if (time <= -2) {
			rootScreen.gameObject.SetActive(false);
		} else {
			text.text = time == 0 ? "FLY!" : time.ToString();
		}

		if (time < 1 && rootImage != null) {
			Color c = rootImage.color;
			c.a -= Time.deltaTime * 0.5f;
			rootImage.color = c;

			if (c.a <= 0.0001f) {
				rootScreen.gameObject.SetActive(false);
			}
		}
	}
}
