using CommuniMerge.Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data
{
    public class LogContext : DbContext
    {

        public LogContext(DbContextOptions<LogContext> options) : base(options)
        {
                        
        }
        public DbSet<LogEntry> Logs { get; set; }

    }
}
