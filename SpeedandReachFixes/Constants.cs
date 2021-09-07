// Contains constants used for determining null types.
namespace SpeedandReachFixes
{
    /// <summary>
    /// Contains default value constants used internally.
    /// </summary>
    internal struct Constants
    {
        public const float NullFloat = -0F; // default value assigned to null stat values, indicates that they should be skipped.
        public const int DefaultPriority = -1; // default priority returned when no category matches were found
    }
}
