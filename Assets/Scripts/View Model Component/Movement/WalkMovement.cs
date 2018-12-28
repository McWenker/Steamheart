using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkMovement : Movement
{
	// override Movement.ExpandSearch, retain base implementation (distance check)
	// but also check for height to see if character can jump that high
	// also check for blocking content

	protected override bool ExpandSearch(Tile from, Tile to)
	{
		// skip if the distance in height is too great
		if ((Mathf.Abs(from.height - to.height) > jumpHeight))
			return false;

		// skip if tile occupied by enemy
		if (to.content != null)
			return false;

		return base.ExpandSearch(from, to);
	}

	public override IEnumerator Traverse(Tile tile)
	{
		unit.Place (tile);

		// build a list of way points from the unit's
		// starting tile to the destination tile
		List<Tile> targets = new List<Tile>();
		while (tile != null)
		{
			targets.Insert (0, tile);
			tile = tile.prev;
		}

		// move to each way point in succession
		for (int i = 1; i < targets.Count; ++i)
		{
			Tile from = targets[i-1];
			Tile to = targets[i];

			Directions dir = from.GetDirection(to);
			if (unit.dir != dir)
				yield return StartCoroutine(Turn(dir));

            if (from.height - to.height <= Tile.stepHeight*2f && from.height - to.height >= -Tile.stepHeight*2f)
				yield return StartCoroutine(Walk(to));
			else
				yield return StartCoroutine(Jump(to));
		}

		yield return null;
	}

	IEnumerator Walk(Tile target)
	{
		Tweener tweener = transform.MoveTo (target.center, 0.5f, EasingEquations.Linear);
		while (tweener != null)
			yield return null;
	}

	IEnumerator Jump(Tile to)
	{
		Tweener tweener = transform.MoveTo(to.center, 0.5f, EasingEquations.Linear);

		Tweener t2 = jumper.MoveToLocal(new Vector3(0, Tile.stepHeight * 2f, 0), tweener.duration / 2f, EasingEquations.EaseOutQuad);
		t2.loopCount = 1;
		t2.loopType = EasingControl.LoopType.PingPong;

		while (tweener != null)
			yield return null;
	}
}
