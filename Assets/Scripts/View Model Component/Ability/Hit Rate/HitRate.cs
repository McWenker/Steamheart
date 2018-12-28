using UnityEngine;
using System.Collections;

public abstract class HitRate : MonoBehaviour
{
	#region Notifications
	/// <summary>
	/// Includes a toggleable MatchException argument which defaults "false"
	/// </summary>
	public const string AutomaticHitCheckNotification = "HitRate.AutomaticHitCheckNotification";

	/// <summary>
	/// Includes a toggleable MatchException argument which defaults "false"
	/// </summary>
	public const string AutomaticMissCheckNotification = "HitRate.AutomaticMissCheckNotification";

	/// <summary>
	/// Includes an Info argument with three parameters: Attacker (Unit), Target (Unit),
	/// and Target's calculated Evade/Resistance(int). Status effects which affect hit rate
	/// should modify the arg2 parameter
	/// </summary>
	public const string StatusCheckNotification = "HitRate.StatusCheckNotification";
	#endregion

	#region Fields
	public virtual bool IsAngleBased { get { return true; }}
	protected Unit attacker;
	#endregion

	#region MonoBehaviour
	protected virtual void Start()
	{
		attacker = GetComponentInParent<Unit>();
	}
	#endregion

	#region Public
	/// <summary>
	/// Returns a value in the range of 0 to 100 as a percent chance of an ability
	/// succeeding to hit
	/// </summary>
	public abstract int Calculate(Tile target);

	public virtual bool RollForHit(Tile target)
	{
		int roll = UnityEngine.Random.Range(0,101);
		int chance = Calculate(target);
		return roll <= chance;
	}
	#endregion

	#region Protected
	protected virtual bool AutomaticHit(Unit target)
	{
		MatchException exc = new MatchException(attacker, target);
		this.PostNotification(AutomaticHitCheckNotification, exc);
		return exc.toggle;
	}

	protected virtual bool AutomaticMiss(Unit target)
	{
		MatchException exc = new MatchException(attacker, target);
		this.PostNotification(AutomaticMissCheckNotification, exc);
		return exc.toggle;
	}

	protected virtual int AdjustForStatusEffects(Unit target, int rate)
	{
		Info<Unit, Unit, int> args = new Info<Unit, Unit, int>(attacker, target, rate);
		this.PostNotification(StatusCheckNotification, args);
		return args.arg2;
	}

	protected virtual int Final(int evade)
	{
		return 100 - evade;
	}
	#endregion
}