using System.Collections;
using UnityEngine;

public class Player : Shooter
{
    [SerializeField] float accelerationSpeed;
    [SerializeField] float maxAcceleration;
    [SerializeField] float rotationSpeed;
    [SerializeField] float mouseControlRotationSpeed;
    [SerializeField] AudioClip thrustSound;
    [SerializeField] LayerMask backdropMask;

    private Animator anim;

    public int lives;
    public int currentPoints;

    public static Player Instance;

    private void Awake()
    {
        Instance = this;
    }

    override protected void Start()
    {
        base.Start();
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if ((Input.GetButtonDown("FireKeyboard") && !UserInterface.mouseControls) || (Input.GetButtonDown("FireMouse") && UserInterface.mouseControls))
        {
            Shooting(transform);
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (UserInterface.Instance.state == UserInterface.States.Play)
        {
            Turning();
            Acceleration();

            KeepVelocityBelowMax();
        }
    }


    void Turning()
    {
        if (!UserInterface.mouseControls)
        {
            transform.Rotate(0, 0, Input.GetAxis("Horizontal") * -rotationSpeed * Time.deltaTime);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
            {
                    Vector3 direction = hitInfo.point - transform.position;
                    direction.z = 0f;
                    direction.Normalize();

                    Quaternion qTo = Quaternion.LookRotation(direction, new Vector3(0, 0, -1));

                    //transform.up = direction;

                    qTo *= Quaternion.Euler(90, 90, 90);


                    transform.rotation = Quaternion.Slerp(transform.rotation, qTo, Time.deltaTime * mouseControlRotationSpeed);
            }
        }

    }

    void Acceleration()
    {
        if ((Input.GetAxis("Vertical") > 0 && !UserInterface.mouseControls) || (Input.GetButton("Thrust") && UserInterface.mouseControls))
        {
            if (!AudioCenter.Instance.GetComponent<AudioSource>().isPlaying)
            {
                AudioCenter.Instance.PlaySound(thrustSound);
            }

            rb.AddForce(transform.up * accelerationSpeed);
        }
    }

    private void KeepVelocityBelowMax()
    {
        if (rb.velocity.magnitude > maxAcceleration)
        {
            rb.velocity = rb.velocity.normalized;
            rb.velocity = rb.velocity * maxAcceleration;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AlienBullet"))
        {
            other.gameObject.SetActive(false);
            Crash();
        }
        else if (other.CompareTag("Asteroid") || other.CompareTag("Alien"))
        {
            Crash();
        }
    }

    void Crash()
    {
        if (destroyedSound)
        {
            AudioCenter.Instance.PlaySound(destroyedSound);
        }

        --lives;

        AudioCenter.Instance.PlaySound(destroyedSound);
        UserInterface.Instance.UpdateLivesCounter(lives);

        if (lives >= 1)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) - cam.transform.position;
            StartCoroutine(Invincible());
        }
        else
        {
            UserInterface.Instance.ShowDeathPrompt();
            gameObject.SetActive(false);
        }
    }

    IEnumerator Invincible()
    {
        anim.SetBool("Invincible", true);
        rb.detectCollisions = false;
        yield return new WaitForSeconds(3f);
        rb.detectCollisions = true;
        anim.SetBool("Invincible", false);
    }

    public void MorePoints(int points)
    {
        currentPoints += points;
        UserInterface.Instance.UpdatePointsCounter(currentPoints);
    }
}
