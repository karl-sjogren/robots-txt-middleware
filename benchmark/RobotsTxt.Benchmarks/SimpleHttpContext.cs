using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace RobotsTxt.Benchmarks;

// 2021-09-09 KEEP KJ
// To run the benchmarks we need a HttpContext. Constructing a new
// DefaultHttpContext is quite expensive so by faking it with these
// classes lowers the benchmark time with about 50% (or 400 μs).
public class SimpleHttpContext : HttpContext {
    private readonly HttpRequest _request;
    private readonly HttpResponse _response;

    public SimpleHttpContext() {
        _request = new SimpleHttpRequest();
        _response = new SimpleHttpResponse();
    }

    public override IFeatureCollection Features => throw new NotImplementedException();

    public override HttpRequest Request => _request;

    public override HttpResponse Response => _response;

    public override ConnectionInfo Connection => throw new NotImplementedException();

    public override WebSocketManager WebSockets => throw new NotImplementedException();

    public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override CancellationToken RequestAborted { get => CancellationToken.None; set => throw new NotImplementedException(); }
    public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Abort() {
        throw new NotImplementedException();
    }
}

public class SimpleHttpRequest : HttpRequest {
    public override HttpContext HttpContext => throw new NotImplementedException();

    public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override PathString Path { get => new("/robots.txt"); set => throw new NotImplementedException(); }
    public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override IHeaderDictionary Headers => throw new NotImplementedException();

    public override IRequestCookieCollection Cookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override bool HasFormContentType => throw new NotImplementedException();

    public override IFormCollection Form { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}

public class SimpleHttpResponse : HttpResponse {
    private readonly Stream _body = new MemoryStream();
    private string _contentType = string.Empty;

    public SimpleHttpResponse() {
    }

    public override HttpContext HttpContext => throw new NotImplementedException();

    public override int StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override IHeaderDictionary Headers => new HeaderDictionary();

    public override Stream Body { get => _body; set => throw new NotImplementedException(); }
    public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string ContentType { get => _contentType; set => _contentType = value; }

    public override IResponseCookies Cookies => throw new NotImplementedException();

    public override bool HasStarted => throw new NotImplementedException();

    public override void OnCompleted(Func<object, Task> callback, object state) {
    }

    public override void OnStarting(Func<object, Task> callback, object state) {
    }

    public override void Redirect(string location, bool permanent) {
    }
}
