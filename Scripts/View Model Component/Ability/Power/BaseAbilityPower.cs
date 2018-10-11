using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseAbilityPower : MonoBehaviour
{
    [SerializeField] int power;
    [SerializeField] int potency;
    protected abstract int GetBaseAttack();
	protected abstract int GetBaseDefense(Unit target);
    protected abstract int GetWeaponPower();

    void OnEnable()
	{
		this.AddObserver(OnGetBaseAttack, DamageAbilityEffect.GetAttackNotification);
		this.AddObserver(OnGetBaseDefense, DamageAbilityEffect.GetDefenseNotification);
        this.AddObserver(OnGetWeaponPower, DamageAbilityEffect.GetWeaponPowerNotification);
        this.AddObserver(OnGetAbilityPower, DamageAbilityEffect.GetAbilityPowerNotification);
        this.AddObserver(OnGetPotency, DamageAbilityEffect.GetPotencyNotification);
    }

	void OnDisable()
	{
		this.RemoveObserver(OnGetBaseAttack, DamageAbilityEffect.GetAttackNotification);
		this.RemoveObserver(OnGetBaseDefense, DamageAbilityEffect.GetDefenseNotification);
        this.AddObserver(OnGetWeaponPower, DamageAbilityEffect.GetWeaponPowerNotification);
        this.RemoveObserver(OnGetAbilityPower, DamageAbilityEffect.GetAbilityPowerNotification);
        this.AddObserver(OnGetPotency, DamageAbilityEffect.GetPotencyNotification);
    }

	void OnGetBaseAttack(object sender, object args)
	{
        if (IsMyEffect(sender))
        {
            var info = args as Info<Unit, Unit, List<ValueModifier>>;
            info.arg2.Add(new AddValueModifier(0, GetBaseAttack()));
        }
    }

	void OnGetBaseDefense(object sender, object args)
	{
        if (IsMyEffect(sender))
        {
            var info = args as Info<Unit, Unit, List<ValueModifier>>;
            info.arg2.Add(new AddValueModifier(0, GetBaseDefense(info.arg1)));
        }
    }

    void OnGetWeaponPower(object sender, object args)
    {
        if (IsMyEffect(sender))
        {
            var info = args as Info<Unit, Unit, List<ValueModifier>>;
            info.arg2.Add(new AddValueModifier(0, GetWeaponPower()));
        }
    }

    void OnGetAbilityPower(object sender, object args)
	{
        if (IsMyEffect(sender))
        {
            var info = args as Info<Unit, Unit, List<ValueModifier>>;
            info.arg2.Add(new AddValueModifier(0, GetAbilityPower()));
        }
    }

    void OnGetPotency(object sender, object args)
    {
        if(IsMyEffect(sender))
        {
            var info = args as Info<Unit, Unit, List<ValueModifier>>;
            info.arg2.Add(new AddValueModifier(0, GetPotency()));
        }
    }

    bool IsMyEffect(object sender)
    {
        MonoBehaviour obj = sender as MonoBehaviour;
        return (obj != null && obj.transform.parent == transform);
    }

    protected int GetAbilityPower()
    {
        return power;
    }

    protected int GetPotency()
    {
        return potency;
    }

    protected int UnarmedPower()
    {
        return GetComponentInParent<Stats>()[StatTypes.MEL];
    }

    protected int PowerFromEquippedWeapon()
    {
        int power = 0;
        Equipment eq = GetComponentInParent<Equipment>();
        Equippable item = eq.GetItem(EquipSlots.Primary);
        
        if (item == null || !item.GetComponentInChildren<WeaponAbilityPower>().GetType().Equals(GetType()))
            item = eq.GetItem(EquipSlots.Secondary);
        if (item == null || !item.GetComponentInChildren<WeaponAbilityPower>().GetType().Equals(GetType()))
            return power;
        if (item != null && item.GetComponentInChildren<WeaponAbilityPower>().GetType().Equals(GetType()))
        {
            StatModifierFeature[] features = item.GetComponentsInChildren<StatModifierFeature>();

            for (int i = 0; i < features.Length; ++i)
            {
                if (features[i].type == StatTypes.MEL)
                    power += features[i].amount;
            }
        }
        return power;
    }
}