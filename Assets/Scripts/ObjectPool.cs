using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private List<GameObject> _pooledObjects;
    public GameObject ObjectPrefab;
    public int AmountToPool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _pooledObjects = new List<GameObject>();
        GameObject tempGameObject;
        for(int i = 0; i < AmountToPool; i++)
        {
            tempGameObject = Instantiate(ObjectPrefab);
            tempGameObject.SetActive(false);
            _pooledObjects.Add(tempGameObject);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < AmountToPool; i++)
        {
            if(!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }
        return null;
    }
    
}
