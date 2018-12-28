using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grit : MonoBehaviour
{
    #region Fields
    public int GP
    {
        get { return stats[StatTypes.GP]; }
        set { stats[StatTypes.HP] = value; }
    }

    public int MGP
    {
        get { return stats[StatTypes.MGP]; }
        set { stats[StatTypes.MGP] = value; }
    }

    Unit unit;
    Stats stats;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        stats = GetComponent<Stats>();
        unit = GetComponent<Unit>();
    }

    void OnEnable()
    {
        this.AddObserver(OnGPWillChange, Stats.WillChangeNotification(StatTypes.GP), stats);
        this.AddObserver(OnMGPDidChange, Stats.DidChangeNotification(StatTypes.MGP), stats);
        this.AddObserver(OnTurnBegan, TurnOrderController.TurnBeganNotification, unit);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnGPWillChange, Stats.WillChangeNotification(StatTypes.GP), stats);
        this.RemoveObserver(OnMGPDidChange, Stats.DidChangeNotification(StatTypes.MGP), stats);
        this.RemoveObserver(OnTurnBegan, TurnOrderController.TurnBeganNotification, unit);
    }
    #endregion

    #region Event Handlers
    void OnGPWillChange(object sender, object args)
    {
        ValueChangeException vce = args as ValueChangeException;
        vce.AddModifier(new ClampValueModifier(int.MaxValue, 0, stats[StatTypes.MGP]));
    }

    void OnMGPDidChange(object sender, object args)
    {
        int oldMGP = (int)args;
        if (MGP > oldMGP)
            GP += MGP - oldMGP;
        else
            GP = Mathf.Clamp(GP, 0, MGP);
    }

    void OnTurnBegan(object sender, object args)
    {
        if (GP < MGP)
            GP += Mathf.Max(Mathf.FloorToInt(MGP * 0.1f), 1);
    }
    #endregion
}
