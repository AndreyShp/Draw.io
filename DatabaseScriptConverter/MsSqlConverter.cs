using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DatabaseScriptConverter {
    /// <summary>
    /// Converter MSSQL-scripts to SQL script for draw.io
    /// </summary>
    public class MsSqlConverter {
        private readonly Regex _table = new Regex(
            @"CREATE\s+TABLE\s+\[dbo\].\[(?<tableName>[^\]]+)\]\((?<line>\s+\[(?<columnName>\w+)\]\s\[(?<type>\w+)\][\w\s\d\(\),]+,)+.*?\[(?<primaryKey>[^\]]+)\]\s(?:ASC|DESC).+?GO\s+",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private readonly IDictionary<string, string> _typesToReplace = new Dictionary<string, string> {};

        /// <summary>
        /// Convert SQL-text to draw.io
        /// </summary>
        /// <param name="originalSqlText">text to convert</param>
        /// <returns>converted text</returns>
        public string Convert(string originalSqlText) {
            var result = new StringBuilder();

            var tablesMatches = _table.Matches(originalSqlText);
            foreach (Match match in tablesMatches) {
                var tableName = match.Groups["tableName"].Value;
                var lines = match.Groups["line"].Captures;
                var primaryKey = match.Groups["primaryKey"].Value;
                var columnTypes = match.Groups["type"].Captures;
                var columnNames = match.Groups["columnName"].Captures;
                
                result.AppendLine($"CREATE TABLE {tableName}\r\n(");
                for (var i = 0; i < lines.Count; i++) {
                    string convertedLine = lines[i].Value;
                    string columnType = columnTypes[i].Value;
                    string columnName = columnNames[i].Value;

                    convertedLine = convertedLine.Replace("\r\n", "");
                    convertedLine = convertedLine.Replace("\t", "");
                    convertedLine = convertedLine.Replace("[", "").Replace("]", "");
                    if (_typesToReplace.TryGetValue(columnType, out var newType)) {
                        convertedLine = convertedLine.Replace(columnType, newType, StringComparison.InvariantCultureIgnoreCase);
                    }

                    if (columnName == primaryKey) {
                        convertedLine = convertedLine.Replace("IDENTITY(1,1)", "", StringComparison.InvariantCultureIgnoreCase);
                        convertedLine = convertedLine.TrimEnd(new char[] {','});
                        convertedLine += " PRIMARY KEY,";
                    }
                    result.AppendLine(convertedLine);
                }
                result.AppendLine(");");
            }

            return result.ToString();
        }
    }
}