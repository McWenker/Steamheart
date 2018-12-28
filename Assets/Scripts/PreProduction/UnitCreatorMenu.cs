using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCreatorMenu : MonoBehaviour
{
    #region Fields / Properties
    static readonly StatTypes[] statOrder = new StatTypes[]
    {
        StatTypes.MHP,
        StatTypes.MMP,
        StatTypes.MGP,
        StatTypes.MEL,
        StatTypes.RNG,
        StatTypes.MAG,
        StatTypes.DEF,
        StatTypes.MDF,
        StatTypes.SPD,
        StatTypes.MOV,
        StatTypes.JMP,
        StatTypes.CRT,
        StatTypes.RES,
        StatTypes.EVD
    };

    static string[] cardinals = new string[]
    {
        "Adept",
        "Healer",
        "Levy",
        "Rogue"
    };

    [Serializable]
    class StatSheet
    {
        [SerializeField] string name;
        [SerializeField] public Text statText;
        [SerializeField] public Text bonusText;
    }

    [SerializeField] TMP_ColorGradient gold;
    [SerializeField] TMP_ColorGradient red;

    [SerializeField] DemoPartyBuilder partyBuilder;

    [SerializeField] TMP_InputField inputArea;
    [SerializeField] TextMeshProUGUI nameNotice;
    [SerializeField] TextMeshProUGUI racialNotice;
    [SerializeField] TextMeshProUGUI classNotice;
    [SerializeField] TextMeshProUGUI maxPartyNotice;

    [SerializeField] TextMeshProUGUI racialText;
    [SerializeField] TextMeshProUGUI cardinalText;
    [SerializeField] TextMeshProUGUI perkText;
    [SerializeField] GameObject heroPrefab;
    [SerializeField] StatSheet[] statSheet;

    [SerializeField] Color defaultColor;
    [SerializeField] Color positiveColor;
    [SerializeField] Color negativeColor;

    string fallBackRaceDesc;
    string fallBackCardinalDesc;
    string fallBackPerkDesc;
    string unitName;
    string raceName;
    string cardinalName;
    Cardinals unitCardinal;
    string unitLevel = "1";
    string[] unitInfo = new string[4];
    GameObject unit;
    Stats stats;

    [SerializeField] PerkTreeVertex[] perkVertices = new PerkTreeVertex[7];
    public Dictionary<Cardinals, string[]> perkImportDict = new Dictionary<Cardinals, string[]>();
    Dictionary<string, string> descriptionDict = new Dictionary<string, string>();
    public DictPerkBoolDict unitPerkDict = new DictPerkBoolDict();

    string UnitName
    {
        set
        {
            unitName = value;
            unitInfo[0] = unitName;
            unit.name = unitName;
        }
    }

    string RaceName
    {
        set
        {
            raceName = value;
            unitInfo[1] = raceName;
        }
    }

    string CardinalName
    {
        set
        {
            cardinalName = value;
            unitInfo[2] = cardinalName;
        }
    }

    int UnitLevel
    {
        get
        {
            return Int32.Parse(unitLevel);
        }
        set
        {
            unitLevel = value.ToString();
            unitInfo[3] = unitLevel;
        }
    }
    #endregion

    #region public
    public void CreateButton()
    {
        if (unitInfo[0] != null && unitInfo[1] != null && unitInfo[2] != null && inputArea.text != "")
        {
            if (unitInfo[3] == null)
                unitInfo[3] = "1";
            partyBuilder.NewUnit(unitName, raceName, cardinalName, unitPerkDict, UnitLevel);
            gameObject.SetActive(false);
            partyBuilder.gameObject.SetActive(true);
            ResetUnitCreator(false);
        }
        else
        {
            if (unitInfo[0] == null || inputArea.text == "")
            {
                StartCoroutine(UnitCreatorNotice(nameNotice.gameObject));
            }
            if (unitInfo[1] == null)
            {
                StartCoroutine(UnitCreatorNotice(racialNotice.gameObject));
            }
            if(unitInfo[2] == null)
            {
                StartCoroutine(UnitCreatorNotice(classNotice.gameObject));
            }            
        }
    }

    public void RemoveNameNotice()
    {
        nameNotice.gameObject.SetActive(false);
    }

    public void ResetDesc(string toReset)
    {
        if (toReset == "cardinal")
            cardinalText.text = fallBackCardinalDesc;
        if (toReset == "race")
            racialText.text = fallBackRaceDesc;
        if (toReset == "perk")
            perkText.text = fallBackPerkDesc;
    }

    public void ResetUnitCreator(bool resetCount)
    {
        int unitPerks = 0;
        if (unit != null)
            Destroy(unit);
        CreateUnitGameObject();
        for(int i = 0; i < unitInfo.Length; ++i)
        {
            unitInfo[i] = null;
        }
        foreach(var key in unitPerkDict.Keys.ToList())
        {
            if(unitPerkDict[key] == true)
            {
                unitPerks++;
            }
            unitPerkDict[key] = false;
        }

        if(resetCount)
            partyBuilder.DemoPerkCount -= unitPerks;

        foreach (ToggleGroup tg in GetComponentsInChildren<ToggleGroup>())
        {
            foreach (Toggle t in tg.gameObject.GetComponentsInChildren<Toggle>())
            {
                t.GetComponent<CreatorToggle>().affectPerk = false;
                t.isOn = false;
                t.GetComponent<CreatorToggle>().affectPerk = true;
            }
        }
        for (int j = 0; j < perkVertices.Length; ++j)
        {
            perkVertices[j].perkName = null;
            perkVertices[j].IsLearned = false;
            perkVertices[j].RefreshBridges();
        }
        UnitLevel = 0;
        CardinalName = null;
        inputArea.text = "";
        fallBackRaceDesc = "";
        fallBackCardinalDesc = "";
        fallBackPerkDesc = "";
        ResetDesc("race");
        ResetDesc("cardinal");
        ResetDesc("perk");
    }

    public bool AddPerk(string perkName, string cardinalName)
    {
        if (partyBuilder.DemoPerkCount == 15)
        {
            StartCoroutine(UnitCreatorNotice(maxPartyNotice.gameObject));
            return false;
        }
        partyBuilder.DemoPerkCount++;
        DictPerk dp = new DictPerk(perkName, cardinalName);
        unitPerkDict[dp] = true;
        fallBackPerkDesc = descriptionDict[perkName];
        perkText.text = descriptionDict[perkName];
        UnitFactory.AddPerk(unit, cardinalName, perkName);
        ++UnitLevel;
        RefreshPerkTree();
        StatSheetPerkHover(perkName);
        RefreshStatSheet();
        return true;
    }

    public void RemovePerk(string perkName, string cardinalName)
    {
        partyBuilder.DemoPerkCount--;
        DictPerk dp = new DictPerk(perkName, cardinalName);
        unitPerkDict[dp] = false;
        foreach(Perk p in unit.GetComponentsInChildren<Perk>())
        {
            if(p.perkName == perkName)
            {
                p.UnloadDefaultStats();
                p.UnEmploy();
                Destroy(p.gameObject);
                if(cardinals.Contains(perkName))
                {
                    cardinalName = "";
                    perkText.text = "";
                    cardinalText.text = "";
                    fallBackPerkDesc = "";
                    fallBackCardinalDesc = "";
                    ResetPerkTree();
                }
                if(unit.GetComponentsInChildren<Perk>().Length-1 == 1)
                {
                    //TODO: add a coroutine to allow a wait period after resetting perkDesc
                    if (cardinalName != "")
                    SetPerkDesc(cardinalName);
                    else
                        SetPerkDesc("");
                }
            }
        }
        --UnitLevel;
        RefreshPerkTree();
        StatSheetPerkHover(perkName);
        RefreshStatSheet();
    }

    public void RemoveRace(string raceName)
    {
        foreach(Race r in unit.GetComponentsInChildren<Race>())
        {
            r.UnloadDefaultStats();
            r.UnEmploy();
            Destroy(r.gameObject);
        }
        fallBackRaceDesc = "";
        ResetDesc("race");
        this.raceName = "";
        StatSheetRaceHover(raceName);
        RefreshStatSheet();
    }

    public void SetCardinal(string cardinalName)
    {
        if (descriptionDict.ContainsKey(cardinalName))
        {
            CardinalName = cardinalName;
            fallBackCardinalDesc = descriptionDict[cardinalName];
            cardinalText.text = descriptionDict[cardinalName];
            SetPerkDesc(cardinalName);
        }
        unitLevel = "0";
        SetPerkTree();
        RefreshStatSheet();
    }

    public void SetCardinalDesc(string cardinalName)
    {
        if (descriptionDict.ContainsKey(cardinalName))
        {
            cardinalText.text = descriptionDict[cardinalName];
        }
    }

    public void SetName(string unitName)
    {
        UnitName = unitName;
    }

    public void SetPerkDesc(string perkName)
    {
        if (descriptionDict.ContainsKey(perkName) && cardinalName != "")
        {
            perkText.text = descriptionDict[perkName];
        }
        else
            perkText.text = "";
    }

    public void SetOrResetRace(string raceName)
    {
        if(descriptionDict.ContainsKey(raceName))
        {
            Race thisRace = unit.GetComponentInChildren<Race>();
            if (thisRace != null)
            {
                if (thisRace.raceName != raceName)
                    Destroy(thisRace.gameObject);
            }
            RaceName = raceName;
            fallBackRaceDesc = descriptionDict[raceName];
            racialText.text = descriptionDict[raceName];
            UnitFactory.AddRace(unit, raceName);
        }
        StatSheetRaceHover(raceName);
        RefreshStatSheet();
    }

    public void SetRacialDesc(string raceName)
    {
        if (descriptionDict.ContainsKey(raceName))
        {
            racialText.text = descriptionDict[raceName];
        }
    }

    public void StatSheetPerkHover(string perkName)
    {
        if (cardinalName == "" || cardinalName == null)
            return;
        string perkFile;
        bool isCardinal;
        if (cardinals.Contains(perkName))
        {
            perkFile = string.Format("Perks/" + perkName + "/" + perkName);
            isCardinal = true;
        }
        else if(cardinalName != null && cardinalName != "")
        {
            perkFile = string.Format("Perks/" + cardinalName + "/" + perkName);
            isCardinal = false;
        }
        else
        {
            return;
        }

        GameObject thisPerk = UnitFactory.InstantiatePrefab(perkFile);
        if (thisPerk == null)
        {
            Debug.LogError("No Perk Found: " + name);
            return;
        }

        Perk holder = thisPerk.GetComponent<Perk>();
        for (int i = 0; i < statOrder.Length; ++i)
        {
            if (holder.statDict.ContainsKey(statOrder[i]))
            {
                int thisVal;
                // is the input perk a root cardinal perk? is it known?
                // if so, differential equals the loss of class
                if (isCardinal && perkName == cardinalName)
                    thisVal = -holder.statDict[statOrder[i]];
                // is the input perk a root cardinal? is it different from the current unit's cardinal?
                // if so, differential equals the change between classes
                else if (isCardinal && cardinalName != perkName)
                {
                    // does the current unit have a cardinal perk?
                    // if so, the differential equals change in classes
                    if (unit.GetComponentInChildren<Perk>() != null)
                    {
                        // does the current unit have the current stat modified?
                        // if so, the differential equals the change from taking input class
                        if (unit.GetComponentInChildren<Perk>().statDict.ContainsKey(statOrder[i]))
                            thisVal = holder.statDict[statOrder[i]] - unit.GetComponentInChildren<Perk>().statDict[statOrder[i]];
                        else
                            thisVal = holder.statDict[statOrder[i]];
                    }
                    // otherwise, the differential equals the bonus from taking class
                    else
                    {
                        thisVal = holder.statDict[statOrder[i]];
                    }
                        
                }
                // otherwise, the input perk is a tier perk
                else
                {
                    bool perkKnown = false;
                    foreach (Perk p in unit.GetComponentsInChildren<Perk>())
                    {
                        if (p.perkName == perkName)
                        {
                            perkKnown = true;
                            break;
                        }
                    }
                    thisVal = perkKnown ? -holder.statDict[statOrder[i]] : holder.statDict[statOrder[i]];
                }

                if (thisVal != 0)
                {
                    statSheet[i].bonusText.color = thisVal > 0 ? positiveColor : negativeColor;
                    statSheet[i].bonusText.text = thisVal > 0 ? "+" + thisVal.ToString() : thisVal.ToString();
                }
                else
                {
                    statSheet[i].bonusText.color = defaultColor;
                    statSheet[i].bonusText.text = "-";
                }
            }
            else
            {
                statSheet[i].bonusText.color = defaultColor;
                statSheet[i].bonusText.text = "-";
            }
        }
        Destroy(thisPerk);
    }

    public void StatSheetRaceHover(string raceName)
    {
        string raceFile = string.Format("Races/"+ raceName);
        GameObject thisRace = UnitFactory.InstantiatePrefab(raceFile);
        if (thisRace == null)
        {
            Debug.LogError("No Race Found: " + name);
            return;
        }
        Race holder = thisRace.GetComponent<Race>();
        for (int i = 0; i < statOrder.Length; ++i)
        {
            // check if this race modifies this stat
            if(holder.baseStatDict.ContainsKey(statOrder[i]))
            {
                // holder value for the stat differential
                int thisVal;
                if (this.raceName != raceName && unit.GetComponentInChildren<Race>() != null)
                {
                    if (this.raceName == null || this.raceName == "")
                        thisVal = holder.baseStatDict[statOrder[i]];
                    else if (unit.GetComponentInChildren<Race>().baseStatDict.ContainsKey(statOrder[i]))
                        thisVal = holder.baseStatDict[statOrder[i]] - unit.GetComponentInChildren<Race>().baseStatDict[statOrder[i]];
                    else
                        thisVal = holder.baseStatDict[statOrder[i]];
                }
                else if(this.raceName == raceName)
                    thisVal = -holder.baseStatDict[statOrder[i]];
                else
                    thisVal = holder.baseStatDict[statOrder[i]];


                if (thisVal != 0)
                {
                    statSheet[i].bonusText.color = thisVal > 0 ? positiveColor : negativeColor;
                    statSheet[i].bonusText.text = thisVal > 0 ? "+" + thisVal.ToString() : thisVal.ToString();
                }
                else
                {
                    statSheet[i].bonusText.color = defaultColor;
                    statSheet[i].bonusText.text = "-";
                }
            }
            else
            {
                statSheet[i].bonusText.color = defaultColor;
                statSheet[i].bonusText.text = "-";
            }
        }
        Destroy(thisRace);
    }

    public void ResetStatSheetHover()
    {
        for(int i = 0; i < statSheet.Length; ++i)
        {
            statSheet[i].bonusText.text = "";
        }
    }
    #endregion

    #region Private
    private void CreateUnitGameObject()
    {
        unit = Instantiate(heroPrefab);
        unit.name = unitName;
        unit.AddComponent<Unit>();
        stats = unit.AddComponent<Stats>();
        unit.AddComponent<Status>();
        unit.AddComponent<Health>();
        unit.AddComponent<Mana>();
        GameObject abCatalog = new GameObject("Ability Catalog");
        GameObject peCatalog = new GameObject("Perk Catalog");
        abCatalog.transform.SetParent(unit.transform);
        abCatalog.AddComponent<AbilityCatalog>();
        peCatalog.transform.SetParent(unit.transform);
        peCatalog.AddComponent<PerkCatalog>();
    }

    private void OnEnable()
    {
        ResetUnitCreator(false);
        RefreshPerkTree();
    }

    private void PopulateDictionaries()
    {
        descriptionDict.Clear();
        unitPerkDict.Clear();
        foreach (TextAsset txt in Resources.LoadAll("Texts/"))
        {
            descriptionDict.Add(txt.name, txt.text);
        }

        foreach (TextAsset txt in Resources.LoadAll("Texts/Perks/Adept/"))
        {
            DictPerk dp = new DictPerk(txt.name.Trim(new char[] { '_' }), "Adept");

            //Debug.Log(dp.PerkName + ", " + dp.CardinalName);
            unitPerkDict.Add(dp, false);
        }
        foreach (TextAsset txt in Resources.LoadAll("Texts/Perks/Healer/"))
        {
            DictPerk dp = new DictPerk(txt.name.Trim(new char[] { '_' }), "Healer");

            //Debug.Log(dp.PerkName + ", " + dp.CardinalName);
            unitPerkDict.Add(dp, false);
        }
        foreach (TextAsset txt in Resources.LoadAll("Texts/Perks/Levy/"))
        {
            DictPerk dp = new DictPerk(txt.name.Trim(new char[] { '_' }), "Levy");

            //Debug.Log(dp.PerkName + ", " + dp.CardinalName);
            unitPerkDict.Add(dp, false);
        }
        foreach (TextAsset txt in Resources.LoadAll("Texts/Perks/Rogue/"))
        {
            DictPerk dp = new DictPerk(txt.name.Trim(new char[] { '_' }), "Rogue");

            //Debug.Log(dp.PerkName + ", " + dp.CardinalName);
            unitPerkDict.Add(dp, false);
        }
    }

    private void SetPerkTree()
    {
        ResetPerkTree();
        if(cardinalName == null)
        {
            return;
        }
        AddPerk(cardinalName, cardinalName);
        unitCardinal = (Cardinals)Enum.Parse(typeof(Cardinals), cardinalName);
        for (int i = 0; i < perkVertices.Length; ++i)
        {
            perkVertices[i].perkCardinal = unitCardinal;
            perkVertices[i].perkName = perkImportDict[unitCardinal][i];
            perkVertices[i].description = descriptionDict[perkVertices[i].perkName];
        }
        perkText.text = perkVertices[0].description;
        fallBackPerkDesc = perkVertices[0].description;
        RefreshPerkTree();
        RefreshStatSheet();
    }

    private void ResetPerkTree()
    {
        int unitPerks = 0;
        for (int i = 0; i < perkVertices.Length; ++i)
        {
            perkVertices[i].perkName = null;
            perkVertices[i].description = null;
            perkVertices[i].IsLearned = false;
        }
        foreach (var key in unitPerkDict.ToList())
        {
            if (unitPerkDict[key.Key] == true)
                unitPerks++;
            unitPerkDict[key.Key] = false;
        }
        partyBuilder.DemoPerkCount -= unitPerks;
    }

    private void RefreshPerkTree()
    {
        for(int i = 0; i < perkVertices.Length; ++i)
        {
            perkVertices[i].RefreshBridges();
        }
    }

    private void RefreshStatSheet()
    {
        if (stats != null)
        {
            for (int i = 0; i < statOrder.Length; ++i)
            {
                statSheet[i].statText.text = stats[statOrder[i]].ToString();
            }
        }
    }

    private void Start()
    {
        inputArea.onEndEdit.AddListener(delegate { SetName(inputArea.text); });
        PopulateDictionaries();
    }

    private IEnumerator UnitCreatorNotice(GameObject notice)
    {
        nameNotice.gameObject.SetActive(false);
        racialNotice.gameObject.SetActive(false);
        classNotice.gameObject.SetActive(false);
        maxPartyNotice.gameObject.SetActive(false);
        notice.SetActive(true);
        yield return new WaitForSeconds(2f);
        notice.SetActive(false);
    }
    #endregion
}
