using UnityEngine;

public class Asteroid : ScreenWrappingObject, IPooledObject, ILootableObject
{
    public int timesBroken;
    [SerializeField] float BreakAngle;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    private float speed;
    private float lesserAsteroidSpeed;

    public bool wasPushed;

    override protected void Start(){}

    public void OnSpawnFromPool()
    {
        base.Start();

        wasPushed = false;
        rb.velocity = Vector3.zero;

        transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<SphereCollider>().radius = 0.895f;
        ObjectSpawner.Instance.ThereIsOneMoreAsteroid(true);
    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag("PlayerBullet"))
            {
                Loot();

                if (timesBroken < 2)
                {
                    lesserAsteroidSpeed = Random.Range(minSpeed, maxSpeed);

                    for (int i = 0; i < 2; i++)
                    {
                        GameObject lesserAsteroid = ObjectPooler.Instance.SpawnFromPool("Asteroids", transform.position, Quaternion.identity);//Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                        lesserAsteroid.transform.localScale = new Vector3(transform.localScale.x / 2f, transform.localScale.y / 2f, transform.localScale.z);
                        lesserAsteroid.GetComponent<SphereCollider>().radius = gameObject.GetComponent<SphereCollider>().radius / 2f;
                        
                        lesserAsteroid.GetComponent<Asteroid>().BreakAngle = -BreakAngle;
                        lesserAsteroid.GetComponent<Asteroid>().speed = lesserAsteroidSpeed;
                        lesserAsteroid.GetComponent<Asteroid>().PushSetup(timesBroken + 1, gameObject);

                        BreakAngle = -BreakAngle;

                    }
                }

                ObjectSpawner.Instance.ThereIsOneMoreAsteroid(false);

                rb.velocity = Vector3.zero;
                other.gameObject.GetComponent<ScreenWrappingObject>().Destroyed();
                Destroyed();
            }
    }

    public void PushSetup(int brokenTimes, GameObject parentAsteroid = null)
    {
        timesBroken = brokenTimes;

        if (timesBroken == 0)
        {
            speed = Random.Range(minSpeed, maxSpeed);
            rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0) * speed);
        }
        else
        {
            Vector3 newDirection = Quaternion.AngleAxis(-BreakAngle, parentAsteroid.transform.forward) * parentAsteroid.GetComponent<Rigidbody>().velocity;
            newDirection.Normalize();
            rb.AddForce(newDirection.x * speed, newDirection.y * speed, 0);
        }

        wasPushed = true;
    }

    public void Loot()
    {
        switch (timesBroken)
        {
            case 0:
                Player.Instance.MorePoints(20);
                break;
            case 1:
                Player.Instance.MorePoints(50);
                break;
            case 2:
                Player.Instance.MorePoints(100);
                break;
            default:
                break;
        }
    }
}
