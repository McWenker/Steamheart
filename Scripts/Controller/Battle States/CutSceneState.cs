using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutSceneState : BattleState
{
    ConvoController ConvoController;
    ConvoData data;

    protected override void Awake()
    {
        base.Awake();
        ConvoController = owner.GetComponentInChildren<ConvoController>();
    }

    public override void Enter()
    {
        base.Enter();
        if (IsBattleOver())
        {
            if (DidPlayerWin())
                data = Resources.Load<ConvoData>("Convo/" + owner.outroWinScene);
            else
                data = Resources.Load<ConvoData>("Convo/" + owner.outroLoseScene);
        }
        else
        {
            data = Resources.Load<ConvoData>("Convo/" + owner.introScene);
        }
        ConvoController.Show(data);
    }

    public override void Exit()
    {
        base.Exit();
        if (data)
            Resources.UnloadAsset(data);
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        ConvoController.completeEvent += OnCompleteConvo;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        ConvoController.completeEvent -= OnCompleteConvo;
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (owner.isPaused)
            return;
        ConvoController.Next();
    }

    void OnCompleteConvo(object sender, System.EventArgs e)
    {
        if (IsBattleOver())
            owner.ChangeState<EndBattleState>();
        else
            owner.ChangeState<SelectUnitState>();
    }
}