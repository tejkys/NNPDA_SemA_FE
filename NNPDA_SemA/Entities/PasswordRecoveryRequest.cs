namespace NNPDA_SemA.Entities;

public class PasswordRecoveryRequest
{
    public string Username { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string NewPasswordAgain { get; set; }
}