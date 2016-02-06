using System.Collections.Generic;

namespace InterviewTest.Database
{
    public interface IDatabase
    {
        void DeleteAll<T>();
        T Get<T>(string id) where T : class;
        List<T> GetAll<T>() where T : class, IPersistable;
        void Save<T>(T obj) where T : IPersistable;
    }
}