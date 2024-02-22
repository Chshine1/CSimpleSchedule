namespace DiaryProject.Shared.Contact;

public class LocalResponse<T>
{
    public string? Message { get; set; }

    public bool Status { get; set; }

    public string? FileOperated { get; set; }

    public T? Result { get; set; }
}