// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 21:11:04
// Filename: State.cs
// Summary: Хранит состояние приложения, текущие JSON, настройки фильтраций, сортировки и т.д.

using System.Text;
using CSClasses;
using JSONLibrary.Classes;
using JSONLibrary.Interfaces;

public class State
{
    /// <summary>
    /// Список последователей
    /// </summary>
    private List<Follower> _followers = [];

    public State()
    {
    }

    enum IOMode
    {
        File,
        Console
    }

    /// <summary>
    /// Относительный путь к файлу ввода данных.
    /// </summary>
    private string? inputFilePath = null;

    /// <summary>
    /// Режим вывода
    /// </summary>
    private IOMode inputMode = IOMode.Console;

    /// <summary>
    /// Изменяет источник ввода данных на консоль
    /// </summary>
    public void SetInputToConsole()
    {
        inputMode = IOMode.Console;
        inputFilePath = null;
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

        inputMode = IOMode.File;
        inputFilePath = filePath;
    }

    /// <summary>
    /// Пытается считать данные выбранным способом
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public bool TryReadData()
    {
        try
        {
            StreamReader inputStream = null;

            switch (inputMode)
            {
                case IOMode.File:
                {
                    Console.WriteLine("Данные считываются по пути: " + inputFilePath);
                    inputStream = new StreamReader(inputFilePath);
                    break;
                }

                case IOMode.Console:
                {
                    Console.WriteLine("Введите данные в консоль, для окончания ввода нажмите Enter, а затем Ctrl+Z: " +
                                      inputFilePath);
                    inputStream = new StreamReader(Console.OpenStandardInput());
                    break;
                }

                default: throw new NotImplementedException("cannot read data");
            }
            
            JsonParser.ReadJson(null, inputStream);

            return true;
        }

        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}