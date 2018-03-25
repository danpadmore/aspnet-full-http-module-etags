using System;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
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
        private const string ETagHeader = "ETag";
        private const string IfNoneMatchHttpHeader = "If-None-Match";
        private const int NotModifiedStatusCode = (int)HttpStatusCode.NotModified;

        private static readonly ConcurrentDictionary<string, string> ETags;
        
        static ETagModule()
        {
            ETags = new ConcurrentDictionary<string, string>();
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            RecordResponseOutput(context);

            ValidateETag(context);
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            var request = context.Request;

            RespondWithETag(context.Response, request);
        }

        public void Dispose()
        {
        }

        private void RecordResponseOutput(HttpContext context)
        {
            var workaround = context.Response.Filter;   // Fixes "Response filter is not valid" bug, by getting before setting it
            context.Response.Filter = new HttpResponseRecorder(context);
        }

        private void ValidateETag(HttpContext context)
        {
            var request = context.Request;

            if (!TryGetClientETag(request, out string clientEtag))
                return;

            if (!TryGetServerETag(request, out string serverEtag))
                return;

            if (HasResourceChanged(clientEtag, serverEtag))
                return;

            AbortAndRespond(context.Response, NotModifiedStatusCode);
        }

        private bool TryGetClientETag(HttpRequest request, out string etag)
        {
            etag = request.Headers[IfNoneMatchHttpHeader];

            return !string.IsNullOrWhiteSpace(etag);
        }

        private bool TryGetServerETag(HttpRequest request, out string etag)
        {
            var resourceKey = request.Url.AbsolutePath;
            return ETags.TryGetValue(resourceKey, out etag);
        }

        private bool HasResourceChanged(string clientEtag, string serverEtag)
        {
            return !string.Equals(clientEtag, serverEtag);
        }

        private void AbortAndRespond(HttpResponse response, int statusCode)
        {
            response.Clear();
            response.StatusCode = statusCode;
            response.End();
        }

        private void RespondWithETag(HttpResponse response, HttpRequest request)
        {
            if (!TryGetServerETag(request, out string serverEtag))
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

                serverEtag = ETags.AddOrUpdate(resourceKey, serverEtag, (key, value) => serverEtag);

                return serverEtag;
            }
        }
    }
}
