using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunderStatusEffect : StatusEffect
{
    Stats stats;

    void OnEnable()
    {
        stats = GetComponentInParent<Stats>();
        if (stats)
            this.AddObserver(OnDEFWillChange, Stats.WillChangeNotification(StatTypes.DEF), stats);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnDEFWillChange, Stats.WillChangeNotification(StatTypes.DEF), stats);
    }

    void OnDEFWillChange(object sender, object args)
    {
        ValueChangeException exc = args as ValueChangeException;
        MultDeltaModifier m = new MultDeltaModifier(0, 0.75f);
        exc.AddModifier(m);
    }
}
