// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 17:56:01
// Filename: FollowerXtriggers.cs
// Summary: Обертка над Dictionary<string, string> реализующая интерфейс IJsonObject
// Хранит Xtriggers, включена в Follower

using JSONLibrary.Classes;
using JSONLibrary.Interfaces;
namespace CSClasses
{
    public readonly struct FollowerXtriggers : IJsonObject
    {
        public readonly Dictionary<string, string> WrappedDictionary;

        public FollowerXtriggers(string stringifyDictionary)
        {
            Dictionary<string, string> tmp = JsonParser.Parse(stringifyDictionary);
            WrappedDictionary = [];
            foreach (KeyValuePair<string, string> pair in tmp)
            {
                WrappedDictionary.Add(pair.Key, pair.Value);
            }
        }
    
        public IEnumerable<string> GetAllFields()
        {
            return WrappedDictionary.Keys;
        }

        public string? GetField(string fieldName)
        {
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            return WrappedDictionary.GetValueOrDefault(fieldName);
        }

        public void SetField(string fieldName, string value)
        {
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            if (!WrappedDictionary.ContainsKey(fieldName))
            {
                throw new KeyNotFoundException("wrong field name");
            }

            WrappedDictionary[fieldName] = value;
        }
    }
}