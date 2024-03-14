using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BlogApp.Domain.DTOs
{
    public class EMailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
    }
}
