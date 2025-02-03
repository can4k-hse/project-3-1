// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 17:39:23
// Filename: FollowerAspects.cs
// Summary: Обертка над Dictionary<string, int> реализующая интерфейс IJsonObject
// Хранит Aspects, включена в Follower

using JSONLibrary.Classes;
using JSONLibrary.Interfaces;
namespace CSClasses
{
    public readonly struct FollowerAspects : IJsonObject
    {
        private readonly Dictionary<string, int> _wrappedDictionary;

        public FollowerAspects(string stringifyDictionary)
        {
            Dictionary<string, string> tmp = JsonParser.Parse(stringifyDictionary);
            _wrappedDictionary = [];
            foreach (KeyValuePair<string, string> pair in tmp)
            {
                if (!int.TryParse(pair.Value, out int parsed))
                {
                    throw new ArgumentException("invalid JSON data: wrong type");
                }
            
                _wrappedDictionary.Add(pair.Key, parsed);
            }
        }
    
        public IEnumerable<string> GetAllFields()
        {
            return _wrappedDictionary.Keys;
        }

        public string? GetField(string fieldName)
        {
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            return _wrappedDictionary.TryGetValue(fieldName, out int value) ? value.ToString() : null;
        }

        public void SetField(string fieldName, string value)
        {   
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            if (!_wrappedDictionary.ContainsKey(fieldName))
            {
                throw new KeyNotFoundException("wrong field name");
            }
        
            _wrappedDictionary[fieldName] = int.Parse(value);
        }
    }
}