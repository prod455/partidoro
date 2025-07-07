namespace Partidoro.Application.Helpers
{
    public static class MethodHelper
    {
        public static void Retry(Action action, int maxRetries = 3)
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
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
