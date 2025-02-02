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
        var handler = new Handler(new State());
        handler.Start();
        //
        // const string fieldName = "123";
        // FollowerComparer tmp = new((a, b) => 
        //     string.Compare(a.GetField(fieldName), b.GetField(fieldName), 
        //         StringComparison.Ordinal));
        //
        //
    }
}