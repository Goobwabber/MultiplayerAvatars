using System;

namespace MultiplayerAvatars.Providers.Attributes
{
    public class AvatarProviderPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public AvatarProviderPriorityAttribute(
            int priority)
        {
            Priority = priority;
        }
    }
}
