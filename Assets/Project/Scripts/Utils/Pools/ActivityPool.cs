using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class ActivityPool<T> where T : Component
    {
        private List<T> _elements;
        private T _prefab;
        private Transform _container;

        public ActivityPool(T prefab, Transform container = null, int initialAmount = 10)
        {
            _prefab = prefab;
            _container = container;
            _elements = new List<T>(initialAmount);
            for (int i = 0; i < initialAmount; i++)
            {
                Create();
            }
        }

        public virtual T Get()
        {
            T obj;
            for (int i = 0; i < _elements.Count; i++)
            {
                obj = _elements[i];
                if (obj.IsActive() == false)
                {
                    obj.Activate();
                    return obj;
                }
            }

            obj = Create();
            obj.Activate();
            return obj;
        }

        public virtual void HideAll()
        {
            foreach (var obj in _elements)
            {
                obj.Disactivate();
            }
        }

        protected virtual T Create()
        {
            T obj = Object.Instantiate(_prefab);
            if (_container != null) obj.transform.SetParent(_container);
            obj.name = $"Element #{_elements.Count}";
            _elements.Add(obj);
            obj.Disactivate();

            return obj;
        }
    }
}