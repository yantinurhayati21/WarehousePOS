using System.Net.Http.Json;
using System.Text.Json;
using WarehousePOS.DTOs;

namespace WarehousePOS.Services
{
    public interface IAuthService
    {
        Task<GenerateTokenResponse> GenerateTokenAsync(GenerateTokenRequest? request = null);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> FullAuthAsync(LoginRequest loginRequest);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://api-wms.wit.co.id";

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<GenerateTokenResponse> GenerateTokenAsync(GenerateTokenRequest? request = null)
        {
            try
            {
                request ??= new GenerateTokenRequest();

                _logger.LogInformation("Generating token from external API...");
                
                var payload = new
                {
                    app_name = request.AppName,
                    app_key = request.AppKey,
                    device_id = request.DeviceId,
                    device_type = request.DeviceType,
                    fcm_token = request.FcmToken,
                    ip_address = request.IpAddress
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/GenerateToken", jsonContent);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("GenerateToken Response Status: {Status}, Content: {Content}", response.StatusCode, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GenerateTokenResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (result == null)
                    {
                        return new GenerateTokenResponse 
                        { 
                            MessageEn = "Failed to parse response" 
                        };
                    }
                    
                    return result;
                }

                return new GenerateTokenResponse
                {
                    MessageEn = $"Failed to generate token: {response.StatusCode} - {content}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token");
                return new GenerateTokenResponse
                {
                    MessageEn = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Step 2: Login dengan token dari Step 1
        /// </summary>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // Step 1: Generate App Token dulu
                var tokenResponse = await GenerateTokenAsync();
                
                if (!tokenResponse.Status || string.IsNullOrEmpty(tokenResponse.Token))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = $"Failed to get app token: {tokenResponse.Message}"
                    };
                }

                _logger.LogInformation("App token generated successfully, proceeding to login...");

                // Step 2: Login dengan token di header
                var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/Login");
                loginRequest.Headers.Add("token", tokenResponse.Token);
                loginRequest.Content = JsonContent.Create(request);

                var response = await _httpClient.SendAsync(loginRequest);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Login Response: {Content}", content);

                if (response.IsSuccessStatusCode)
                {
                    var loginResult = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loginResult != null && loginResult.Success)
                    {
                        return new AuthResponse
                        {
                            Success = true,
                            Message = "Login successful",
                            AppToken = tokenResponse.Token,
                            UserToken = loginResult.Token,
                            User = loginResult.Data
                        };
                    }

                    return new AuthResponse
                    {
                        Success = false,
                        Message = loginResult?.Message ?? "Login failed"
                    };
                }

                return new AuthResponse
                {
                    Success = false,
                    Message = $"Login failed: {response.StatusCode} - {content}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Full Auth: Generate Token + Login dalam satu call
        /// </summary>
        public async Task<AuthResponse> FullAuthAsync(LoginRequest loginRequest)
        {
            return await LoginAsync(loginRequest);
        }
    }
}
