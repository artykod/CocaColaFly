using UnityEngine;
using System.Collections;

public class BubblePlayerDie : MonoBehaviour {

	private Vector3 velocity = Vector3.zero;
	private Animator animator = null;
	private SpriteRenderer sprite = null;

	private void Awake() {
		sprite = GetComponent<SpriteRenderer>();
		if (sprite != null) {
			float red = Random.value;
			sprite.color = Color.Lerp(Color.red, Color.white, red);
		}

		animator = GetComponent<Animator>();
		if (animator != null) {
			animator.enabled = false;
		}
	}

	public void Play(Vector3 velocity) {
		this.velocity = velocity;
		StartCoroutine(EnableAnimatorWithDelay(Random.value * 0.5f));
	}

	private IEnumerator EnableAnimatorWithDelay(float delay) {
		while (delay > 0f) {
			delay -= Time.deltaTime;
			yield return null;
		}

		if (animator != null) {
			animator.enabled = true;
		}
	}

	private void Update() {
		velocity.y += Config.Gravity * Time.deltaTime;
		transform.position += velocity * Time.deltaTime;

		if (sprite != null) {
			Color c = sprite.color;
			c.a -= Time.deltaTime;
			sprite.color = c;
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		Destroy(gameObject);
	}
}
