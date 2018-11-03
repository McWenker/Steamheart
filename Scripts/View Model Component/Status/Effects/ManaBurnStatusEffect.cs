using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBurnStatusEffect : StatusEffect
{
    Unit owner;

    void OnEnable()
    {
        owner = GetComponentInParent<Unit>();
        if (owner)
        {
            this.AddObserver(OnNewTurn, TurnOrderController.TurnBeganNotification, owner);
        }
    }

    void OnDisable()
    {
        this.RemoveObserver(OnNewTurn, TurnOrderController.TurnBeganNotification, owner);
    }

    void OnNewTurn(object sender, object args)
    {
        Stats s = GetComponentInParent<Stats>();
        int currentMP = s[StatTypes.MP];
        int maxMP = s[StatTypes.MMP];
        int reduce = Mathf.Min(currentMP, maxMP / 15);
        s.SetValue(StatTypes.HP, (currentMP - reduce), false);
    }
}
