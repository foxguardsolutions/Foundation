namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// Common behaviour required for filter wrappers.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/IFilterWrapper.cs </remarks>
    internal interface IFilterWrapper
    {
        /// <summary>
        /// Gets the metadata key used to retrieve the filter metadata.
        /// </summary>
        string MetadataKey { get; }
    }
}
