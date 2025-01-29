// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 20:13:23
// Filename: Program.cs
// Summary: Точка входа в приложение


using JSONLibrary.Classes;
using System.Globalization;

// TODO реализовать цикл повторения программы
class Program
{
    private static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        
        var handler = new Handler(new State());
        try
        {
            handler.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadLine();
        }
    }
}