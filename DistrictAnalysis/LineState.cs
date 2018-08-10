using System;

public enum LineState : byte
{
    COMPLETE,
    MISSING_TERM_ENDS,
    MISSING_CERTAIN_EMAILS,
    ONLY_GENERAL_CONTACT,
    MISSING_ANY_CONTACT,
    INCOMPLETE
}

public static class StateUtils
{
    public static ConsoleColor GetColour(this LineState state)
    {
        switch (state)
        {
            case LineState.COMPLETE:
                return ConsoleColor.Green;
            case LineState.MISSING_TERM_ENDS:
                return ConsoleColor.DarkGreen;
            case LineState.MISSING_CERTAIN_EMAILS:
                return ConsoleColor.Yellow;
            case LineState.MISSING_ANY_CONTACT:
                return ConsoleColor.Red;
            case LineState.ONLY_GENERAL_CONTACT:
                return ConsoleColor.DarkYellow;
            case LineState.INCOMPLETE:
                return ConsoleColor.DarkGray;
            default:
                return ConsoleColor.Gray;
        }
    }
}
