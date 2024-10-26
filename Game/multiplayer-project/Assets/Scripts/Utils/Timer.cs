using System;

public struct Timer
{
    private float duration;
    private float timeRemaining;
    private bool isRunning;
    private Action onTimeUp;

    public Timer(float duration, Action onTimeUp)
    {
        this.duration = duration;
        this.timeRemaining = duration;
        this.isRunning = false;
        this.onTimeUp = onTimeUp;
    }

    public void Start()
    {
        if (isRunning)
            return;

        isRunning = true;
        timeRemaining = duration;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Reset()
    {
        timeRemaining = duration;
        isRunning = false;
    }

    public void Restart()
    {
        timeRemaining = duration;
        isRunning = true;
    }

    public void Update(float deltaTime)
    {
        if (!isRunning)
            return;

        timeRemaining -= deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            onTimeUp?.Invoke();
        }
    }

    public bool IsRunning => isRunning;
    public float TimeRemaining => timeRemaining;
}
