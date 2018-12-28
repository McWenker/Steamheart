using UnityEngine;
using System.Collections;

public class MeleeWeaponAbilityPower : WeaponAbilityPower
{
    protected override int GetBaseAttack()
    {
        return GetComponentInParent<Stats>()[StatTypes.MEL];
    }

    protected override int GetBaseDefense(Unit target)
    {
        return target.GetComponent<Stats>()[StatTypes.DEF];
    }
}