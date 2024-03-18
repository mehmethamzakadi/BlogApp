using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BlogApp.Domain.DTOs
{
    public record EMailRequest(string ToEmail, string Subject, string Body, List<IFormFile> Attachments);
}
