// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 20:41:19
// Filename: Handler.cs
// Summary: Нестатический клас, выполняющий взаимодействие с пользователем

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
        
        Console.WriteLine("Добро пожаловать!\n");
        ShowCommands();

        while (_status is MenuStatus.Enable)
        {
            Console.Write(LineSign + " ");
            string? command = Console.ReadLine();

            if (command is null)
            {
                break;
            }

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
            "1. Изменить источник ввода данных",
            "[]2. Ввести данные",
            "[]3. Отфильтровать данные",
            "[]4. Отсортировать данные",
            "[]5. Узнать",
            "[]6. Вывести данные",
            "[]7. Выход"
        ];
        
        Console.WriteLine(string.Join("\n", commands) + "\n");
    }

    /// <summary>
    /// Завершает работу меню
    /// </summary>
    private void Exit()
    {
        _status = MenuStatus.Disable;
        Console.WriteLine("До свидания!");
    }

    /// <summary>
    /// Выводит сообщение о неизвестной команде
    /// </summary>
    private void UnknownCommand()
    {
        Console.WriteLine("Неизвестная команда...");
    }

    private void InputDataCommand()
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
    /// Обработчик для команд
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
            case "1": InputDataCommand(); break;
            case "2": throw new NotImplementedException();
            case "3": throw new NotImplementedException();
            case "4": throw new NotImplementedException();
            case "5": throw new NotImplementedException();
            case "6": throw new NotImplementedException();
            default: UnknownCommand(); break;
        }
    }
}