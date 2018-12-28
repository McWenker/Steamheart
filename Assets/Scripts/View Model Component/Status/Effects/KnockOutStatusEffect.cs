using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockOutStatusEffect : StatusEffect
{
    Unit owner;
    Stats stats;

    private void Awake()
    {
        owner = GetComponentInParent<Unit>();
        stats = owner.GetComponent<Stats>();
    }

    private void OnEnable()
    {
        owner.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        this.AddObserver(OnTurnCheck, TurnOrderController.TurnCheckNotification, owner);
        this.AddObserver(OnStatCounterWillChange, Stats.WillChangeNotification(StatTypes.CTR), stats);
    }

    private void OnDisable()
    {
        owner.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        this.RemoveObserver(OnTurnCheck, TurnOrderController.TurnCheckNotification, owner);
        this.RemoveObserver(OnStatCounterWillChange, Stats.WillChangeNotification(StatTypes.CTR), stats);
    }

    private void OnTurnCheck(object sender, object args)
    {
        // KO'd unit takes no turns
        BaseException exc = args as BaseException;
        if (exc.defaultToggle == true)
            exc.FlipToggle();
    }

    private void OnStatCounterWillChange(object sender, object args)
    {
        // KO'd unit gains no turn counter
        ValueChangeException exc = args as ValueChangeException;
        if (exc.toValue > exc.fromValue)
            exc.FlipToggle();
    }
}
