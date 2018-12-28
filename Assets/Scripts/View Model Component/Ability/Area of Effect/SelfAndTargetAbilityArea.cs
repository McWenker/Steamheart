using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfAndTargetAbilityArea : AbilityArea
{
    Unit u;
    public override List<Tile> GetTilesInArea(Board board, Point pos)
    {
        List<Tile> retValue = new List<Tile>();
        u = GetComponentInParent<Unit>();
        if (u != null)
            retValue.Add(u.tile);

        Tile tile = board.GetTile(pos);
        if (tile != null)
            retValue.Add(tile);
        return retValue;
    }
}
