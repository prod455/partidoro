namespace Partidoro.Application.Helpers
{
    public static class MethodHelper
    {
        public static void Retry(Action action, int maxRetries = 5)
        {
            for (int retry = 1; retry <= maxRetries; retry++)
            {
                try
                {
                    action();
                    return;
                }
                catch
                {
                    if (retry == maxRetries)
                        throw;
                    Thread.Sleep(maxRetries * 1000);
                }
            }
        }
    }
}
