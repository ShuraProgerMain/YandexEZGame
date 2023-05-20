using System.Collections.Generic;
using UnityEngine;

namespace SimplePooling
{
    public class ObjectPooler<T> where T : MonoBehaviour
    {
        private Queue<T> _objectsQueue;

        public ObjectPooler(T obj, Transform parent, int countObjectsInPool)
        {
            _objectsQueue = new Queue<T>();

            for (int i = 0; i < countObjectsInPool; i++)
            {
                var newObj = Object.Instantiate(obj, parent);
                newObj.gameObject.SetActive(false);
                _objectsQueue.Enqueue(newObj);
            }
        }

        public T Take()
        {
            var obj = _objectsQueue.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _objectsQueue.Enqueue(obj);
        }
    }
}