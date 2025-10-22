namespace Challenge.WebApi.Dtos;

public record UserEventDto
{
    public string UserId { get; set; }
    public string Type { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? Data { get; set; }
}