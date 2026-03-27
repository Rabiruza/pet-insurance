using RestSharp;

namespace PetInsurance.Tests.Core
{
    internal static class RetryHelper
    {

        public static async Task<RestResponse> ExecuteWithRetryAsync(
            RestClient client,
            RestRequest request,
            int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var response = await client.ExecuteAsync(request);

                // Якщо успіх або клієнтська помилка (4xx) - повертаємо одразу
                if (response.IsSuccessStatusCode ||
                    (int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    return response;
                }

                // Якщо серверна помилка (5xx) або таймаут - чекаємо і пробуємо знову
                TestContext.Out.WriteLine($"Attempt {i + 1} failed, retrying...");
                await Task.Delay(1000 * (i + 1)); // Експоненційна затримка
            }

            return await client.ExecuteAsync(request); // Остання спроба
        }
    }
}