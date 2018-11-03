using UnityEngine;
using System.Collections;

public class Perk : MonoBehaviour
{
    #region Fields / Properties
    public static readonly StatTypes[] statOrder = new StatTypes[]
    {
        StatTypes.MHP,
        StatTypes.MMP,
        StatTypes.MGP,
        StatTypes.MEL,
        StatTypes.RNG,
        StatTypes.MAG,
        StatTypes.DEF,
        StatTypes.MDF,
        StatTypes.CRT,
        StatTypes.EVD,
        StatTypes.RES,
        StatTypes.SPD,
        StatTypes.MOV,
        StatTypes.JMP
    };
    public StatIntDict statDict;
	public string perkName;
    public Ability[] learnedAbilities;

    Stats stats;
    #endregion

    #region MonoBehaviour
    /*void OnDestroy()
	{
		this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL));
	}*/
    #endregion

    #region Public
    public void Employ()
	{
		stats = gameObject.GetComponentInParent<Stats>();
		//this.AddObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);

		Feature[] features = GetComponentsInChildren<Feature>();
		for(int i = 0; i < features.Length; ++i)
			features[i].Activate (transform.parent.parent.gameObject);

        Transform catalog = transform.parent.parent.Find("Ability Catalog");
        for (int j = 0; j < learnedAbilities.Length; ++j)
        {
            GameObject thisAbility = Instantiate(learnedAbilities[j].gameObject);
            Transform abilitySchool = catalog.Find(learnedAbilities[j].abilitySchool);
            GameObject aSchool = abilitySchool == null ? new GameObject() : abilitySchool.gameObject;
            if (abilitySchool == null)
            {
                aSchool.name = learnedAbilities[j].abilitySchool;
            }
            aSchool.transform.parent = catalog;
            thisAbility.transform.parent = aSchool.transform;
            thisAbility.name = learnedAbilities[j].name;
        }
	}

	public void UnEmploy()
	{
		Feature[] features = GetComponentsInChildren<Feature>();
		for(int i = 0; i < features.Length; ++i)
			features[i].Deactivate();

        Transform catalog = transform.parent.parent.Find("Ability Catalog");
        for (int j = 0; j < learnedAbilities.Length; ++j)
        {
            foreach(Transform t in catalog)
            {
                foreach(Transform v in t)
                {
                    if (v.GetComponent<Ability>() != null)
                    {
                        if (v.GetComponent<Ability>().name == learnedAbilities[j].name)
                        {
                            v.parent = null;
                            Destroy(v.gameObject);
                            if (t.childCount == 0)
                                Destroy(t.gameObject);
                        }
                    }
                }
            }
        }

        //this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(StatTypes.LVL), stats);
        stats = null;
	}

	public void LoadDefaultStats()
	{
		for(int i = 0; i < statOrder.Length; ++i)
		{
			StatTypes type = statOrder[i];
            if(statDict.ContainsKey(type))
            {
                stats.AddValue(type, statDict[type], false);
            }
		}

		stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
		stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
		stats.SetValue(StatTypes.GP, stats[StatTypes.MGP], false);
	}

    public void UnloadDefaultStats()
    {
        for (int i = 0; i < statOrder.Length; ++i)
        {
            StatTypes type = statOrder[i];
            if (statDict.ContainsKey(type))
            {
                stats.AddValue(type, -statDict[type], false);
            }
        }

        stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
        stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
        stats.SetValue(StatTypes.GP, stats[StatTypes.MGP], false);
    }
	#endregion
}