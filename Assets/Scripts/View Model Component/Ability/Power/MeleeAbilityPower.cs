﻿using UnityEngine;
using System.Collections;

public class MeleeAbilityPower : BaseAbilityPower
{
    protected override int GetBaseAttack()
	{
		return GetComponentInParent<Stats>()[StatTypes.MEL];
	}

	protected override int GetBaseDefense(Unit target)
	{
		return target.GetComponent<Stats>()[StatTypes.DEF];
	}

    protected override int GetWeaponPower()
    {
        return 0;
    }
}