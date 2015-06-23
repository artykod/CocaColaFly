using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public PlayerInfo SelfPlayerInfo {
		get {
			return selfPlayerInfo;
		}
	}

	private float startIdleDelay = 2f;
	private bool startedPlay = false;

	private new Transform transform = null;
	private float velocity = 0f;

	private PlayerInfo selfPlayerInfo = null;

	public delegate void OnFinishDelegate(Player player);
	public event OnFinishDelegate OnFinish;

	[SerializeField]
	private PlayerVisual visualPrefab = null;
	[SerializeField]
	private BubblePlayerDie bubblePrefab = null;

	private PlayerVisual visual = null;
	private List<BubblePlayerDie> bubbles = new List<BubblePlayerDie>(100);

	private Collider2D lastCollider = null;

	private float immortalTime = 0f;

	private float totalLiveTime = 0f;
	private float totalLiveTimePrevious = 0f;

	private void Jump() {
		startIdleDelay = 0f;
		startedPlay = true;
		velocity = Config.Jump;

		visual.PlayJump();
	}

	private void Awake() {
		this.transform = base.transform;

		visual = Instantiate(visualPrefab);
		visual.PlayerRef = this;
		visual.transform.SetParent(transform, false);
		visual.OnCollision += OnTriggerEnter2DInternal;

		startIdleDelay = Game.StartTimer - 1f;

		for (int i = 0; i < 50; i++) {
			float angle = Random.value * Mathf.PI * 2f;
			Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
			dir *= Random.value;
			float size = Random.value * 0.25f + 0.25f;

			BubblePlayerDie bubble = Instantiate(bubblePrefab);
			bubble.transform.SetParent(transform, false);
			bubble.transform.localPosition = dir;
			bubble.transform.localScale = new Vector3(size, size, 1f);
			bubble.gameObject.SetActive(false);

			bubbles.Add(bubble);
		}

		immortalTime = 0f;
		totalLiveTime = 0f;
	}

	public void FetchPlayerInfo(PlayerInfo playerInfo) {
		selfPlayerInfo = playerInfo;

		selfPlayerInfo.OnJump += selfPlayerInfo_OnJump;
		selfPlayerInfo.OnLifeChange += selfPlayerInfo_OnLifeChange;

		visual.ApplySkinById(selfPlayerInfo.CharacterId);
	}

	private void selfPlayerInfo_OnJump() {
		if (startIdleDelay > 0f) {
			return;
		}

		Jump();
	}

	void selfPlayerInfo_OnLifeChange(int changeValue) {
		visual.PlayLife(changeValue);
	}

	public bool DoneObstacle(Obstacle obstacle) {

		// scores by done obstacles

		/*int reward = obstacle.TakeRewardForPlayer(selfPlayerInfo.Guid);

		if (reward != 0) {
			selfPlayerInfo.AddScores(reward);
			return true;
		}*/

		return false;
	}

	private void Update() {
		if (startIdleDelay > 0f) {
			startIdleDelay -= Time.deltaTime;
			return;
		} else if (!startedPlay) {
			startedPlay = true;
			visual.PlayFall();
		}

		Vector3 pos = transform.position;
		velocity += Config.Gravity * Time.deltaTime;
		pos.y += velocity * Time.deltaTime;
		transform.position = pos;

		visual.RefreshScore(selfPlayerInfo.Scores);

		if (immortalTime > 0f) {
			immortalTime -= Time.deltaTime;
		}

		totalLiveTime += Time.deltaTime;
		float diff = totalLiveTime - totalLiveTimePrevious;
		if (diff >= Config.ScoreAddIntervalSeconds) {
			selfPlayerInfo.AddScores(1);
			totalLiveTimePrevious = totalLiveTime;
		}
	}

	private bool Finish() {

		if (!selfPlayerInfo.Finish()) {
			return false;
		}

		if (OnFinish != null) {
			OnFinish(this);
		}

		visual.PlayDie();

		selfPlayerInfo.OnJump -= selfPlayerInfo_OnJump;
		selfPlayerInfo.OnLifeChange -= selfPlayerInfo_OnLifeChange;

		enabled = false;

		foreach (var i in bubbles) {
			if (i != null) {
				Vector3 velocity = i.transform.localPosition.normalized * Random.value * 2f;
				velocity.y -= Config.Gravity * 0.25f;
				i.transform.SetParent(null, true);
				i.gameObject.SetActive(true);
				i.Play(velocity);
			}
		}

		return true;
	}

	private void OnTriggerEnter2DInternal(Collider2D collider, BoxCollider2D selfCollider) {
		if (!enabled) {
			return;
		}

		if (lastCollider == collider) {
			return;
		}

		lastCollider = collider;

		bool isGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");

		if (immortalTime <= 0f || isGround) {

			bool collision = true;

			/*if (!isGround) {
				BoxCollider2D box = collider as BoxCollider2D;
				BoxCollider2D self = selfCollider;
				if (box != null && self != null) {
					Vector3 pos = transform.position;
					Vector3 boxSize = (box.size + self.size) * 0.5f;
					Vector3 boxPos = box.transform.position;
					Vector3 diff = pos - boxPos;

					if (boxSize.y - Mathf.Abs(diff.y) < 0.5f) {
						collision = false;
					}
				}
			}*/

			if (collision) {
				Finish();
			}
		}

		if (!enabled) {
			return;
		}

		if (SelfPlayerInfo.Lifes > 0 && isGround) {
			Vector3 pos = transform.position;
			pos.y = 0f;
			transform.position = pos;
			velocity = 0f;
			lastCollider = null;

			immortalTime = Config.ImmortalTimeAfterGroundDeath;
			startedPlay = false;
			visual.PlayIdle();
			visual.PlayImmortal(immortalTime);
		}
	}
}
