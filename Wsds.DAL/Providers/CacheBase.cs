using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using Wsds.DAL.Infrastructure.Extensions;

namespace Wsds.DAL.Providers
{
    public interface ICacheService<T> where T : class {
        IEnumerable<T> Items { get; }
    }

    public class CacheService<T> : ICacheService<T> where T : class {

        private CacheBase<T> _cb; 
        public CacheService (string connString, int timerInterval,
                             Func<T, bool> filterFunc,params Expression<Func<T, object>>[] includies) {
            _cb = new CacheBase<T>(connString, timerInterval,filterFunc,includies);
        }

        public IEnumerable<T> Items
        {
            get
            { 
                return _cb.Items;
            }
        }

    }
    public class CacheBase<T> where T : class
    {
        private object listLoadingSemaphore = new Object();
        private List<T> _list1 = new List<T>();
        private List<T> _list2 = new List<T>();
        private int _curListIndex = 0;
        private Timer _timer = new Timer();
        private string _connString;
        private Func<T, bool> _filter;
        private Expression<Func<T, object>>[] _includies;

        public CacheBase (string connString, int timerInterval, 
                          Func<T, bool> filter, Expression<Func<T, object>>[] includies) {

            _timer.Interval = timerInterval;
            _connString = connString;
            _filter = filter;
            _includies = includies;
            LoadData();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        
        public void Configure (int timerInterval) {
            _timer.Stop();
            _timer.Interval = timerInterval;
            _timer.Start();
        }

        private List<T> _tempitems
        {
            get
            {
                return (_curListIndex == 1) ? _list1 : _list2;
            }

            set
            {
                if (_curListIndex == 1)
                {
                    _list1 = value;
                }
                else
                {
                    _list2 = value;
                }
            }
        }

        public List<T> Items
        {
            get
            {
                return (_curListIndex == 0) ? _list1 : _list2;
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            using (var db = new ORM.FoxStoreDBContext(_connString))
            {
                lock (listLoadingSemaphore)
                {
                    try
                    {
                        _timer.Stop();
                         var dbEntity = db.Set<T>();

                        _tempitems = (_includies == null)
                            ? dbEntity.IncludeLink(_filter)
                            : dbEntity.IncludeLink(_includies, _filter);
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
}