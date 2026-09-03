using UnityEngine;

public class TwoDirectionSpin : MonoBehaviour
{
    [Header("Spin Directions")]
    public Vector3 directionA = new Vector3(0f, 1f, 0f);
    public Vector3 directionB = new Vector3(1f, 0f, 0f);

    [Header("Spin Speed")]
    public float spinSpeed = 30f;

    [Header("Change Timing")]
    public float minChangeTime = 2f;
    public float maxChangeTime = 5f;

    private Vector3 currentDirection;
    private float nextChangeTime;
    private bool usingDirectionA = true;

    void Start()
    {
        currentDirection = directionA.normalized;
        SetNextChangeTime();
    }

    void Update()
    {
        transform.Rotate(
            currentDirection * spinSpeed * Time.deltaTime,
            Space.Self
        );

        if (Time.time >= nextChangeTime)
        {
            SwitchDirection();
        }
    }

    void SwitchDirection()
    {
        usingDirectionA = !usingDirectionA;

        currentDirection = usingDirectionA
            ? directionA.normalized
            : directionB.normalized;

        SetNextChangeTime();
    }

    void SetNextChangeTime()
    {
        nextChangeTime =
            Time.time + Random.Range(minChangeTime, maxChangeTime);
    }
}