# DataExercise
An exercise to write a program to output the Top N numbers from a large file.

# Solution
This solution assumes memory is constrained so an External Merge Sort (https://en.wikipedia.org/wiki/External_sorting) is used. The input file is read in “chunks” into an array. Once the array is full, the data is then sorted using an optimized quicksort that forgoes sorting data that is known to be lower than the top Nth number in that array. As we only require the top N numbers, only the top N numbers found in the partially sorted array are written to disk (from highest to lowest) and the remaining numbers are discarded. This process is repeated until the input file is read to the end.
A K-way merge is then performed to read the TOP N numbers from the sorted chunks. Data from each chunk is read in small batches into a queue. Each queue is then peeked at to see which contains the largest number. Once the queue with largest number has been identified, it is dequeued and the number is output. When a queue is emptied, it is repopulated with another batch of data from its corresponding chunk. Once the TOP N numbers have been found, the merge is abandoned and the chunks are deleted from the disk.
The quick sort average performance is O(n log n) and worst case O(n2). The merging operation performance is O(n). Space complexity is constant as the maximum memory allocation is fixed (see chunk size below).

# Assumptions:
1.	Program does not have to return distinct TOP N numbers (duplicates are acceptable)
2.	Disk space is not constrained

# Notes:
1.	The chunk size (amount of data that can be loaded into memory) should proportional to the available RAM. I have set the chunk size to 50,000,000 numbers. A memory profile should be performed to tune the optimal chunk size.

# Testing:
1.	NUnit unit tests are included
2.	A simple command line utility is included to generate unsorted data files

# Further enhancement not implemented:
1.	If the entire file can be read into a single chunk, then sort it and return the TOP N without writing to disk.
2.	Rather than reading a chunk into memory and sorting it before writing it to disk, unsorted chunks could be written to disk and then the sorting operations could be distributed and run in parallel.
3.	Allow setting the chunk size from tests to validate merging functionality in less time by using smaller data files.
4.	Cleaning up leftover chunks in the case of an exception occurring.
