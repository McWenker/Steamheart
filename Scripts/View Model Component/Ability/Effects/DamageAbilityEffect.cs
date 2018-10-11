using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageAbilityEffect : BaseAbilityEffect
{
	#region Public
	public override int Predict(Tile target)
	{
		Unit attacker = GetComponentInParent<Unit>();
		Unit defender = target.content.GetComponent<Unit>();

		// Get the attacker's base attack stat considering mission check, support check,
		// status check, equipment check, etc
		int attack = GetStat(attacker, defender, GetAttackNotification, 0);
        Debug.Log("initial attack: " + attack);

		// Get the target's base defense stat considering mission check, support check,
		// status check, equipment check, etc
		int defense = GetStat(attacker, defender, GetDefenseNotification, 0);
        Debug.Log("initial defense: " + defense);

        // Get the weapon's power stat considering possible variations
        // Then calculate base damage
        int wPower = GetStat(attacker, defender, GetWeaponPowerNotification, 0);
        int damage = (attack + wPower) - (defense / 2);
        Debug.Log("base damage: " + damage);

        // Get the ability's potency stat considering possible variations
        int potency = GetStat(attacker, defender, GetPotencyNotification, 0);

        // Get the ability's power stat considering possible variations
        int aPower = GetStat(attacker, defender, GetAbilityPowerNotification, 0);

        // Apply potency and power bonuses bonus, then secondary defense
        if(potency != 0)
            damage = potency * damage / 100;
        damage = aPower + damage;
        damage = damage - (defense / 4);
		damage = Mathf.Max(damage, 1);
        Debug.Log("modified damage: " + damage);

		// Tweak the damage based on a variety of other checks like elemental damage,
		// critical hits, etc
		damage = GetStat(attacker, defender, TweakDamageNotification, damage);
        Debug.Log("tweaked damage: " + damage);

		// Clamp the damage to a range
		damage = Mathf.Clamp(damage, minDamage, maxDamage);
        Debug.Log("final damage: " + damage);
        return -damage;
	}

	protected override int OnApply(Tile target)
	{
		Unit defender = target.content.GetComponent<Unit>();

		// Start with the predicted damage value
		int value = Predict(target);
		
		// Add some random variance
		value = Mathf.FloorToInt(value * UnityEngine.Random.Range(0.9f, 1.1f));
		
		// Clamp the damage to a range
		value = Mathf.Clamp(value, minDamage, maxDamage);
		
		// Apply the damage to the target
		Stats s = defender.GetComponent<Stats>();
		s[StatTypes.HP] += value;
        Debug.Log("Damage dealt: " + value + ", target HP: " + s[StatTypes.HP]);
		return value;
	}
	#endregion
}