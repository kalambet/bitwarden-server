using Bit.Core.Models.Api;
using Bit.Core.SecretsManager.Entities;

namespace Bit.Api.SecretsManager.Models.Response;

public class SecretWithProjectsListResponseModel : ResponseModel
{
    private const string _objectName = "SecretsWithProjectsList";

    public SecretWithProjectsListResponseModel(IEnumerable<Secret> secrets) : base(_objectName)
    {
        Secrets = secrets.Select(s => new InnerSecret2(s));
        Projects = secrets.SelectMany(s => s.Projects).DistinctBy(p => p.Id).Select(p => new InnerProject2(p));
    }

    public SecretWithProjectsListResponseModel() : base(_objectName)
    {
    }

    public IEnumerable<InnerSecret2> Secrets { get; set; }
    public IEnumerable<InnerProject2> Projects { get; set; }

    public class InnerProject2
    {
        public InnerProject2(Project project)
        {
            Id = project.Id;
            Name = project.Name;
        }

        public InnerProject2()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class InnerSecret2
    {
        public InnerSecret2(Secret secret)
        {
            Id = secret.Id.ToString();
            OrganizationId = secret.OrganizationId.ToString();
            Key = secret.Key;
            CreationDate = secret.CreationDate;
            RevisionDate = secret.RevisionDate;
            Projects = secret.Projects?.Select(p => new InnerProject2(p));
        }

        public InnerSecret2()
        {
        }

        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string Key { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime RevisionDate { get; set; }

        public IEnumerable<InnerProject2> Projects { get; set; }
    }
}


