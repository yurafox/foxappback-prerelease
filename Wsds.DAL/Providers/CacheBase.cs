using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.RedisObjects;


namespace Wsds.DAL.Providers
{
    public interface ICacheService<T> where T : class
    {
        string EntityName { get; }
        T Item(long id);
        IDictionary<long, T> Items { get; }
    }

    public class CacheService<T> : ICacheService<T> where T : class
    {

        private CacheBase<T> _cb;
        public string EntityName { get; }
        public CacheService(string entityName,
                            int timerInterval, 
                            Context redisCache,
                            bool preLoadData = true)
        {
            EntityName = entityName;
            _cb = new CacheBase<T>(entityName, 
                                   timerInterval,
                                   redisCache,
                                   preLoadData);
        }

        public IDictionary<long, T> Items => _cb.Items;
        
        public T Item (long id)  => _cb.Item(id);
    }

    public class CacheBase<T> where T : class
    {
        private object listLoadingSemaphore = new Object();
        private int _curListIndex = 0;
        private Timer _timer = new Timer();
        private string _entityName;
        private bool _preLoadData;
        private Context _redisCache;
        private EntityConfig _entityConfig; 


        public CacheBase(string entityName, 
                         int timerInterval,
                         Context redisCache,
                         bool preLoadData)
        {
            _timer.Interval = timerInterval;
            _entityName = entityName;
            _redisCache = redisCache;
            _preLoadData = preLoadData;
            _entityConfig = EntityConfigDictionary.GetConfig(_entityName);

            if (_preLoadData)
                LoadData();

            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Start();
        }

        private IRedisDictionary<long, T> TempHash => 
            _redisCache.Collections.GetRedisDictionary<long, T>(TempKey);

        private IRedisDictionary<long, T> ItemsHash => 
            _redisCache.Collections.GetRedisDictionary<long, T>(ItemsKey);

        public T Item(long id)
        {
            if (Items.ContainsKey(id))
            {
                Items.TryGetValue(id, out T res);
                return res;
            }
            else
            {
                T item =  new EntityProvider<T>(_entityConfig)
                              .GetItem(id);
                if (item != null) {
                    ItemsHash.Add(id, item);
                }
                return item;
            }
        }

        public void Configure(int timerInterval)
        {
            _timer.Stop();
            _timer.Interval = timerInterval;
            _timer.Start();
        }

        private string ItemsKey => (_curListIndex == 1) ? _entityName + ":hash1" : _entityName + ":hash2";

        private string TempKey => (_curListIndex == 0) ? _entityName + ":hash1" : _entityName + ":hash2";

        public IDictionary<long, T> Items => _redisCache.Collections.GetRedisDictionary<long, T>(ItemsKey);

        private void OnTimedEvent(object source, ElapsedEventArgs e) => LoadData();

        private void FillCache(IDictionary<long, T> list)
        {
            try
            {
                TempHash.Clear();
                TempHash.AddRange(list);
            }
            catch
            {
                Console.WriteLine("Error during Redis AddRange call");
            }
        }

        public void LoadData()
        {
            lock (listLoadingSemaphore)
            {
                try
                {
                    _timer.Stop();
                    IDictionary<long, T> dict = new Dictionary<long, T>();
                    var prov = new EntityProvider<T>(_entityConfig);
                    FillCache(prov.GetItems().ToDictionary(
                                    x => Int64.Parse(x.GetType().GetProperty(_entityConfig.KeyField)
                                        .GetValue(x).ToString()),
                                    x => x
                                    )
                                );

                    //Переключаем списки
                    _curListIndex = (_curListIndex == 0) ? 1 : 0;
                }
                finally
                {
                    _timer.Start();
                }
            }
        }


    }
}