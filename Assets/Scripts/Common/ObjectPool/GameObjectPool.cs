using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectPool", menuName = "BMFrameWork/Scriptable/Pool", order = 2)]
public class GameObjectPool : ScriptableReference
{
    private static GameObjectPool _instance;

    [SerializeField] private string containerTag = "GameObjectPool";

    private readonly Dictionary<int, Stack<GameObject>> pooledGameObjectsByProtoId = new Dictionary<int, Stack<GameObject>>();
    private readonly Dictionary<GameObject, int> takenObjectProtoIds = new Dictionary<GameObject, int>();
    private Transform container;

    protected override void OnRegistered()
    {
        container = GameObject.FindGameObjectWithTag(containerTag).transform;

        _instance = this;
    }

    protected override void OnUnregister()
    {
        foreach (var pooledObjects in pooledGameObjectsByProtoId)
        {
            foreach (var pooledObject in pooledObjects.Value)
            {
                Destroy(pooledObject);
            }
        }

        pooledGameObjectsByProtoId.Clear();
        takenObjectProtoIds.Clear();
        _instance = null;
    }

    private bool ProcessReturn(GameObject returnedObject, bool destroyed)
    {
        if (!takenObjectProtoIds.TryGetValue(returnedObject, out int protoId))
        {
            return false;
        }

        takenObjectProtoIds.Remove(returnedObject);
        if (destroyed)
        {
            return false;
        }

        ProcessPooling(returnedObject, protoId);
        return true;
    }

    private void ProcessPooling(GameObject pooledObject, int protoId)
    {
        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(container);

        if (pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
        {
            pooledObjects.Push(pooledObject);
        }
        else
        {
            pooledObjects = new Stack<GameObject>();
            pooledObjects.Push(pooledObject);
            pooledGameObjectsByProtoId.Add(protoId, pooledObjects);
        }
    }

    private T TakeOrCreate<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent) where T : Behaviour
    {
        return TakeOrCreate(prototype.gameObject, position, rotation, parent).GetComponent<T>();
    }

    private GameObject TakeOrCreate(GameObject prototype, Vector3 position, Quaternion rotation, Transform parent)
    {
        int protoId = prototype.GetInstanceID();
        GameObject newObject = TakeIfAvailable(protoId, position, rotation, parent);
        if (null == newObject)
        {
            newObject = Instantiate(prototype, position, rotation, parent);
            takenObjectProtoIds.Add(newObject, protoId);
        }

        return newObject;
    }

    private GameObject TakeIfAvailable(int protoId, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
        {
            while (0 < pooledObjects.Count)
            {
                GameObject objectToTake = pooledObjects.Pop();
                if (null == objectToTake)
                {
                    continue;
                }

                objectToTake.transform.position = position;
                objectToTake.transform.rotation = rotation;
                objectToTake.transform.SetParent(parent ?? container);
                objectToTake.SetActive(true);

                takenObjectProtoIds.Add(objectToTake, protoId);
                return objectToTake;
            }
        }
        return null;
    }

    public static void PreInstantiate(GameObject prototype, int preinstantiatedCount)
    {
        if (null == _instance)
        {
            return;
        }

        int protoId = prototype.GetInstanceID();
        int existingCount = 0;

        if (_instance.pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
        {
            existingCount = pooledObjects.Count;
        }

        for (int i = existingCount; i < preinstantiatedCount; ++i)
        {
            _instance.ProcessPooling(Instantiate(prototype, Vector3.zero, Quaternion.identity), protoId);
        }
    }

    public static void PreInstantiate<T>(T prototypeBehaviour, int preinstantiatedCount) where T : Behaviour
    {
        PreInstantiate(prototypeBehaviour.gameObject, preinstantiatedCount);
    }

    public static GameObject Take(GameObject prototype, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return _instance != null ? _instance.TakeOrCreate(prototype, position, rotation, parent) : Instantiate(prototype, position, rotation, parent);
    }

    public static T Take<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent = null) where T : Behaviour
    {
        return _instance != null ? _instance.TakeOrCreate(prototype, position, rotation, parent) : Instantiate(prototype, position, rotation, parent);
    }

    public static T Take<T>(T prototype) where T : Behaviour
    {
        return _instance != null
            ? _instance.TakeOrCreate(prototype, prototype.transform.localPosition, prototype.transform.rotation, null)
            : Instantiate(prototype, prototype.transform.localPosition, prototype.transform.rotation, null);
    }

    public static void Return(GameObject takenObject, bool destroyed)
    {
        bool returnedSuccessfully = _instance?.ProcessReturn(takenObject, destroyed) ?? false;
        if (!returnedSuccessfully && !destroyed)
        {
            Destroy(takenObject);
        }
    }

    public static void Return<T>(T takenObject, bool destroyed) where T : Behaviour
    {
        Return(takenObject.gameObject, destroyed);
    }
}