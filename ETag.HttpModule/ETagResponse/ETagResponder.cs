using System;
using System.Security.Cryptography;
using System.Web;

namespace ETag.HttpModule.ETagResponse
{
    internal class ETagResponder : IETagResponder
    {
        private const string ETagHeader = "ETag";
        private readonly ETagStore ETagStore;

        public ETagResponder()
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

            var resourceKey = context.Request.Url.AbsolutePath;

            if (ETagStore.Exists(resourceKey))
                return;

            RecordResponseOutput(context);
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            RespondWithETag(context.Response, context.Request);
        }

        private void RecordResponseOutput(HttpContext context)
        {
            var workaround = context.Response.Filter;   // Fixes "Response filter is not valid" bug, by getting before setting it

            context.Response.Filter = new HttpResponseRecorder(context);
        }

        private void RespondWithETag(HttpResponse response, HttpRequest request)
        {
            var resourceKey = request.Url.AbsolutePath;

            if (!ETagStore.TryGet(resourceKey, out string serverEtag))
            {
                var httpResponseRecorder = response.Filter as HttpResponseRecorder;
                serverEtag = GenerateETag(request.Url.AbsolutePath, httpResponseRecorder);
            }

            response.Headers[ETagHeader] = serverEtag;
        }

        private string GenerateETag(string resourceKey, HttpResponseRecorder httpResponseRecorder)
        {
            using (var sha512 = new SHA512Managed())
            {
                var output = httpResponseRecorder.Output;
                var hash = sha512.ComputeHash(output);

                var serverEtag = Convert.ToBase64String(hash);

                serverEtag = ETagStore.AddOrUpdate(resourceKey, serverEtag);

                return serverEtag;
            }
        }
    }
}
