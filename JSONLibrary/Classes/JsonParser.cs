// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 21:19:23
// Filename: JsonParser.cs
// Summary: Статический класс для преобразования строки в IJsonObject и обратно.
// Описание JSON содержится на странице https://www.json.org/json-en.html

using System.Text;
using System.Text.RegularExpressions;
using JSONLibrary.Interfaces;

namespace JSONLibrary.Classes;

public static class JsonParser
{
    private static readonly char[] Whitespaces = [' ', '\n', '\r', '\t'];
    internal static readonly char[] JsonSyntax = ['{', '}', ',', ':', '[', ']'];

    /// <summary>
    /// Считывает данные в IJsonObject из потока
    /// </summary>
    /// <param name="jsonObject">Объект для считывания</param>
    /// <param name="stream">Поток ввода</param>
    /// <exception cref="FormatException">Невозможно запарсить строку в объект</exception>
    public static void ReadJson(IJsonObject jsonObject, StreamReader stream)
    {
        
        // Временно изменяем поток ввода на данный
        Console.SetIn(stream);
        string result = Console.In.ReadToEnd();

        // Парсим строку и прокидываем значения в jsonObject
        var parsed = Parse(result);
        
        // Возвращаем стандартный ввод для корректной работы меню
        stream.Dispose();
        var defaultInput = new StreamReader(Console.OpenStandardInput());
        Console.SetIn(defaultInput);
        
        // Пытаемся считать данные
        try
        {
            foreach (var keyValue in parsed)
            {
                jsonObject.SetField(keyValue.Key, keyValue.Value);
            }
        }
        catch (KeyNotFoundException e)
        {
            throw new FormatException("Json data error: " + e.Message);
        }
    }
    
    public static void WriteJson(IJsonObject json)
    {
        Console.Write(Stringify(json));
    }

    public static string Stringify(IJsonObject jsonObject)
    {
        Dictionary<string, string> converter = [];
        foreach (var key in jsonObject.GetAllFields())
        {
            converter[key] = jsonObject.GetField(key);
        }

        return Stringify(converter);
    }

    public static string Stringify(Dictionary<string, string> dictionary)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append('{');

        foreach (var key in dictionary.Keys)
        {
            sb.Append(key + ':' + dictionary[key]);
            sb.Append(',');
        }

        // Удаляем висячую запятую
        if (sb[^1] == ',')
        {
            sb.Remove(sb.Length - 1, 1);
        }

        sb.Append('}');

        return sb.ToString();
    }

    public static string Stringify(List<string> array)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append('[');

        foreach (var value in array)
        {
            sb.Append(value);
            sb.Append(',');
        }

        // Удаляем висячую запятую
        if (sb[^1] == ',')
        {
            sb.Remove(sb.Length - 1, 1);
        }

        sb.Append(']');

        return sb.ToString();
    }

    /// <summary>
    /// Преобразует переданную строку в IJsonObject.
    /// </summary>
    /// <param name="stringToParse">Строковое представление JSON</param>
    /// <exception cref="FormatException">stringToParse является невалидным представлением JSON</exception>
    /// <returns>Полученное значение. Если в качестве stringToParse был передан массив, возвратит объект, где ключами
    /// будут выступать строковые представления индексов значений</returns>
    public static Dictionary<string, string> Parse(string stringToParse)
    {
        Tuple<List<JsonToken>, Dictionary<int, int>> lexRes;
        lexRes = LexicalAnalysis(stringToParse);

        var tokens = lexRes.Item1;
        var bracketsMatches = lexRes.Item2;

        if (tokens.Count == 0)
        {
            return null;
        }

        if (bracketsMatches[0] != tokens.Count - 1) // Проверяем, что stringToParse задан корректно
        {
            throw new FormatException("invalid JSON format");
        }

        var result = new Dictionary<string, string>();

        try
        {
            if (tokens[0].IsFigureBracket == 0)
            {
                ParseObject(tokens, bracketsMatches, 0, out var res);
                result = res;
            }
            else if (tokens[0].IsSquareBracket == 0)
            {
                ParseArray(tokens, bracketsMatches, 0, out var res);
                for (int i = 0; i < res.Count; i++)
                {
                    result[i.ToString()] = res[i];
                }
            }
        }
        catch (Exception e)
        {
            throw new FormatException("invalid JSON format: " + e.Message);
        }

        return result;
    }

    /// <summary>
    /// Преобразует данный лист токенов в словарь строка-строка, где в качестве ключей используются ключи первого
    /// уровня(вложения), а в качестве значений - строковые представления данных - примитивов и объектов
    /// </summary>
    /// <param name="tokens">Список JSON токенов</param>
    /// <param name="bracketsMatches">Словарь int-int, в котором определенны соответствующие пары скобок</param>
    /// <param name="startIndex">Индекс начала парсера</param>
    /// <param name="result">Словарь строка-строка</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    private static void ParseObject(List<JsonToken> tokens, Dictionary<int, int> bracketsMatches, int startIndex,
        out Dictionary<string, string> result)
    {
        result = new Dictionary<string, string>();

        int endIndex;
        try
        {
            endIndex = bracketsMatches[startIndex];
        }
        catch (Exception)
        {
            throw new Exception("unexpected error");
        }

        result = new Dictionary<string, string>();

        // Парсер JSON по стадиям, описанным на рис. 1 https://www.json.org/json-en.html
        for (int i = startIndex + 1; i < endIndex;)
        {
            string key, value;

            var cur = tokens[i];
            if (!cur.IsString)
            {
                throw new FormatException("invalid JSON format: string expected");
            }

            key = (string)cur.Value;

            // Может произойти выход на пределы массива
            i++;
            var colon = tokens[i]; // ожидаем двоеточие

            if (!colon.IsColon)
            {
                throw new FormatException("invalid JSON format: colon expected");
            }

            i++;
            var next = tokens[i];
            if (next.IsPrimitive)
            {
                value = next.ToString();
            }
            else if (next.IsFigureBracket == 0) // object
            {
                ParseObject(tokens, bracketsMatches, i, out var nested);
                value = Stringify(nested);
                i = bracketsMatches[i];
            }
            else if (next.IsSquareBracket == 0) // array
            {
                ParseArray(tokens, bracketsMatches, i, out var nested);
                value = Stringify(nested);
                i = bracketsMatches[i];
            }
            else
            {
                throw new FormatException("invalid JSON format: primitive, array or object expected");
            }

            if (result.ContainsKey(key))
            {
                throw new FormatException("invalid JSON format: duplicate key");
            }

            result[key] = value;

            i++;
            if (i == endIndex)
            {
                break;
            }

            var sep = tokens[i]; // ожидаем запятую
            if (!sep.IsComma)
            {
                throw new FormatException("invalid JSON format: comma expected");
            }

            i++; // ожидаем новую пару ключ-значение

            if (i == endIndex) // обнаружена висячая запятая
            {
                throw new FormatException("invalid JSON format: key-value pair expected");
            }
        }
    }

    private static void ParseArray(List<JsonToken> tokens, Dictionary<int, int> bracketsMatches, int startIndex,
        out List<string> result)
    {
        int endIndex;
        try
        {
            endIndex = bracketsMatches[startIndex];
        }
        catch (Exception)
        {
            throw new Exception("unexpected error");
        }

        result = [];

        for (int i = startIndex + 1; i < endIndex;)
        {
            JsonToken token = tokens[i];
            if (token.IsPrimitive)
            {
                result.Add(token.ToString());
                i++;
            }
            else if (token.IsFigureBracket == 0)
            {
                ParseObject(tokens, bracketsMatches, i, out var nested);
                result.Add(Stringify(nested));
                i = bracketsMatches[i] + 1;
            }
            else if (token.IsSquareBracket == 0)
            {
                ParseArray(tokens, bracketsMatches, i, out var nested);
                result.Add(Stringify(nested));
                i = bracketsMatches[i] + 1;
            }
            else
            {
                throw new FormatException("invalid JSON syntax: array contains wrong element");
            }

            if (i == endIndex)
            {
                break;
            }

            var sep = tokens[i]; // ожидаем сепаратор
            if (!sep.IsComma)
            {
                throw new FormatException("invalid JSON syntax: comma expected");
            }

            i++; // ожидаем новое значение
            if (i == endIndex) // обнаруживается висячая запятая
            {
                throw new FormatException("invalid JSON syntax: value expected");
            }
        }
    }

    /// <summary>
    /// Удаляет все Whitespaces элементы, которые лежат вне строк. Алгоритм работы заключается в поиске
    /// символов, лежащих в JsonSyntax и вне строк. Далее производится удаление элементов равных Whitespaces, которые
    /// лежат левее или правее найденных JsonSyntax элементов. 
    /// </summary>
    /// <param name="stringToParse">Строка для обработки</param>
    /// <returns>Обработанная строка с удаленными Whitespaces</returns>
    private static string ClearWhitespaces(string stringToParse)
    {
        // Результирующая строка
        StringBuilder sb = new();

        // Позиции символов из JsonSyntax
        List<int> syntaxPositions = [];

        // Позиции кавычек
        var quotePositions = GetQuotesPositions(stringToParse);
        bool inString = false;

        for (int i = 0; i < stringToParse.Length; i++)
        {
            if (quotePositions.Contains(i))
            {
                inString ^= true;
            }

            if (!inString && JsonSyntax.Contains(stringToParse[i]))
            {
                syntaxPositions.Add(i);
            }
        }

        // Список позиций, символы на которых являются Whitespaces, безопасными для удаления
        List<int> hs = [];
        foreach (var pos in syntaxPositions)
        {
            for (int i = pos - 1; i >= 0 && Whitespaces.Contains(stringToParse[i]); i--)
            {
                hs.Add(i);
            }

            for (int i = pos + 1; i < stringToParse.Length && Whitespaces.Contains(stringToParse[i]); i++)
            {
                hs.Add(i);
            }
        }

        for (int i = 0; i < stringToParse.Length; i++)
        {
            if (!hs.Contains(i))
            {
                sb.Append(stringToParse[i]);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Проверяет, что данная строка является 4-значным hex числом
    /// </summary>
    /// <param name="numberStr"></param>
    /// <returns></returns>
    private static bool IsCorrectHex(string numberStr)
    {
        // Регулярное выражение для 4 символов HEX
        string pattern = @"^[0-9A-Fa-f]{4}$";
        return Regex.IsMatch(numberStr, pattern);
    }

    /// <summary>
    /// Регулярное выражение для проверки строки на валидность
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool IsValidString(string input)
    {
        try
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsControl(input[i]))
                {
                    throw new ArgumentException("string contains illegal character");
                }

                if (input[i] == '\\')
                {
                    if (i + 1 == input.Length)
                    {
                        throw new ArgumentException("string contains illegal character");
                    }

                    // Проверяем \ на корректность согласно стандарту JSON
                    switch (input[i + 1])
                    {
                        case '"':
                        case '\\':
                        case '/':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                            i++; break;
                        case 'u':
                        {
                            if (i + 5 >= input.Length)
                            {
                                throw new ArgumentException("string contains illegal character");
                            }

                            string digits = input[(i + 2)..(i + 6)];
                            if (!IsCorrectHex(digits))
                            {
                                throw new ArgumentException("string contains illegal character");
                            }

                            break;
                        }
                        default: throw new ArgumentException("string contains illegal character");
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Пытается выделить обособленную кавычками строку из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенная строка</param>
    /// <param name="quotePositions">Индексы кавычек, обосабливающих строки</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <example>stringToSelect="abc"d"f, startIndex=4 -> result=d, return=7</example>
    /// <exception cref="FormatException">Не удалось выделить строку</exception>
    private static int SelectString(string stringToSelect, int startIndex, HashSet<int> quotePositions,
        out string result)
    {
        // На позиции startIndex должна быть пометка о начале строки
        if (!quotePositions.Contains(startIndex))
        {
            throw new FormatException("incorrect sequence start sign");
        }

        StringBuilder res = new();
        for (int i = startIndex + 1; i < stringToSelect.Length; i++)
        {
            if (quotePositions.Contains(i))
            {
                result = res.ToString();

                // Добавляем обособление строки кавычками
                result = '"' + result + '"';

                if (!IsValidString(result))
                {
                    throw new FormatException("incorrect string");
                }

                // Возвращаем следующий индекс
                return i + 1;
            }

            res.Append(stringToSelect[i]);
        }

        // Исключение срабатывает в случае, если программе не удалось найти конец строки
        throw new FormatException("string sequence end not found");
    }

    /// <summary>
    /// Пытается выделить целое числовое значения из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <example>stringToSelect=123:some-test, startIndex=1 -> result=23, return = 3</example>
    /// <exception cref="FormatException">Не удалось выделить число</exception>
    private static int SelectInt(string stringToSelect, int startIndex, out int result)
    {
        // Результирующая строка
        StringBuilder sb = new();

        for (int i = startIndex; i < stringToSelect.Length; i++)
        {
            // Числовая последовательность должна закончится в JsonSyntax-символе
            if (JsonSyntax.Contains(stringToSelect[i]))
            {
                if (!int.TryParse(sb.ToString(), out result))
                {
                    throw new FormatException("incorrect int-like sequence");
                }

                // Возвращаем новую позицию как индекс JsonSyntax-символа
                return i;
            }

            sb.Append(stringToSelect[i]);
        }

        throw new FormatException("int-like sequence end not found");
    }

    /// <summary>
    /// Пытается выделить дробное числовое значения из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <example>stringToSelect=123:some-test, startIndex=1 -> result=23, return = 3</example>
    /// <exception cref="FormatException">Не удалось выделить число</exception>
    private static int SelectDouble(string stringToSelect, int startIndex, out double result)
    {
        // Результирующая строка
        StringBuilder sb = new();

        for (int i = startIndex; i < stringToSelect.Length; i++)
        {
            // Числовая последовательность должна закончится в JsonSyntax-символе
            if (JsonSyntax.Contains(stringToSelect[i]))
            {
                if (!double.TryParse(sb.ToString(), out result))
                {
                    throw new FormatException("incorrect double-like sequence");
                }

                // Возвращаем новую позицию как индекс JsonSyntax-символа
                return i;
            }

            sb.Append(stringToSelect[i]);
        }

        throw new FormatException("double-like sequence end not found");
    }

    /// <summary>
    /// Пытается выделить bool значение из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <example>stringToSelect=true:some-test, startIndex=0 -> result=true, return = 4</example>
    /// <exception cref="FormatException">Не удалось выделить bool</exception>
    private static int SelectBoolean(string stringToSelect, int startIndex, out bool result)
    {
        if (startIndex + 3 < stringToSelect.Length && stringToSelect[startIndex..(startIndex + 4)] == "true")
        {
            result = true;
            startIndex += 4;
        }
        else if (startIndex + 4 < stringToSelect.Length && stringToSelect[startIndex..(startIndex + 5)] == "false")
        {
            result = false;
            startIndex += 5;
        }
        else
        {
            throw new FormatException("boolean sequence not found");
        }

        // Проверяем, стоит ли после токена JsonSyntax-символ
        if (startIndex >= stringToSelect.Length || !JsonSyntax.Contains(stringToSelect[startIndex]))
        {
            throw new FormatException("boolean sequence end not found");
        }

        return startIndex;
    }

    /// <summary>
    /// Пытается выделить null значение из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <exception cref="FormatException">Не удалось выделить null</exception>
    private static int SelectNull(string stringToSelect, int startIndex)
    {
        if (startIndex + 3 < stringToSelect.Length && stringToSelect[startIndex..(startIndex + 4)] == "null")
        {
            startIndex += 4;
        }
        else
        {
            throw new FormatException("null-sequence not found");
        }

        // Проверяем, стоит ли после токена JsonSyntax-символ
        if (startIndex >= stringToSelect.Length || !JsonSyntax.Contains(stringToSelect[startIndex]))
        {
            throw new FormatException("null sequence end not found");
        }

        return startIndex;
    }

    /// <summary>
    /// Пытается выделить JsonSyntax-символ  из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <exception cref="FormatException">Не удалось выделить JsonSyntax-символ</exception>
    private static int SelectJsonSyntax(string stringToSelect, int startIndex, out char result)
    {
        if (JsonSyntax.Contains(stringToSelect[startIndex]))
        {
            result = stringToSelect[startIndex];
            return ++startIndex;
        }

        throw new FormatException("JsonSyntax not found");
    }

    /// <summary>
    /// Пытается выделить токен из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенное значение</param>
    /// <param name="quotePositions">Индексы кавычек, обосабливающих строки</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <exception cref="FormatException">Не удалось выделить токен</exception>
    private static int SelectToken(string stringToSelect, int startIndex, HashSet<int> quotePositions,
        out JsonToken result)
    {
        int newIndex;

        try
        {
            newIndex = SelectString(stringToSelect, startIndex, quotePositions, out string res);
            result = new JsonToken(res);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            newIndex = SelectInt(stringToSelect, startIndex, out int res);
            result = new JsonToken(res);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            newIndex = SelectDouble(stringToSelect, startIndex, out double res);
            result = new JsonToken(res);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            newIndex = SelectBoolean(stringToSelect, startIndex, out bool res);
            result = new JsonToken(res);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            newIndex = SelectNull(stringToSelect, startIndex);
            result = new JsonToken(null);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            newIndex = SelectJsonSyntax(stringToSelect, startIndex, out char res);
            result = new JsonToken(res);
            return newIndex;
        }
        catch (Exception e)
        {
            // ignored
        }

        throw new FormatException("token is wrong, value expected instead");
    }

    /// <summary>
    /// Разделяет данную строку на токены JsonToken 
    /// </summary>
    /// <param name="stringToSplit">Данная стока</param>
    /// <param name="quotePositions">Индексы кавычек, обосабливающих строки</param>
    /// <exception cref="FormatException ">Не удалось выделить токены</exception>
    /// <returns></returns>
    private static List<JsonToken> SplitIntoTokens(string stringToSplit, HashSet<int> quotePositions)
    {
        List<JsonToken> tokens = [];

        int index = 0;
        while (index != stringToSplit.Length)
        {
            index = SelectToken(stringToSplit, index, quotePositions, out JsonToken token);
            tokens.Add(token);
        }

        return tokens;
    }

    /// <summary>
    /// Возвращает индексы кавычек, обособляющих строки
    /// </summary>
    /// <param name="stringToParse"></param>
    /// <returns></returns>
    private static HashSet<int> GetQuotesPositions(string stringToParse)
    {
        HashSet<int> quotes = [];

        for (int i = 0; i < stringToParse.Length; i++)
        {
            if (stringToParse[i] == '\\') // Встретив обратный \ проигнорируем следующий символ 
            {
                i++;
                continue;
            }

            if (stringToParse[i] == '"')
            {
                quotes.Add(i);
            }
        }

        return quotes;
    }

    /// <summary>
    /// Решает задачу о проверке списка токенов из JsonSyntax, задающих скобки, на соответствие правильной
    /// скобочной последовательности
    /// </summary>
    /// <param name="tokens">Список токенов</param>
    /// <returns>Словарь в котором парой ключ-значение являются индексы соответствующих скобок</returns>
    private static Dictionary<int, int> CheckBracketsMatches(List<JsonToken> tokens)
    {
        Dictionary<int, int> result = new();

        // Стек пригодится для решения задачи о поиске ПСП
        Stack<Tuple<JsonToken, int>> bracketsStack = new();

        for (int i = 0; i < tokens.Count; i++)
        {
            JsonToken token = tokens[i];

            int bracketMode = token.IsBracket();

            // Пропускаем *не скобку*
            if (bracketMode == -1)
            {
                continue;
            }

            if (bracketMode is 0 or 2) // Открывающаяся скобка 
            {
                bracketsStack.Push(new Tuple<JsonToken, int>(token, i));
            }
            else // Закрывающаяся скобка
            {
                // Проверяем, что скобки одного типа и соответствуют {} или []
                if
                (
                    bracketsStack.Count == 0 ||
                    bracketsStack.Peek().Item1.IsBracket() + 1 != bracketMode
                )
                {
                    throw new ArgumentException("brackets don't match");
                }

                // Устанавливаем соответствие
                result[bracketsStack.Peek().Item2] = i;
                bracketsStack.Pop();
            }
        }

        if (bracketsStack.Count != 0) // Если в стеке остались лишние скобки
        {
            throw new ArgumentException("brackets don't match");
        }

        return result;
    }

    /// <summary>
    /// Проводит лексический анализ строки. Удаляет пробелы и лишние символы. Возвращает типизированный
    /// список токенов - минимальных единиц синтаксиса JSON. Список является правильной скобочной последовательностью
    /// относительно квадратных и фигурных скобок.
    /// </summary>
    /// <param name="stringToParse"></param>
    /// <returns></returns>
    /// <exception cref="FormatException">Неверный формат JSON</exception>
    private static Tuple<List<JsonToken>, Dictionary<int, int>> LexicalAnalysis(string stringToParse)
    {
        try
        {
            stringToParse = ClearWhitespaces(stringToParse);
            HashSet<int> quotePositions = GetQuotesPositions(stringToParse);
            List<JsonToken> tokens = SplitIntoTokens(stringToParse, quotePositions);

            Dictionary<int, int> bracketMatches;
            try
            {
                bracketMatches = CheckBracketsMatches(tokens);
            }
            catch (Exception)
            {
                // Пробрасываем исключение
                throw new Exception("mismatched brackets");
            }

            return new Tuple<List<JsonToken>, Dictionary<int, int>>(tokens, bracketMatches);
        }
        catch (Exception e)
        {
            throw new FormatException("JSON syntax error: " + e.Message);
        }
    }
    
    /// <summary>
    /// Преобразует псевдомассив (словарь, где в качестве ключей используются строковые представления индексов)
    /// в массив строк. Обратите внимание, что индексы не должны пропускаться.
    /// </summary>
    /// <param name="pseudoArray"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static List<string> ConvertPseudoArrayToList(Dictionary<string, string> pseudoArray)
    {
        var maxKey = -1;
        
        // Проверяем, что все ключи pseudoArray являются строковыми представлениями чисел
        foreach (var keyValue in pseudoArray)
        {
            if (!int.TryParse(keyValue.Key, out int integerKey) || integerKey < 0)
            {
                throw new FormatException("invalid pseudo array");
            }
            
            maxKey = Math.Max(maxKey, integerKey);
        }

        string[] result = new string[maxKey + 1];
        
        for (int i = 0; i <= maxKey; i++)
        {
            if (pseudoArray.ContainsKey(i.ToString()))
            {
                result[i] = (pseudoArray[i.ToString()]);
            }
            else
            {
                // В случае пропуска индекса пробрасываем исключение
                throw new FormatException("invalid pseudo array");
            }
        }

        return result.ToList();
    }

    /// <summary>
    /// Возвращает строковое представление списка IJsonObject
    /// </summary>
    /// <param name="jsonObjects"></param>
    /// <returns></returns>
    public static string StringifyIJsonList(List<IJsonObject> jsonObjects)
    {
        return JsonParser.Stringify(jsonObjects.Select(JsonParser.Stringify).ToList());
    }
}