using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using MPack.Aseprite;

public class Canon : MonoBehaviour
{
    [SerializeField]
    private GameObject warning;
    [SerializeField]
    private AseAnimator animator;
    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private GameObjectPoolReference canonPrefabPool;
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
        warning.SetActive(false);
    }

    void OnEnable()
    {
        waitTimer.Reset();
        warning.SetActive(true);
        animator.Play(0);
    }


    void Update()
    {
        if (!waitTimer.Running)
        {
            if (animator.IsStopped)
                canonPrefabPool.Put(gameObject);
            return;
        }

        if (!waitTimer.UpdateEnd)
            return;
        waitTimer.Running = false;

        for (int i = 0; i < directions.Length; i++)
        {
            Bullet bullet = bulletPrefabPool.Get();
            bullet.Shoot(transform.position, directions[i]);
        }

        warning.SetActive(false);
    }
}
