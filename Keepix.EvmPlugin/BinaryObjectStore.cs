using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Keepix.EvmPlugin
{
    internal class BinaryObjectStore
    {
        private readonly string _filePath;
        private Dictionary<string, byte[]> _store;

        // this.store = new BinaryObjectStore("evm_" + this.EVMConnectionConfig.Id + ".bin");
        public BinaryObjectStore(string filePath)
        {
            _filePath = filePath;

            if (File.Exists(_filePath))
            {
                LoadFromFile();
            }
            else
            {
                _store = new Dictionary<string, byte[]>();
            }
        }

        public void Store<T>(string id, T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                _store[id] = memoryStream.ToArray();
            }

            SaveToFile();
        }

        public T Retrieve<T>(string id)
        {
            if (!_store.ContainsKey(id))
            {
                throw new KeyNotFoundException($"No data found for the given ID: {id}");
            }

            using (var memoryStream = new MemoryStream(_store[id]))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        private void LoadFromFile()
        {
            using (var fileStream = new FileStream(_filePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                _store = (Dictionary<string, byte[]>)formatter.Deserialize(fileStream);
            }
        }

        private void SaveToFile()
        {
            using (var fileStream = new FileStream(_filePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, _store);
            }
        }
    }
}
