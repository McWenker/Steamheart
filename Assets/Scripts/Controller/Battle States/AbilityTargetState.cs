using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTargetState : BattleState
{
	List<Tile> tiles;
	AbilityRange ar;

	public override void Enter()
	{
		base.Enter();
		ar = turn.ability.GetComponent<AbilityRange>();
		SelectTiles();
		statPanelController.ShowPrimary(turn.actor.gameObject);
        if(pos != turn.actor.tile.pos)
            RefreshSecondaryStatPanel(pos);
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerHighlightTarget());
    }

	public override void Exit()
	{
		base.Exit();
		board.DeSelectTiles(tiles);
		statPanelController.HidePrimary();
		statPanelController.HideSecondary();
	}

	protected override void OnMove(object sender, InfoEventArgs<Point> e)
	{
		if(ar.directionOriented)
		{
            int cameraDirIndex = owner.cameraRig.FacingIndex;
            ChangeDirection(e.info, true);
            if (cameraDirIndex == 0) // camera facing north
                ChangeDirection(e.info, true);
            if (cameraDirIndex == 1) // camera facing east
                ChangeDirection(new Point(e.info.y, -e.info.x), true);
            if (cameraDirIndex == 2) // camera facing south
                ChangeDirection(new Point(-e.info.x, -e.info.y), true);
            if (cameraDirIndex == 3) // camera facing west
                ChangeDirection(new Point(-e.info.y, e.info.x), true);
        }
		else
        {
            ChangeDirection(e.info + pos, false);
            SelectTile(e.info + pos);
            if (pos != turn.actor.tile.pos)
                RefreshSecondaryStatPanel(pos);
            else
                statPanelController.HideSecondary();
		}
	}

	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		if(e.info == 0 && (InRange(pos) || ar.directionOriented))
		{
			turn.hasUnitActed = true;
			if(turn.hasUnitMoved)
				turn.lockMove = true;
			owner.ChangeState<ConfirmAbilityTargetState>();
		}
        else if(e.info == 0)
        {
            SelectTile(turn.actor.tile.pos);
        }
		else
		{
			owner.ChangeState<CategorySelectionState>();
		}
	}

	void ChangeDirection(Point p, bool directionOriented)
	{
        Directions dir;
        if (!directionOriented)
            dir = DirectionsExtensions.GetDirection(turn.actor.tile.pos, p);
        else
            dir = p.GetDirection();

        if (turn.actor.dir != dir)
		{
			board.DeSelectTiles(tiles);
			turn.actor.dir = dir;
			turn.actor.Match();
			SelectTiles();
		}
	}

    void ChangeDirection(Point p)
    {
        Directions dir;
        dir = p.GetDirection();
        if (turn.actor.dir != dir)
        {
            board.DeSelectTiles(tiles);
            turn.actor.dir = dir;
            turn.actor.Match();
            SelectTiles();
        }
    }

    bool InRange(Point p)
    {
        foreach(Tile t in tiles)
        {
            if (p == t.pos)
                return true;
        }
        return false;
    }

	void SelectTiles()
	{
		tiles = ar.GetTilesInRange(board);
		board.SelectTiles(tiles);
	}

    IEnumerator ComputerHighlightTarget()
    {
        Point cursorPos = pos;
        while (cursorPos != turn.plan.fireLocation)
        {
            if (cursorPos.x < turn.plan.fireLocation.x) cursorPos.x++;
            if (cursorPos.x > turn.plan.fireLocation.x) cursorPos.x--;
            if (cursorPos.y < turn.plan.fireLocation.y) cursorPos.y++;
            if (cursorPos.y > turn.plan.fireLocation.y) cursorPos.y--;
            SelectTile(cursorPos);
            yield return new WaitForSeconds(0.25f);
        }
        ChangeDirection(turn.plan.attackDirection.GetNormal());
        yield return new WaitForSeconds(0.5f);
        owner.ChangeState<ConfirmAbilityTargetState>();
    }
}