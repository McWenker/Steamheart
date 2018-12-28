using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionState : BaseAbilityMenuState
{
    Ability[] weaponOptions;

    public override void Enter()
    {
        base.Enter();
        statPanelController.ShowPrimary(turn.actor.gameObject);
    }

    public override void Exit()
    {
        base.Exit();
        statPanelController.HidePrimary();
    }

    protected override void LoadMenu()
    {
        menuTitle = "Attack";
        Equippable[] equipments = turn.actor.transform.Find("Equipment Catalog").GetComponentsInChildren<Equippable>();
        int count = 1;

        for (int i = 0; i < equipments.Length; ++i)
        {
            if (equipments[i].GetComponentInChildren<Ability>() != false)
            {
                if (equipments[i].defaultSlots == EquipSlots.Primary || equipments[i].secondarySlots == EquipSlots.Secondary)
                    count++;
            }
        }

        if (menuOptions == null)
            menuOptions = new List<string>(count);
        else
            menuOptions.Clear();

        weaponOptions = new Ability[count];

        for (int i = 0; i < equipments.Length; ++i)
        {
            if (equipments[i].GetComponentInChildren<Ability>() != false)
            {
                if (equipments[i].slots == EquipSlots.Primary)
                {
                    menuOptions.Add(equipments[i].name);
                    weaponOptions[0] = equipments[i].GetComponentInChildren<Ability>();
                }
                if (equipments[i].slots == EquipSlots.Secondary)
                {
                    menuOptions.Add(equipments[i].name);
                    weaponOptions[1] = equipments[i].GetComponentInChildren<Ability>();
                }
            }
        }

        if(turn.actor.transform.Find("Unarmed Attack").GetComponent<Ability>() != null)
        {
            menuOptions.Add("Unarmed Attack");
            weaponOptions[weaponOptions.Length-1] = turn.actor.transform.Find("Unarmed Attack").GetComponent<Ability>();
        }

        abilityMenuPanelController.Show(menuTitle, menuOptions);
    }

    protected override void Confirm()
    {
        turn.ability = weaponOptions[abilityMenuPanelController.selection];
        owner.ChangeState<AbilityTargetState>();
    }

    protected override void Cancel()
    {
        owner.ChangeState<CategorySelectionState>();
    }
}
