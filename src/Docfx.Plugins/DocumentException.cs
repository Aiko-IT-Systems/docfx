﻿namespace Docfx.Plugins;

public class DocumentException : Exception
{
    public string File { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public DocumentException() { }
    public DocumentException(string message) : base(message) { }
    public DocumentException(string message, Exception inner) : base(message, inner) { }

    public static void RunAll(params Action[] actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        DocumentException firstException = null;
        foreach (var action in actions)
        {
            try
            {
                action();
            }
            catch (DocumentException ex)
            {
                firstException ??= ex;
            }
        }
        if (firstException != null)
        {
            throw new DocumentException(firstException.Message, firstException);
        }
    }
}
