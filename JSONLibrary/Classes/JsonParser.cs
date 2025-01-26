// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 21:19:23
// Filename: JsonParser.cs
// Summary: Статический класс для преобразования строки в IJsonObject и обратно.
// Описание JSON содержится на странице https://www.json.org/json-en.html

using System.Text;
using System.Text.RegularExpressions;

namespace JSONLibrary.Classes;

// TODO FIX починить те случаи, когда парсер чистит whitespace когда это не нужно
public static class JsonParser
{
    internal static readonly char[] Whitespaces = [' ', '\n', '\r', '\t'];
    internal static readonly char[] JsonSyntax = ['{', '}', ',', ':', '[', ']'];
    
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
            } else if (next.IsFigureBracket == 0)
            {
                ParseObject(tokens, bracketsMatches, i, out var nested);
                value = Stringify(nested);
                i = bracketsMatches[i];
            } else if (next.IsSquareBracket == 0)
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

        for (int i = startIndex + 1; i <= endIndex;)
        {
            JsonToken token = tokens[i];
            if (token.IsPrimitive)
            {
                result.Add(token.ToString());
                i++;
            } else if (token.IsFigureBracket == 0)
            {
                ParseObject(tokens, bracketsMatches, i, out var nested);
                result.Add(Stringify(nested));
                i = bracketsMatches[i] + 1;
            } else if (token.IsSquareBracket == 0)
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
    /// Удаляет все whitespace элементы, которые лежат вне строк
    /// </summary>
    /// <param name="stringToParse"></param>
    /// <returns></returns>
    private static string ClearWhitespaces(string stringToParse)
    {
        // Результирующая строка
        StringBuilder sb = new();

        // Флаг состояния, обозначающее нахождения внутри некой строки
        bool inString = false;

        for (int i = 0; i < stringToParse.Length; i++)
        {
            // Изменяем состояние, если кавычка не является частью строки
            if (stringToParse[i] == '"' && (i == 0 || stringToParse[i - 1] != '\\'))
            {
                inString ^= true;
            }

            if (inString || !Whitespaces.Contains(stringToParse[i]))
            {
                sb.Append(stringToParse[i]); 
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Пытается выделить обособленную кавычками строку из данной строки
    /// </summary>
    /// <param name="stringToSelect">Данная строка</param>
    /// <param name="startIndex">Номер, с которого должна выделяться строка</param>
    /// <param name="result">Выделенная строка</param>
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <example>stringToSelect="abc"d"f, startIndex=4 -> result=d, return=7</example>
    /// <exception cref="FormatException">Не удалось выделить строку</exception>
    private static int SelectString(string stringToSelect, int startIndex, out string result)
    {
        // На позиции startIndex должна быть пометка о начале строки
        if (stringToSelect[startIndex] != '"')
        {
            throw new FormatException("incorrect sequence start sign");
        }

        StringBuilder res = new();
        for (int i = startIndex + 1; i < stringToSelect.Length; i++)
        {
            if (stringToSelect[i] == '"' && (i == 0 || stringToSelect[i - 1] != '\\'))
            {
                result = res.ToString();

                // Добавляем обособление строки кавычками
                result = '"' + result + '"';
                
                // Возвращаем следущий индекс
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
    /// <returns>Инкрементированный индекс конца токена</returns>
    /// <exception cref="FormatException">Не удалось выделить токен</exception>
    private static int SelectToken(string stringToSelect, int startIndex, out JsonToken result)
    {
        int newIndex;

        try
        {
            newIndex = SelectString(stringToSelect, startIndex, out string res);
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

        throw new FormatException("token not found");
    }

    /// <summary>
    /// Разделяет данную строку на токены JsonToken 
    /// </summary>
    /// <param name="stringToSplit"></param>
    /// <exception cref="FormatException ">Не удалось выделить токены</exception>
    /// <returns></returns>
    public static List<JsonToken> SplitIntoTokens(string stringToSplit)
    {
        List<JsonToken> tokens = [];

        int index = 0;
        while (index != stringToSplit.Length)
        {
            index = SelectToken(stringToSplit, index, out JsonToken token);
            tokens.Add(token);
        }

        return tokens;
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
            List<JsonToken> tokens = SplitIntoTokens(stringToParse);

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
}