using System;
using System.Text;

namespace NoAcg.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = Encoding.UTF32.GetBytes("I cover my face because someone might think I'm hentai, are you? 😣");
            Console.WriteLine("😣".Length);

            var end = 66 * 4;
            var r = Encoding.UTF32.GetString(data[0..end]);
            Console.WriteLine(r);

            var b = "I cover my face because someone might think I'm hentai, are you? ";
            var c = "I cover my face because someone might think I'm hentai, are you? 😣";
            var a = c[0..66];
            Console.WriteLine(a);
        }
    }
}