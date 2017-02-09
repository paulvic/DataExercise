namespace TopN
{
    /// <summary>
    /// An optimized quicksort implementation to sort the nth largest numbers in a given array.
    /// </summary>
    class TopNQuickSort
    {
        private int[] numbers;

        /// <summary>
        /// Sorts the numbers in the array up to to maxIndex from lowest to highest.
        /// Forgoes sorting numbers to left of the nth largest number once its position
        /// has been established.
        /// </summary>
        /// <param name="values">An integer array</param>
        /// <param name="upperIndex">The </param>
        /// <param name="n"></param>
        public void Sort(int[] values, int upperIndex, int n)
        {
            this.numbers = values;
            int nthLargestIndex = upperIndex - (n - 1); // Set this to zero to see the difference in performance
            QuickSortRecursive(0, upperIndex, nthLargestIndex);
        }

        private void QuickSortRecursive(int low, int high, int nthLargestIndex)
        {
            // Choose the pivot from the middle
            int pivot = this.numbers[low + (high - low) / 2];

            int i = low;
            int j = high;
            while (i <= j)
            {
                // Find an element on the left that's larger than the pivot
                while (this.numbers[i] < pivot)
                {
                    i++;
                }

                // Find an element on the right that's smaller than the pivot
                while (this.numbers[j] > pivot)
                {
                    j--;
                }
                
                if (i <= j)
                {
                    // We've found a value on the left that's larger and an element on the
                    // right that's smaller. Swap them and move on.
                    Swap(i, j);
                    i++;
                    j--;
                }
            }
            if (low < j && j >= nthLargestIndex)
            {
                // Only sort to the left if the nth largest index is within this range
                QuickSortRecursive(low, j, nthLargestIndex);
            }
            if (i < high)
            {
                QuickSortRecursive(i, high, nthLargestIndex);
            }
        }

        private void Swap(int i, int j)
        {
            int temp = this.numbers[i];
            this.numbers[i] = this.numbers[j];
            this.numbers[j] = temp;
        }
    }
}
