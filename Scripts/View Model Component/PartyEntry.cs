using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyEntry : MonoBehaviour
{
    [SerializeField] Sprite avatar;
    [SerializeField] TextMeshProUGUI label;
    DemoPartyBuilder partyBuilder;
    string unitName;
    string raceName;
    string className;
    string unitLevel;
    string[] unitInfo = new string[4];

    public string[] UnitInfo
    {
        get { return unitInfo; }
        set
        {
            unitName = value[0];
            raceName = value[1];
            className = value[2];
            unitLevel = value[3];
            for(int i = 0; i < unitInfo.Length; ++i)
            {
                unitInfo[i] = value[i];
            }
            label.SetText(unitName + ", " + raceName + " " + className + " " + unitLevel);
        }
    }

    public void ButtonPress()
    {
        if (partyBuilder.DeleteActive)
        {
            partyBuilder.RemoveUnit(this);
        }
        else
        {
            return;
        }
    }

    void Reset()
    {
        UnitInfo = new string[] { null, null, null, null };
    }

    private void Start()
    {
        partyBuilder = GetComponentInParent<DemoPartyBuilder>();
    }
}
