using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// The main <see cref="IBenchmarkOutput"/> implementation used internally by the library to output execution logs.
    /// It supports the definition of the indent level, in order to format a series of related logs.
    /// </summary>
    internal class BenchmarkOutput : IBenchmarkOutput
    {
        private const string _Indent = "  ";

        private int _indentLevel;
        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                if (_maxOutputLevel > OutputLevel.ErrorsOnly)
                {
                    _indentLevel = value;

                    if (_indentLevel < 0)
                        _indentLevel = 0;
                }
            }
        }

        private readonly OutputLevel _maxOutputLevel;

        /// <summary>
        /// Creates a new instance given the max output level defined by the configuration.
        /// </summary>
        /// <param name="maxOutputLevel">The max output level.</param>
        public BenchmarkOutput(OutputLevel maxOutputLevel)
        {
            _maxOutputLevel = maxOutputLevel;
            IndentLevel = 0;
        }

        /// <summary>
        /// Checks the level against the max output level defined by the configuration: returns true when a log with the given level
        /// can be sent to the output.
        /// </summary>
        /// <param name="level">The log level to evaluate.</param>
        /// <returns>true if a log with the given level can be sent to the output; otherwise, false.</returns>
        public bool IsShown(OutputLevel level) => level <= _maxOutputLevel;

        /// <summary>
        /// Writes a new line with the log message with a Verbose level.
        /// This method is invoked by the benchmarks.
        /// </summary>
        /// <param name="message">The log message.</param>
        void IBenchmarkOutput.WriteLine(string message) => this.WriteLine(OutputLevel.Verbose, message);

        /// <summary>
        /// Writes a new line with the log message with the given log, if the configuration allows it.
        /// </summary>
        /// <param name="outputLevel"></param>
        /// <param name="message">The log message.</param>
        public void WriteLine(OutputLevel outputLevel, string message)
        {
            if (outputLevel > _maxOutputLevel) return;
            var text = GetTextWithIndent(message);
            Console.WriteLine(text);
        }

        private string GetTextWithIndent(string message)
        {
            string text = message;

            if (this.IndentLevel > 0)
            {
                var prefixBuilder = new StringBuilder();
                for (int i = 0; i < this.IndentLevel; i++)
                    prefixBuilder.Append(_Indent);

                prefixBuilder.Append(message);

                text = prefixBuilder.ToString();
            }

            return text;
        }

        /// <summary>
        /// Creates a <see cref="ProgressWriter"/> with the log configuration of this <see cref="BenchmarkOutput"/> instance.
        /// </summary>
        /// <param name="outputLevel">The output level that the progress will write logs with.</param>
        /// <param name="totalItems">The total items that the execution tracked by this writer will process.</param>
        /// <param name="progressLength">The length of the text progress bar, in characters.</param>
        /// <returns></returns>
        public ProgressWriter ProgressFor(OutputLevel outputLevel, int totalItems, int progressLength = 40)
        {
            var progressWriter = new ProgressWriter(totalItems, p =>
            {
                if (!IsShown(outputLevel)) return;
                var text = GetTextWithIndent(p);
                Console.Write(text);
            }, progressLength: progressLength);

            return progressWriter;
        }
    }
}
