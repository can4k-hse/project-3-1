// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 21:04:29
// Filename: IJsonObject.cs
// Summary: Интерфейс для представления класса/структуры как JSON-подобного объекта


namespace JSONLibrary.Interfaces
{
    public interface IJsonObject
    {
        /// <summary>
        /// Возвращает набор ключей первого уровня JSON-подобного объекта. В качестве полей в GetAllFields могут
        /// быть как примитивы, так и строковые представления JSON-подобных объектов более глубокого уровня.
        /// Для доступа к значениям более глубокого уровня потребуется многократное приведение строкового представления
        /// в IJsonObject и обращение к полям.
        /// Все ключи возвращаются в кавычках. 
        /// </summary>
        IEnumerable<string> GetAllFields();

        /// <summary>
        /// Возвращает строковое значение по ключу
        /// </summary>
        /// <param name="fieldName">Ключ в кавычках</param>
        /// <returns>Значение по данному ключу или null, если данный ключ не найден</returns>
        string? GetField(string fieldName);

        /// <summary>
        /// Изменяет поле по ключу, присваивает новое значение
        /// </summary>
        /// <param name="fieldName">Ключ в кавычках</param>
        /// <param name="value">Корректное с точки зрение нотации значение</param>
        /// <exception cref="KeyNotFoundException">Возникает при попытке изменить несуществующее поле</exception>
        /// <exception cref="FormatException">Возникает при попытке присвоить неверное значение полю</exception>
        void SetField(string fieldName, string value);
    }
}