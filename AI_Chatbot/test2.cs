using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        var currentContents = new List<object>();
        
        string json = ""[{\""role\"":\""user\"",\""parts\"":[{\""text\"":\""Th? dô hà n?i\""}]}]"";
        var savedHistory = JsonConvert.DeserializeObject<List<object>>(json);
        currentContents.AddRange(savedHistory);

        currentContents.Add(new {
            role = ""user"",
            parts = new[] { new { text = ""Dân s? là bao nhiêu?"" } }
        });
        
        var requestBody = new { contents = currentContents };
        string updatedHistoryJson = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
        Console.WriteLine(updatedHistoryJson);
    }
}
