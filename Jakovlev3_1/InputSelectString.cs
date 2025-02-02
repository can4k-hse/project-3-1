// Author: Alexander Yakovlev
// CreatedAt: 2 февраля 2025 г. 18:04:53
// Filename: InputSelectString.cs
// Summary: Класс, предоставляющий интуитивный пользователю интерфейс для ввода значения из списка

public class InputSelectString
{
    /// <summary>
    /// Массив возможных для строки значений
    /// </summary>
    private string[] _values = [];

    /// <summary>
    /// Массив возможных подсказок
    /// </summary>
    private string[] _hunches = [];

    /// <summary>
    /// Введенный пользователем префикс строки
    /// </summary>
    private string _prefix = "";

    /// <summary>
    /// Предполагаемый суффикс строки, выводимый в качестве подсказки пользователю
    /// </summary>
    private string? _hunch;

    /// <summary>
    /// Длина неизменяемого префикса строки
    /// </summary>
    private int _welcomeStringLen = 0;

    public InputSelectString(string[] values)
    {
        _values = values;
    }

    /// <summary>
    /// Обрабатывает введенную клавишу
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Ответ на вопрос, а правда ли то, что пользователь ввел Enter?</returns>
    private bool HandleConsoleKey(ConsoleKeyInfo key)
    {
        if (char.IsLetterOrDigit(key.KeyChar))
        {
            _prefix += (key.KeyChar);
        }
        else
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                {
                    if (_prefix.Length > 0)
                    {
                        _prefix = _prefix.Remove(_prefix.Length - 1);
                    }

                    break;
                }
                case ConsoleKey.UpArrow:
                    ShiftHunch(false);
                    break;
                case ConsoleKey.DownArrow:
                    ShiftHunch(true);
                    break;
                case ConsoleKey.Tab:
                    MergeHunch();
                    break;
                case ConsoleKey.Enter:
                    return true;
            }
        return false;
    }

    /// <summary>
    /// Возвращает список значений из _values, для которых совпадает префикс с _prefix.
    /// </summary>
    /// <returns></returns>
    private string[] GetMatchedOptions()
    {
        return _values.Where(option => option.StartsWith(_prefix)).ToArray();
    }

    /// <summary>
    /// Устанавливает значения, связанные с пользовательским вводом, по умолчанию
    /// </summary>
    private void SetDefault()
    {
        _hunches = [];
        _welcomeStringLen = 0;
        _prefix = "";
        _hunch = "";
    }
    
    /// <summary>
    /// Возвращает введенную пользователем строку
    /// </summary>
    /// <param name="welcomeMessage">Строковое сообщение пользователю перед вводом</param>
    /// <returns></returns>
    public string Input(string welcomeString = "Начинайте вводить: ")
    {
        SetDefault();
        
        _welcomeStringLen = welcomeString.Length;
        Console.Write(welcomeString);
        
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();

            if (HandleConsoleKey(key)) // пользователь нажал Enter
            {
                // Зачищаем все лишнее
                _hunch = string.Empty;
                UpdateUI(ConsoleColor.Gray);
                Console.WriteLine(); // Профилактический вывод в консоль
                
                // Возвращаем введенный префикс
                return _prefix;
            }
            
            UpdateHunches();

            // Обновляем UI
            if (_hunches.Length == 0)
            {
                UpdateUI(ConsoleColor.Red);
            }
            else if (_values.Contains(_prefix))
            {
                UpdateUI(ConsoleColor.Green);
            }
            else
            {
                UpdateUI(ConsoleColor.Yellow);
            }
        }
    }

    /// <summary>
    /// Выдает текст и предположение для вывода по введенной строке
    /// </summary>
    private void UpdateHunches()
    {
        // Получаем список возможных подсказок в виде суффиксов
        _hunches = GetMatchedOptions()
            .Select(hunch => _prefix.Length == hunch.Length ? "" : hunch.Substring(_prefix.Length)).ToArray();

        // Нет смысла изменять подсказку, если она соответствует новым ограничениям
        if (_hunches.Contains(_hunch))
        {
            return;
        }
        
        if (_hunches.Length == 0 || _prefix.Length == 0)
        {
            _hunch = null;
        }
        else
        {
            _hunch = _hunches[0];
        }
    }

    /// <summary>
    /// Сдвигает текущую подсказку вверх или вниз (согласно down), в массиве hunches 
    /// </summary>
    /// <param name="down">Направление сдвига (down=false => вверх)</param>
    private void ShiftHunch(bool down)
    {
        if (_hunches.Length == 0)
        {
            return;
        }

        int index = _hunches.ToList().FindIndex(item => item == _hunch);

        if (index == -1) // Если не удалось найти элемент, ничего не меняем
        {
            return;
        }

        _hunch = _hunches[(index + (down ? 1 : -1) + _hunches.Length) % _hunches.Length];
    }

    /// <summary>
    /// Связывает полученный префикс и догадку
    /// </summary>
    private void MergeHunch()
    {
        if (_hunch is not null)
        {
            _prefix += _hunch;
        }
    }

    /// <summary>
    /// Обновляет выводимую строку согласно цвету, тексту 
    /// </summary>
    /// <param name="color"></param>
    private void UpdateUI(ConsoleColor color)
    {
        // Захватываем позицию ввода
        var cursorTop = Console.CursorTop;

        // Сохраняем предыдущий цвет консоли
        ConsoleColor previousColor = Console.ForegroundColor;

        // Чистим последнюю строку консоли
        Console.SetCursorPosition(_welcomeStringLen, cursorTop);
        Console.Write(new string(' ', Console.BufferWidth - _welcomeStringLen));

        // Возвращаем курсор на исходную позицию
        Console.SetCursorPosition(_welcomeStringLen, cursorTop);

        // Задаем данный цвет консоли и выводим text
        Console.ForegroundColor = color;
        Console.Write(_prefix);

        // Выводим догадку, если она есть
        if (_hunch is not null)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(_hunch);
        }

        // Возвращаем исходный цвет
        Console.ForegroundColor = previousColor;
    }
}