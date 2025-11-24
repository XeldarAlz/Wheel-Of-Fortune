namespace WheelOfFortune.Infrastructure.DependencyInjection
{
    public static class ServiceResolver
    {
        public static T Resolve<T>(ref T cachedService) where T : class
        {
            if (cachedService == null)
            {
                cachedService = ServiceLocator.Instance.Get<T>();
            }

            return cachedService;
        }
    }
}