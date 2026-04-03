namespace Loupedeck.NotifyBuzzPlugin
{
    using System;

    public class TestHapticCommand : PluginDynamicCommand
    {
        public TestHapticCommand()
            : base("Test Haptic", "Manually trigger notification haptic", "NotifyBuzz")
        {
        }

        protected override void RunCommand(string actionParameter)
        {
            this.Plugin.PluginEvents.RaiseEvent("notificationReceived");
        }
    }
}
