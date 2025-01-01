using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform playerPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerPosition.position + new Vector3(0, 9.5f, -20);
    }
}
