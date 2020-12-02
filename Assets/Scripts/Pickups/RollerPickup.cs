using UnityEngine;

public class RollerPickup : Pickup
{
	[SerializeField] private float increasedStickTimer;

	protected override void ApplyPickup(Collider2D collision)
	{
		PlayerController player = collision.GetComponent<PlayerController>();
		player.AddToStickTimer(increasedStickTimer);
	}
}
