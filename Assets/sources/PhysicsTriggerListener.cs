using UnityEngine;
using System.Collections;

public class PhysicsTriggerListener : MonoBehaviour {
	public delegate void OnTriggerEnterDelegate(Collider2D collider, Collider2D selfCollider);
	public event OnTriggerEnterDelegate OnTriggerEnter;

	private Collider2D selfCollider = null;

	private void Awake() {
		selfCollider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (OnTriggerEnter != null) {
			OnTriggerEnter(collider, selfCollider);
		}
	}
}
