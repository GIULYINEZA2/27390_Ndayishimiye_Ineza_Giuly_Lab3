namespace LabTestSystem.Models;

public static class StatusRules
{
    public const string Requested = "REQUESTED";
    public const string Sampled = "SAMPLED";
    public const string Processed = "PROCESSED";
    public const string Rejected = "REJECTED";

    // REQUESTED -> SAMPLED -> PROCESSED, or SAMPLED -> REJECTED
    public static bool IsAllowed(string from, string to) =>
        (from == Requested && to == Sampled) ||
        (from == Sampled && to == Processed) ||
        (from == Sampled && to == Rejected);
}