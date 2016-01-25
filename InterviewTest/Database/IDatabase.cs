using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace InterviewTest.Database
{
    public interface IDatabase
    {
        T Get<T>(string id) where T : class;

        List<T> GetAll<T>() where T : class, IPersistable;

        void Save<T>(T obj) where T : IPersistable;

        void DeleteAll<T>();

        T Deserialise<T>(string fqFilename);
    }
}