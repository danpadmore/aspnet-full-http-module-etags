using System;
using System.Net;
using System.Web;

namespace ETag.HttpModule.ETagValidation
{
    internal class ETagValidator : IETagValidator
    {
        private const string IfNoneMatchHttpHeader = "If-None-Match";
        private const int NotModifiedStatusCode = (int)HttpStatusCode.NotModified;

        private readonly ETagStore ETagStore;

        public ETagValidator()
        {
            ETagStore = new ETagStore();
        }

        public void Init(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));

            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            ValidateETag(context);
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
        }

        private void ValidateETag(HttpContext context)
        {
            var request = context.Request;

            if (!TryGetClientETag(request, out string clientEtag))
                return;

            if (!ETagStore.TryGet(request.Url.AbsolutePath, out string serverEtag))
                return;

            if (HasResourceChanged(clientEtag, serverEtag))
                return;

            RespondNotModified(context.Response);
        }

        private bool TryGetClientETag(HttpRequest request, out string etag)
        {
            etag = request.Headers[IfNoneMatchHttpHeader];

            return !string.IsNullOrWhiteSpace(etag);
        }

        private bool HasResourceChanged(string clientEtag, string serverEtag)
        {
            return !string.Equals(clientEtag, serverEtag);
        }

        private void RespondNotModified(HttpResponse response)
        {
            response.Clear();
            response.StatusCode = NotModifiedStatusCode;
            response.End();
        }
    }
}
