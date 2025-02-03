// Author: Alexander Yakovlev
// CreatedAt: 2 февраля 2025 г. 22:48:50
// Filename: ConsoleUtilities.cs
// Summary: Статический класс методов, для расширения базовых возможностей вывода

namespace ConsoleLibrary
{
    public static class ConsoleUtilities
    {
        /// <summary>
        /// Выводит сообщения указанным цветом
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="color">Цвет из ConsoleColor или White по умолчанию</param>
        /// <param name="end">Знак окончания вывода</param>
        public static void Write(string message, char end='\n', ConsoleColor color = ConsoleColor.Gray)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message + end);
            Console.ForegroundColor = oldColor;
        }
    
        // Обертки над Write для вывода сообщения зеленым, желтым и красным цветом
        public static void WriteSuccess(string message, char end='\n')
        {
            Write(message, end, ConsoleColor.Green);
        }

        public static void WriteWarning(string message, char end='\n')
        {
            Write(message, end, ConsoleColor.Yellow);
        }

        public static void WriteError(string message, char end='\n')
        {
            Write(message, end, ConsoleColor.Red);
        }
    }
}