using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudsdaleLib {
    public static class CloudsdaleEscaper {
        private static readonly Dictionary<char, string> EscapeMappings = new Dictionary<char, string> {
            {'\\', "\\\\"}, {'\n', "\\n"}, {'\r', ""}, {'\t', "\\t"}
        };
        private static readonly Dictionary<char, char> ParseMappings = new Dictionary<char, char> {
            {'\\', '\\'}, {'n', '\n'}, {'r', '\r'}, {'t', '\t'}
        };

        public static string EscapeMessage(this string message) {
            return EscapeMappings.Aggregate(message, (current, mapping) =>
                current.Replace(mapping.Key.ToString(), mapping.Value));
        }

        public static string ParseMessage(this string message) {
            var builder = new StringBuilder();

            for (var i = 0; i < message.Length; ++i) {
                if (message[i] == '\\') {
                    if (ParseMappings.ContainsKey(message[++i])) {
                        builder.Append(ParseMappings[message[i]]);
                    } else {
                        builder.Append('\\').Append(message[i]);
                    }
                } else {
                    builder.Append(message[i]);
                }
            }

            return builder.ToString();
        }
    }
}
