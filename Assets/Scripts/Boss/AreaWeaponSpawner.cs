using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class AreaWeaponSpawner : AbstractTriggerFire
{
    [SerializeField]
    private GameObjectPoolReference prefabPoolList;
    [SerializeField]
    private Vector2 boxSize;
    [SerializeField]
    private int spawnCount;
    private int _spawnCount;
    [SerializeField]
    private Timer spawnIntervalTimer;
    [SerializeField]
    private bool spawnOneRightAfterTrigger;

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

    private void SpawnWeapon()
    {
        var position = new Vector3(
                    Random.Range(0, boxSize.x) - (boxSize.x / 2),
                    Random.Range(0, boxSize.y) - (boxSize.y / 2));

        GameObject gameObject = prefabPoolList.Get();
        gameObject.transform.position = position;

        if (++_spawnCount >= spawnCount)
            enabled = false;
    }

    public override void TriggerFire()
    {
        if (enabled)
            return;
        enabled = true;

        if (spawnOneRightAfterTrigger)
        {
            _spawnCount = 1;
            SpawnWeapon();
        }
        else
            _spawnCount = 0;
        spawnIntervalTimer.Reset();
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, boxSize);
        Gizmos.DrawLine(transform.position - (Vector3)boxSize / 2, transform.position + (Vector3)boxSize / 2);
    }
}
