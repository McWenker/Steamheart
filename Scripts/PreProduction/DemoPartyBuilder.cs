using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class DemoPartyBuilder : MonoBehaviour
{
    const string DemoUnitKey = "DemoParty.Entry";
    const int DemoPartySize = 7;

    [SerializeField] GameObject PartyListPrefab;
    [SerializeField] GameObject partyNotice;
    [SerializeField] UnitCreatorMenu unitCreatorMenu;
    [SerializeField] TMP_ColorGradient gold;
    [SerializeField] TMP_ColorGradient red;
    [SerializeField] GridLayoutGroup panel;
    [SerializeField] TextMeshProUGUI deleteThis;
    List<PartyEntry> partyEntries = new List<PartyEntry>(DemoPartySize);
    bool deleteActive = false;

    public bool DeleteActive
    {
        get { return deleteActive; }
    }

    #region Public
    public void AddUnit()
    {
        if (partyEntries.Count == DemoPartySize)
            StartCoroutine(FullPartyNotice());
        else
        {
            gameObject.SetActive(false);
            unitCreatorMenu.gameObject.SetActive(true);
        }
    }

    public void DeleteButton()
    {
        deleteActive = true;
    }

    public void NewUnit(string unitName, string raceName, string cardinalName, DictPerkBoolDict perkDict, int level)
    {
        PartyEntry entry = Dequeue();
        if (entry.UnitInfo != null)
        {
            entry.UnitInfo = new string[] { unitName, raceName, cardinalName, level.ToString() };
            foreach (var key in perkDict.ToList())
            {
                entry.PerkDict.Add(key.Key, key.Value);
            }
            partyEntries.Add(entry);
        }
        else
            Enqueue(entry);
    }

    public void PlayGame()
    {
        foreach(PartyEntry entry in partyEntries)
        {
            entry.transform.SetParent(null, true);
            DontDestroyOnLoad(entry.gameObject);
        }
        GameObjectPoolController.ClearEntry(DemoUnitKey);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RemoveUnit(PartyEntry thisUnit)
    {
        partyEntries.Remove(thisUnit);
        Enqueue(thisUnit);
        deleteActive = false;
    }
    #endregion

    #region Private
    private void Awake()
    {
        GameObjectPoolController.AddEntry(DemoUnitKey, PartyListPrefab, DemoPartySize, int.MaxValue);
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf)
            return;
        if (deleteActive == true)
        {
            deleteThis.colorGradientPreset = red;
            foreach (Transform tr in panel.transform)
            {
                Image img = tr.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
                img.GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = red;
            }
        }
        else
        {
            deleteThis.colorGradientPreset = gold;
            foreach (Transform tr in panel.transform)
            {
                Image img = tr.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, .4f);
                img.GetComponentInChildren<TextMeshProUGUI>().colorGradientPreset = gold;
            }
        }
    }

    PartyEntry Dequeue()
    {
        Poolable p = GameObjectPoolController.Dequeue(DemoUnitKey);
        PartyEntry entry = p.GetComponent<PartyEntry>();
        entry.transform.SetParent(panel.transform, false);
        entry.transform.localScale = Vector3.one;
        entry.gameObject.SetActive(true);
        return entry;
    }

    void Enqueue(PartyEntry entry)
    {
        Poolable p = entry.GetComponent<Poolable>();
        GameObjectPoolController.Enqueue(p);
    }

    IEnumerator FullPartyNotice()
    {
        partyNotice.SetActive(true);
        yield return new WaitForSeconds(2f);
        partyNotice.SetActive(false);
    }
    #endregion
}
