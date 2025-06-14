using System.Net;
using System.Net.Http.Json;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Shouldly;

namespace Jennifer.Account.E2ETests.Auth;

public class SignUpFlowTests
{
    [Fact]
    public async Task Should_SignUp_Then_Failed_Flow()
    {
        var signUpRequest = new RegisterRequest()
        {
            Email = "test@test.com",
            Password = "qawsEDRF",
            PhoneNumber = "01011112222",
            VerifyType = ENUM_EMAIL_VERIFY_TYPE.SIGN_UP_BEFORE.Name,
            UserName = "tester",
            VerifyCode = string.Empty
        };
        var signUpResponse = await HttpClientFactory.Create().PostAsJsonAsync("/api/v1/auth/signup", signUpRequest);
        signUpResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        var signInRequest = new SignInRequest("test@test.com", "qawsEDRF");
        var signInResponse = await HttpClientFactory.Create().PostAsJsonAsync("/api/v1/auth/signin", signInRequest);
        signInResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var tokenResponse = await signInResponse.Content.ReadFromJsonAsync<Result<TokenResponse>>();
        tokenResponse.ShouldNotBeNull();
        tokenResponse!.IsSuccess.ShouldBeFalse();
    }
}