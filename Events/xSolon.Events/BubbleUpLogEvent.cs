namespace xSolon.Events
{
    /// <summary>
    /// Delegate used for events that notify subscribers of events in the current class
    /// </summary>
    public delegate void BubbleUpLogEvent(EventEntry entry);
}