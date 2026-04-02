using System.Collections.Generic;
using UnityEngine;

namespace EMA.Scripts.PatternClasses
{
    [System.Serializable]
    public class ObjectPool
    {
        [SerializeField] private int hmObjCreateAtFirst;
        [Tooltip("Set 0 for infinity")][SerializeField] private int poolCapacity;
        [SerializeField] private GameObject pooledObjectPrefab;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool autoGrow;

        private int activeObjCount = 0;

        private Queue<GameObject> poolQueue;
        
        public delegate void ResetObj(GameObject pooledObj);

        public void CreatePool()
        {
            poolQueue = new Queue<GameObject>(poolCapacity);

            for (int i = 0; i < hmObjCreateAtFirst; i++) {
              
                var obj = GameObject.Instantiate(pooledObjectPrefab, parentTransform);
                obj.SetActive(false);
                
                poolQueue.Enqueue(obj);
            }
            
            activeObjCount = hmObjCreateAtFirst;
        }

        public GameObject GetPooledObject()
        {
            Start :
            if (poolQueue.Count > 0)
            {
                var obj = poolQueue.Dequeue();
                obj.SetActive(true);
                activeObjCount++;
                return obj;
            }
            
            else if (autoGrow || activeObjCount == 0)
            {
                var obj = GameObject.Instantiate(pooledObjectPrefab, parentTransform);
                poolQueue.Enqueue(obj);
                goto Start;
            }

            else if (!autoGrow || activeObjCount >= poolCapacity)
            {
                Debug.LogError("No Object In Pool.");
                return null;
            }
            
            var newObj = GameObject.Instantiate(pooledObjectPrefab, parentTransform);
            poolQueue.Enqueue(newObj);
            goto Start;
        }

        public void SendObjectToPool(GameObject pooledObj, ResetObj resetObj = null)
        {
            resetObj?.Invoke(pooledObj);
            pooledObj.SetActive(false);
            poolQueue.Enqueue(pooledObj);
            activeObjCount--;
        }

        public GameObject[] GetArrayOfPooledObjects()
        {
            return poolQueue.ToArray();
        }

    }
}