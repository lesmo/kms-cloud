using System;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Kms.Cloud.Api.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);

        string GetRemarks(MemberInfo member);

        string GetRemarks(Type member);

        string GetRemarks(HttpActionDescriptor action);
    }
}