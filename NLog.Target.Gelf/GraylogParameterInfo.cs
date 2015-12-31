using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets.Gelf
{
    [NLogConfigurationItem]
    public class GraylogParameterInfo
    {
        [RequiredParameter]
        public string Name { get; set; }

        [RequiredParameter]
        public Layout Layout { get; set; }
    }
}
