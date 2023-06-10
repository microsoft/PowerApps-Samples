namespace PowerApps.Samples.Types
{
    /// <summary>
    /// Contains the data to describe a solution component that is required by a solution but not found in the system.
    /// </summary>
    public class MissingComponent
    {
        /// <summary>
        /// Information about the solution component in the solution file that is dependent on a missing solution component
        /// </summary>
        ComponentDetail DependentComponent { get; set; }


        /// <summary>
        /// Information about the required solution component that is missing.
        /// </summary>
        ComponentDetail RequiredComponent { get; set; }
    }
}
