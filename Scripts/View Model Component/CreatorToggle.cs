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
    }
}
