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
        StatTypes.DEF,
        StatTypes.MAG,
        StatTypes.MDF,
        StatTypes.SPD,
        StatTypes.EVD,
        StatTypes.RES,
        StatTypes.MOV,
        StatTypes.JMP
    };

    [SerializeField] TMP_ColorGradient gold;
    [SerializeField] TMP_ColorGradient red;

    [SerializeField] DemoPartyBuilder partyBuilder;

    [SerializeField] TMP_InputField inputArea;
    [SerializeField] TextMeshProUGUI nameNotice;
    [SerializeField] TextMeshProUGUI racialNotice;
    [SerializeField] TextMeshProUGUI classNotice;

    [SerializeField] Text racialText;
    [SerializeField] Text cardinalText;
    [SerializeField] Text perkText;
    [SerializeField] GameObject heroPrefab;
    [SerializeField] Text[] statSheet;
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
    Dictionary<string, bool> unitPerkDict = new Dictionary<string, bool>();

    string UnitName
    {
        set
        {
            unitName = value;
            unitInfo[0] = unitName;
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

    string UnitLevel
    {
        set
        {
            unitLevel = value;
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
            partyBuilder.NewUnit(unitInfo);
            gameObject.SetActive(false);
            partyBuilder.gameObject.SetActive(true);
            ResetUnit();
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

    public void ResetUnit()
    {
        if (unit != null)
            Destroy(unit);
        CreateUnitGameObject();
        for(int i = 0; i < unitInfo.Length; i++)
        {
            unitInfo[i] = null;
        }
        foreach(var key in unitPerkDict.Keys.ToList())
        {
            unitPerkDict[key] = false;
        }
        inputArea.text = "";
        fallBackRaceDesc = "";
        fallBackCardinalDesc = "";
        fallBackPerkDesc = "";
        ResetDesc("race");
        ResetDesc("cardinal");
        ResetDesc("perk");
    }

    public void AddPerk(string perkName)
    {
        if(unitPerkDict.ContainsKey(perkName))
        {
            unitPerkDict[perkName] = true;
            fallBackPerkDesc = descriptionDict[perkName];
            perkText.text = descriptionDict[perkName];
            /*string perkFile = string.Format("Perks/{0}/{1}", cardinalName, perkName);
            GameObject perkPrefab = Resources.Load<GameObject>(perkFile);
            if (perkPrefab == null)
            {
                Debug.LogError("No Perk Found: " + perkFile);
                return;
            }
            GameObject perkInstance = Instantiate(perkPrefab);
            perkInstance.transform.SetParent(unit.transform.Find("Perk Catalog"));
            perkInstance.name = perkName;
            Perk thisPerk = perkInstance.GetComponent<Perk>();
            thisPerk.Employ();
            thisPerk.LoadDefaultStats();*/
        }
        RefreshPerkTree();
    }

    public void RemovePerk(string perkName)
    {
        if (unitPerkDict.ContainsKey(perkName))
        {
            unitPerkDict[perkName] = false;
            foreach(Perk p in unit.GetComponentsInChildren<Perk>())
            {
                if(p.perkName == perkName)
                {
                    p.UnloadDefaultStats();
                    p.UnEmploy();
                    Destroy(p.gameObject);
                }
            }
        }
        RefreshPerkTree();
    }

    public void SetCardinal(string cardinalName)
    {
        if(this.cardinalName == cardinalName) //unselect
        {
            CardinalName = null;
            fallBackCardinalDesc = "";
            cardinalText.text = "";
            RemovePerk(cardinalName);
        }
        if (descriptionDict.ContainsKey(cardinalName))
        {
            CardinalName = cardinalName;
            fallBackCardinalDesc = descriptionDict[cardinalName];
            cardinalText.text = descriptionDict[cardinalName];
            AddPerk(cardinalName);
        }
        SetPerkTree();
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
        if (descriptionDict.ContainsKey(perkName))
        {
            perkText.text = descriptionDict[perkName];
        }
    }

    public void SetRace(string raceName)
    {
        if(descriptionDict.ContainsKey(raceName))
        {
            RaceName = raceName;
            fallBackRaceDesc = descriptionDict[raceName];
            racialText.text = descriptionDict[raceName];
        }
    }

    public void SetRacialDesc(string raceName)
    {
        if (descriptionDict.ContainsKey(raceName))
        {
            racialText.text = descriptionDict[raceName];
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
        ResetUnit();
    }

    private void PopulateDictionaries()
    {
        descriptionDict.Clear();
        unitPerkDict.Clear();
        foreach (TextAsset txt in Resources.LoadAll("Texts/"))
        {
            descriptionDict.Add(txt.name, txt.text);
        }

        foreach (TextAsset txt in Resources.LoadAll("Texts/Perks/"))
        {
            unitPerkDict.Add(txt.name, false);
        }
    }

    private void SetPerkTree()
    {
        ResetPerkTree();
        if(cardinalName == null)
        {
            return;
        }
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
    }

    private void ResetPerkTree()
    {
        for (int i = 0; i < perkVertices.Length; ++i)
        {
            perkVertices[i].perkName = null;
            perkVertices[i].description = null;
            perkVertices[i].IsLearned = false;
        }
    }

    private void RefreshPerkTree()
    {
        for(int i = 0; i < perkVertices.Length; ++i)
        {
            perkVertices[i].RefreshBridges();
        }
    }

    private void Start()
    {
        inputArea.onEndEdit.AddListener(delegate { SetName(inputArea.text); });
        PopulateDictionaries();
    }

    private IEnumerator UnitCreatorNotice(GameObject notice)
    {
        notice.SetActive(true);
        yield return new WaitForSeconds(2f);
        notice.SetActive(false);
    }

    private void UpdateStatSheet()
    {
        if(stats != null)
        {
            for(int i = 0; i < Race.statOrder.Length; ++i)
            {
                statSheet[i].text = stats[Race.statOrder[i]].ToString();
            }
        }
    }
    #endregion
}
