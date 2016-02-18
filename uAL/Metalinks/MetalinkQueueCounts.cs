using System.Collections.Generic;
using System.Linq;

namespace uAL.Metalinks
{
    public struct MetalinkQueueCounts
    {
        public int Total;
        public int Ready;
        public int Error;
        public int Success;

        public MetalinkQueueCounts(ICollection<MetalinkQueueItem> queue, string fileName)
        {
            var currentQueue = queue.Where(queueItem => queueItem.Metalink.FullName == fileName);
            Total = currentQueue.Count();
            Ready = currentQueue.Count(queueItem => queueItem.Status.IsInProgress);
            Error = currentQueue.Count(queueItem => queueItem.Status.IsError);
            Success = currentQueue.Count(queueItem => queueItem.Status.IsSuccess);
        }
    }
}
