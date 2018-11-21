using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkTreeVertex : MonoBehaviour
{
    public string perkName;
    public Cardinals perkCardinal;
    public string description;
    [SerializeField] UnitCreatorMenu unitCreatorMenu;
    [SerializeField] PerkTreeVertex[] reqPerks;
    [SerializeField] PerkTreeVertex[] subPerks;
    [SerializeField] Image icon;
    [SerializeField] PerkTreeBridge[] bridges;
    [SerializeField] Sprite deactivatedPerkSprite;
    [SerializeField] Sprite activatedPerkSprite;
    [SerializeField] ToggleGroup cardinals;
    [SerializeField] bool isBase;
    Toggle toggle;
    bool isLearned = false;
    bool canBeUnLearned = true;
    //public GameObject perkPrefab;

    public bool IsAvailable
    {
        get
        {
            return CheckAvailability();
        }
    }

    public bool IsLearned
    {
        get
        {
            return isLearned;
        }
        set
        {
            if (!canBeUnLearned && value == false)
                return;
            isLearned = value;
            ChangeSprite(value);            
        }
    }

    public void ChangeDesc()
    {
        if(perkName != "" && perkName != null)
            unitCreatorMenu.SetPerkDesc(perkName);
    }

    public void Click()
    {
        if(CheckAvailability())
        {
            if (perkName != "" && perkName != null)
            {
                if (!isLearned)
                {
                    IsLearned = unitCreatorMenu.AddPerk(perkName, perkCardinal.ToString());
                }
                else if (canBeUnLearned)
                {
                    if (CheckSubPerks())
                    {
                        IsLearned = false;
                        unitCreatorMenu.RemovePerk(perkName, perkCardinal.ToString());
                    }
                }
            }
        }
    }

    public bool HasOtherSupport(PerkTreeVertex reqToCheck)
    {
        if (reqPerks.Length == 0 || bridges.Length == 0)
            return true;
        for (int i = 0; i < reqPerks.Length; ++i)
        {
            if(reqPerks[i] != reqToCheck)
            {
                if (reqPerks[i].IsLearned)
                    return true;
            }
        }
        return false;
    }

    public void RefreshBridges()
    {
        if (bridges.Length == 0)
            return;
        for(int i = 0; i < bridges.Length; ++i)
        {
            bridges[i].CheckPerks();
        }
    }

    public void StatSheet()
    {
        if(perkName != "" && perkName != null)
            unitCreatorMenu.StatSheetPerkHover(perkName);
    }

    void ChangeSprite(bool learned)
    {
        if(isBase)
        {
            icon.sprite = activatedPerkSprite;
            icon.color = Color.white;
        }
        else if (learned && canBeUnLearned)
        {
            icon.sprite = activatedPerkSprite;
            icon.color = cardinals.transform.Find(perkCardinal.ToString()).GetComponent<Toggle>().colors.pressedColor;
        }
        else if (!learned)
        {
            icon.sprite = deactivatedPerkSprite;
            icon.color = Color.white;
        }
    }

    // Returns true if any prerequisite Perks are learned
    bool CheckAvailability()
    {
        if (reqPerks.Length == 0 || bridges.Length == 0)
            return true;
        for (int i = 0; i < reqPerks.Length; ++i)
        {
            if (reqPerks[i].IsLearned)
                return true;
        }
        return false;
    }

    // Returns true if there are no subsequent Perks requiring this one
    bool CheckSubPerks()
    {
        if (subPerks.Length == 0)
            return true;
        else
        {
            for (int i = 0; i < subPerks.Length; ++i)
            {
                if (subPerks[i].IsLearned && !subPerks[i].HasOtherSupport(this))
                    return false;
            }
        }        
        return true;
    }

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        if (toggle.isOn)
        {
            IsLearned = true;
            canBeUnLearned = false;
        }
    }
}
