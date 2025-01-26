using JSONLibrary.Classes;

class Program
{
    private static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        string js2;
        js2 = File.ReadAllText("./../../../test.txt");
        var parsed = JsonParser.Parse(js2);
        Console.WriteLine(JsonParser.Stringify(parsed));
    }
}