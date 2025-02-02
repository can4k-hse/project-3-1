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
    public string Id { get; private set; } = "\"\"";
    public string Icon { get; private set; } = "\"\"";
    public string Label { get; private set; } = "\"\"";
    public string Description { get; private set; } = "\"\"";
    public string Comments { get; private set; } = "\"\"";
    public int Lifetime { get; private set; } = 0;
    public string UniquenessGroup { get; private set; } = "\"\"";
    
    public string DecayTo { get; private set; } = "\"\"";
    
    public FollowerAspects Aspects { get; private set; }
    public FollowerXtriggers Xtriggers { get; private set; }
    public Follower() {}
    
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
            throw new FormatException("wrong follower data " + e.Message);
        }
    }
    
    
    public IEnumerable<string> GetAllFields()
    {
        return
        [
            "\"id\"",
            "\"icon\"",
            "\"decayTo\"",
            "\"label\"",
            "\"description\"",
            "\"uniquenessgroup\"",
            "\"lifetime\"",
            "\"comments\"",
            "\"xtriggers\"",
            "\"aspects\""
        ];
    }
    
    /// <summary>
    /// Возвращает список полей объекта, в которых хранятся примитивные типы
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> PrimitiveFields =>
    [
        "\"id\"",
        "\"icon\"",
        "\"decayTo\"",
        "\"label\"",
        "\"description\"",
        "\"uniquenessgroup\"",
        "\"lifetime\"",
        "\"comments\"",
    ];

    public string? GetField(string fieldName)
    {
        // В случае, если в fieldName забыли указать кавычки
        // добавим их
        
        
        return fieldName switch
        {
            "\"id\"" => $"{Id}",
            "\"icon\"" => $"{Icon}",
            "\"decayTo\"" => $"{DecayTo}",
            "\"label\"" => $"{Label}",
            "\"description\"" => $"{Description}",
            "\"uniquenessgroup\"" => $"{UniquenessGroup}",
            "\"lifetime\"" => $"{Lifetime}",
            "\"comments\"" => $"{Comments}",
            "\"xtriggers\"" => JsonParser.Stringify(Xtriggers),
            "\"aspects\"" => JsonParser.Stringify(Aspects),
            _ => null
        };
    }
    
    public void SetField(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "\"id\"": Id = value; break;
            case "\"icon\"": Icon = value; break;
            case "\"label\"": Label = value; break;
            case "\"description\"": Description = value; break;
            case "\"uniquenessgroup\"": UniquenessGroup = value; break;
            case "\"lifetime\"": Lifetime = int.Parse(value); break;
            case "\"decayTo\"": DecayTo = value; break; 
            case "\"comments\"": Comments = value; break;
            
            // Для вложенных полей добавляем конструкторы вложенных классов 
            case "\"xtriggers\"": Xtriggers = new FollowerXtriggers(value); break;
            case "\"aspects\"": Aspects = new FollowerAspects(value); break;
            default: throw new KeyNotFoundException(fieldName);
        }
    }
}