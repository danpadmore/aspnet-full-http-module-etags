using ETag.HttpModule.ETagResponse;
using ETag.HttpModule.ETagValidation;
using System.Web;

namespace ETag.HttpModule
{
    /// <summary>
    /// Adds ETag to all responses
    /// Aborts request with a 304 when If-None-Match matches ETag of requested resource
    /// 
    /// Note: compares received ETag as is, doesn't implement strong/weak validation
    /// 
    /// Based on https://en.wikipedia.org/wiki/HTTP_ETag
    /// </summary>
    public class ETagModule : IHttpModule
    {
        private readonly IETagResponder ETagResponder;
        private readonly IETagValidator ETagValidator;

        public ETagModule()
        {
            ETagResponder = new ETagResponder();
            ETagValidator = new ETagValidator();
        }

        public void Init(HttpApplication application)
        {
            ETagResponder.Init(application);
            ETagValidator.Init(application);
        }

        public void Dispose()
        {
        }
    }
}
