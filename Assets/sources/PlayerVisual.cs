using UnityEngine;
using System.Collections;

public class PlayerVisual : MonoBehaviour {

	[System.Serializable]
	public class SkinInfo {
		public string id = "";
		public Sprite body = null;
	}

	private const string TRIGGER_IDLE = "idle";
	private const string TRIGGER_JUMP = "jump";
	private const string TRIGGER_FALL = "fall";
	private const string TRIGGER_DIE  = "die";

	public delegate void OnCollisionDelegate(Collider2D collider, BoxCollider2D selfCollider);
	public event OnCollisionDelegate OnCollision;

	[SerializeField]
	private SkinInfo[] skins = null;
	[SerializeField]
	private SpriteRenderer bodyRenderer = null;

	[SerializeField]
	private Animator lifeAnimator = null;
	[SerializeField]
	private SpriteRenderer lifeSprite = null;
	[SerializeField]
	private TextMesh lifeText = null;

	[SerializeField]
	private Animator lifeAnimatorMinus = null;
	[SerializeField]
	private SpriteRenderer lifeSpriteMinus = null;
	[SerializeField]
	private TextMesh lifeTextMinus = null;

	[SerializeField]
	private TextMesh scoreText = null;
	[SerializeField]
	private Transform scoreRoot = null;

	[SerializeField]
	private Material playerMaterial = null;

	[SerializeField]
	private MeshRenderer scoreIcon = null;

	[SerializeField]
	private PhysicsTriggerListener bodyTrigger = null;

	private Animator animator = null;

	private IEnumerator immortalAnimation = null;

	private float scoreIconWaveOffset = 0f;

	public Player PlayerRef {
		get;
		set;
	}

	private void Awake() {
		animator = GetComponent<Animator>();

		scoreIcon.sharedMaterial = Instantiate(scoreIcon.sharedMaterial);

		scoreText.text = "";

		RefreshScore(0);

		Material cloneMaterial = Instantiate(playerMaterial);
		SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (var i in sr) {
			if (i.sharedMaterial == playerMaterial) {
				i.sharedMaterial = cloneMaterial;
			}
		}
		playerMaterial = cloneMaterial;

		bodyTrigger.OnTriggerEnter += bodyTrigger_OnTriggerEnter;
	}

	void bodyTrigger_OnTriggerEnter(Collider2D collider, Collider2D selfCollider) {
		if (OnCollision != null) {
			OnCollision(collider, selfCollider as BoxCollider2D);
		}
	}

	private void Update() {
		scoreIconWaveOffset -= Time.deltaTime * 1.5f;
		scoreIconWaveOffset %= 1f;
		scoreIcon.sharedMaterial.SetFloat("_WrapX", scoreIconWaveOffset);
	}

	public void ApplySkinById(string skinId) {
		foreach (var i in skins) {
			if (skinId == i.id) {
				bodyRenderer.sprite = i.body;
				break;
			}
		}
	}

	public void PlayLife(int lifes) {
		if (lifes == 0 || this == null) {
			return;
		}

		if (lifes < 0) {
			lifeSpriteMinus.color = Color.black;
			lifeTextMinus.text = "-" + Mathf.Abs(lifes);
			lifeAnimatorMinus.SetTrigger("show");
		} else {
			lifeSprite.color = Color.white;
			lifeText.text = "+" + Mathf.Abs(lifes);
			lifeAnimator.SetTrigger("show");
		}
	}

	public void RefreshScore(int score) {
		string scoreStr = score.ToString();

		if (scoreStr != scoreText.text) {
			scoreText.text = scoreStr;

			float k = (float)score / (float)Config.MaxPlayerScore;
			float scale = 1f;//Mathf.Max(k, 0.5f);

			scoreRoot.localScale = new Vector3(scale, scale, 1f);
			scoreIcon.sharedMaterial.SetFloat("_WrapY", k);
		}
	}

	public void PlayIdle() {
		if (animator == null) {
			return;
		}

		animator.SetTrigger(TRIGGER_IDLE);
	}

	public void PlayJump() {
		if (animator == null) {
			return;
		}

		animator.SetTrigger(TRIGGER_JUMP);
	}

	public void PlayFall() {
		if (animator == null) {
			return;
		}

		animator.SetTrigger(TRIGGER_FALL);
	}

	public void PlayDie() {
		if (animator == null) {
			return;
		}

		animator.SetTrigger(TRIGGER_DIE);

		scoreRoot.gameObject.SetActive(false);
	}

	public void PlayImmortal(float time) {
		if (immortalAnimation != null) {
			StopCoroutine(immortalAnimation);
			immortalAnimation = null;
		}
		SetPlayerAlpha(1f);
		StartCoroutine(immortalAnimation = PlayImmortalAnimation(time));
	}

	private void SetPlayerAlpha(float alpha) {
		playerMaterial.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
	}

	private IEnumerator PlayImmortalAnimation(float time) {
		float alpha = 1f;
		float delta = 0f;
		while (time > 0f) {
			time -= Time.deltaTime;
			delta += Time.deltaTime * 4f;
			int intDelta = (int)delta;
			alpha = delta - intDelta;
			if (intDelta % 2 != 0) {
				alpha = 1f - alpha;
			}

			SetPlayerAlpha(alpha);

			yield return null;
		}

		SetPlayerAlpha(1f);

		yield return null;

		immortalAnimation = null;
	}
}
