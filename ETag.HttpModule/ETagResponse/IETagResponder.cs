using System.Web;

namespace ETag.HttpModule.ETagResponse
{
    internal interface IETagResponder
    {
        void Init(HttpApplication application);
    }
}