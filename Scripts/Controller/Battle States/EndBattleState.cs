using UnityEngine.SceneManagement;

public class EndBattleState : BattleState
{
    public override void Enter()
    {
        base.Enter();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}