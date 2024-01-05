using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public GameObject player;

    public float countdownDuration = 60.0f; // Set the countdown duration in seconds
    public Text countdownText; // Assign the UI text element in the Inspector

    private float remainingTime;
    private bool isCountingDown = false;

    private void Start()
    {
        remainingTime = countdownDuration;
        isCountingDown = true;
    }

    private void Update()
    {
        if (isCountingDown)
        {
            remainingTime -= Time.deltaTime;

            // Update the UI text to show the remaining time
            countdownText.text = "Time Left:\n" + Mathf.Max(0, Mathf.RoundToInt(remainingTime)).ToString() + " Sec";

            if (remainingTime <= 0)
            {
                isCountingDown = false;
                ScoreManager.Instance.TimeOver();
                Destroy(player);
            }
        }
    }
}
