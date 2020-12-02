using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	protected abstract void ApplyPickup(Collider2D collision);

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player")) {
			ApplyPickup(collision);
			Destroy(gameObject);
		}
	}
}
