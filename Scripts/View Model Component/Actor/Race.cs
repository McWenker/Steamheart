using UnityEngine;
using System.Collections;

public class Race : MonoBehaviour
{
    #region Fields / Properties
    public static readonly StatTypes[] statOrder = new StatTypes[]
    {
        StatTypes.MHP,
        StatTypes.MMP,
        StatTypes.MGP,
        StatTypes.MEL,
        StatTypes.RNG,
        StatTypes.DEF,
        StatTypes.MAG,
        StatTypes.MDF,
        StatTypes.SPD,
        StatTypes.EVD,
        StatTypes.RES,
        StatTypes.MOV,
        StatTypes.JMP
    };
    public StatIntDict baseStatDict;
    public StatIntDict growthStatDict;
	public string raceName;
    public MovementTypes moveType;
    public Sprite avatar;

	Stats stats;
	#endregion

	#region Public
	public void Employ()
	{
		stats = gameObject.GetComponentInParent<Stats>();
        this.AddObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);
        

        Feature[] features = GetComponentsInChildren<Feature>();
		for(int i = 0; i < features.Length; ++i)
			features[i].Activate (gameObject);
	}

    public void LoadDefaultStats()
    {
        for (int i = 0; i < statOrder.Length; ++i)
        {
            StatTypes type = statOrder[i];
            if (baseStatDict.ContainsKey(type))
            {
                stats.AddValue(type, baseStatDict[type], false);
            }
        }

        stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
        stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
        stats.SetValue(StatTypes.GP, stats[StatTypes.MGP], false);
    }
    #endregion

    #region Event Handlers
	protected virtual void OnLvlChangeNotification (object sender, object args)
	{
		int oldValue = (int)args;
		int newValue = stats[StatTypes.LVL];

		for (int i = oldValue; i < newValue; ++i)
			LevelUp();
	}
	#endregion

	#region Private
	void LevelUp()
	{
		for(int i = 0; i < statOrder.Length; ++i)
		{
			StatTypes type = statOrder[i];
            if (growthStatDict.ContainsKey(type))
            {
                int whole = Mathf.FloorToInt(growthStatDict[type]);
                float fraction = growthStatDict[type] - whole;

                int value = stats[type];
                value += whole;
                if (UnityEngine.Random.value > (1f - fraction))
                    value++;

                stats.SetValue(type, value, false);
            }
		}

		stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
		stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
		stats.SetValue(StatTypes.GP, stats[StatTypes.MGP], false);
	}
	#endregion
}
