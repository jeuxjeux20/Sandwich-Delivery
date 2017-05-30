using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NoWarningsException : Exception
{
    public NoWarningsException()
    {
    }

    public NoWarningsException(string message)
        : base(message)
    {
    }

    public NoWarningsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}