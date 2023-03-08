using System.ComponentModel.DataAnnotations;

namespace CloudMockApi;

public enum ResponseStatus
{
    Ok,
    Accepted,
    Created
}

public enum ContentType
{
    Json,
}

public class MockApiEntry
{
    public MockApiEntry(string name, ResponseStatus status)
    {
        Name = name;
        Status = status;
    }

    public MockApiEntry(string name, ResponseStatus status, string? serializedContent, ContentType? type) : this(name, status)
    {
        Content = serializedContent;
        Type = type;
    }

    public MockApiEntry(MockApiCreateModel create) : this(create.Name, create.Status, create.Content, create.Type) { }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public ResponseStatus Status { get; set; }
    public string? Content { get; set; }
    public ContentType? Type { get; set; }
}

public class MockApiCreateModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public ResponseStatus Status { get; set; } = ResponseStatus.Ok;
    public string? Content { get; set; }
    public ContentType? Type { get; set; }
    public bool IsValid() => !string.IsNullOrWhiteSpace(Name);
}