using System.Text.Json.Serialization;

namespace WarehousePOS.DTOs
{
    // Request untuk Generate Token (Step 1)
    public class GenerateTokenRequest
    {
        [JsonPropertyName("app_name")]
        public string AppName { get; set; } = "wms";

        [JsonPropertyName("app_key")]
        public string AppKey { get; set; } = "wms-app-key";

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; } = "web-client";

        [JsonPropertyName("device_type")]
        public string DeviceType { get; set; } = "00031310";

        [JsonPropertyName("fcm_token")]
        public string FcmToken { get; set; } = "";

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; } = "0.0.0.0";
    }

    // Response dari Generate Token (struktur dari WIT API)
    public class GenerateTokenResponse
    {
        [JsonPropertyName("app_name")]
        public string? AppName { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("response")]
        public WitResponseData? Response { get; set; }

        [JsonPropertyName("message_en")]
        public string? MessageEn { get; set; }

        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }

        // Helper properties
        public bool Status => Response?.Code == "00";
        public string? Message => MessageEn ?? MessageId;
        public string? Token => Response?.Data?.Token;
    }

    public class WitResponseData
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("data")]
        public WitTokenData? Data { get; set; }
    }

    public class WitTokenData
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("token_expired")]
        public string? TokenExpired { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("is_login")]
        public bool IsLogin { get; set; }
    }

    // Request untuk Login (Step 2)
    public class LoginRequest
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    // Response dari Login
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public LoginUserData? Data { get; set; }
    }

    public class LoginUserData
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // Combined Auth Response untuk frontend
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? AppToken { get; set; }
        public string? UserToken { get; set; }
        public LoginUserData? User { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
