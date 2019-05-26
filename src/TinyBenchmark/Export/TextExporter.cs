using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Creates a text export of a report.
    /// </summary>
    public class TextExporter : IExporter
    {
        /// <summary>
        /// When set to true, the data of the iterations is included in the export.
        /// </summary>
        public bool IncludeIterations { get; set; } = true;

        private int _indentLevel;
        
        /// <summary>
        /// Defines an indentation level for the text.
        /// </summary>
        protected virtual int IndentLevel
        {
            get => _indentLevel;
            set => _indentLevel = value < 0 ? 0 : value;
        }

        private readonly StringBuilder _sb = new StringBuilder();

        void IExporter.Visit(BenchmarksContainerReport cr)
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
                    (report as IReport).Accept(this);
                    AppendLine();
                }

                this.IndentLevel--;

                AppendLine();
            }

            this.IndentLevel--;
        }

        void IExporter.Visit(BenchmarkReport r)
        {
            AppendLine($"{r.Name}");

            this.IndentLevel++;

            AppendLine($"started:        {FormatAsLocal(r.StartedAtUtc)}");
            AppendLine($"duration:       {Format(r.Duration)}");
            AppendLine($"init:           {Format(r.InitDuration)}");
            AppendLine($"warmup:         {Format(r.Warmup)}");
            AppendLine($"iterations:     {r.IterationReports.Count}");
            AppendLine($"avg. duration:  {Format(r.AvgIterationDuration)}");

            if (r.IsBaseline)
            {
                AppendLine($"BASELINE");
            }
            else if (r.BaselineStats != null)
            {
                AppendLine($"ratio:          {FormatRatio(r.BaselineStats.Ratio)}");
                AppendLine($"efficiency:     {FormatEfficiency(r.BaselineStats.Efficiency)}");
                AppendLine($"avg. time diff: {Format(r.BaselineStats.AvgTimeDifference)}");
            }

            if (r.HasExceptions)
                AppendLine($"threw {r.Exception.InnerExceptions.Count} exceptions");

            if (this.IncludeIterations)
            {
                AppendLine();
                AppendLine($"iterations ({r.IterationReports.Count})");

                this.IndentLevel++;

                foreach (var ir in r.IterationReports)
                    (ir as IReport).Accept(this);

                this.IndentLevel--;
            }

            this.IndentLevel--;
        }

        void IExporter.Visit(IterationReport ir)
        {
            if (!this.IncludeIterations) return;

            if (ir.Arguments != null)
            {
                var argumentsStrings = ir.Arguments.Select(argument => $"{argument.VariableName} = {argument.Value}");
                var argumentsString = string.Join(", ", argumentsStrings);
                AppendLine($"arguments:      {argumentsString}");
            }

            AppendLine($"started:        {FormatAsLocal(ir.StartedAtUtc)}");
            AppendLine($"duration:       {Format(ir.Duration)}");

            if (ir.BaselineStats != null)
            {
                AppendLine($"ratio:          {FormatRatio(ir.BaselineStats.Ratio)}");
                AppendLine($"efficiency:     {FormatEfficiency(ir.BaselineStats.Efficiency)}");
                AppendLine($"avg. time diff: {Format(ir.BaselineStats.AvgTimeDifference)}");
            }

            if (ir.Failed)
                AppendLine($"[Failed] {ir.Exception.Message}");

            AppendLine();
        }

        /// <summary>
        /// Creates the text export.
        /// </summary>
        /// <returns>The text export.</returns>
        public virtual string GetText() => _sb.ToString().Trim();

        #region Helpers

        /// <summary>
        /// Appends a line to the export prefixed with the defined <see cref="IndentLevel"/>.
        /// </summary>
        /// <param name="text">The text to append to the export.</param>
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

        /// <summary>
        /// Appends the text to the export prefixed with the defined <see cref="IndentLevel"/>.
        /// </summary>
        /// <param name="text">The text to append to the export.</param>
        protected virtual void Append(string text)
        {
            var indent = new string(' ', this.IndentLevel * 2);
            _sb.Append($"{indent}{text}");
        }

        /// <summary>
        /// Formats a DateTime.
        /// </summary>
        /// <param name="dateTime">The value to format as text.</param>
        /// <returns>The text format of the value.</returns>
        protected virtual string Format(DateTime dateTime) => dateTime.ToString();

        /// <summary>
        /// Formats a DateTime as a local time.
        /// </summary>
        /// <param name="dateTime">The value to format as text.</param>
        /// <returns>The text format of the value.</returns>
        protected virtual string FormatAsLocal(DateTime dateTime) =>
            $"{Format(dateTime.ToLocalTime())} (local time)";

        /// <summary>
        /// Formats a TimeSpan.
        /// </summary>
        /// <param name="timeSpan">The value to format as text.</param>
        /// <returns>The text format of the value.</returns>
        protected virtual string Format(TimeSpan timeSpan) => timeSpan.ToString();

        /// <summary>
        /// Formats a value as a ratio.
        /// </summary>
        /// <param name="ratio">The value to format as text.</param>
        /// <returns>The text format of the value.</returns>
        protected virtual string FormatRatio(decimal ratio) => Math.Round(ratio, 8).ToString();

        /// <summary>
        /// Formats a value as an efficiency indicator.
        /// </summary>
        /// <param name="ratio">The value to format as text.</param>
        /// <returns>The text format of the value.</returns>
        protected virtual string FormatEfficiency(decimal ratio) => Math.Round(ratio, 5).ToString();

        #endregion
    }
}
