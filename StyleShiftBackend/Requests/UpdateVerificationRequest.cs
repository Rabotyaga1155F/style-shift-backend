namespace StyleShiftBackend.Requests;

public class UpdateVerificationRequest
{
    public string Email { get; set; }
    public bool Verification { get; set; }
}