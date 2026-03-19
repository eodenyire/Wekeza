namespace WekezaERMS.Application.DTOs;

public class CreateControlDto
{
    public string ControlName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string TestingFrequency { get; set; } = string.Empty;
}
