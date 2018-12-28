using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoundStatusEffect : StatusEffect
{
    Stats stats;

    void OnEnable()
    {
        stats = GetComponentInParent<Stats>();
        if (stats)
            this.AddObserver(OnMHPWillChange, Stats.WillChangeNotification(StatTypes.MHP), stats);
    }

    void OnDisable()
    {
        if(stats)
            this.RemoveObserver(OnMHPWillChange, Stats.WillChangeNotification(StatTypes.MHP), stats);
    }

    void OnMHPWillChange(object sender, object args)
    {
        ValueChangeException exc = args as ValueChangeException;
        MultDeltaModifier m = new MultDeltaModifier(0, 0.75f);
        exc.AddModifier(m);
    }
}
