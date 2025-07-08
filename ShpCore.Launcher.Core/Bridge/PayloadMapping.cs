using MSharp.Validation.Models;
using ShpCore.Logging;

namespace MSharp.Validation.Payloads;
public class AdapterResponse
{
    public string Status { get; set; } = string.Empty;
    public string? Info { get; set; }
    public string? Uuid { get; set; }

}