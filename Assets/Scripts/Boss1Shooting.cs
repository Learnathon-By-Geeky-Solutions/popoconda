using UnityEngine;

public class Boss1Shooting : MonoBehaviour
{
    public GameObject player;
    private GameObject gunRotatePoint;
    public GameObject gun;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float fireRate;
    private float nextFireTime = 0f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunRotatePoint = gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        // Get the direction to the player
        Vector3 directionToPlayer = player.transform.position - gun.transform.position;

        // Calculate the rotation angle
        float rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Rotate the gun towards the player
        gunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        // Shoot at the player at regular intervals
        if (Time.time >= nextFireTime)
        {
            FireBullet(directionToPlayer, rotationZ);
            nextFireTime = Time.time + fireRate; // Set the time for the next shot
        }
    }

    void FireBullet(Vector3 direction, float rotationZ)
    {
        // Fire the bullet
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * bulletSpeed;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        Debug.Log("Boss1AI: Bullet fired!");
    }
}