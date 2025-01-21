﻿using System.Net;
using System.Net.Http.Headers;

using Todoo.Api.JwtTests.Helpers;

using Xunit;

namespace Todoo.Api.JwtTests.Tests;

/// <summary>
/// Test class to test the issuer ("iss") claim in JWTs.
/// </summary>
public class IssuerTests(TargetApiWebApplicationFactory factory) : JwtGuardTestBase(factory)
{
    [Theory(DisplayName = "When a token uses allowed values for the issuer claim, the API should not return a 401 Unauthorized response.")]
    [MemberData(nameof(GetAllowedIssuers))]
    internal async Task Accessing_AuthorizedUrl_Is_Authorized_For_Allowed_Issuer(string? issuer)
    {
        if (issuer is null)
        {
            Assert.Null(issuer);
            return;
        }

        // Arrange
        var jwt = await GetJwtAsync(issuer);
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var response = await Client.GetAsync(TestSettings.CurrentTestSettings.TargetUrl);

        // Assert
        TestSettings.CurrentTestSettings.AssertAuthorizedResponse(response);
    }

    [Theory(DisplayName = "When a token uses disallowed values for the issuer claim, the API should return a 401 Unauthorized response.")]
    [MemberData(nameof(GetDisallowedIssuers))]
    internal async Task Accessing_AuthorizedUrl_Is_Unauthorized_For_Disallowed_Issuers(string? issuer)
    {
        if (issuer is null)
        {
            Assert.Null(issuer);
            return;
        }

        // Arrange
        var jwt = await GetJwtAsync(issuer);
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var response = await Client.GetAsync(TestSettings.CurrentTestSettings.TargetUrl);

        // Assert
        TestSettings.CurrentTestSettings.AssertUnauthorizedResponse(response);
    }

    private Task<string> GetJwtAsync(string issuer)
    {
        return Factory.CreateJwtBuilder()
            .WithIssuer(issuer)
            .BuildAsync();
    }

    /// <summary>
    /// Retrieves the allowed issuers for our test theories.
    /// </summary>
    public static TheoryData<string?> GetAllowedIssuers()
    {
        return TestSettings.CurrentTestSettings.AllowedIssuers.Count == 0
            ? new TheoryData<string?>([null])
            : new TheoryData<string?>(TestSettings.CurrentTestSettings.AllowedIssuers);
    }

    /// <summary>
    /// Retrieves the disallowed issuers for our test theories.
    /// </summary>
    public static TheoryData<string?> GetDisallowedIssuers()
    {
        return TestSettings.CurrentTestSettings.DisallowedIssuers.Count == 0
            ? new TheoryData<string?>([null])
            : new TheoryData<string?>(TestSettings.CurrentTestSettings.DisallowedIssuers);
    }
}