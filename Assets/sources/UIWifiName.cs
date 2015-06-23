using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIWifiName : MonoBehaviour {
	private void Awake() {
		Text text = GetComponent<Text>();
		if (text != null) {
			text.text = string.Format(text.text, Config.WiFiName);
		}
	}
}
