using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatorToggle : MonoBehaviour
{
    Toggle toggle;
    ColorBlock offBlock = new ColorBlock();
    ColorBlock onBlock = new ColorBlock();
    [SerializeField] Color onColor;
    [SerializeField] UnitCreatorMenu creatorMenu;
    [SerializeField] bool forRace;
    
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        offBlock = toggle.colors;
        onBlock.normalColor = onColor;
        onBlock.highlightedColor = onColor;
        onBlock.pressedColor = onColor;

        offBlock.colorMultiplier = 1;
        onBlock.colorMultiplier = 1;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        toggle.colors = isOn ? onBlock : offBlock;
        if (isOn)
        {
            if(forRace)
                creatorMenu.SetOrResetRace(name);
            else
                creatorMenu.SetCardinal(name);
        }
        else
        {
            if (forRace)
                creatorMenu.RemoveRace(name);
            else
                creatorMenu.RemovePerk(name, name);
        }
    }
}
