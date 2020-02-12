using System;

namespace etap1
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet run <formula>");
                return;
            }
            Rpn r = new Rpn(args[0]);

            var res = r.CalculateXY(2,3,100);
            foreach(Point p in res)
            {
                Console.WriteLine(p.x + " => " + p.y);
            }
        }
    }
}
