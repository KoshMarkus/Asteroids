using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Shooter, ILootableObject, IDestroyableObject
{
    [SerializeField] int priceForAHead;

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

        Shooting(bulletSpawn);;
        reloadTime = Random.Range(2.0f, 5.0f);

        if (transform.position == destination)
        {
            Destroyed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            other.gameObject.GetComponent<IDestroyableObject>().Destroyed();
            Loot();
            Destroyed();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroyed();
    }

    override public void Destroyed()
    {
        AudioCenter.Instance.PlaySound(destroyedSound);
        ObjectSpawner.Instance.AlienDied();
        Destroy(gameObject);
    }

    public void Loot()
    {
        Player.Instance.MorePoints(priceForAHead);
    }
}
