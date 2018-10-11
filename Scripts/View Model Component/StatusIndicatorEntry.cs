using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicatorEntry : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Text counter;
    [SerializeField] Text rank;

    public Sprite Img
    {
        get { return img.sprite; }
        set { img.sprite = value; }
    }

    public string Title
    {
        get { return img.name; }
        set { img.name = value; }
    }

    public string Duration
    {
        get { return counter.text; }
        set { counter.text = value; }
    }

    public string Level
    {
        get { return rank.text; }
        set { rank.text = value; }
    }

    public GameObject Counter
    {
        get { return counter.gameObject; }
    }
}
