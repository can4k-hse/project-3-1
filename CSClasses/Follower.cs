// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 22:34:24
// Filename: Follower.cs
// Summary: Структура, описывающая последователя. Реализует интерфейс для JSON-подобного представления объекта. 

using JSONLibrary.Classes;
using JSONLibrary.Interfaces;
namespace CSClasses;


// TODO сделать заполнение по умолчанию
public struct Follower : IJsonObject
{
    public string Id { get; private set; }
    public string Label { get; private set; }
    public string Description { get; private set; }
    public string UniquenessGroup { get; private set; }
    
    public FollowerAspects Aspects { get; private set; }
    public FollowerXtriggers Xtriggers { get; private set; }
    
    // TODO добавить парсеры для словарей
    
    /// <summary>
    /// Конструктор структуры Follower
    /// </summary>
    /// <param name="data">Данный IJsonObject составляет с Follower композицию, Follower напрямую связан с data</param>
    public Follower(Dictionary<string, string> data)
    {
        try
        {
            foreach (var pair in data)
            {
                SetField(pair.Key, pair.Value);
            }
        }
        catch (Exception e)
        {
            throw new FormatException("Wrong JSON data: " + e.Message);
        }
    }
    
    
    /// <summary>
    /// Перечисление всех полей тождественного JSON
    /// </summary>
    private const string AllFields = "id label description xtrigger aspects uniquenessgroup";
    public IEnumerable<string> GetAllFields()
    {
        return AllFields.Split(' ');
    }
    
    public string? GetField(string fieldName)
    {
        return fieldName switch
        {
            "id" => $"{Id}",
            "label" => $"{Label}",
            "description" => $"{Description}",
            "uniquenessgroup" => $"{UniquenessGroup}",
            "xtriggers" => JsonParser.Stringify(Xtriggers),
            "aspects" => JsonParser.Stringify(Aspects),
            _ => null
        };
    }
    
    public void SetField(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "\"id\"": Id = value; break;
            case "\"label\"": Label = value; break;
            case "\"description\"": Description = value; break;
            case "\"uniquenessgroup\"": UniquenessGroup = value; break;
            
            // Для вложенных полей добавляем конструкторы вложенных классов 
            case "\"xtriggers\"": Xtriggers = new FollowerXtriggers(value); break;
            case "\"aspects\"": Aspects = new FollowerAspects(value); break;
            default: throw new KeyNotFoundException();
        }
    }
}