using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusStatusEffect : StatusEffect
{
    Stats stats;

    void OnEnable()
    {
        stats = GetComponentInParent<Stats>();
        if (stats)
            this.AddObserver(OnCRTWillChange, Stats.WillChangeNotification(StatTypes.CRT), stats);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnCRTWillChange, Stats.WillChangeNotification(StatTypes.CRT), stats);
    }

    void OnCRTWillChange(object sender, object args)
    {
        ValueChangeException exc = args as ValueChangeException;
        MultDeltaModifier m = new MultDeltaModifier(0, 1.10f);
        exc.AddModifier(m);
    }
}