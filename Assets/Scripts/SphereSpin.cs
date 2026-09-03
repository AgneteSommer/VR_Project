using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float xSpeed = 0f;
    public float ySpeed = 30f;
    public float zSpeed = 0f;

    void Update()
    {
        transform.Rotate(
            xSpeed * Time.deltaTime,
            ySpeed * Time.deltaTime,
            zSpeed * Time.deltaTime
        );
    }
}