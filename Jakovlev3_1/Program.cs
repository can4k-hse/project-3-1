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
        
        // string js2;
        // js2 = File.ReadAllText("./../../../followers.json");
        // var tmp = JsonParser.Parse(js2);
        //
        // var c = JsonParser.Parse(tmp["\"elements\""]);
        // foreach (var k in c)
        // {
        //     Console.WriteLine(k.Key);
        //     Console.WriteLine(k.Value);
        //     Console.WriteLine("");
        // }
        //
        // Console.ReadLine();

        var handler = new Handler(new State());
        handler.Start();
    }
}