// Author: Alexander Yakovlev
// CreatedAt: 27 января 2025 г. 21:11:04
// Filename: State.cs
// Summary: Хранит состояние приложения, текущие JSON, настройки фильтраций, сортировки и т.д.

public class State
{
    public State() {}

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
    public void SetInputToConsole()
    {
        inputMode = IOMode.Console;
        inputFilePath = null;
    }

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
}