using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTargetState : BattleState
{
	List<Tile> tiles;

	public override void Enter()
	{
		base.Enter ();
		Movement mover = turn.actor.GetComponent<Movement>();
		tiles = mover.GetTilesInRange(board);
		board.SelectTiles(tiles);
		RefreshPrimaryStatPanel(pos);
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerHighlightMoveTarget());
    }

	public override void Exit()
	{
		base.Exit ();
		board.DeSelectTiles(tiles);
		tiles = null;
		statPanelController.HidePrimary();
	}

	protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        int cameraDirIndex = owner.cameraRig.FacingIndex;
        if (cameraDirIndex == 0) // camera facing north
            SelectTile(e.info + pos);
        if(cameraDirIndex == 1) // camera facing east
            SelectTile(new Point(e.info.y + pos.x, -e.info.x + pos.y));
        if(cameraDirIndex == 2) // camera facing south
            SelectTile(new Point(-e.info.x + pos.x, -e.info.y + pos.y));
        if(cameraDirIndex == 3) // camera facing west
            SelectTile(new Point(-e.info.y + pos.x, e.info.x + pos.y));
        RefreshPrimaryStatPanel(pos);
	}

	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		if(e.info == 0)
		{
			if(tiles.Contains(owner.currentTile))
				owner.ChangeState<MoveSequenceState>();
		}
		else
		{
			owner.ChangeState<CommandSelectionState>();
		}
	}

    IEnumerator ComputerHighlightMoveTarget()
    {
        Point cursorPos = pos;
        while (cursorPos != turn.plan.moveLocation)
        {
            if (cursorPos.x < turn.plan.moveLocation.x) cursorPos.x++;
            if (cursorPos.x > turn.plan.moveLocation.x) cursorPos.x--;
            if (cursorPos.y < turn.plan.moveLocation.y) cursorPos.y++;
            if (cursorPos.y > turn.plan.moveLocation.y) cursorPos.y--;
            SelectTile(cursorPos);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.5f);
        owner.ChangeState<MoveSequenceState>();
    }
}
