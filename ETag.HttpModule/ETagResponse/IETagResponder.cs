using System.Web;

namespace ETag.HttpModule.ETagResponse
{
    /// <summary>
    /// Adds ETag header for requested resource to response
    /// </summary>
    internal interface IETagResponder
    {
        void Init(HttpApplication application);
    }
}