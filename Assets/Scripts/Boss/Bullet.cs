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
    private new Rigidbody2D rigidbody2D;
    [SerializeField]
    private GameObjectPoolReference poolReference;
    private int _collideGroundCount;


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
            poolReference.IPrefabPool.PutGameObject(gameObject);
            collision.collider.GetComponent<PlayerBehaviour>().OnTakeDamage();
        }
        else
        {
            Vector2 velocity = rigidbody2D.velocity;

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90);

            if (++_collideGroundCount < maxCollideGroundCount)
                return;

            poolReference.IPrefabPool.PutGameObject(gameObject);
        }
    }
}
