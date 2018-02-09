using System;

namespace Wsds.DAL.Providers
{
    public class SingletonProvider<T> where T : new()
    {
        private static object syncRootSemaphore = new Object();

        SingletonProvider() { }

        public static T Instance
        {
            get
            {
                return SingletonCreator.Instance;
            }
        }

        class SingletonCreator
        {
            private static T _instance;
            private static object syncRootSemaphore = new Object();
            static SingletonCreator() { }

            internal static T Instance
            {
                get {

                    if (_instance == null)
                    {
                        lock (syncRootSemaphore)
                        {
                            if (_instance == null)
                                _instance = new T();
                        }
                    }

                    return _instance;
                }
            }
        }
    }
}