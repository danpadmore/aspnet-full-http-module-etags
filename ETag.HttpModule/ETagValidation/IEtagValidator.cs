using System.Web;

namespace ETag.HttpModule.ETagValidation
{
    internal interface IETagValidator
    {
        void Init(HttpApplication application);
    }
}
