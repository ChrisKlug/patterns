using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new HttpClientTestingHandler();
            var client = new HttpClient(handler);
            var circuitBreaker = new HttpCircuitBreaker(2, 3);

            Console.WriteLine(".: Making Requests With Internal Server Error :.\n");
            handler.FailWith(HttpStatusCode.InternalServerError);
            for (int i = 0; i < 6; i++)
            {
                RequestHelper.MakeRequest(client, circuitBreaker);
            }

            Console.WriteLine("\n\n.: Making Requests Without Errors :. \n");
            handler.Succeed();
            for (int i = 0; i < 5; i++)
            {
                RequestHelper.MakeRequest(client, circuitBreaker);
            }

            Console.Write("\nDone!");
            Console.ReadKey();
        }
    }

    class HttpCircuitBreaker
    {
        private readonly int _maxFailures;
        private readonly int _retryCount;
        private int _failureCount = 0;

        public HttpCircuitBreaker(int maxFailures, int retryCount)
        {
            _maxFailures = maxFailures;
            _retryCount = retryCount;
        }

        public async Task<HttpResponseMessage> Execute(Func<Task<HttpResponseMessage>> action)
        {
            if (CurrentState == State.Open)
            {
                FailureCount++;
                return null;
            }

            return await MakeRequest(action);
            
        }

        private async Task<HttpResponseMessage> MakeRequest(Func<Task<HttpResponseMessage>> action)
        {
            HttpResponseMessage response;
            try
            {
                response = await action();
                FailureCount = response.IsSuccessStatusCode ? 0 : FailureCount + 1;
                return response;
            }
            catch
            {
                Console.WriteLine("Failure - Exception");
                FailureCount++;
                throw;
            }
        }

        public State CurrentState { get; private set; }
        private int FailureCount
        {
            get { return _failureCount; }
            set
            {
                _failureCount = value;
                
                if (CurrentState == State.Open && (_failureCount - _maxFailures) % _retryCount == 0)
                {
                    CurrentState = State.TryingToClose;
                }
                else if (_failureCount >= _maxFailures)
                {
                    CurrentState = State.Open;
                }
                else
                {
                    CurrentState = State.Closed;
                }
            }
        }

        public enum State
        {
            Closed,
            Open,
            TryingToClose
        }
    }
}
