using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class Canon : MonoBehaviour
{
    [SerializeField]
    private Bullet bulletPrefab;
    [SerializeField]
    private GameObjectPoolReference gameObjectPoolReference;
    private PrefabPool<Bullet> bulletPrefabPool;
    [SerializeField]
    private float[] directions;

    [Header("Parameter")]
    [SerializeField]
    private Timer waitTimer;


    void Awake()
    {
        bulletPrefabPool = new PrefabPool<Bullet>(bulletPrefab, true, "Bullet Prefab");
        gameObjectPoolReference.IPrefabPool = bulletPrefabPool;
    }

    void OnEnable()
    {
        waitTimer.Reset();
    }


    void Update()
    {
        if (!waitTimer.UpdateEnd)
            return;

        for (int i = 0; i < directions.Length; i++)
        {
            Bullet bullet = bulletPrefabPool.Get();
            bullet.Shoot(transform.position, directions[i]);
        }

        gameObject.SetActive(false);
    }
}
