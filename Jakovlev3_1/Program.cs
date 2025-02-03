// Author: Alexander Yakovlev (Александр Александрович Яковлев)
// CreatedAt: 20 января 2025 г. 20:13:23
// Filename: Program.cs
// Summary: Точка входа в программу
// Дисциплина: Программирование на C#
// Тема: Проект 3.1 - Основная задача
// Вариант: 16


using System.Globalization;

namespace Jakovlev3_1
{
    internal abstract class Program
    {
        private static void Main(string[] args)
        {
            // Устанавливаем нужную культуру, чтобы не противоречить формату JSON (дробные числа)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            while (true) // Бесконечный цикл выполнения программы
            {
                Handler handler = new Handler(new State());
                handler.Start();
            }
        }
    }
}