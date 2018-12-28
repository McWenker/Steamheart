using UnityEngine;
using System.Collections;

public class EndFacingState : BattleState
{
	Directions startDir;

	public override void Enter()
    {
        base.Enter();
        startDir = turn.actor.dir;
        owner.facingIndicator.gameObject.SetActive(true);
        owner.facingIndicator.SetDirection(turn.actor.dir);
        SelectTile(turn.actor.tile.pos);
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerControl());
    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
	{
        int cameraDirIndex = owner.cameraRig.FacingIndex;
        if (cameraDirIndex == 0) // camera facing north
            turn.actor.dir = e.info.GetDirection();
        if (cameraDirIndex == 1) // camera facing east
            turn.actor.dir = new Point(e.info.y, -e.info.x).GetDirection();
        if (cameraDirIndex == 2) // camera facing south
            turn.actor.dir = new Point(-e.info.x, -e.info.y).GetDirection();
        if (cameraDirIndex == 3) // camera facing west
            turn.actor.dir = new Point(-e.info.y, e.info.x).GetDirection();
        turn.actor.Match ();
        owner.facingIndicator.SetDirection(turn.actor.dir);
    }

	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		switch(e.info)
		{
		case 0:
            owner.facingIndicator.gameObject.SetActive(false);
			owner.ChangeState<SelectUnitState>();
			break;
		case 1:
            owner.facingIndicator.gameObject.SetActive(false);
            turn.actor.dir = startDir;
			turn.actor.Match();
			owner.ChangeState<CommandSelectionState>();
			break;
		}
	}

    IEnumerator ComputerControl()
    {
        yield return new WaitForSeconds(0.5f);
        turn.actor.dir = owner.cpu.DetermineEndFacingDirection();
        turn.actor.Match();
        owner.facingIndicator.SetDirection(turn.actor.dir);
        yield return new WaitForSeconds(0.5f);
        owner.facingIndicator.gameObject.SetActive(false);
        owner.ChangeState<SelectUnitState>();
    }
}
