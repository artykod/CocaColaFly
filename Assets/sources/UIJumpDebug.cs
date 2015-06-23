using UnityEngine;
using System.Collections;

public class UIJumpDebug : MonoBehaviour {
	private TextMesh text = null;
	private PlayerVisual visual = null;

	private void Awake() {
		if (!Config.DebugMode) {
			Destroy(gameObject);
			return;
		}

		text = GetComponent<TextMesh>();
		visual = GetComponentInParent<PlayerVisual>();
	}

	private void Update() {
		if (visual == null || text == null) {
			return;
		}

		string guid = "jumps: ";
		int jumps = 0;
		if (visual != null && visual.PlayerRef != null && visual.PlayerRef.SelfPlayerInfo != null) {
			jumps = GameCore.GetJumpsDebugForPlayerGuid(visual.PlayerRef.SelfPlayerInfo.Guid);
		}
		guid += jumps.ToString();

		text.text = guid;
	}
}
