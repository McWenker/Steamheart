using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkTreeBridge : MonoBehaviour
{
    [SerializeField] Sprite deactivatedBridgeSprite;
    [SerializeField] Sprite activatedBridgeSprite;
    [SerializeField] PerkTreeVertex[] connectedPerks;
    Image image;
    bool active;

    public bool Active
    {
        get
        {
            return active;
        }

        set
        {
            image.sprite = value ? activatedBridgeSprite : deactivatedBridgeSprite;
            active = value;
        }
    }

    public void CheckPerks()
    {
        bool hasFoundMatch = false; // has a vertex at either end been learned?
        for(int i = 0; i < connectedPerks.Length; ++i)
        {
            if (connectedPerks[i].IsLearned)
            {
                hasFoundMatch = true;
                break;
            }                
        }
        Active = hasFoundMatch;
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }
}
