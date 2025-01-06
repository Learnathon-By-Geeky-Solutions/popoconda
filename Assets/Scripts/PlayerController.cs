using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerObject;
    public Rigidbody playerRigidbody;

    public float moveSpeed;
    public float flySpeed;
    public float jetpackFuel;
    public float fuelConsumeRate;
    public float fuelFillRate;

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
            playerRigidbody.AddRelativeForce(Vector3.right * (xInput * Time.deltaTime * moveSpeed));
        }

        // Space key
        if (Input.GetButton("Jump") && jetpackFuel > 0)
        {
            playerRigidbody.AddRelativeForce(Vector3.up * (Time.deltaTime * flySpeed));
            jetpackFuel -= Time.deltaTime * fuelConsumeRate;
        }
        // refill jetpack when fuel is less than 5 and space key is not pressed
        else if (jetpackFuel < 5 && !Input.GetButton("Jump"))
        {
            jetpackFuel += Time.deltaTime * fuelFillRate;
        }
    }
}
