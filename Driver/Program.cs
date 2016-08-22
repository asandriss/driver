using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            var tickers = new[] { "msft", "orcl", "ebay" };
            var analyzers = StockAnalyzer.StockAnalyzer.GetAnalyzers(tickers, 3);

            foreach (var a in analyzers)
                Console.WriteLine("Ret:{0}\tStdDev:{1}", a.Return, a.StdDev);
        }
    }
}
