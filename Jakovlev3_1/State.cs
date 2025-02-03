// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 21:11:04
// Filename: State.cs
// Summary: Хранит состояние приложения, текущие JSON, настройки фильтраций, сортировки и т.д.

using ConsoleLibrary;
using CSClasses;
using JSONLibrary.Classes;
using JSONLibrary.Interfaces;

namespace Jakovlev3_1
{
    public class State
    {
        /// <summary>
        /// Список последователей
        /// </summary>
        private FollowersList _followers = new();

        private FollowersList _loadedFollowers = new();

        /// <summary>
        ///  Хранит поле, по которому совершена сортировка или null
        /// </summary>
        public string? SortedByField { get; private set; }

        /// <summary>
        /// Хранит поле, по которому совершена фильтрация или null
        /// </summary>
        public string? FilteredByField { get; private set; }

        /// <summary>
        /// Хранит список указанных полей для фильтрации или null
        /// </summary>
        public string[]? FilterValues { get; private set; }

        public enum IoMode
        {
            File,
            Console
        }

        /// <summary>
        /// Относительный путь к файлу ввода данных.
        /// </summary>
        public string? InputFilePath { get; private set; }

        /// <summary>
        /// Режим вывода
        /// </summary>
        public IoMode InputMode { get; private set; } = IoMode.Console;

        /// <summary>
        /// Изменяет источник ввода данных на консоль
        /// </summary>
        public void SetInputToConsole()
        {
            InputMode = IoMode.Console;
            InputFilePath = null;
        }

        /// <summary>
        /// Изменяет источник ввода данных на данный файл
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void SetInputToFile(string filePath)
        {
            // Проверяем файл
            if (!Path.Exists(filePath))
            {
                throw new FileNotFoundException("Файл для ввода не обнаружен");
            }

            InputMode = IoMode.File;
            InputFilePath = filePath;
        }

        /// <summary>
        /// Пытается считать данные выбранным способом и записать в Followers
        /// </summary>
        /// <returns></returns>
        public void ReadData()
        {
            StreamReader inputStream;

            switch (InputMode)
            {
                case IoMode.File:
                {
                    Console.WriteLine("Данные считываются из файла по пути: " + InputFilePath);
                    inputStream = new StreamReader(InputFilePath ?? "");
                    break;
                }

                case IoMode.Console:
                {
                    ConsoleUtilities.Write(
                        "Введите данные в консоль, для окончания ввода введите Enter, а затем Ctrl+Z (+Enter): " +
                        InputFilePath, color: ConsoleColor.White);
                    inputStream = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
                    break;
                }

                default: throw new Exception("wrong input method");
            }

            // Пытаемся считать данные в tmp список
            FollowersList tmp = new FollowersList();
            JsonParser.ReadJson(tmp, inputStream);

            // В случае успешного считывания назначаем ссылку
            _loadedFollowers = _followers = tmp;

            // После загрузки сбрасываем фильтр и сортировку
            ResetFilter();
            ResetSort();
        }

        /// <summary>
        /// Относительный путь к файлу вывода данных.
        /// </summary>
        public string? OutputFilePath { get; private set; }

        /// <summary>
        /// Режим вывода
        /// </summary>
        public IoMode OutputMode { get; private set; } = IoMode.Console;

        /// <summary>
        /// Изменяет способ вывода данных на консоль
        /// </summary>
        public void SetOutputToConsole()
        {
            OutputMode = IoMode.Console;
            OutputFilePath = null;
        }

        /// <summary>
        /// Изменяет источник вывода данных на данный файл
        /// </summary>
        /// <param name="filePath"></param>
        public void SetOutputToFile(string filePath)
        {
            OutputMode = IoMode.File;
            OutputFilePath = filePath;
        }

        /// <summary>
        /// Пытается вывести данные выбранным способом из Followers
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteData()
        {
            StreamWriter outputStream;
            switch (OutputMode)
            {
                case IoMode.Console:
                {
                    outputStream = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
                    break;
                }
                case IoMode.File:
                {
                    Console.WriteLine("Данные выведутся в файл по пути: " + OutputFilePath);

                    // Пытаемся удалить файл, если он уже существует
                    try
                    {
                        File.Delete(OutputFilePath ?? "");
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    outputStream = new StreamWriter(File.Open(OutputFilePath ?? "", FileMode.Create));
                    break;
                }
                default: throw new Exception("wrong output method");
            }

            outputStream.AutoFlush = true;
            JsonParser.WriteJson(_followers, outputStream);
        }

        /// <summary>
        /// Сортирует объекты в _followers по данному полю
        /// </summary>
        /// <param name="fieldName">Поле объекта Follower</param>
        public void SortByField(string fieldName)
        {
            // Создаем глубокую копию текущего списка FollowersList
            FollowersList tempFollowers = new(JsonParser.Parse(JsonParser.Stringify(_followers)));

            // Сортируем followers по полю в лексикографическом формате
            tempFollowers.Followers.Sort(new FollowerComparer((followerA, followerB) =>
                    string.Compare(
                        followerA.GetField(fieldName),
                        followerB.GetField(fieldName),
                        StringComparison.Ordinal)
                )
            );

            // Назначаем новую ссылку
            _followers = tempFollowers;

            // Указываем новый параметр сортировки
            SortedByField = fieldName;
        }

        /// <summary>
        /// Фильтрует массив Followers по полю и данным значениям
        /// </summary>
        /// <param name="fieldName">Поле Follower</param>
        /// <param name="values">Список возможных значений</param>
        public void FilterByField(string fieldName, string[]? values)
        {
            // Скидываем предыдущие параметры фильтрации
            FilteredByField = null;
            FilterValues = [];

            // В данном контексте произведет сортировку, игнорируя предыдущий фильтр
            ReFilterAndSort();

            // Создаем глубокую копию текущего списка FollowersList
            FollowersList tempFollowers = new(JsonParser.Parse(JsonParser.Stringify(_followers)));

            // Добавляем кавычки в элементы values
            values = (values ?? []).Select(JsonUtility.AddQuotes).ToArray();

            // Создаем список IJsonObject, для которых значение данного поля лежит в values
            List<IJsonObject> filteredList = tempFollowers.Followers
                .Where(follower => values.Contains(follower.GetField(fieldName)))
                .Select(json => json as IJsonObject)
                .ToList();

            // Изменяем в tempFollowers значение elements на полученный список
            tempFollowers.SetField("elements", JsonParser.Stringify(filteredList));

            // Переназначаем ссылку
            _followers = tempFollowers;

            // Изменяем параметры фильтрации
            FilterValues = values;
            FilteredByField = fieldName;
        }

        /// <summary>
        /// Сбрасывает текущую сортировку
        /// </summary>
        public void ResetSort()
        {
            SortedByField = null;
            ReFilterAndSort();
        }

        /// <summary>
        /// Сбрасывает фильтр
        /// </summary>
        public void ResetFilter()
        {
            FilterValues = null;
            FilteredByField = null;

            ReFilterAndSort(); // Обновляем _followers
        }

        /// <summary>
        /// Сбрасывает текущий _followers и производит повторную сортировку и фильтрацию
        /// </summary>
        private void ReFilterAndSort()
        {
            _followers = _loadedFollowers;

            if (SortedByField is not null) // Производим сортировку, если указано поле для сортировки
            {
                SortByField(SortedByField);
            }

            if (FilteredByField is not null) // Производим фильтрацию, если указано поле для фильтрации
            {
                FilterByField(FilteredByField, FilterValues);
            }
        }

        /// <summary>
        /// Решает основную задачу для данного Id. Отображает всевозможные пути развития последователя.
        /// Гарантируется, что ни в одном пути не образуется цикл. 
        /// </summary>
        /// <param name="startId">Id стартового Follower</param>
        /// <exception cref="KeyNotFoundException">Если не удалось найти данного последователя</exception>
        public void BaseTask(string startId)
        {
            int startIndex = _followers.Followers.FindIndex(follower => follower.Id == JsonUtility.AddQuotes(startId));

            if (startIndex == -1) // Follower не найден
            {
                throw new KeyNotFoundException("В текущем списке follower с данным id не найден");
            }

            ConsoleUtilities.Write("Будут выведены всевозможны пути развития последователя, исключающие зацикливания");

            // Множество использованных Id
            HashSet<string> usedIds = new();

            // Запускаем отображение эволюции
            DisplayEvolution(_followers.Followers[startIndex], "Изначально", usedIds, 0);
        }

        /// <summary>
        /// Выводит всевозможные пути развития данного follower
        /// </summary>
        /// <param name="follower">Данный follower</param>
        /// <param name="way">Метод развития, по которому данный follower получился</param>
        /// <param name="usedIds">Использованные в данном пути id Followers</param>
        /// <param name="tabulation">Необходимый отступ для корректного отображения</param>
        private void DisplayEvolution(Follower follower, string way, HashSet<string> usedIds, int tabulation)
        {
            if (!usedIds.Add(follower.Id)) // Помечаем данного follower как использованного
            {
                return;
            }

            // Выводим табуляцию
            string output = new string(' ', tabulation);

            // Добавляем данные в вывод, удаляя ненужные кавычки 
            output += JsonUtility.RemoveQuotes(way) + " --> " +
                      JsonUtility.RemoveQuotes(follower.Id)
                      + $"({JsonUtility.RemoveQuotes(follower.Label)})";

            // Выводим полученную строку
            Console.WriteLine(output);

            // Увеличиваем табуляцию
            tabulation = (int)(output.Length * 0.75);

            // Пробегаем по триггерам
            foreach (KeyValuePair<string, string> trigger in follower.Xtriggers.WrappedDictionary)
            {
                // Находим следующего follower
                int nextId = _followers.Followers.FindIndex(follower1 => follower1.Id == trigger.Value);
                if (nextId == -1) // Follower не найден
                {
                    string tmpOutput = new string(' ', tabulation)
                                       + JsonUtility.RemoveQuotes(trigger.Key)
                                       + " --> "
                                       + JsonUtility.RemoveQuotes(trigger.Value);
                    // Отобразим результат
                    Console.WriteLine(tmpOutput);

                    // Тобави триггер в данный used
                    usedIds.Add(trigger.Value);

                    // Пропустим дальнейшую рекурсию
                    continue;
                }

                // Создадим новое множество использованных
                // Данное множество поддерживает то, что на пути развития мы не перейдем в уже пройденного
                // на данном пути персонажа
                HashSet<string> newUsed = usedIds.ToList().ToHashSet();

                // Запустим вложенную рекурсию
                DisplayEvolution(_followers.Followers[nextId], trigger.Key, newUsed, tabulation);
            }
        }
    }
}