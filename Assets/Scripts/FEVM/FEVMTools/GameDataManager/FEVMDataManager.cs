using System;
using System.Collections.Generic;

namespace FEVM.Data
{
    public sealed class FEVMDataManager
    {
        private static readonly Lazy<FEVMDataManager> _instance = new Lazy<FEVMDataManager>(() => new FEVMDataManager());

        public static FEVMDataManager Instance => _instance.Value;

        private readonly Dictionary<Type, IDataKeeper> _keepers = new();

        private FEVMDataManager() { }

        public void AddDataKeeper<T>(T data) where T : IDataKeeper
        {
            var key = typeof(T);
            if (_keepers.ContainsKey(key))
            {
                _keepers[key] = data;
            }
            else
            {
                _keepers.Add(key, data);
            }
        }

        public T GetData<T>() where T : BaseData
        {
            var key = typeof(T);
            if (!_keepers.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Key '{key}' not found in Keepers.");
            }

            return _keepers[key].FetchData() as T;
        }

        public void SaveData<T>() where T : BaseData
        {
            var key = typeof(T);
            if (_keepers.ContainsKey(key))
            {
                _keepers[key].Save();
            }
        }

        public void SaveAll()
        {
            foreach (var keeper in _keepers.Values)
            {
                keeper.Save();
            }
        }

        public void WriteData<T>(T data) where T : BaseData
        {
            var key = typeof(T);
            if (_keepers.ContainsKey(key))
            {
                _keepers[key].Write(data);
            }
        }

        public void WriteAndSaveData<T>(T data) where T : BaseData
        {
            WriteData(data);
            SaveData<T>();
        }
    }
}