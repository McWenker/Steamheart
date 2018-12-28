using UnityEngine;
using System.Collections;

public abstract class AddStatusFeature<T> : Feature where T : StatusEffect
{
	#region Fields
	StatusCondition statusCondition;
    [SerializeField] int rank;
	#endregion

	#region Protected
	protected override void OnApply()
	{
		Status status = GetComponentInParent<Status>();
		statusCondition = status.Add<T, StatusCondition>(rank);
	}

	protected override void OnRemove()
	{
		if(statusCondition != null)
			statusCondition.Remove();
	}
	#endregion
}
