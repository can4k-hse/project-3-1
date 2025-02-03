// Author: Alexander Yakovlev
// CreatedAt: 30 января 2025 г. 16:29:50
// Filename: JsonUtility.cs
// Summary: Статический класс, конвертирующий объекты формата Json.

using System.Text.RegularExpressions;
namespace JSONLibrary.Classes
{
    public static class JsonUtility
    {
        /// <summary>
        /// Преобразует pseudo массив (словарь, где в качестве ключей используются строковые представления индексов)
        /// в массив строк. Обратите внимание, что индексы не должны пропускаться.
        /// </summary>
        /// <param name="pseudoArray"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static List<string> ConvertPseudoArrayToList(Dictionary<string, string> pseudoArray)
        {
            int maxKey = -1;
        
            // Проверяем, что все ключи pseudoArray являются строковыми представлениями чисел
            foreach (KeyValuePair<string, string> keyValue in pseudoArray)
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
                    result[i] = pseudoArray[i.ToString()];
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
        public static bool IsValidString(string input)
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
        /// Удаляет кавычки из строкового литерала
        /// </summary>
        /// <param name="str">Данная строка</param>
        /// <returns>Измененная строка или исходная, если str не начинается или заканчивается с \"</returns>
        public static string RemoveQuotes(string str)
        {
            if (str.StartsWith($"\"") && str.EndsWith($"\""))
            {
                return str[1..^1];
            }

            // Возвращаем исходную строку
            return str;
        }

        /// <summary>
        /// Добавляет в начало и в конец строки кавычки, чтобы однозначно обозначить строковой литерал согласно JSON
        /// </summary>
        /// <param name="str">Данная строка</param>
        /// <returns>Измененную строку</returns>
        public static string AddQuotes(string str)
        {
            if (str.Length == 0) // Рассмотрим отдельный случай для пустой строки
            {
                return "\"\"";
            }
        
            // Проверим суффикс и префикс на соответствие
            if (!str.StartsWith($"\""))
            {
                str = "\"" + str;
            }
            if (!str.EndsWith($"\""))
            {
                str += "\"";
            }

            return str;
        }
    }
}