using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ReqresInApiClient.Exceptions;
using ReqresInApiClient.Models;
using ReqresInApiClient.Options;

namespace ReqresInApiClient.Services
{
    public class ReqresInService : IReqresInService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ReqresInOptions _options;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ReqresInService(
            HttpClient httpClient,
            IOptions<ReqresInOptions> options,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            string cacheKey = $"user_{userId}";

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            var response = await _httpClient.GetAsync($"users/{userId}");
            await HandleResponseErrors(response, userId);

            var content = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<SingleUserResponse<User>>(content, _jsonOptions);

            var user = userResponse?.Data;
            if (user == null) throw new ApiException("User data is missing in response");

            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_options.CacheDurationMinutes));
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string cacheKey = "all_users";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedUsers))
            {
                return cachedUsers;
            }

            var allUsers = new List<User>();
            int currentPage = 1;
            int totalPages = 5;

            while (currentPage <= totalPages)
            {
                var response = await _httpClient.GetAsync($"users?page={currentPage}");
                await HandleResponseErrors(response);

                var content = await response.Content.ReadAsStringAsync();
                var pagedResponse = JsonSerializer.Deserialize<PagedResponse<User>>(content, _jsonOptions);

                if (pagedResponse?.Data == null)
                    throw new ApiException("Paged user data is missing in response");

                allUsers.AddRange(pagedResponse.Data);
                totalPages = pagedResponse.TotalPages;
                currentPage++;
            }

            _cache.Set(cacheKey, allUsers, TimeSpan.FromMinutes(_options.CacheDurationMinutes));
            return allUsers;
        }

        private async Task HandleResponseErrors(HttpResponseMessage response, int? userId = null)
        {
            if (response.IsSuccessStatusCode) return;

            var content = await response.Content.ReadAsStringAsync();
            throw response.StatusCode switch
            {
                HttpStatusCode.NotFound when userId.HasValue =>
                    new UserNotFoundException($"User with ID {userId.Value} not found"),
                HttpStatusCode.NotFound =>
                    new ApiException("Requested resource not found"),
                _ => new ApiException($"API request failed: {response.StatusCode} - {content}")
            };
        }
    }
}