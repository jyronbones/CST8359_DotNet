using System.Diagnostics;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> words = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            string choice;
            do
            {
                Console.Write("Menu:\n" +
                    "1 - Import words from File\n" +
                    "2 - Bubble sort words\n" +
                    "3 - LINQ/Lambda sort words\n" +
                    "4 - Count the distinct words\n" +
                    "5 - Take the last 10 words\n" +
                    "6 - Reverse print the words\n" +
                    "7 - Get and display words that end with 'a' and display the count\n" +
                    "8 - Get and display words that start with 'm' and display the count\n" +
                    "9 - Get and display words that are more than 3 characters long and start with the letter ‘a’\n" +
                    "x - Exit\n" +
                    "Enter your choice: ");

                choice = Console.ReadLine();

                if (choice != "1" && choice != "x" && !wordOperations.WordsLoaded((List<string>)words))
                {
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        words = wordOperations.ImportWordsFromFile();
                        break;
                    case "2":
                        wordOperations.BubbleSort(words, stopwatch);
                        break;
                    case "3":
                        wordOperations.LINQSort(words, stopwatch).ToList();
                        break;
                    case "4":
                        wordOperations.CountDistinctWords(words);
                        break;
                    case "5":
                        wordOperations.TakeLast10Words(words);
                        break;
                    case "6":
                        wordOperations.ReversePrintWords(words);
                        break;
                    case "7":
                        wordOperations.DisplayWordsEndingWithD(words);
                        break;
                    case "8":
                        wordOperations.DisplayWordsContaingQ(words);
                        break;
                    case "9":
                        wordOperations.DisplayWordsContainingA(words);
                        break;
                    case "x":
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            } while (choice != "x");
        }
    }
}