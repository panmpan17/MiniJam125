using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AreaWeaponSpawner : AbstractTriggerFire
{
    [SerializeField]
    private GameObjectPoolReference prefabPoolList;
    [SerializeField]
    private int spawnCount;
    private int _spawnCount;
    [SerializeField]
    private Timer spawnIntervalTimer;
    [SerializeField]
    private bool spawnOneRightAfterTrigger;

    [Header("Spawn Area")]
    [SerializeField]
    private Vector2 boxSize;
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private RangeReference followTargetNearByRadius;
    [SerializeField]
    [Range(0, 1)]
    private float totalRandomChance, nearByFollowerTarget;

    void Awake()
    {
        prefabPoolList.CreatePool();
    }

    void Update()
    {
        if (!spawnIntervalTimer.UpdateEnd)
            return;
        spawnIntervalTimer.ContinuousReset();

        SpawnWeapon();
    }

    Vector2 GetOnUnitCircle() => Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.up;
    Vector2 GetOnUnitCircle(float radius) => Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector2(0, radius);

    Vector3 GetRandomPosition()
    {
        float randomValue = Random.Range(0, totalRandomChance + nearByFollowerTarget);

        Vector2 basePosition = transform.position;

        Vector2 halfBoxSize = boxSize / 2;
        Vector2 min = basePosition - halfBoxSize;
        Vector2 max = basePosition + halfBoxSize;

        if (randomValue <= totalRandomChance)
        {
            Vector2 half = boxSize / 2;
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
        }
        else
        {
            Vector2 position = followTarget.position + (Vector3)GetOnUnitCircle(followTargetNearByRadius.PickRandomNumber()); 

            if (position.x < min.x) position.x = min.x;
            if (position.x > max.x) position.x = max.x;
            if (position.y < min.y) position.y = min.y;
            if (position.y > max.y) position.y = max.y;

            return position;
        }
    }

    private void SpawnWeapon()
    {
        GameObject gameObject = prefabPoolList.Get();
        gameObject.transform.position = GetRandomPosition();

        if (++_spawnCount >= spawnCount)
            enabled = false;
    }

    public override void TriggerFire()
    {
        if (enabled)
            return;
        enabled = true;

        _spawnCount = 0;
        if (spawnOneRightAfterTrigger)
            SpawnWeapon();
        spawnIntervalTimer.Reset();
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, boxSize);
        Gizmos.DrawLine(transform.position - (Vector3)boxSize / 2, transform.position + (Vector3)boxSize / 2);

        if (followTarget)
        {
            Gizmos.DrawWireSphere(followTarget.position, followTargetNearByRadius.Min);
            Gizmos.DrawWireSphere(followTarget.position, followTargetNearByRadius.Max);
        }
    }
}
