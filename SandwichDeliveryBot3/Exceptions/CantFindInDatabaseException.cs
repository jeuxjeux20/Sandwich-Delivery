using System;

public class CantFindInDatabaseException : Exception
{
    public CantFindInDatabaseException()
    {
    }

    public CantFindInDatabaseException(string message)
        : base(message)
    {
    }

    public CantFindInDatabaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}