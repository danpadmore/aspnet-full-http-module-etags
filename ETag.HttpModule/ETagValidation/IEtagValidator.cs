using System.Web;

namespace ETag.HttpModule.ETagValidation
{
    /// <summary>
    /// Validates incoming ETag (If-None-Match header) and responds with a 304 when ETag matches
    /// 
    /// Note: compares received ETag as is, doesn't implement strong/weak validation
    /// </summary>
    internal interface IETagValidator
    {
        void Init(HttpApplication application);
    }
}
