using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NLog.Targets.Gelf
{
    internal class GraylogMessageBuilder
    {
        private readonly JObject _graylogMessage = new JObject();

        public GraylogMessageBuilder WithLevel(LogLevel level)
        {
            return WithProperty("level", level.GelfSeverity());
        }

        public GraylogMessageBuilder WithProperty(string propertyName, object value)
        {
            _graylogMessage[propertyName] = value.ToString();
            return this;
        }

        public GraylogMessageBuilder WithCustomProperty(string propertyName, object value)
        {
            return WithProperty($"_{propertyName}", value);
        }

        public GraylogMessageBuilder WithCustomPropertyRange(Dictionary<string, string> properties)
        {
            return properties.Aggregate(this, (builder, pair) => builder.WithCustomProperty(pair.Key, pair.Value));
        }

        public string Render()
        {
            _graylogMessage["timestamp"] = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            _graylogMessage["version"] = "1.1";
            return _graylogMessage.ToString();
        }
    }
}
