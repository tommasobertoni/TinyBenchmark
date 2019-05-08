using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class TextExporter : IExporter
    {
        public bool IncludeIterations { get; set; } = true;

        private int _indentLevel;
        protected virtual int IndentLevel
        {
            get => _indentLevel;
            set => _indentLevel = value < 0 ? 0 : value;
        }

        private readonly StringBuilder _sb = new StringBuilder();

        public virtual void Visit(BenchmarksContainerReport cr)
        {
            AppendLine($"Container \"{cr.Name}\"");

            this.IndentLevel++;
            AppendLine($"started:        {FormatAsLocal(cr.StartedAtUtc)}");
            AppendLine($"duration:       {Format(cr.Duration)}");
            AppendLine();

            if (cr.Reports.Any() == false)
            {
                AppendLine($"No benchmarks");
                this.IndentLevel--;
                return;
            }

            var allAppliedParameters = cr.Reports.SelectMany(r => r.AppliedParameters).Distinct();

            foreach (var parameters in allAppliedParameters)
            {
                var benchmarksWithParameters = cr.Reports.Where(r => r.AppliedParameters.Contains(parameters));

                if (parameters == null)
                {
                    AppendLine("without parameters");
                }
                else
                {
                    AppendLine("parameters:");

                    this.IndentLevel++;

                    foreach (var pValue in parameters.Values)
                        AppendLine($"- {pValue.PropertyName} = {pValue.Value}");

                    this.IndentLevel--;
                }

                AppendLine();

                this.IndentLevel++;

                foreach (var report in benchmarksWithParameters)
                {
                    (report as IBenchmark).Accept(this);
                    AppendLine();
                }

                this.IndentLevel--;

                AppendLine();
            }

            this.IndentLevel--;
        }

        public virtual void Visit(BenchmarkReport r)
        {
            AppendLine($"{r.Name}");

            this.IndentLevel++;

            AppendLine($"started:        {FormatAsLocal(r.StartedAtUtc)}");
            AppendLine($"duration:       {Format(r.Duration)}");
            AppendLine($"avg. warmup:    {Format(r.AvgIterationWarmup)}");
            AppendLine($"avg. duration:  {Format(r.AvgIterationDuration)}");

            if (r.BaselineRatio.HasValue)
                AppendLine($"ratio:          {FormatRatio(r.BaselineRatio.Value)}");

            if (r.HasExceptions)
                AppendLine($"threw {r.Exception.InnerExceptions.Count} exceptions");

            if (this.IncludeIterations)
            {
                AppendLine();
                AppendLine($"iterations ({r.IterationReports.Count})");

                this.IndentLevel++;

                foreach (var ir in r.IterationReports)
                    (ir as IBenchmark).Accept(this);

                this.IndentLevel--;
            }

            this.IndentLevel--;
        }

        public virtual void Visit(IterationReport ir)
        {
            if (!this.IncludeIterations) return;

            if (ir.Arguments != null)
            {
                var argumentsStrings = ir.Arguments.Select(argument => $"{argument.VariableName} = {argument.Value}");
                var argumentsString = string.Join(", ", argumentsStrings);
                AppendLine($"arguments:      {argumentsString}");
            }

            AppendLine($"started:        {FormatAsLocal(ir.StartedAtUtc)}");
            AppendLine($"warmup:         {Format(ir.Warmup)}");
            AppendLine($"duration:       {Format(ir.Duration)}");

            if (ir.BaselineRatio.HasValue)
                AppendLine($"ratio:          {FormatRatio(ir.BaselineRatio.Value)}");

            if (ir.Failed)
                AppendLine($"[Failed] {ir.Exception.Message}");

            AppendLine();
        }

        public virtual string GetText() => _sb.ToString().Trim();

        #region Helpers

        protected virtual void AppendLine(string text = null)
        {
            if (text == null)
            {
                _sb.AppendLine();
            }
            else
            {
                var indent = new string(' ', this.IndentLevel * 2);
                _sb.AppendLine($"{indent}{text}");
            }
        }

        protected virtual void Append(string text)
        {
            var indent = new string(' ', this.IndentLevel * 2);
            _sb.Append($"{indent}{text}");
        }

        protected virtual string Format(DateTime dateTime) => dateTime.ToString();

        protected virtual string FormatAsLocal(DateTime dateTime) =>
            $"{Format(dateTime.ToLocalTime())} (local time)";

        protected virtual string Format(TimeSpan timeSpan) => timeSpan.ToString();

        protected virtual string FormatRatio(decimal ratio) => Math.Round(ratio, 5).ToString();

        #endregion
    }
}
