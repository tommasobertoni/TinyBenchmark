//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace TinyBenchmark
//{
//    internal class OutputBuffer : IOutputBuffer
//    {
//        private readonly BenchmarkOutput _output;
//        private readonly Queue<(OutputLevel outputLevel, string message)> _messagesQueue = new Queue<(OutputLevel, string)>();

//        public OutputBuffer(BenchmarkOutput output)
//        {
//            _output = output;
//        }

//        public void AppendLine(string message) => this.AppendLine(OutputLevel.Verbose, message);

//        public void AppendLine(OutputLevel outputLevel, string message) => _messagesQueue.Enqueue((outputLevel, message));

//        public void Write()
//        {
//            foreach (var (outputLevel, text) in _messagesQueue)
//                _output.WriteLine(outputLevel, text);
//        }
//    }
//}
