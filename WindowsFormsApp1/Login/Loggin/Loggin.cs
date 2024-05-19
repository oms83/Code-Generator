using System;
using System.Diagnostics;
public class Loggin
{
    private static bool CreateLoggin(string sourceName)
    {
        try
        {
            if (!EventLog.Exists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, "Application");
                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public static void WriteToEventViewer(string sourceName, string Message, EventLogEntryType EntryType)
    {
        CreateLoggin(sourceName);
        EventLog.WriteEntry(sourceName, Message, EntryType);
    }
}
