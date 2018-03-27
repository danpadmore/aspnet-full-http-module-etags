using ETag.HttpModule.ETagResponse;
using ETag.HttpModule.ETagValidation;
using System.Web;

namespace ETag.HttpModule
{
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
