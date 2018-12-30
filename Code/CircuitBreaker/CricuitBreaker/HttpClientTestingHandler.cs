using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CricuitBreaker
{
    class HttpClientTestingHandler : HttpMessageHandler
    {
        HttpStatusCode? _statusCode;
        Exception _exception;
        private readonly TimeSpan _delay;

        public HttpClientTestingHandler() : this(TimeSpan.FromSeconds(1))
        {

        }
        public HttpClientTestingHandler(TimeSpan delay)
        {
            _delay = delay;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_exception == null && !_statusCode.HasValue)
            {
                return Task.Delay(_delay).ContinueWith(x => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Hello world") });
            }


            return Task.Delay(_delay).ContinueWith(x =>
            {
                if (_exception != null)
                {
                    throw _exception;
                }

                return new HttpResponseMessage(_statusCode.Value);
            });
        }

        public void FailWith(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            _exception = null;
        }
        public void FailWith(Exception ex)
        {
            _exception = ex;
            _statusCode = null;
        }
        public void Succeed()
        {
            _statusCode = null;
            _exception = null;
        }
    }
}
