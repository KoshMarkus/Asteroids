using UnityEngine;

public class Asteroid : ScreenWrappingObject
{
    [SerializeField] float BreakAngle;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;

    [SerializeField] int bigAsteroidReward;
    [SerializeField] int mediumAsteroidReward;
    [SerializeField] int smallAsteroidReward;

    public enum Sizes { Big = 0, Medium = 1, Small = 2 };
    public Sizes size;

    private float speed;
    private float lesserAsteroidSpeed;

    public bool wasPushed;

    override protected void Start(){}

    public void OnEnable()
    {
        base.Start();

        wasPushed = false;
        rb.velocity = Vector3.zero;

        transform.localScale = new Vector3(1, 1, 1);
        ObjectSpawner.Instance.ThereIsOneMoreAsteroid(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            if (destroyedSound)
            {
                AudioCenter.Instance.PlaySound(destroyedSound);
            }

            Loot();

            if (size != Sizes.Small)
            {
                lesserAsteroidSpeed = Random.Range(minSpeed, maxSpeed);

                for (int i = 0; i < 2; i++)
                {
                    GameObject lesserAsteroid = ObjectPooler.Instance.SpawnFromPool("Asteroids", transform.position, Quaternion.identity);//Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                    lesserAsteroid.transform.localScale = new Vector3(transform.localScale.x / 2f, transform.localScale.y / 2f, transform.localScale.z);
                    lesserAsteroid.GetComponent<SphereCollider>().radius = gameObject.GetComponent<SphereCollider>().radius / 2f;

                    Asteroid options = lesserAsteroid.GetComponent<Asteroid>();

                    options.BreakAngle = -BreakAngle;
                    options.speed = lesserAsteroidSpeed;
                    options.PushSetup((int)size + 1, gameObject);

                    BreakAngle = -BreakAngle;

                }
            }

            rb.velocity = Vector3.zero;
            other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void PushSetup(int brokenTimes, GameObject parentAsteroid = null)
    {
        size = (Sizes)brokenTimes;

        if (size == Sizes.Big)
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
        switch (size)
        {
            case Sizes.Big:
                Player.Instance.MorePoints(bigAsteroidReward);
                break;
            case Sizes.Medium:
                Player.Instance.MorePoints(mediumAsteroidReward);
                break;
            case Sizes.Small:
                Player.Instance.MorePoints(smallAsteroidReward);
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        ObjectSpawner.Instance.ThereIsOneMoreAsteroid(false);
    }
}
