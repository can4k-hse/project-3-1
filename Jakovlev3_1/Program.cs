// Author: Alexander Yakovlev
// CreatedAt: 20 января 2025 г. 20:13:23
// Filename: Program.cs
// Summary: Точка входа в программу


using System.Globalization;

// TODO реализовать цикл повторения программы
class Program
{
    private static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        InputSelectString test = new(["а", "аб", "абв", "привет", "1", "12", "123", "хуй", "хуйхуйхуй"]);
        
        Console.WriteLine("test");
        Console.WriteLine(test.Input());

        // var handler = new Handler(new State());
        // handler.Start();
        //
        // const string fieldName = "123";
        // FollowerComparer tmp = new((a, b) => 
        //     string.Compare(a.GetField(fieldName), b.GetField(fieldName), 
        //         StringComparison.Ordinal));
        //
        //
    }
}