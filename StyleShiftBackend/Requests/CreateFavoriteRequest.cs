namespace StyleShiftBackend.Requests;

public class CreateFavoriteRequest
{
    public string UserID { get; set; } = string.Empty;
    public string ProductID { get; set; } = string.Empty;
}