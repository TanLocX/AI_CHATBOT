using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        var currentContents = new List<object>();
        currentContents.Add(new {
            role = ""user"",
            parts = new[] { new { text = ""Th? dô hà n?i"" } }
        });
        currentContents.Add(new {
            role = ""model"",
            parts = new[] { new { text = ""Hà N?i là th? dô c?a Vi?t Nam."" } }
        });
        string updatedHistoryJson = JsonConvert.SerializeObject(currentContents, Formatting.Indented);
        File.WriteAllText(""chat_history.json"", updatedHistoryJson);
        Console.WriteLine(""Done!"");
    }
}
