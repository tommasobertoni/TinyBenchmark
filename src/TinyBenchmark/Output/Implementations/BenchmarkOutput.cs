using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal class BenchmarkOutput : IBenchmarkOutput
    {
        private const string _Indent = "  ";

        private int _indentLevel;
        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                _indentLevel = value;

                if (_indentLevel < 0)
                    _indentLevel = 0;
            }
        }

        private readonly OutputLevel _maxOutputLevel;

        public BenchmarkOutput(OutputLevel maxOutputLevel)
        {
            _maxOutputLevel = maxOutputLevel;
            IndentLevel = 0;
        }

        public bool IsShown(OutputLevel level) => level <= _maxOutputLevel;

        public void WriteLine(string message) => this.WriteLine(OutputLevel.Verbose, message);

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

        public ProgressWriter ProgressFor(OutputLevel outputLevel, int totalItems, int progressLength = 40)
        {
            var progressWriter = new ProgressWriter(totalItems, p =>
            {
                if (outputLevel > _maxOutputLevel) return;
                var text = GetTextWithIndent(p);
                Console.Write(text);
            }, progressLength: progressLength);

            return progressWriter;
        }

        //public IOutputBuffer Buffer() => new OutputBuffer(this);
    }
}
