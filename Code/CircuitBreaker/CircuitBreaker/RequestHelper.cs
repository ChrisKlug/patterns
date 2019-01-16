using System;
using System.Net.Http;

namespace CircuitBreaker
{
    static class RequestHelper
    {
        public static void MakeRequest(HttpClient client, HttpCircuitBreaker circuitBreaker)
        {
            Console.Write("Making request...");
            var currentState = circuitBreaker.CurrentState;
            var task = circuitBreaker.Execute(() => client.GetAsync("http://www.dummy.com/"));
            try
            {
                task.Wait();
            }
            catch (Exception)
            {
                Console.WriteLine("Failure - Exception");
            }

            if (task.Result == null)
            {
                Console.WriteLine("Short circuited");
            }
            else if (!task.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Failure - StatusCode");
            }
            else
            {
                Console.WriteLine("Success!");
            }

            if (currentState != circuitBreaker.CurrentState)
            {
                Console.WriteLine("--- Changing circuit breaker state to: " + circuitBreaker.CurrentState + " ---");
            }
        }
    }
}
