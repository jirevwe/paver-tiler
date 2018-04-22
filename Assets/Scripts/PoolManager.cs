using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

    static PoolManager _instance;

    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolManager>();
            }
            return _instance;
        }
    }

    public void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<ObjectInstance>());

            GameObject poolHolder = new GameObject(prefab.name + " pool");
            poolHolder.transform.parent = transform;

            for (int i = 0; i < poolSize; i++)
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
                poolDictionary[poolKey].Enqueue(newObject);
                newObject.SetParent(poolHolder.transform);
            }
        }
    }
    
    public void ReuseObject<T>(GameObject prefab, Vector3 position, Quaternion rotation, T args)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation, args);
        }
    }

    public List<ObjectInstance> GetActiveObjects(GameObject prefab)
    {
        int poolKey = prefab.GetInstanceID();
        var active = new List<ObjectInstance>();

        if (!poolDictionary.ContainsKey(poolKey))
            return null;

        var prefabInstances = poolDictionary[poolKey].ToList();
        for (int i = 0; i < prefabInstances.Count; i++)
        {
            if (prefabInstances[i].GameObject.activeInHierarchy)
                active.Add(prefabInstances[i]);
        }
        return active;
    }
}

 [System.Serializable]
public class ObjectInstance {

	GameObject gameObject;
	Transform transform;

	bool hasPoolObjectComponent;
	PoolObject poolObjectScript;

    public GameObject GameObject
    {
        get { return gameObject; }
    }

	public ObjectInstance(GameObject objectInstance) {
		gameObject = objectInstance;
		transform = gameObject.transform;
		gameObject.SetActive(false);

		if (gameObject.GetComponent<PoolObject>()) {
			hasPoolObjectComponent = true;
			poolObjectScript = gameObject.GetComponent<PoolObject>();
		}
	}

	public void Reuse<T>(Vector3 position, Quaternion rotation, T args) {
		transform.position = position;
		transform.rotation = rotation;

		if (hasPoolObjectComponent) {
			poolObjectScript.OnObjectReuse (args);
		}

		gameObject.SetActive (true);
	}

	public void SetParent(Transform parent) {
		transform.parent = parent;
	}
}
