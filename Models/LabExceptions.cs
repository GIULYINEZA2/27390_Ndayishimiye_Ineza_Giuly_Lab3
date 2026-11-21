using System;

namespace LabTestSystem.Models;

public class InvalidStateTransitionException : Exception
{
    public InvalidStateTransitionException(string from, string to)
        : base("A request cannot change from " + from + " to " + to +
               ". The allowed path is REQUESTED -> SAMPLED -> PROCESSED (or SAMPLED -> REJECTED).")
    {
    }
}

public class UncertifiedSectionException : Exception
{
    public UncertifiedSectionException(string message) : base(message)
    {
    }
}