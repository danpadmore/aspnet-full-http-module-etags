# aspnet-full-http-module-etags
Testing an approach to handle ETags in ASP.NET classic

In summary:
* ETag.HttpModule contains logic that implements naive ETag protocol
* ETag.Example uses the HttpModule (registered in web.config) and allows you to test drive it
  * Click the "Get dummy data"
  * After the first request the ETag header is received
  * Subsequent requests pass the ETag in the If-None-Match header, resulting in the dummy data being served from cache
  * Edit the received ETag in the input box, to demonstrate an ETag miss resulting in the resource being returned from the server
 
