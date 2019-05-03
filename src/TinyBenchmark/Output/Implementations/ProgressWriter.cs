using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal class ProgressWriter
    {
        public const char FillChar = '█';
        public const char EmptyChar = ' ';

        public int TotalItems { get; }

        public int ProcessedItems { get; private set; }

        public decimal ProgressPercentage => this.ProcessedItems * 100 / this.TotalItems;

        private readonly Action<string> _writer;
        private readonly char _progressChar;
        private readonly char _emptyChar;
        private readonly int _progressLength;

        public ProgressWriter(
            int totalItems,
            Action<string> writer,
            char progressChar = FillChar,
            char emptyChar = EmptyChar,
            int progressLength = 40)
        {
            this.TotalItems = totalItems > 0 ? totalItems : throw new ArgumentException($"{nameof(totalItems)} must be positive.");
            this.ProcessedItems = 0;
            _writer = writer;
            _progressChar = progressChar;
            _emptyChar = emptyChar;
            _progressLength = progressLength;

            WriteProgress();
        }

        public void IncreaseProcessedItems()
        {
            if (this.ProcessedItems < this.TotalItems)
            {
                this.ProcessedItems++;
                WriteProgress();
            }
        }

        private void WriteProgress()
        {
            var completedCharsCount = this.ProcessedItems * _progressLength / this.TotalItems;
            var completed = new string(_progressChar, completedCharsCount);
            var uncompletedCharsCount = _progressLength - completedCharsCount;
            var uncompleted = new string(_emptyChar, uncompletedCharsCount);

            var progressString = $"[{completed}{uncompleted}] {this.ProgressPercentage}%";

            if (this.ProcessedItems >= this.TotalItems)
                _writer($"\r{progressString}\n"); // Completed
            else
                _writer($"\r{progressString}");
        }
    }
}
