using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace ETag.HttpModule
{
    /// <summary>
    /// Records data written to response, so it can be inspected after the response stream has closed
    /// </summary>
    public class HttpResponseRecorder : MemoryStream
    {
        private readonly HttpContext context;
        private readonly List<byte> output;

        public byte[] Output
        {
            get => output.ToArray();
        }

        public HttpResponseRecorder(HttpContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            output = new List<byte>();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            output.AddRange(buffer);

            var response = Encoding.UTF8.GetString(buffer);
            context.Response.Write(response.ToCharArray(), 0, response.Length);
        }
    }
}
