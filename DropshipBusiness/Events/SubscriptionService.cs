using System.Collections.Generic;


namespace DropshipBusiness.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            //return .Current.ResolveAll<IConsumer<T>>();
            return DropshipCommon.Infrastructure.DropshipWebContext.Instance.ResolveAll<IConsumer<T>>();
        }
    }
}
