using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private BattleController owner;
    [SerializeField] Canvas pauseCanvas;

    private void Awake()
    {
        owner = GetComponentInParent<BattleController>();
    }

    public void Activate()
    {
        owner.isPaused = true;
        Time.timeScale = 0;
        pauseCanvas.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        owner.isPaused = false;
        Time.timeScale = 1;
        pauseCanvas.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        owner.isPaused = false;
        Time.timeScale = 1;
        pauseCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
