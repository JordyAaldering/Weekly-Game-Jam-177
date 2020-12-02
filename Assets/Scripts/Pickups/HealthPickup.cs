using UnityEngine;

public class HealthPickup : Pickup
{
	[SerializeField] private int moveIncreaseAmount;

	protected override void ApplyPickup(Collider2D collision)
	{
		LevelManager.Instance.MoveManager.IncreaseCounter(moveIncreaseAmount);
	}
}
