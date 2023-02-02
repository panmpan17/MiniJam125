using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class FanSpreadBullet : MonoBehaviour
{
    [SerializeField]
    private Bullet bulletPrefab;
    [SerializeField]
    private GameObjectPoolReference gameObjectPoolReference;
    private PrefabPool<Bullet> bulletPrefabPool;
    [SerializeField]
    [Range(0, 360)]
    private float startAngle, endAngle;
    [SerializeField]
    private float anglePerBullet;
    [SerializeField]
    private float turnSpeed;
    private bool _forwarding = true;

    private float _zAngle;
    private float _angleTurned;

    void Awake()
    {
        bulletPrefabPool = new PrefabPool<Bullet>(bulletPrefab, true, "Bullet Prefab");
        gameObjectPoolReference.IPrefabPool = bulletPrefabPool;
    }

    void OnDisable()
    {
        _forwarding = !_forwarding;
    }

    void FixedUpdate()
    {
        float addAmount = turnSpeed * Time.fixedDeltaTime;

        if (_forwarding) _zAngle += addAmount;
        else _zAngle -= addAmount;
        transform.localRotation = Quaternion.Euler(0, 0, _zAngle);

        _angleTurned += addAmount;
        if (_angleTurned >= anglePerBullet)
        {
            _angleTurned -= anglePerBullet;
            FireAtRotation(_zAngle - _angleTurned);
        }

        if (_forwarding)
        {
            if (_zAngle >= endAngle) enabled = false;
        }
        else
        {
            if (_zAngle <= startAngle) enabled = false;
        }
    }

    void FireAtRotation(float zRotation)
    {
        Bullet bullet = bulletPrefabPool.Get();
        bullet.Shoot(transform.position, zRotation);
    }

    public void TriggerFire()
    {
        if (enabled)
            return;
        enabled = true;

        _zAngle = _forwarding ? startAngle : endAngle;
        _angleTurned = 0;
        transform.localRotation = Quaternion.Euler(0, 0, _zAngle);
    }

    void OnDrawGizmos()
    {
        Quaternion startRotation = Quaternion.Euler(0, 0, startAngle);
        Quaternion endRotation = Quaternion.Euler(0, 0, endAngle);
        Quaternion centerRotation = Quaternion.Euler(0, 0, (startAngle + endAngle) / 2);

        Vector3 startLinePosition = transform.position + (startRotation * Vector3.up * 3f);
        Vector3 endLinePosition = transform.position + (endRotation * Vector3.up * 3f);
        Vector3 centerLinePosition = transform.position + (centerRotation * Vector3.up * 3f);

        Gizmos.DrawLine(transform.position, startLinePosition);
        Gizmos.DrawLine(transform.position, endLinePosition);
        Gizmos.DrawLine(startLinePosition, centerLinePosition);
        Gizmos.DrawLine(centerLinePosition, endLinePosition);
    }
}
