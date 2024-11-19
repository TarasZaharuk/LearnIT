using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace LearnIT
{
    public class UserAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;

        public UserAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await _localStorageService.RemoveItemAsync("authToken");
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
            if (authenticationState.User.Identity != null && authenticationState.User.Identity.IsAuthenticated)
                return true;
            return false;
        }

        public async Task<bool> IsUserInRole(string role)
        {
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            bool isUserAuthenticated = await IsUserAuthenticated();
            if (isUserAuthenticated && authenticationState.User.IsInRole(role))
                return true;
            return false;
        }

        public async Task<int?> GetAuthenticatedUserIdAsync()
        {
            int userId;
            AuthenticationState authenticationState = await GetAuthenticationStateAsync();
            Claim? tutorIdClaim = authenticationState.User.FindFirst("Id");
            if (tutorIdClaim == null)
                return null;

            bool isClaimInteger = int.TryParse(tutorIdClaim.Value, out userId);
            if (isClaimInteger)
                return userId;

            return null;
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorageService.SetItemAsync("authToken", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
