namespace Loupedeck.NotifyBuzzPlugin
{
    using System;

    public class NotifyBuzzApplication : ClientApplication
    {
        public NotifyBuzzApplication()
        {
        }

        protected override String GetProcessName() => "";
        protected override String GetBundleName() => "";
        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
    }
}
