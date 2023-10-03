using System.Diagnostics;

namespace Lab1{
    static class wordOperations
    {
        public static bool WordsLoaded(List<string> words)
        {
            if (words.Count == 0)
            {
                Console.WriteLine("Please load words first!");
                return false;
            }
            return true;
        }

        public static IList<string> ImportWordsFromFile()
        {
            IList<string> words = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader("Words.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            words.Add(line);
                        }
                    }
                }
                Console.WriteLine($"Imported {words.Count} words.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
            return words;
        }

        public static IList<string> BubbleSort(IList<string> words, Stopwatch timer)
        {
            var sortedWords = new List<string>(words);
            bool swapped;

            timer.Restart();

            do
            {
                swapped = false;
                for (int i = 0; i < sortedWords.Count - 1; i++)
                {
                    if (string.Compare(sortedWords[i], sortedWords[i + 1]) > 0)
                    {
                        var temp = sortedWords[i];
                        sortedWords[i] = sortedWords[i + 1];
                        sortedWords[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);

            timer.Stop();

            Console.WriteLine($"Execution time: {timer.ElapsedMilliseconds} ms");
            return sortedWords;
        }

        public static IList<string> LINQSort(IList<string> words, Stopwatch timer)
        {
            var sortedWords = new List<string>(words);

            timer.Restart();

            sortedWords = sortedWords.OrderBy(word => word).ToList();

            timer.Stop();

            Console.WriteLine($"Execution time: {timer.ElapsedMilliseconds} ms");
            return sortedWords;
        }

        public static void CountDistinctWords(IList<string> words)
        {
            Console.WriteLine($"Number of distinct words: {words.Distinct().Count()}");
        }

        public static void TakeLast10Words(IList<string> words)
        {
            var last10Words = words.Reverse().Take(10).ToList();
            last10Words.ForEach(word => Console.WriteLine(word));
        }

        public static IList<string>ReversePrintWords(IList<string> words)
        {
            var reversedWords = new List<string>(words);
            reversedWords.Reverse();
            reversedWords.ToList().ForEach(word => Console.WriteLine(word));
            return reversedWords;
        }

        public static void DisplayWordsEndingWithD(IList<string> words)
        {
            var wordsEndingWithA = words.Where(w => w.EndsWith("d")).ToList();
            Console.WriteLine($"The {wordsEndingWithA.Count()} words that end with 'a' are:");
            foreach (var word in wordsEndingWithA)
            {
                Console.WriteLine(word);
            }
        }

        public static void DisplayWordsContaingQ(IList<string> words)
        {
            var wordsStartingWithM = words.Where(w => w.Contains("q")).ToList();
            Console.WriteLine($"The {wordsStartingWithM.Count()} words that start with the letter 'm' are:");
            foreach (var word in wordsStartingWithM)
            {
                Console.WriteLine(word);
            }
        }
        public static void DisplayWordsContainingA(IList<string> words)
        {
            var shortWordsContainingI = words.Where(w => w.Length > 3 && w.Contains("a")).ToList();
            Console.WriteLine($"The {shortWordsContainingI.Count()} words that have more than 3 characters and include the letter 'a' are:");
            foreach (var word in shortWordsContainingI)
            {
                Console.WriteLine(word);
            }
        }
    }
}