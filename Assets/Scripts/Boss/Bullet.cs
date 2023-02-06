using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class Bullet : MonoBehaviour, IPoolableObj
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int maxCollideGroundCount;
    [SerializeField]
    private bool disapearWhenHitInvinciblePlayer;

    [Header("Reference")]
    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    [SerializeField]
    private GameObjectPoolReference poolReference;
    private int _collideGroundCount;
    [SerializeField]
    private EffectReference hitEffect;
    [SerializeField]
    private EffectReference disapearEffect;

    [SerializeField]
    private SFX hitWallSFX;


    public void DeactivateObj(Transform collectionTransform)
    {
        transform.SetParent(collectionTransform);
        gameObject.SetActive(false);
    }

    public void Instantiate() {}

    public void Reinstantiate()
    {
        gameObject.SetActive(true);
    }

    public void Shoot(Vector3 position, float rotationAngle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
        transform.SetPositionAndRotation(position, rotation);
        rigidbody2D.velocity = rotation * Vector3.up * speed;
        _collideGroundCount = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            bool isSuccess = collision.collider.GetComponent<PlayerBehaviour>().OnTakeDamage();

            if (!disapearWhenHitInvinciblePlayer && !isSuccess)
                return;

            poolReference.IPrefabPool.PutGameObject(gameObject);
        }
        else
            HandleCollideWithNonPlayer(collision);
    }

    void HandleCollideWithNonPlayer(Collision2D collision)
    {
        Vector2 velocity = rigidbody2D.velocity;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);

        if (++_collideGroundCount < maxCollideGroundCount)
        {
            OnRebound(collision);
            return;
        }

        poolReference.IPrefabPool.PutGameObject(gameObject);

        if (disapearEffect)
        {
            ContactPoint2D contactPoint2D = collision.contacts[0];
            Vector2 normal = contactPoint2D.normal;
            disapearEffect.AddWaitingList(contactPoint2D.point, Quaternion.Euler(0, 0, Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90));
        }

        hitWallSFX?.Play();
    }

    void OnRebound(Collision2D collision)
    {
        if (!hitEffect)
            return;

        ContactPoint2D contactPoint2D = collision.contacts[0];
        Vector2 normal = contactPoint2D.normal;
        hitEffect.AddWaitingList(contactPoint2D.point, Quaternion.Euler(0, 0, Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90));
    }
}
