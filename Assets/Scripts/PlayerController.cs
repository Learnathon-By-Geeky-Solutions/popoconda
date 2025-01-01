using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody playerRigidbody;

    public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = playerObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // AD or Left and Right Arrow keys
        float xInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(playerRigidbody.linearVelocity.x) < 8)
        {
            playerRigidbody.AddRelativeForce(Vector3.right * xInput * Time.deltaTime * moveSpeed);
        }
    }
}
