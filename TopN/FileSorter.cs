using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TopN
{
    public class FileSorter
    {
        /// <summary>
        /// Max count of numbers that will stored in a single chunk in memory.
        /// This should be proportional to available RAM
        /// </summary>
        private const int MaxChunkCapacity = 50000000; // 50M numbers

        /// <summary>
        /// Outputs the Top N largest numbers in the given file to console
        /// </summary>
        /// <param name="filePath">Path the input file</param>
        /// <param name="n">Number of elements to be retreived</param>
        /// <returns>Total elements retrieved</returns>
        public int TopN(string filePath, int n)
        {
            using (var consoleWriter = new StreamWriter(Console.OpenStandardOutput()))
            {
                consoleWriter.AutoFlush = true;
                Console.SetOut(consoleWriter);

                return TopN(filePath, n, consoleWriter);
            }
        }

        /// <summary>
        /// Outputs the Top N largest numbers in the given file to a file
        /// </summary>
        /// <param name="filePath">Path the input file</param>
        /// <param name="n">Number of elements to be retreived</param>
        /// <returns>Total elements retrieved</returns>
        public int TopN(string filePath, int n, string outputFilePath)
        {
            if (String.IsNullOrEmpty(outputFilePath))
            {
                throw new ArgumentException("outputFilePath must no be empty");
            }

            using (StreamWriter fileWriter = new StreamWriter(outputFilePath))
            {
                return TopN(filePath, n, fileWriter);
            }
        }

        /// <summary>
        /// Outputs the Top N largest numbers in the given file to a file
        /// </summary>
        /// <param name="filePath">Path the input file</param>
        /// <param name="n">Number of elements to be retreived</param>
        /// <param name="streamWriter">A streamwriter object where the retrieved elements will be written</param>
        /// <returns>Total elements retrieved</returns>
        public int TopN(string filePath, int n, StreamWriter streamWriter)
        {
            if (n < 1)
            {
                throw new ArgumentOutOfRangeException("n must be greater than 0");
            }

            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("filePath must not be empty");
            }

            var chunksPaths = this.CreateSortedChunks(filePath, n);

            return MergeSortedChunks(chunksPaths.ToList(), streamWriter, n);
        }

        /// <summary>
        /// Splits specified file into sorted chunk files
        /// </summary>
        /// <returns>A list of paths to sorted file chunks</returns>
        private IEnumerable<string> CreateSortedChunks(string filePath, int n)
        {
            using (NumberReader numberReader = new NumberReader(filePath))
            {
                int i = 0;
                var numbers = new int[MaxChunkCapacity];

                while (!numberReader.EndOfStream)
                {
                    numbers[i] = numberReader.ReadNumber();

                    if ((i == MaxChunkCapacity - 1) ||
                        (numberReader.EndOfStream))
                    {
                        // We're now at max chunk size or EOF
                        yield return WriteDataToSortedChunk(numbers, i, n);
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Sorts numbers and writes them to disk from highest to lowest
        /// </summary>
        /// <param name="numbers">The unsorted numbers</param>
        /// <param name="upperIndex">The index of the last number in the array</param>
        /// <param name="n">The number of numbers to be written</param>
        /// <returns>The path to the file where the sorted numbers are stored</returns>
        private string WriteDataToSortedChunk(int[] numbers, int upperIndex, int n)
        {
            TopNQuickSort sort = new TopNQuickSort();
            sort.Sort(numbers, upperIndex, n);

            var fileName = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Only write at most the TOP N numbers, we can discard the rest
                int minIndex = Math.Max((upperIndex - n) + 1, 0);
                for (int i = upperIndex; i >= minIndex; i--)
                {
                    // Write the numbers in reverse order so the largest numbers are at the top
                    writer.WriteLine(numbers[i]);
                }
            }

            return fileName;
        }

        /// <summary>
        /// Merges sorted files and writes the top N numbers to the given StreamWriter
        /// </summary>
        /// <returns>Total elements retrieved</returns>
        private int MergeSortedChunks(List<string> chunkPaths, StreamWriter writer, int n)
        {
            int totalChunks = chunkPaths.Count();

            if (totalChunks == 0)
            {
                return 0;
            }

            int queueCapacity = Math.Min(MaxChunkCapacity / totalChunks, n);

            NumberReader[] sortedChunks = new NumberReader[totalChunks];
            for (int i = 0; i < totalChunks; i++)
            {
                sortedChunks[i] = new NumberReader(chunkPaths[i]);
            }

            // Create and populate queues to store reads from each chunk
            Queue<int>[] chunkQueues = new Queue<int>[totalChunks];
            for (int i = 0; i < totalChunks; i++)
            {
                chunkQueues[i] = new Queue<int>(queueCapacity);
                PopulateQueue(sortedChunks[i], chunkQueues[i], queueCapacity);
            }

            int elementsRetrieved = 0;
            int maxValue;
            int maxQueueIndex;
            
            while (true)
            {
                maxValue = Int32.MinValue;
                maxQueueIndex = -1;

                // Find the queue with the highest value
                for (int i = 0; i < totalChunks; i++)
                {
                    if ((chunkQueues[i].Count() > 0) &&
                        (chunkQueues[i].Peek() >= maxValue))
                    {
                        maxValue = chunkQueues[i].Peek();
                        maxQueueIndex = i;
                    }
                }

                if (maxQueueIndex == -1)
                {
                    // All queues are empty
                    break;
                }

                // Write the highest value
                writer.WriteLine(maxValue);
                if (++elementsRetrieved == n)
                {
                    // We've found the top N sorted numbers
                    break;
                }

                chunkQueues[maxQueueIndex].Dequeue();

                // If the queue is now empty we need to repopulate it
                if (chunkQueues[maxQueueIndex].Count == 0)
                {
                    PopulateQueue(sortedChunks[maxQueueIndex], chunkQueues[maxQueueIndex], queueCapacity);
                }
            }

            // Close and delete the chunks
            for (int i = 0; i < totalChunks; i++)
            {
                sortedChunks[i].Close();
                File.Delete(chunkPaths[i]);
            }

            return elementsRetrieved;
        }

        private void PopulateQueue(NumberReader sortedFile, Queue<int> queue, int queueCapacity)
        {
            for (int i = 0; i < queueCapacity; i++)
            {
                if (sortedFile.EndOfStream)
                {
                    break;
                }
                else
                {
                    queue.Enqueue(sortedFile.ReadNumber());
                }
            }
        }

    }
}
