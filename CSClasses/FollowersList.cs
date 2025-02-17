// Author: Alexander Yakovlev
// CreatedAt: 30 января 2025 г. 10:49:36
// Filename: FollowersList.cs
// Summary: Список последователей, принимаемый на вход в программу в качестве JSON


using JSONLibrary.Classes;
using JSONLibrary.Interfaces;

namespace CSClasses
{
    public class FollowersList : IJsonObject
    {
        public FollowersList() {}
    
        public FollowersList(Dictionary<string, string> data)
        {
            try
            {
                foreach (KeyValuePair<string, string> pair in data)
                {
                    SetField(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                throw new FormatException("wrong follower list data " + e.Message);
            }
        }

        public List<Follower> Followers
        {
            get;
            private set;
        } = [];

        public IEnumerable<string> GetAllFields()
        {
            return ["\"elements\""]; // Возвращаем единственное поле
        }

        public string? GetField(string fieldName)
        {
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            try
            {
                if (fieldName == "\"elements\"")
                {
                    // Возвращаем строковое представление массива элементов
                    return JsonParser.Stringify(
                        Followers.Select(obj => (IJsonObject)obj).ToList()
                    );
                }

                throw new KeyNotFoundException(); // Пробрасываем исключение не найденного ключа
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetField(string fieldName, string value)
        {
            // В случае, если в fieldName забыли указать кавычки
            // добавим их
            fieldName = JsonUtility.AddQuotes(fieldName);
        
            if (fieldName != "\"elements\"")
            {
                throw new KeyNotFoundException();
            }

            try
            {
                // Парисим данные
                Dictionary<string, string> parsedValue = JsonParser.Parse(value);
                List<string> sArray = JsonUtility.ConvertPseudoArrayToList(parsedValue);

                // Записываем данные в промежуточный массив
                List<Follower> tmp = new List<Follower>();
                foreach (string sRep in sArray)
                {
                    // Вставляем нового последователя
                    tmp.Add(new Follower(JsonParser.Parse(sRep)));
                }

                // Переназначаем ссылку только если новое значение полностью корректное
                Followers = tmp;
            }
            catch (Exception e)
            {
                throw new FormatException("invalid data: " + e.Message);
            }
        }
    }
}