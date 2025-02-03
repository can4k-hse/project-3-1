// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 20:41:19
// Filename: Handler.cs
// Summary: Нестатический клас, выполняющий взаимодействие с пользователем

using CSClasses;
using ConsoleLibrary;
using JSONLibrary.Classes;

namespace Jakovlev3_1
{
    public class Handler
    {
        public Handler(State state)
        {
            _state = state;
        }

        private readonly State _state;

        /// <summary>
        /// Статусы меню
        /// </summary>
        private enum MenuStatus
        {
            Disable,
            Enable,
        }

        /// <summary>
        /// Статус меню
        /// </summary>
        private MenuStatus _status = MenuStatus.Disable;

        /// <summary>
        /// Начало работы меню
        /// </summary>
        public void Start()
        {
            _status = MenuStatus.Enable;

            ConsoleUtilities.Write("Created by Alexander Yakovlev (@Can4k)\n", color: ConsoleColor.White);
            ShowCommands();

            // Создаем поле для ввода команд
            InputSelectString selectString = new(_commads);
        
        
            while (_status is MenuStatus.Enable)
            {
                string command = selectString.Input(LineSign + " ");
                Console.WriteLine(); // Выводим сепаратор
            
                try
                {
                    HandleCommand(command);
                }
                catch (Exception e)
                {
                    ConsoleUtilities.WriteError(e.Message);
                }

                Console.WriteLine(); // Сепаратор между командами
            }
        }

        /// <summary>
        /// Знак начала строки
        /// </summary>
        private const char LineSign = '$';

        /// <summary>
        /// Выводит все доступные команды
        /// </summary>
        private void ShowCommands()
        {
            ConsoleUtilities.Write("Список доступных команд >>>", color: ConsoleColor.White);
            string[] commands =
            [
                "> clear - Очистить консоль",
                "> help - Вывести список доступных команд",
                "> exit - Завершить сессию",
                "> state - Вывести текущую конфигурацию",
                "> 1 - Ввести данные",
                "> 2 - Изменить источник ввода данных",
                "> 3 - Вывести данные",
                "> 4 - Изменить источник вывода данных",
                "> 5 - Отфильтровать данные",
                "> 6 - Отсортировать данные",
                "> 7 - Отобразить дерево возможных продвижений",
            ];

            Console.WriteLine(string.Join("\n", commands));
        }

        /// <summary>
        /// Список доступных команд
        /// </summary>
        private readonly string[] _commads =
        [
            "clear",
            "help",
            "exit",
            "state",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
        ];

        /// <summary>
        /// Завершает работу меню
        /// </summary>
        private void Exit()
        {
            _status = MenuStatus.Disable;
            Console.WriteLine("Сеанс завершен.");
        }

        /// <summary>
        /// Выводит сообщение о неизвестной команде
        /// </summary>
        private void UnknownCommand()
        {
            ConsoleUtilities.Write("Неизвестная команда...", color: ConsoleColor.Red);
        }

        /// <summary>
        /// Отчищает консоль
        /// </summary>
        private void ClearCommand()
        {
            Console.Clear();
        }

        /// <summary>
        /// Обработчик команды способ ввода данных
        /// </summary>
        private void SwitchInputCommand()
        {
            ConsoleUtilities.Write("Укажите новый способ ввода данных:", color:ConsoleColor.White);
            string[] commands =
            [
                "1 - Ввод через консоль",
                "2 - Ввод через файл",
                "3 - Отмена"
            ];

            Console.WriteLine(string.Join("\n", commands));

            // Создаем поле ввода для команд
            InputSelectString selectString = new("1 2 3".Split(' '));
            string cmd = selectString.Input(LineSign + " ");

            switch (cmd)
            {
                case "1":
                    {
                        _state.SetInputToConsole();
                        ConsoleUtilities.WriteSuccess("Данные будут вводиться из консоли");
                        return;
                    }
                case "2":
                    {
                        Console.Write("Введите путь к файлу: ");
                        string filePath = Console.ReadLine() ?? "";
                        _state.SetInputToFile(filePath);
                        ConsoleUtilities.WriteSuccess("Данные будут вводиться из файла");
                        return;
                    }
                case "3":
                    {
                        return;
                    }
                default: UnknownCommand(); break;
            }
        }

        /// <summary>
        /// Обработчик команды смены способа вывода данных
        /// </summary>
        private void SwitchOutputCommand()
        {
            ConsoleUtilities.Write("Укажите новый способ вывода данных:", color:ConsoleColor.White);
            string[] commands =
            [
                "1 - Вывод через консоль",
                "2 - Вывод через файл",
                "3 - Отмена"
            ];

            Console.WriteLine(string.Join("\n", commands));

            // Создаем поле ввода для команд
            InputSelectString selectString = new("1 2 3".Split(' '));
            string cmd = selectString.Input(LineSign + " ");

            switch (cmd)
            {
                case "1":
                    {
                        _state.SetOutputToConsole();
                        ConsoleUtilities.WriteSuccess("Данные будут выводиться в консоль.");
                        return;
                    }
                case "2":
                    {
                        Console.Write("Введите путь к файлу: ");
                        string filePath = Console.ReadLine() ?? "";
                        _state.SetOutputToFile(filePath);
                        ConsoleUtilities.WriteSuccess("Данные будут выводиться в файл");
                        return;
                    }
                case "3":
                    {
                        return;
                    }
                default: UnknownCommand(); break;
            }
        }

        /// <summary>
        /// Обработчик команды ввода данных
        /// </summary>
        private void InputCommand()
        {
            try
            {
                _state.ReadData();
                ConsoleUtilities.WriteSuccess("Данные успешно введены.");
            }
            catch (Exception e)
            {
                ConsoleUtilities.WriteError("При вводе данных произошла ошибка: " + e.Message);
            }
        }

        /// <summary>
        /// Обработчик команды вывода данных
        /// </summary>
        private void OutputCommand()
        {
            try
            {
                _state.WriteData();
                ConsoleUtilities.WriteSuccess("Данные успешно выведены.");
            }
            catch (Exception e)
            {
                ConsoleUtilities.WriteError("При выводе данных произошла ошибка: " + e.Message);
            }
        }

        /// <summary>
        /// Обработчик команды сортировки
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SortCommand()
        {
            string[] options;

            ConsoleUtilities.Write("Укажите действие:", color: ConsoleColor.White);

            // Если уже указана сортировка, предложим пользователю возможность её сброса
            if (_state.SortedByField is not null)
            {
                // Вводим варианты
                options = "1 2 3".Split(' ');

                // Первый пункт
                Console.Write("1 - Сбросить текущую сортировку по полю: ");
                ConsoleUtilities.WriteWarning(_state.SortedByField);
            }
            else
            {
                // Если SortedByField null, сброс фильтра не возможен
                options = "2 3".Split(' ');
            }

            // Второй пункт
            Console.WriteLine("2 - Сортировать по новому значению");

            // Третий пункт
            Console.WriteLine("3 - Отмена");

            // Вводим команду
            InputSelectString selectString = new(options);
            string cmd = selectString.Input(LineSign + " ");

            switch (cmd)
            {
                case "1":
                    {
                        if (_state.SortedByField is not null)
                        {
                            _state.ResetSort();
                            ConsoleUtilities.WriteSuccess("Сортировка сброшена.");
                            return;
                        }

                        goto default; // В противном случае команда не допустима
                    }
                case "2": break; // См. Дальнейший код
                case "3": return;
                default:
                    UnknownCommand();
                    return;
            }

            // Получаем список всех примитивов Follower, удаляем кавычки
            string[] fields = Follower.PrimitiveFields.Select(JsonUtility.RemoveQuotes).ToArray();

            // Выводим поля для сортировки
            ConsoleUtilities.Write("Сортировку можно осуществить по следующим полям:", color: ConsoleColor.White);
            Console.WriteLine(string.Join(' ', fields));

            // Вводим название поля
            InputSelectString inputSelect = new(fields);
            string field = inputSelect.Input("Название поля: ");

            if (!fields.Contains(field))
            {
                throw new Exception("wrong field name: " + field);
            }

            try
            {
                _state.SortByField(field);
                ConsoleUtilities.WriteSuccess("Объекты успешно отсортированы");
            }
            catch (Exception e)
            {
                ConsoleUtilities.WriteError("При сортировки произошла ошибка:" + e.Message);
            }
        }

        /// <summary>
        /// Обработчик команды фильтрации
        /// </summary>
        private void FilterCommand()
        {
            string[] options;

            ConsoleUtilities.Write("Укажите действие:", color: ConsoleColor.White);

            if (_state.FilteredByField is not null)
            {
                options = "1 2 3 4".Split(' ');

                // Первый пункт
                Console.Write("1 - Сбросить текущий фильтр по полю: ");
                ConsoleUtilities.WriteWarning(_state.FilteredByField);

                // Второй пункт
                Console.WriteLine("2 - Изменить допустимый набор значений");
            }
            else
            {
                options = "3 4".Split(' ');
            }

            // Третий пункт
            Console.WriteLine("3 - Указать новый фильтр");

            // Четвертый пункт
            Console.WriteLine("4 - Отмена");

            // Вводим команду
            InputSelectString selectString = new(options);
            string cmd = selectString.Input(LineSign + " ");

            switch (cmd)
            {
                case "1":
                    {
                        if (_state.FilteredByField is null)
                        {
                            goto default;
                        }

                        _state.ResetFilter();
                        ConsoleUtilities.WriteSuccess("Фильтр сброшен.");
                        return;
                    }
                case "2":
                    {
                        if (_state.FilteredByField is null)
                        {
                            goto default;
                        }

                        Console.Write("Введите доступные значения поля через точку с запятой(;): ");
                        string tmpInput = Console.ReadLine() ?? "";

                        try
                        {
                            string[] values = tmpInput.Split(';');
                            _state.FilterByField(_state.FilteredByField, values);
                            ConsoleUtilities.WriteSuccess("Фильтр успешно применен");
                        }
                        catch (Exception e)
                        {
                            ConsoleUtilities.WriteError("При фильтрации произошла ошибка:" + e.Message);
                        }

                        return;
                    }
                case "3": break;
                case "4": return;
                default:
                    UnknownCommand();
                    return;
            }

            // Получаем список всех примитивов Follower, удаляем кавычки
            string[] fields = Follower.PrimitiveFields.Select(JsonUtility.RemoveQuotes).ToArray();

            // Выводим поля для сортировки
            ConsoleUtilities.Write("Фильтрацию можно осуществить по полям:", color: ConsoleColor.White);
            Console.WriteLine(string.Join(' ', fields));

            // Вводим название поля
            InputSelectString inputSelect = new(fields);
            string field = inputSelect.Input("Название поля: ");

            if (!fields.Contains(field))
            {
                throw new Exception("wrong field name: " + field);
            }

            Console.Write("Введите доступные значения поля через точку с запятой(;): ");
            string input = Console.ReadLine() ?? "";

            try
            {
                string[] values = input.Split(';');
                _state.FilterByField(field, values);
                ConsoleUtilities.WriteSuccess("Фильтр успешно применен");
            }
            catch (Exception e)
            {
                ConsoleUtilities.WriteError("При фильтрации произошла ошибка:" + e.Message);
            }
        }

        /// <summary>
        /// Обработчик команды отображения состояния приложения
        /// </summary>
        private void StateCommand()
        {
            ConsoleUtilities.Write("Состояние приложения >>>", color: ConsoleColor.White);

            // Данные по вводу
            Console.Write("Данные вводятся из: ");
            ConsoleUtilities.WriteWarning(_state.InputMode == State.IoMode.Console 
                ? "консоли" 
                : _state.InputFilePath ?? "");

            // Данные по выводу
            Console.Write("Данные выводятся в: ");
            ConsoleUtilities.WriteWarning(_state.OutputMode == State.IoMode.Console
                ? "консоль"
                : _state.OutputFilePath ?? "");

            // Данные по сортировке
            Console.Write(_state.SortedByField is null ? "Данные не сортируются\n" : "Данные сортируются по полю: ");
            if (_state.SortedByField is not null)
            {
                ConsoleUtilities.WriteWarning(_state.SortedByField); // Выводим поле для сортировки
            }
        
            // Данные по фильтрации
            Console.Write(_state.FilteredByField is null ? "Данные не фильтруются\n" : "Данные фильтруются по полю: ");
            if (_state.FilteredByField is not null)
            {
                ConsoleUtilities.WriteWarning(_state.FilteredByField); // Выводим поле для фильтрации

                ConsoleUtilities.Write("Список значений: ", end:' ');
                foreach (string value in _state.FilterValues ?? []) // Выводим значения фильтрации
                {
                    ConsoleUtilities.WriteWarning(value, '\0');
                    Console.Write(';');
                }
                Console.WriteLine(); // Добавляем сепаратор
            }
        }

        /// <summary>
        /// Обработчик команды выполнения основной задачи
        /// </summary>
        private void BaseTaskCommand()
        {
            ConsoleUtilities.Write("Введите ID начального последователя: ", end:' ', color:ConsoleColor.White);
            string startId = Console.ReadLine() ?? "";
            try
            {
                _state.BaseTask(startId);
            }
            catch (Exception e)
            {
                ConsoleUtilities.WriteError("При выполнении основной задачи произошла ошибка: " + e.Message);
            }
        }

        /// <summary>
        /// Обработчик команд
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="Exception"></exception>
        private void HandleCommand(string command)
        {
            if (_status != MenuStatus.Enable)
            {
                throw new Exception("menu is disabled!");
            }

            switch (command)
            {
                case "1": InputCommand(); break;
                case "2": SwitchInputCommand(); break;
                case "3": OutputCommand(); break;
                case "4": SwitchOutputCommand(); break;
                case "5": FilterCommand(); break;
                case "6": SortCommand(); break;
                case "7": BaseTaskCommand(); break;
                case "exit": Exit(); break;
                case "help": ShowCommands(); break;
                case "clear": ClearCommand(); break;
                case "state": StateCommand(); break;
                default: UnknownCommand(); break;
            }
        }
    }
}