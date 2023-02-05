using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using MPack.Aseprite;
using TMPro;


public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObjectPoolReference gameObjectPool;
    [SerializeField]
    private Collider2D explosionCollider;
    [SerializeField]
    private AseAnimator animator;

    [Header("Parameter")]
    [SerializeField]
    private Timer waitExplodeTimer;
    private Timer _explodeDisapearTimer;
    [SerializeField]
    private ImpluseData impluseData;

    [SerializeField]
    private LayerMaskReference effectLayer;
    private ContactFilter2D _contactFilter;
    private Collider2D[] _colliders = new Collider2D[1];

    private BombState _state;
    private enum BombState { WaitToExplode, WaitExplodeEnd }


    void Awake()
    {
        _contactFilter = new ContactFilter2D {
            useLayerMask = true,
            layerMask = effectLayer.Value,
        };
    }

    void OnEnable()
    {
        Place();
    }


    void Update()
    {
        switch (_state)
        {
            case BombState.WaitToExplode:
                UpdateWaitToExplode();
                break;

            case BombState.WaitExplodeEnd:
                UpdateWaitExplodeEnd();
                break;
        }
    }

    private void UpdateWaitToExplode()
    {
        if (!waitExplodeTimer.UpdateEnd)
            return;
        Explode();
    }

    void UpdateWaitExplodeEnd()
    {
        if (!_explodeDisapearTimer.UpdateEnd)
            return;

        gameObjectPool.Put(gameObject);
    }

    void Explode()
    {
        _state = BombState.WaitExplodeEnd;

        animator.Play(1);
        if (impluseData) ImpluseCamera.ins.GenerateImpluse(impluseData);

        int length = Physics2D.OverlapCollider(explosionCollider, _contactFilter, _colliders);
        if (length <= 0)
            return;

        var player = _colliders[0].GetComponent<PlayerBehaviour>();
        if (!player)
        {
            return;
        }
        player.OnTakeDamage();
    }

    public void Place()
    {
        _state = BombState.WaitToExplode;
        animator.Play(0);

        waitExplodeTimer.Reset();
        _explodeDisapearTimer.TargetTime = animator.GetAnimationDuration(1);
        _explodeDisapearTimer.Reset();
    }
}
