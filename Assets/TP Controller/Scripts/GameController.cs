using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60;

    [SerializeField]
    private float slowMotion = 0.2f;

    protected virtual void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    protected virtual void Update()
    {
        if (Input.GetButtonDown("Slow Motion"))
        {
            this.ToggleSlowMotion();
        }
    }

    private void ToggleSlowMotion()
    {
        Time.timeScale = Time.timeScale == 1f ? this.slowMotion : 1f;
    }
}
