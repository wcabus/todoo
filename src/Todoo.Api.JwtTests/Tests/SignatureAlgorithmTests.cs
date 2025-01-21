﻿using System.Net.Http.Headers;
using Todoo.Api.JwtTests.Helpers;
using Xunit;

namespace Todoo.Api.JwtTests.Tests;

/// <summary>
/// Test class to test the signature algorithm ("alg") claim in JWTs.
/// </summary>
public class SignatureAlgorithmTests(TargetApiWebApplicationFactory factory) : JwtGuardTestBase(factory)
{
    [Fact]
    public async Task When_Algorithm_Is_none_Then_Token_Is_Invalid()
    {
        // TODO Implement me
    }
    
    [Fact]
    public async Task When_Algorithm_Is_nOnE_Then_Token_Is_Invalid()
    {
        // TODO Implement me
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    [Theory(DisplayName = "When a token uses a supported signature algorithm, the API should not return a 401 Unauthorized response.")]
    [MemberData(nameof(GetAllowedAlgorithms))]
    internal async Task Accessing_AuthorizedUrl_Is_Authorized_For_Supported_Signature_Algorithms(string? signatureAlgorithm)
    {
        if (signatureAlgorithm is null)
        {
            Assert.Null(signatureAlgorithm);
            return;
        }

        // Arrange
        var jwt = await GetJwtAsync(signatureAlgorithm);
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var response = await Client.GetAsync(TestSettings.CurrentTestSettings.TargetUrl);

        // Assert
        TestSettings.CurrentTestSettings.AssertAuthorizedResponse(response);
    }

    [Theory(DisplayName = "When a token uses an unsupported signature algorithm, the API should return a 401 Unauthorized response.")]
    [MemberData(nameof(GetDisallowedAlgorithms))]
    internal async Task Accessing_AuthorizedUrl_Is_Unauthorized_For_Unsupported_Signature_Algorithms(string? signatureAlgorithm)
    {
        if (signatureAlgorithm is null)
        {
            Assert.Null(signatureAlgorithm);
            return;
        }

        // Arrange
        var jwt = await GetJwtAsync(signatureAlgorithm);
        Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        // Act
        var response = await Client.GetAsync(TestSettings.CurrentTestSettings.TargetUrl);

        // Assert
        TestSettings.CurrentTestSettings.AssertUnauthorizedResponse(response);
    }

    private Task<string> GetJwtAsync(string signatureAlgorithm)
    {
        return Factory.CreateJwtBuilder()
            .WithSignatureAlgorithm(signatureAlgorithm)
            .BuildAsync();
    }

    /// <summary>
    /// Retrieves the allowed signature algorithms for our test theories.
    /// </summary>
    public static TheoryData<string?> GetAllowedAlgorithms()
    {
        return TestSettings.CurrentTestSettings.AllowedAlgorithms.Count == 0
            ? new TheoryData<string?>([null])
            : new TheoryData<string?>(TestSettings.CurrentTestSettings.AllowedAlgorithms);
    }

    /// <summary>
    /// Retrieves the disallowed signature algorithms for our test theories.
    /// </summary>
    public static TheoryData<string?> GetDisallowedAlgorithms()
    {
        return TestSettings.CurrentTestSettings.DisallowedAlgorithms.Count == 0
            ? new TheoryData<string?>([null])
            : new TheoryData<string?>(TestSettings.CurrentTestSettings.DisallowedAlgorithms);
    }
}