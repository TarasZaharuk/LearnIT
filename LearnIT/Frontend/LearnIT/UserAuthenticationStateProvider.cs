using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LearnIT
{
    public class UserAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticatedHttpClient _authenticatedHttpClient;

        public UserAuthenticationStateProvider(ILocalStorageService localStorageService, AuthenticatedHttpClient authenticatedHttpClient)
        {
            _localStorageService = localStorageService;
            _authenticatedHttpClient = authenticatedHttpClient;
        }

        private const string AuthTokenItem = "authToken";

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>(AuthTokenItem);

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await _localStorageService.RemoveItemAsync(AuthTokenItem);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            IEnumerable<Claim> claims = jwtToken.Claims;
            ClaimsIdentity identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            return authenticationState.User.Identity != null && authenticationState.User.Identity.IsAuthenticated;
        }

        public async Task<bool> IsUserInRole(string role)
        {
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            return await IsUserAuthenticated() && authenticationState.User.FindFirst("Role")?.Value == role; ;
        }

        public async Task<int> GetAuthenticatedUserIdAsync()
        {
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            string? tutorIdClaim = authenticationState.User.FindFirst("Id")?.Value;
            bool isConvertedSuccessfully = int.TryParse(tutorIdClaim , out int userId);
            if (isConvertedSuccessfully)
                return userId;
            throw new Exception("Failed to extract 'Id' claim");
        }

        private async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorageService.SetItemAsync(AuthTokenItem, token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Login(UserLoginModel userLoginModel)
        {
            HttpResponseMessage response = await _authenticatedHttpClient.PostAsync<UserLoginModel>("/login", userLoginModel);

            string? token = await response.Content.ReadAsStringAsync();
            if (token != null)
                await MarkUserAsAuthenticated(token);
        }

        public async Task UpdateAuthenticationStateAsync()
        {
            HttpResponseMessage responseMessage = await _authenticatedHttpClient.GetAsync("/token/renewed");
            if (responseMessage.IsSuccessStatusCode)
            {
                string updatedToken = await responseMessage.Content.ReadAsStringAsync();
                await MarkUserAsAuthenticated(updatedToken);
            }
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync(AuthTokenItem);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
