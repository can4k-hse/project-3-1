// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 20:41:19
// Filename: Handler.cs
// Summary: Нестатический клас, выполняющий взаимодействие с пользователем

using CSClasses;

public class Handler
{
    private Handler() {}
    
    public Handler(State state)
    {
        _state = state;
    }

    private State _state;
    
    /// <summary>
    /// Статусы меню
    /// </summary>
    enum MenuStatus
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
        
        Console.WriteLine("Начало сеанса.");
        ShowCommands();

        while (_status is MenuStatus.Enable)
        {
            Console.Write(LineSign + " ");
            string command = (Console.ReadLine() ?? "").Trim();

            try
            {
                HandleCommand(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
        Console.WriteLine("<Список доступных команд>");
        string[] commands =
        [
            "clear - Отчистить консоль",
            "help - Вывести список доступных команд",
            "exit - Завершить сессию",
            "[]state - Вывести текущую конфигурацию",
            "1 - Ввести данные",
            "2 - Изменить источник ввода данных",
            "3 - Вывести данные",
            "4 - Изменить источник вывода данных",
            "[]5 - Отфильтровать данные",
            "[]6 - Отсортировать данные",
            "[]7 - Узнать что-то",
        ];
        
        Console.WriteLine(string.Join("\n", commands) + "\n");
    }

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
        Console.WriteLine("Неизвестная команда...");
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
        Console.WriteLine("Укажите новый способ ввода данных:");
        string[] commands =
        [
            "1. Ввод через консоль",
            "2. Ввод через файл",
            "3. Отмена"
        ];
        
        Console.WriteLine(string.Join("\n", commands));

        string cmd = Console.ReadLine() ?? "";
        switch (cmd)
        {
            case "1":
            {
                _state.SetInputToConsole();
                Console.WriteLine("Данные будут вводиться из консоли");
                return;
            }
            case "2":
            {
                Console.Write("Введите путь к файлу: ");
                string filePath = Console.ReadLine() ?? "";
                _state.SetInputToFile(filePath);
                Console.WriteLine("Данные будут вводиться из файла");
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
        Console.WriteLine("Укажите новый способ вывода данных:");
        string[] commands =
        [
            "1. Вывод через консоль",
            "2. Вывод через файл",
            "3. Отмена"
        ];
        
        Console.WriteLine(string.Join("\n", commands));

        string cmd = Console.ReadLine() ?? "";
        switch (cmd)
        {
            case "1":
            {
                _state.SetOutputToConsole();
                Console.WriteLine("Данные будут выводиться в консоль");
                return;
            }
            case "2":
            {
                Console.Write("Введите путь к файлу: ");
                string filePath = Console.ReadLine() ?? "";
                _state.SetOutputToFile(filePath);
                Console.WriteLine("Данные будут выводиться в файл");
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
            Console.WriteLine("Данные успешно введены.");
        }
        catch (Exception e)
        {
            Console.WriteLine("При вводе данных произошла ошибка: " + e.Message); 
        }
    }
    
    private void OutputCommand()
    {
        try
        {
            _state.WriteData();
            Console.WriteLine("Данные успешно выведены.");
        }
        catch (Exception e)
        {
            Console.WriteLine("При выводе данных произошла ошибка: " + e.Message); 
        }
    }

    private void SortCommand()
    {
        // Выбор поля для сортировки
        // Отображение поля для сортировки в интерфейсе
        // Сортировка по значениям выбранного поля

        var fields = Follower.PrimitiveFields.ToArray();
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
            case "5": throw new NotImplementedException();
            case "6": throw new NotImplementedException();
            case "7": throw new NotImplementedException();
            case "exit": Exit(); break;
            case "help": ShowCommands(); break;
            case "clear": ClearCommand(); break;
            case "state": throw new NotImplementedException();
            default: UnknownCommand(); break;
        }
    }
}