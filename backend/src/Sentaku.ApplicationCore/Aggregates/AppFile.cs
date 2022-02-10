using System;
using System.Net;
using Ardalis.GuardClauses;
using Project.SharedKernel;
using Project.SharedKernel.Interfaces;

namespace Project.ApplicationCore.Aggregates
{
  public class AppFile : BaseEntity<string>, IAggregateRoot
  {
    public byte[] Content { get; }
    public string ContentType { get; }
    public DateTime UploadAt { get; } = DateTime.UtcNow;
    public string UntrustedName { get; }

    private AppFile()
    {

    }

    public AppFile(string untrustedName, byte[] content, string contentType)
    {
      Guard.Against.InvalidInput(content, nameof(content), bytes => bytes.Length < 2097152);

      Guard.Against.NullOrWhiteSpace(untrustedName, nameof(untrustedName));
      Guard.Against.InvalidInput(untrustedName, nameof(untrustedName), s => s.Length <= 256);

      Guard.Against.NullOrWhiteSpace(contentType, nameof(contentType));
      Guard.Against.InvalidInput(contentType, nameof(contentType), s => s.Length <= 256);

      Id = Guid.NewGuid().ToString();
      Content = content;
      ContentType = contentType;
      UntrustedName = untrustedName;
    }

    public string GetTrustedName() => WebUtility.HtmlEncode(UntrustedName);
  }
}
