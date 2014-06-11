using System;
using System.Reflection;

namespace Kms.Cloud.Api.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);

        string GetRemarks(MemberInfo member);

        string GetRemarks(Type member);
    }
}