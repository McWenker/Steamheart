using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
	public Tile tile { get; protected set; }
	public Directions dir;

	public void Place(Tile target)
	{
		// make sure old tile location is not still pointing
		if (tile != null && tile.content == gameObject)
			tile.content = null;

		// link unit and tile references
		tile = target;

		if (target != null)
			target.content = gameObject;
	}

	public void Match()
	{
		transform.localPosition = tile.center;
		transform.localEulerAngles = dir.ToEuler();
	}
}
