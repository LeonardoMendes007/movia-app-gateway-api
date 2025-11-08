namespace MovieApp.AuthApi.API.Config;

public class TokenConfiguration
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int TokenValidityInHours { get; set; }
    public int RefreshTokenValidityInHours { get; set; }
}
