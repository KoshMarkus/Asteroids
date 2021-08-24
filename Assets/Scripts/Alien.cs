using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Shooter
{
    [SerializeField] int priceForAHead;

    [SerializeField] float minReload;
    [SerializeField] float maxReload;

    private Vector3 startPosition;
    private Vector3 destination;

    private float timeForLerp;
    [SerializeField] float timeToReachOtherSideOfScreen;

    public void SetPath(Vector3 startPos)
    {
        startPosition = startPos;
        destination = new Vector3(-startPosition.x, startPosition.y, startPosition.z);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        timeForLerp += Time.deltaTime / timeToReachOtherSideOfScreen;

        transform.position = Vector3.Lerp(startPosition, destination, timeForLerp);

        transform.Rotate(0, 0, 50 * Time.deltaTime);

        bulletSpawn.rotation = Quaternion.LookRotation(Vector3.forward, Player.Instance.transform.position - transform.position);

        Shooting(bulletSpawn);

        if (transform.position == destination)
        {
            Destroy(gameObject);
        }
    }

    protected override void Shooting(Transform rotationSource)
    {
        reloadTime = Random.Range(minReload, maxReload);

        base.Shooting(rotationSource);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            if (destroyedSound)
            {
                AudioCenter.Instance.PlaySound(destroyedSound);
            }

            other.gameObject.SetActive(false);
            Loot();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ObjectSpawner.Instance.AlienDied();
    }

    public void Loot()
    {
        Player.Instance.MorePoints(priceForAHead);
    }
}
