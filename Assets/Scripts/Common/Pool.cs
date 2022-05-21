using System;
using System.Collections.Generic;

namespace PotentialRobot.Common
{
    public class Pool<T>
    {
        private readonly Func<T> _factoryMethod;
        private readonly Func<T, T> _resetMethod;

        private Stack<T> _pool;

        public Pool(Func<T> factoryMethod, Func<T, T> resetMethod = null, int initialCount = 0)
        {
            _factoryMethod = factoryMethod;
            _resetMethod = resetMethod;
            InitPool(initialCount);
        }

        public T Lease()
        {
            if (!TryLease(out T item))
                item = _factoryMethod();

            return item;
        }

        public void Recycle(T item)
        {
            if (_resetMethod != null)
                item = _resetMethod(item);

            _pool.Push(item);
        }

        private void InitPool(int initialCount)
        {
            _pool = new Stack<T>();
            for (int i = 0; i < initialCount; i++)
                _pool.Push(_factoryMethod());
        }

        private bool TryLease(out T item)
        {
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }
    }
}
