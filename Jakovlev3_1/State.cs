// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 21:11:04
// Filename: State.cs
// Summary: Хранит состояние приложения, текущие JSON, настройки фильтраций, сортировки и т.д.

// TODO FIX DEFAULT INPUT

using System.Text;
using CSClasses;
using JSONLibrary.Classes;
public class State
{
    /// <summary>
    /// Список последователей
    /// </summary>
    private FollowersList _followers = new();
    
    private FollowersList _loadedFollowers = new(); 
    
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
    private string? inputFilePath = "./../../../followers.json";

    /// <summary>
    /// Режим вывода
    /// </summary>
    private IOMode inputMode = IOMode.File;

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
    /// Пытается считать данные выбранным способом и записать в Followers
    /// </summary>
    /// <returns></returns>
    public void ReadData()
    {
        StreamReader inputStream;

        switch (inputMode)
        {
            case IOMode.File:
            {
                Console.WriteLine("Данные считываются из файла по пути: " + inputFilePath);
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

            default: throw new NotImplementedException("wrong input method");
        }
            
        // Пытаемся считать данные в tmp список
        FollowersList tmp = new FollowersList();
        JsonParser.ReadJson(tmp, inputStream);
        
        // В случае успешного считывания назначаем ссылку
        _loadedFollowers = tmp;
    }
    
    /// <summary>
    /// Относительный путь к файлу вывода данных.
    /// </summary>
    private string? outputFilePath = "./../../../followers-out.json";

    /// <summary>
    /// Режим вывода
    /// </summary>
    private IOMode outputMode = IOMode.File;

    /// <summary>
    /// Изменяет способ вывода данных на консоль
    /// </summary>
    public void SetOutputToConsole()
    {
        outputMode = IOMode.Console;
        outputFilePath = null;
    }
    
    /// <summary>
    /// Изменяет источник вывода данных на данный файл
    /// </summary>
    /// <param name="filePath"></param>
    public void SetOutputToFile(string filePath)
    {
        outputMode = IOMode.File;
        outputFilePath = filePath;
    }

    /// <summary>
    /// Пытается вывести данные выбранным способом из Followers
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void WriteData()
    {
        StreamWriter outputStream;
        switch (outputMode)
        {
            case IOMode.Console:
            {
                outputStream = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
                break;
            }
            case IOMode.File:
            {
                Console.WriteLine("Данные сохранятся в файл по пути: " + outputFilePath);
                
                // Пытаемся удалить файл, если он уже существует
                try
                {
                    File.Delete(outputFilePath);
                }
                catch (Exception)
                {
                    // ignored
                }
                
                outputStream = new StreamWriter(File.Open(outputFilePath, FileMode.Create));
                break;
            }
            default: throw new NotImplementedException("wrong output method");
        }

        outputStream.AutoFlush = true;
        JsonParser.WriteJson(_loadedFollowers, outputStream);
    }
    
    public void SortByField(string fieldName)
    {
        FollowersList _follower = new(JsonParser.Parse(JsonParser.Stringify(_followers)));
    }
}