using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class StatPanel : MonoBehaviour
{
    const string StatusPoolKey = "StatusIndicator.Entry";
    const int StatusCount = 13;

    public Panel panel;
    public GameObject statusParent;
	public Sprite allyBackground;
	public Sprite enemyBackground;
	public Image background;
	public Image avatar;
	public Text nameLabel;
	public Text hpLabel;
	public Text mpLabel;
	public Text gpLabel;
	public Text lvLabel;
	public Text joLabel;
	public Text raLabel;

    public void Display(GameObject obj)
	{
        Alliance alliance = obj.GetComponent<Alliance>();
        background.sprite = alliance.type == Alliances.Enemy ? enemyBackground : allyBackground;
        avatar.sprite = obj.GetComponentInChildren<Race>().avatar;
		nameLabel.text = obj.name;
		Stats stats = obj.GetComponent<Stats>();
		if(stats)
		{
			hpLabel.text = string.Format("HP {0} / {1}", stats[StatTypes.HP], stats[StatTypes.MHP]);
			mpLabel.text = string.Format("MP {0} / {1}", stats[StatTypes.MP], stats[StatTypes.MMP]);
			gpLabel.text = string.Format("GR {0} / {1}", stats[StatTypes.GP], stats[StatTypes.MGP]);
			lvLabel.text = string.Format("LV. {0}", stats[StatTypes.LVL]);
			joLabel.text = string.Format("{0}", obj.GetComponentInChildren<PerkCatalog>().transform.GetChild(0).name.First().ToString().ToUpper() + obj.GetComponentInChildren<PerkCatalog>().transform.GetChild(0).name.Substring(1).ToLower());
			raLabel.text = string.Format("{0}", obj.GetComponentInChildren<Race>().raceName);
		}
	}
}
