using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour {

	public new Transform transform {
		get;
		private set;
	}

	public delegate void OnReturnToPoolDelegate(Obstacle obstacle);
	public event OnReturnToPoolDelegate OnReturnToPool;

	public delegate void OnMoveDelegate(Obstacle obstacle);
	public event OnMoveDelegate OnMove;

	private Dictionary<string, int> rewardsForPlayers = new Dictionary<string, int>();

	[SerializeField]
	private GameObject bubblePrefab = null;

	private List<GameObject> bubbles = new List<GameObject>(100);

	public void Initialize() {

		Vector2 size = GetComponent<BoxCollider2D>().size * 0.5f;

		for (float x = -size.x; x < size.x; x += 0.4f) {
			for (float y = -size.y; y < size.y; y += 0.4f) {
				GameObject bubble = Instantiate(bubblePrefab);

				bubble.transform.SetParent(transform, false);
				float scale = Random.value * 0.5f + 0.5f;

				float xRandom = (Random.value - 0.5f) * 0.3f;
				float yRandom = (Random.value - 0.5f) * 0.3f;

				bubble.transform.localPosition = new Vector3(x + xRandom, y + yRandom);
				bubble.transform.localScale = new Vector3(scale, scale, 1f);

				bubbles.Add(bubble);

				bubble.SetActive(false);
			}
		}
	}

	public void SetupForVisualize() {
		rewardsForPlayers.Clear();
		foreach (PlayerInfo i in GameCore.CurrentPlayers) {
			rewardsForPlayers.Add(i.Guid, 1);
		}

		foreach (var i in bubbles) {
			i.SetActive(false);
			StartCoroutine(EnableBubbleWithDelay(i, Random.value));
		}
	}

	private IEnumerator EnableBubbleWithDelay(GameObject bubble, float delay) {
		while (delay > 0f) {
			delay -= Time.deltaTime;
			yield return null;
		}

		bubble.SetActive(true);
	}

	public int TakeRewardForPlayer(string playerGuid) {
		int reward = rewardsForPlayers[playerGuid];
		rewardsForPlayers[playerGuid] = 0;
		return reward;
	}

	protected virtual void Awake() {
		this.transform = base.transform;
	}

	protected virtual void Die() {
		ReturnToPool();
	}

	private void Update() {
		transform.position += new Vector3(-Game.LevelSpeed * Time.deltaTime, 0f);

		if (transform.position.x < -Game.ScreenSizeWorld.x - 3f) {
			Die();
			return;
		}

		if (OnMove != null) {
			OnMove(this);
		}
	}

	private void ReturnToPool() {
		if (OnReturnToPool != null) {
			OnReturnToPool(this);
		}
	}
}
