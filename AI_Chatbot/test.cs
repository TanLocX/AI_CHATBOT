using System;
using System.IO;
using System.Threading.Tasks;
using SEMI_FINAL;

class Program
{
    static async Task Main(string[] args)
    {
        var service = new GeminiService(""YOUR_API_KEY"");
        var res1 = await service.GuiTinNhan(""Th? d� h� n?i l� g�?"");
        Console.WriteLine(""Q1: "" + res1);
        var res2 = await service.GuiTinNhan(""D�n s? l� bao nhi�u?"");
        Console.WriteLine(""Q2: "" + res2);
    }
}
