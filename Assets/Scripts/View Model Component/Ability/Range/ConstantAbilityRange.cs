﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstantAbilityRange : AbilityRange
{
    public bool selfTargeting = false;
    
	public override List<Tile> GetTilesInRange(Board board)
	{
        return board.Search(unit.tile, ExpandSearch, selfTargeting);
	}

	bool ExpandSearch(Tile from, Tile to)
	{
		return (from.distance + 1) <= horizontal && Mathf.Abs(to.height - unit.tile.height) <= vertical;
	}
}