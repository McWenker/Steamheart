using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class StatPanelController : MonoBehaviour
{
    #region Const
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    const int statusCount = 13;
    const string EntryPoolKey = "StatPanelController.Entry";
    #endregion

    #region Fields
    [SerializeField] StatPanel primaryPanel;
    [SerializeField] StatPanel secondaryPanel;
    [SerializeField] GameObject StatusIndicatorPrefab;
    List<StatusIndicatorEntry> primaryStatusEntries = new List<StatusIndicatorEntry>(statusCount);
    List<StatusIndicatorEntry> secondaryStatusEntries = new List<StatusIndicatorEntry>(statusCount);

    Tweener primaryTransition;
    Tweener secondaryTransition;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        GameObjectPoolController.AddEntry(EntryPoolKey, StatusIndicatorPrefab, statusCount*2, int.MaxValue);
    }

    void Start()
    {
        if (primaryPanel.panel.CurrentPosition == null)
            primaryPanel.panel.SetPosition(HideKey, false);
        if (secondaryPanel.panel.CurrentPosition == null)
            secondaryPanel.panel.SetPosition(HideKey, false);
    }
    #endregion

    #region Public
    public void ShowPrimary(GameObject obj)
    {
        primaryPanel.Display(obj);
        Clear(primaryStatusEntries);
        foreach (StatusEffect eff in obj.GetComponentsInChildren<StatusEffect>())
        {
            StatusIndicatorEntry entry = Dequeue(primaryPanel);
            entry.Title = eff.GetType().Name.TrimStart(new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_'});
            PrepareRankIndicator(entry, eff.Rank);
            PrepareDurationIndicator(entry, eff.GetComponentInChildren<DurationStatusCondition>());
            primaryStatusEntries.Add(entry);
        }
        MovePanel(primaryPanel, ShowKey, ref primaryTransition);
    }

    public void HidePrimary()
    {
        MovePanel(primaryPanel, HideKey, ref primaryTransition);
        Clear(primaryStatusEntries);
    }

    public void ShowSecondary(GameObject obj)
    {
        secondaryPanel.Display(obj);
        Clear(secondaryStatusEntries);
        foreach (StatusEffect eff in obj.GetComponentsInChildren<StatusEffect>())
        {
            StatusIndicatorEntry entry = Dequeue(secondaryPanel);
            entry.Title = eff.GetType().Name.TrimStart(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_' });
            PrepareRankIndicator(entry, eff.Rank);
            PrepareDurationIndicator(entry, eff.GetComponentInChildren<DurationStatusCondition>());
            secondaryStatusEntries.Add(entry);
        }
        MovePanel(secondaryPanel, ShowKey, ref secondaryTransition);
    }

    public void HideSecondary()
    {
        MovePanel(secondaryPanel, HideKey, ref secondaryTransition);
        Clear(secondaryStatusEntries);
    }
    #endregion

    #region Private
    StatusIndicatorEntry Dequeue(StatPanel panel)
    {
        Poolable p = GameObjectPoolController.Dequeue(EntryPoolKey);
        StatusIndicatorEntry entry = p.GetComponent<StatusIndicatorEntry>();
        entry.transform.SetParent(panel.statusParent.transform, false);
        entry.transform.localScale = Vector3.one;
        entry.gameObject.SetActive(true);
        return entry;
    }

    void Enqueue(StatusIndicatorEntry entry)
    {
        Poolable p = entry.GetComponent<Poolable>();
        GameObjectPoolController.Enqueue(p);
    }

    void Clear(List<StatusIndicatorEntry> toClear)
    {
       for (int i = toClear.Count - 1; i >= 0; --i)
            Enqueue(toClear[i]);
       toClear.Clear();
    }

    void MovePanel(StatPanel obj, string pos, ref Tweener t)
    {
        Panel.Position target = obj.panel[pos];
        if (obj.panel.CurrentPosition != target)
        {
            if (t != null)
                t.Stop();
            t = obj.panel.SetPosition(pos, true);
            t.duration = 0.5f;
            t.equation = EasingEquations.EaseOutQuad;
        }
    }

    void PrepareRankIndicator(StatusIndicatorEntry entry, int rank)
    {
        entry.Img = Resources.Load<Sprite>("Status Icons/" + entry.Title);
        if (rank == 0 || rank > 3)
            entry.Level = " ";
        else if (rank == 1)
            entry.Level = "I";
        else if (rank == 2)
            entry.Level = "II";
        else if (rank == 3)
            entry.Level = "III";
    }

    void PrepareDurationIndicator(StatusIndicatorEntry entry, DurationStatusCondition dur)
    {
        if (dur != null)
        {
            entry.Counter.SetActive(true);
            entry.Duration = dur.duration.ToString();
        }
        else
        {
            entry.Counter.SetActive(false);
        }
    }
    #endregion
}