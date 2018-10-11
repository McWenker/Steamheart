using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleState : State
{
    protected BattleController owner;
    protected Driver driver;
    public CameraRig cameraRig { get { return owner.cameraRig; } }
    public Board board { get { return owner.board; } }
    public LevelData levelData { get { return owner.levelData; } }
    public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; } }
    public Point pos { get { return owner.pos; } set { owner.pos = value; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; } }
    public Turn turn { get { return owner.turn; } }
    public List<Unit> units { get { return owner.units; } }
    public StatPanelController statPanelController { get { return owner.statPanelController; } }
    public HitSuccessIndicator hitSuccessIndicator { get { return owner.hitSuccessIndicator; } }
    public PauseMenuController pause { get { return owner.pauseMenu; } }

    public override void Enter()
    {
        driver = (turn.actor != null) ? turn.actor.GetComponent<Driver>() : null;
        base.Enter();
    }

    protected virtual bool DidPlayerWin()
    {
        return owner.GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero;
    }

    protected virtual bool IsBattleOver()
    {
        return owner.GetComponent<BaseVictoryCondition>().Victor != Alliances.None;
    }

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }

	protected override void AddListeners()
	{
        if (driver == null || driver.Current == Drivers.Human)
        {
            InputController.moveEvent += OnMove;
            InputController.fireEvent += OnFire;
        }
        InputController.rotateEvent += OnRotate;
        InputController.zoomEvent += OnZoom;
        InputController.escapeEvent += OnEscape;
    }

	protected override void RemoveListeners()
	{
		InputController.moveEvent -= OnMove;
		InputController.fireEvent -= OnFire;
        InputController.rotateEvent -= OnRotate;
        InputController.zoomEvent -= OnZoom;
        InputController.escapeEvent -= OnEscape;
    }

	protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
    {

    }

	protected virtual void OnFire(object sender, InfoEventArgs<int> e)
	{
	}

    protected virtual void OnRotate(object sender, InfoEventArgs<string> e)
    {
        if (e.info == "left" && !owner.cameraRig._isRotating)
            owner.cameraRig.FacingIndex = owner.cameraRig.FacingIndex <= 0 ? 3 : owner.cameraRig.FacingIndex - 1;
        if (e.info == "right" && !owner.cameraRig._isRotating)
            owner.cameraRig.FacingIndex = owner.cameraRig.FacingIndex >= 3 ? 0 : owner.cameraRig.FacingIndex + 1;
    }

    protected virtual void OnZoom(object sender, InfoEventArgs<int> e)
    {
        owner.cameraRig.CameraIndex -= e.info;
    }

    protected virtual void OnEscape(object sender, InfoEventArgs<string> e)
    {
        TogglePauseMenu();
    }

    protected virtual void SelectTile(Point p)
	{
		if (pos == p || !board.tiles.ContainsKey(p))
			return;

		pos = p;
        tileSelectionIndicator.localPosition = board.tiles[p].center;

    }

	protected virtual Unit GetUnit(Point p)
	{
		Tile t = board.GetTile (p);
		GameObject content = t != null ? t.content : null;
		return content != null ? content.GetComponent<Unit>() : null;
	}

	protected virtual void RefreshPrimaryStatPanel(Point p)
	{
		Unit target = GetUnit(p);
		if(target != null)
			statPanelController.ShowPrimary(target.gameObject);
		else
			statPanelController.HidePrimary();
	}

	protected virtual void RefreshSecondaryStatPanel(Point p)
	{
		Unit target = GetUnit(p);
		if(target != null)
			statPanelController.ShowSecondary(target.gameObject);
		else
			statPanelController.HideSecondary();
	}

    protected virtual void TogglePauseMenu()
    {
        if(owner.isPaused)
        {
            pause.Deactivate();
        }
        else
        {
            pause.Activate();
        }
    }
}
