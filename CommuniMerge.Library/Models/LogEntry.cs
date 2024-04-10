using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}
