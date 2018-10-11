using UnityEngine;
using System.Collections;

public class PerformAbilityState : BattleState
{
	public override void Enter()
	{
		base.Enter();
		turn.hasUnitActed = true;
		if(turn.hasUnitMoved)
			turn.lockMove = true;

        StartCoroutine(Animate());
	}

    bool UnitHasControl()
    {
        return turn.actor.GetComponentInChildren<KnockOutStatusEffect>() == null;
    }

    IEnumerator Animate()
    {
        if (driver.Current != Drivers.Computer)
        {
            owner.battleMessageController.Display(turn.ability.name);
            yield return new WaitForSeconds(2f);
        }
        // TODO play animations, etc
        yield return null;
		ApplyAbility();

        if (IsBattleOver())
            owner.ChangeState<CutSceneState>();
        else if (!UnitHasControl())
            owner.ChangeState<SelectUnitState>();
        else if (turn.hasUnitActed && turn.hasUnitMoved)
            owner.ChangeState<SelectUnitState>();
        else if (turn.hasUnitMoved)
            owner.ChangeState<EndFacingState>();
        else
            owner.ChangeState<CommandSelectionState>();
	}

	void ApplyAbility()
	{
		BaseAbilityEffect[] effects = turn.ability.GetComponentsInChildren<BaseAbilityEffect>();
		for(int i = 0; i < turn.targets.Count; ++i)
		{
			Tile target = turn.targets[i];
			for(int j = 0; j < effects.Length; ++j)
			{
				BaseAbilityEffect effect = effects[j];
				AbilityEffectTarget targeter = effect.GetComponent<AbilityEffectTarget>();
				if(targeter.IsTarget(target))
				{
					HitRate rate = effect.GetComponent<HitRate>();
					int chance = rate.Calculate(target);
					if(UnityEngine.Random.Range(0,101) > chance)
					   {
                        Debug.Log(turn.actor.name + " missed!");
						// Swing and a miss!
						continue;
						}
					effect.Apply(target);
				}
			}
		}
	}
}