using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60;

    protected virtual void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.ToggleSlowMotion();
        }
    }

    private void ToggleSlowMotion()
    {
        Time.timeScale = Time.timeScale == 1f ? 0.2f : 1f;
    }
}
