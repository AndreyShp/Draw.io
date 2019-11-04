using System;
using System.IO;
using System.Text;

namespace DatabaseScriptConverter {
    internal class Program {
        private static void Main(string[] args) {
            Console.WriteLine("Enter the name of the tables creation script file:");
            var fullName = Console.ReadLine();
                
            Console.WriteLine("Enter the name of the output sql-script file:");
            var outputFileName = Console.ReadLine();
            try {
                var text = File.ReadAllText(fullName, Encoding.UTF8);

                var converter = new MsSqlConverter();
                var convertedText = converter.Convert(text);

                File.WriteAllText(outputFileName, convertedText, Encoding.UTF8);
                Console.WriteLine("Script was successfully converted!");
            } catch (Exception e) {
                Console.WriteLine($"Can't convert because raise an exception {e}");
            }

            Console.ReadLine();
        }
    }
}