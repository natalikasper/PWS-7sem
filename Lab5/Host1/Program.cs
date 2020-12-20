using System;

namespace Host1
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFSiplex.Service1Client service1Client = new WCFSiplex.Service1Client();

            var sumResult = service1Client.Sum(new WCFSiplex.A { f = 3.2f, k = 1, s = "4" }, new WCFSiplex.A { f = 1.3f, k = 2, s = "12" });

            Console.WriteLine($"Sum\nf = {sumResult.f}\nk = {sumResult.k}\ns = {sumResult.s}");
            Console.WriteLine($"\n\nConcat\nresult = " + service1Client.Concat(sumResult.s, sumResult.f));
            Console.WriteLine($"\n\nAdd\nresult = " + service1Client.Add(sumResult.k, 4));

            service1Client.Close();
            Console.ReadKey();
        }
    }
}
