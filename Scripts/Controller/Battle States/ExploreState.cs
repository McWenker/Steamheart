using UnityEngine;
using System.Collections;

public class ExploreState : BattleState
{
	protected override void OnMove(object sender, InfoEventArgs<Point> e)
	{
		SelectTile(e.info + pos);
		RefreshPrimaryStatPanel(pos);
	}

	protected override void OnFire(object sender, InfoEventArgs<int> e)
	{
		if(e.info == 0)
			owner.ChangeState<CommandSelectionState>();
	}

	public override void Enter()
	{
		base.Enter();
		RefreshPrimaryStatPanel(pos);
	}

	public override void Exit()
	{
		base.Exit();
		statPanelController.HidePrimary();
	}
}
