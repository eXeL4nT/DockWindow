using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SnapWindow.Logger;

public class SimpleLogger
{
    private readonly object _fileLock;
    private readonly string _logFileName;
    private readonly string _dateTimeFormat;

    public SimpleLogger()
    {
        _fileLock = new object();
        _logFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.log";
        _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    }

    public void Debug(string text)
    {
        WriteLog(LogLevel.Debug, text);
    }

    private void WriteLog(LogLevel logLevel, string text)
    {
        var infoText = string.Empty;
        
        switch (logLevel)
        {
            case LogLevel.Trace:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [TRACE]   ";
                break;
            case LogLevel.Info:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [INFO]    ";
                break;
            case LogLevel.Debug:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [DEBUG]   ";
                break;
            case LogLevel.Warning:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [WARNING] ";
                break;
            case LogLevel.Error:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [ERROR]   ";
                break;
            case LogLevel.Fatal:
                infoText = DateTime.Now.ToString(_dateTimeFormat) + " [FATAL]   ";
                break;
        }

        WriteLine(infoText + text);
    }

    private void WriteLine(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        lock (_fileLock)
        {
            using var streamWriter = new StreamWriter(_logFileName, true, Encoding.UTF8);
            streamWriter.WriteLine(text);
        }
    }
}