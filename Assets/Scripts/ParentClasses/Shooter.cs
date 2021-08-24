using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : ScreenWrappingObject
{
    private bool canShoot = true;

    [SerializeField] protected Transform bulletSpawn;
    [SerializeField] string bulletPoolName;
    [SerializeField] protected float reloadTime;

    [SerializeField] AudioClip fireSound;

    virtual protected void Shooting(Transform rotationSource)
    {
        if (canShoot)
        {
            if (fireSound)
            {
                AudioCenter.Instance.PlaySound(fireSound);
            }

            ObjectPooler.Instance.SpawnFromPool(bulletPoolName, bulletSpawn.position, rotationSource.rotation);
            StartCoroutine(Reload());
        }
    }

    protected IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }
}
