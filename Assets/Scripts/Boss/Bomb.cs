using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using TMPro;


public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObject bomb;
    [SerializeField]
    private TextMeshPro timeLeftText;
    [SerializeField]
    private GameObject explodeEffect;
    [SerializeField]
    private GameObjectPoolReference gameObjectPool;
    [SerializeField]
    private Collider2D explosionCollider;

    [Header("Parameter")]
    [SerializeField]
    private int waitExplodeTime;
    private int _waitExplodeTimeLeft;
    [SerializeField]
    private Timer explodeDisapearTimer;
    private Timer _oneSecondTimer = new Timer(1);

    [SerializeField]
    private LayerMaskReference effectLayer;
    private ContactFilter2D _contactFilter;
    private Collider2D[] _colliders = new Collider2D[1];

    private BombState _state;
    private enum BombState { WaitToExplode, WaitExplodeEnd }


    void Awake()
    {
        _contactFilter = new ContactFilter2D {
            useTriggers = true,
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
        if (!_oneSecondTimer.UpdateEnd)
            return;
        _oneSecondTimer.ContinuousReset();

        _waitExplodeTimeLeft--;
        if (_waitExplodeTimeLeft > 0)
        {
            timeLeftText.text = _waitExplodeTimeLeft.ToString();
            return;
        }

        Explode();
    }

    void UpdateWaitExplodeEnd()
    {
        if (!explodeDisapearTimer.UpdateEnd)
            return;

        gameObjectPool.Put(gameObject);
    }

    void Explode()
    {
        bomb.SetActive(false);
        explodeEffect.SetActive(true);
        _state = BombState.WaitExplodeEnd;

        Physics2D.OverlapCollider(explosionCollider, _contactFilter, _colliders);
        if (_colliders[0] == null)
            return;

        var player = _colliders[0].GetComponent<PlayerBehaviour>();
        if (!player)
        {
            Debug.Log(player);
            return;
        }
        player.OnTakeDamage();
    }

    public void Place()
    {
        bomb.SetActive(true);
        explodeEffect.SetActive(false);

        _waitExplodeTimeLeft = waitExplodeTime;
        timeLeftText.text = _waitExplodeTimeLeft.ToString();
        _state = BombState.WaitToExplode;

        _oneSecondTimer.Reset();
        explodeDisapearTimer.Reset();
    }
}
