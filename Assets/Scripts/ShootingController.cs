using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private Camera playerCamera;
    private Vector3 mousePosition;
    public GameObject Player;
    public GameObject Gun;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float fireRate;
    private float nextFireTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position in the world 
        Plane xyPlane = new Plane(Vector3.forward, Vector3.zero); // Plane at Z = 0
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (xyPlane.Raycast(ray, out float distance))
        {
            mousePosition = ray.GetPoint(distance); // Get the intersection point on the X-Y plane
        }


        Vector3 Difference = mousePosition - Gun.transform.position;

        Vector3 rotation = mousePosition - transform.position;

        float rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        // if the z rotation is greater than 90 or less than -90, flip the Player GameObject
        if (rotationZ > 90 || rotationZ < -90)
        {
            Player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            Player.transform.localScale = new Vector3(1, 1, 1);
        }

        // Shoot only if left mouse button is pressed and fire rate timer has elapsed
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            FireBullet(Difference, rotationZ);
            nextFireTime = Time.time + fireRate; // Set the time for the next shot
        }
    }
    void FireBullet(Vector3 direction, float rotationZ)
    {
        // Fire the bullet
        GameObject bullet = Instantiate(bulletPrefab, Gun.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * bulletSpeed;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}
