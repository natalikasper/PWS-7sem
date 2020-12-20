using System;

namespace Host2
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFSiplex.Service1Client service1Client = new WCFSiplex.Service1Client("NetTcpBinding_IService1");

            var sumResult = service1Client.Sum(new WCFSiplex.A { f = 4.2f, k = 8, s = "7" }, new WCFSiplex.A { f = 1.8f, k = 5, s = "45" });

            Console.WriteLine($"Sum\nf = {sumResult.f}\nk = {sumResult.k}\ns = {sumResult.s}");
            Console.WriteLine($"\n\nConcat\nresult = " + service1Client.Concat(sumResult.s, sumResult.f));
            Console.WriteLine($"\n\nAdd\nresult = " + service1Client.Add(sumResult.k, 4));

            service1Client.Close();
            Console.ReadKey();
        }
    }
}
