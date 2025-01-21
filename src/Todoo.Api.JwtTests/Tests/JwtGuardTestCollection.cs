using Todoo.Api.JwtTests.Helpers;

using Xunit;

namespace Todoo.Api.JwtTests.Tests;

/// <summary>
/// The JWT Guard test collection.
/// </summary>
[CollectionDefinition(CollectionName)]
public class JwtGuardTestCollection : ICollectionFixture<TargetApiWebApplicationFactory>
{
    /// <summary>
    /// Name of the test collection
    /// </summary>
    public const string CollectionName = "JWT Guard Tests";
}