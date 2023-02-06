using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public abstract class AbstractTriggerFire : MonoBehaviour
{
    public abstract void TriggerFire();
}

public class FanSpreadBullet : AbstractTriggerFire
{

    [SerializeField]
    [Range(0, 360)]
    private float startAngle, endAngle;

    [Header("Warning")]
    [SerializeField]
    private Transform warningLine;
    [SerializeField]
    private Timer warningTimer;
    private bool _warning;

    [Header("Shooting")]
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float anglePerBullet;
    [SerializeField]
    private Bullet bulletPrefab;
    [SerializeField]
    private GameObjectPoolReference gameObjectPoolReference;
    private PrefabPool<Bullet> bulletPrefabPool;
    private bool _forwarding = true;

    [SerializeField]
    private ImpluseData shootImpulse;
    [SerializeField]
    private SFX shootSFX;

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
        if (_warning)
            UpdateWarning();
        else
            UpdateTurnAndShoot();
    }

    void UpdateWarning()
    {
        if (warningTimer.UpdateEnd)
        {
            warningLine.gameObject.SetActive(false);
            _warning = false;
            return;
        }

        float z = 0;
        if (_forwarding)
            z = Mathf.Lerp(startAngle, endAngle, warningTimer.Progress);
        else
            z = Mathf.Lerp(endAngle, startAngle, warningTimer.Progress);

        warningLine.rotation = Quaternion.Euler(0, 0, z);
    }

    void UpdateTurnAndShoot()
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
        if (shootImpulse) ImpluseCamera.ins.GenerateImpluse(shootImpulse);
        shootSFX?.Play();
    }

    public override void TriggerFire()
    {
        if (enabled)
            return;
        enabled = true;

        _warning = true;
        warningTimer.Reset();
        warningLine.gameObject.SetActive(true);

        _zAngle = _forwarding ? startAngle : endAngle;
        _angleTurned = 0;
        transform.localRotation = Quaternion.Euler(0, 0, _zAngle);
    }

    void OnDrawGizmosSelected()
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
