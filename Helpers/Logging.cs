using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EDS.Helpers
{
    public class Logging
    {
        private LogWriter writer;

        public void LogException(Exception ex)
        {
            if(writer == null)
            {
                LogWriterFactory factory = new LogWriterFactory();
                writer = factory.Create();
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Exception Message: " + ex.Message);
            builder.AppendLine("Exception Stack: " + ex.StackTrace);

            if(ex.InnerException != null)
            {
                builder.AppendLine("Inner Exception Message: " + ex.InnerException.Message);
                builder.AppendLine("Inner Exception Stack: " + ex.InnerException.StackTrace);
            }

            LogEntry entry = new LogEntry(builder.ToString(), "General", 1, 9001, System.Diagnostics.TraceEventType.Error, "EDS TIR Error", null);
            writer.Write(entry);
        }
    }
}