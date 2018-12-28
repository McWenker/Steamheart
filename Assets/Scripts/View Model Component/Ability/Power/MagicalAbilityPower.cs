using UnityEngine;
using System.Collections;

public class MagicalAbilityPower : BaseAbilityPower
{
    protected override int GetBaseAttack()
	{
		return GetComponentInParent<Stats>()[StatTypes.MAG];
	}

	protected override int GetBaseDefense(Unit target)
	{
		return target.GetComponent<Stats>()[StatTypes.MDF];
	}

    protected override int GetWeaponPower()
    {
        return 0;
    }
}