// // Author: Alexander Yakovlev
// // CreatedAt: 20 января 2025 г. 22:34:24
// // Filename: Follower.cs
// // Summary: Структура, описывающая последователя. Реализует интерфейс для JSON-подобного представления объекта. 
//
// using JSONLibrary.Interfaces;
//
// namespace CSClasses;
//
// public struct Follower : IJsonObject
// {
//     public string Id { get; private set; }
//     public string Label { get; private set; }
//     public string Description { get; private set; }
//     public string UniquenessGroup { get; private set; }
//     
//     public Dictionary<string, int> Aspects { get; private set; }
//     public Dictionary<string, string> Xtriggers { get; private set; }
//     
//     // TODO переместить это в другую часть кода
//     
//     // // Перечисление всех полей для IJsonObject
//     // private readonly string[] _allFields = [
//     //     "id",
//     //     "label",
//     //     "description",
//     //     "xtrigger",
//     //     "aspects",
//     //     "uniquenessgroup",
//     // ];
//     
//     // TODO добавить парсеры для словарей
//     
//     /// <summary>
//     /// Конструктор структуры Follower
//     /// </summary>
//     /// <param name="data">Данный IJsonObject составляет с Follower композицию, Follower напрямую связан с data</param>
//     public Follower(IJsonObject data)
//     {
//         ArgumentNullException.ThrowIfNull(data);
//
//         // Получаем значение из data, проверяем на не null, записываем в состояние структуры
//         
//         Id = data.GetField("id");
//         Label = data.GetField("label");
//         Description = data.GetField("description");
//         UniquenessGroup = data.GetField("uniquenessgroup");;
//         
//         string? xtriggers = data.GetField("xtriggers");
//         ArgumentNullException.ThrowIfNull(xtriggers);
//         // Xtriggers = xtriggers;
//         
//         string? aspects = data.GetField("aspects");
//         ArgumentNullException.ThrowIfNull(aspects);
//         // Aspects = aspects;
//     }
//     
//     // TODO улучшить работу GetAllFields
//     
//     public IEnumerable<string> GetAllFields()
//     {
//         return (IEnumerable<string>) _allFields.Clone();
//     }
//
//     // TODO добавить строковую интерпритацию Xtriggers и Aspects
//     
//     public string? GetField(string fieldName)
//     {
//         return fieldName switch
//         {
//             "id" => $"\"{Id}\"",
//             "label" => $"\"{Label}\"",
//             "description" =>  $"\"{Description}\"",
//             "uniquenessgroup" => $"\"{UniquenessGroup}\"",
//             
//             // У следующих полей не добавляем кавычки, поскольку они являются JSON-подобными
//             "xtriggers" => Xtriggers,
//             "aspects" => Aspects,
//             _ => null
//         };
//     }
//     
//     // TODO при изменении добавить парсеры для словорей
//     
//     public void SetField(string fieldName, string value)
//     {
//         switch (fieldName)
//         {
//             case "id": Id = value; break;
//             case "label": Label = value; break;
//             case "description": Description = value; break;
//             case "uniquenessgroup": UniquenessGroup = value; break;
//             case "xtriggers": Xtriggers = value; break;
//             case "aspects": Aspects = value; break;
//             default: throw new KeyNotFoundException();
//         }
//     }
// }