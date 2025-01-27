// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 17:56:01
// Filename: FollowerXtriggers.cs
// Summary: Обертка над Dictionary<string, string> реализующая интерфейс IJsonObject
// Хранит Xtriggers, включена в Follower

using JSONLibrary.Classes;
using JSONLibrary.Interfaces;
namespace CSClasses;

public readonly struct FollowerXtriggers : IJsonObject
{
    private readonly Dictionary<string, string> _wrappedDictionary;

    public FollowerXtriggers(string stringifyDictionary)
    {
        Dictionary<string, string> tmp = JsonParser.Parse(stringifyDictionary);
        _wrappedDictionary = [];
        foreach (var pair in tmp)
        {
            _wrappedDictionary.Add(pair.Key, pair.Value);
        }
    }
    
    public IEnumerable<string> GetAllFields()
    {
        return _wrappedDictionary.Keys;
    }

    public string? GetField(string fieldName)
    {
        return _wrappedDictionary.GetValueOrDefault(fieldName);
    }

    public void SetField(string fieldName, string value)
    {
        if (!_wrappedDictionary.ContainsKey(fieldName))
        {
            throw new KeyNotFoundException("wrong field name");
        }

        _wrappedDictionary[fieldName] = value;
    }
}