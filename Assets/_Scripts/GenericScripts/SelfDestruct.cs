using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 2f;

    private float timeAlive;

    void Start()
    {
        timeAlive = 0f;
    }

    void Update()
    {
        timeAlive += Time.unscaledDeltaTime;
        if (timeAlive > lifetime)
        {
            Destroy(gameObject);
        }
    }
}