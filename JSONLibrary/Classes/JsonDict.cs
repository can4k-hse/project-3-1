// Author: Alexander Yakovlev
// CreatedAt: 21 января 2025 г. 15:22:35
// Filename: JsonDict.cs
// Summary: Обертка над Dictionary<string, string>, реализующая интерфейс IJsonObject


using JSONLibrary.Interfaces;

namespace JSONLibrary.Classes;

public class JsonDict : IJsonObject
{
    private Dictionary<string, string> dict = new();

    public IEnumerable<string> GetAllFields()
    {
        return dict.Keys;
    }

    public string? GetField(string fieldName)
    {
        return dict.GetValueOrDefault(fieldName);
    }

    public void SetField(string fieldName, string value)
    {
        if (!dict.TryAdd(fieldName, value))
        {
            throw new KeyNotFoundException();
        }
    }
}