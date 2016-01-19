namespace Torrent.Infrastructure.ContextHandlers
{
    public static class ContextHandlerTypeExtensions
    {
        public static bool IsEnabler(this ContextHandlerType type)
            => type == ContextHandlerType.ImmediateEnabler || type == ContextHandlerType.DeferredEnabler;

        public static bool IsDisabler(this ContextHandlerType type)
            => !type.IsEnabler();
        public static bool IsImmediate(this ContextHandlerType type)
            => type == ContextHandlerType.ImmediateEnabler || type == ContextHandlerType.ImmediateDisabler;

        public static bool IsDeferred(this ContextHandlerType type)
            => !type.IsImmediate();        
    }
}