using System.Collections.Concurrent;


namespace Lab2
{
    public class Result
    {
        public static int result = 0;
        public static ConcurrentStack<int> stack = new ConcurrentStack<int>();
    }
}