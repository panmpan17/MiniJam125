using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="MPack/GameObject Pool")]
public class GameObjectPoolReference : ScriptableObject
{
    public GameObject Prefab;
    public IPrefabPool IPrefabPool;
    public bool CreateCollection;
    public string CollectionName;

    [System.NonSerialized]
    private GameObjectPrefabPool _pool;

    public void CreatePool()
    {
        if (_pool != null)
            return;

        _pool = new GameObjectPrefabPool(
            Prefab,
            CreateCollection,
            CollectionName);
    }
    public void ClearPool() => _pool = null;

    public void PutAllAliveObjects() => _pool.PutAllAliveObjects();

    public GameObject Get() =>_pool.Get();
    public void Put(GameObject target)
    {
        if (_pool == null)
        {
            Destroy(target);
        }
        else
        {
            _pool.Put(target);
        }
    }
}
