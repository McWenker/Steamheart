using UnityEngine;
using System.Collections;

public class StopStatusEffect : StatusEffect
{
    public override Sprite Icon
    {
        get { return img; }
    }

    readonly Sprite img = Resources.Load<Sprite>("Status Icons/stun.png");

    Stats myStats;

	void OnEnable()
	{
		this.AddObserver(OnAutomaticHitCheck, HitRate.AutomaticHitCheckNotification);
		myStats = GetComponentInParent<Stats>();
		if(myStats)
			this.AddObserver(OnCounterWillChange, Stats.WillChangeNotification(StatTypes.CTR), myStats);
	}

	void OnDisable()
	{
		this.RemoveObserver(OnAutomaticHitCheck, HitRate.AutomaticHitCheckNotification);
		this.RemoveObserver(OnCounterWillChange, Stats.WillChangeNotification(StatTypes.CTR), myStats);
	}

	// handler method for auto-hit notification
	void OnAutomaticHitCheck(object sender, object args)
	{
		Unit owner = GetComponentInParent<Unit>();
		MatchException exc = args as MatchException;
		if(owner == exc.target)
			exc.FlipToggle();
	}

	// handler method for speed counter notification
	void OnCounterWillChange(object sender, object args)
	{
		ValueChangeException exc = args as ValueChangeException;
		exc.FlipToggle();
	}
}
