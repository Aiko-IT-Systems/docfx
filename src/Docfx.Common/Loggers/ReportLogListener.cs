﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace Docfx.Common;

public sealed class ReportLogListener : ILoggerListener
{
    private readonly string _repoRoot;
    private readonly string _root;
    private readonly StreamWriter _writer;

    private const LogLevel LogLevelThreshold = LogLevel.Diagnostic;

    public ReportLogListener(string reportPath, string repoRoot, string root)
    {
        var dir = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }
        _writer = new StreamWriter(reportPath, true);
        _repoRoot = repoRoot;
        _root = root;
    }

    public void WriteLine(ILogItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var level = item.LogLevel;
        var message = item.Message;
        var phase = item.Phase;
        var file = item.File;
        var line = item.Line;
        if (level < LogLevelThreshold) return;

        var reportItem = new ReportItem
        {
            Severity = GetSeverity(level),
            Message = message,
            Source = phase,
            File = TransformFile(file),
            Line = line,
            DateTime = DateTime.UtcNow,
            Code = item.Code,
        };

        _writer.WriteLine(JsonUtility.Serialize(reportItem));
    }

    public void Dispose()
    {
        _writer.Dispose();
    }

    private MessageSeverity GetSeverity(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Diagnostic:
                return MessageSeverity.Diagnostic;
            case LogLevel.Verbose:
                return MessageSeverity.Verbose;
            case LogLevel.Info:
                return MessageSeverity.Info;
            case LogLevel.Suggestion:
                return MessageSeverity.Suggestion;
            case LogLevel.Warning:
                return MessageSeverity.Warning;
            case LogLevel.Error:
                return MessageSeverity.Error;
            default:
                throw new NotSupportedException(level.ToString());
        }
    }

    private string TransformFile(string fileFromRoot)
    {
        if (fileFromRoot == null)
        {
            return null;
        }
        if (string.IsNullOrEmpty(_repoRoot))
        {
            return fileFromRoot;
        }

        string file = ((RelativePath)fileFromRoot).RemoveWorkingFolder();
        string basePath = Path.GetFullPath(_repoRoot);
        string fullPath = Path.GetFullPath(Path.Combine(_root, file));
        return PathUtility.MakeRelativePath(basePath, fullPath);
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public class ReportItem
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
        [JsonProperty("line")]
        public string Line { get; set; }
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }
        [JsonProperty("message_severity")]
        public MessageSeverity Severity { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public enum MessageSeverity
    {
        Error,
        Warning,
        Suggestion,
        Info,
        Verbose,
        Diagnostic
    }
}
