namespace Loupedeck.NotifyBuzzPlugin
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.UI.Notifications;
    using Windows.UI.Notifications.Management;

    public class NotifyBuzzPlugin : Plugin
    {
        private const string EventNotification = "notificationReceived";
        private const string EventUrgent = "notificationUrgent";

        private UserNotificationListener _listener;
        private Timer _pollTimer;
        private uint _lastNotificationId = 0;
        private bool _isFirstPoll = true;

        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        public NotifyBuzzPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            this.PluginEvents.AddEvent(
                EventNotification,
                "Notification Received",
                "Fires when a Windows toast notification arrives"
            );

            this.PluginEvents.AddEvent(
                EventUrgent,
                "Urgent Notification",
                "Fires for high-priority or alarm notifications"
            );

            _ = this.InitializeListenerAsync();
        }

        private async Task InitializeListenerAsync()
        {
            try
            {
                _listener = UserNotificationListener.Current;
                var accessStatus = await _listener.RequestAccessAsync();

                if (accessStatus != UserNotificationListenerAccessStatus.Allowed)
                {
                    PluginLog.Warning("NotifyBuzz: Notification access denied. Grant access in Windows Settings > System > Notifications.");
                    return;
                }

                PluginLog.Info("NotifyBuzz: Notification listener initialized.");

                _pollTimer = new Timer(
                    PollNotifications,
                    null,
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(2)
                );
            }
            catch (Exception ex)
            {
                PluginLog.Error($"NotifyBuzz init error: {ex.Message}");
            }
        }

        private async void PollNotifications(object state)
        {
            try
            {
                if (_listener == null) return;

                var notifications = await _listener.GetNotificationsAsync(
                    NotificationKinds.Toast
                );

                if (_isFirstPoll)
                {
                    _lastNotificationId = notifications.Any()
                        ? notifications.Max(n => n.Id)
                        : 0;
                    _isFirstPoll = false;
                    return;
                }

                var newNotifications = notifications
                    .Where(n => n.Id > _lastNotificationId)
                    .OrderBy(n => n.Id)
                    .ToList();

                foreach (var notification in newNotifications)
                {
                    bool isUrgent = false;
                    try
                    {
                        var toastBinding = notification.Notification?.Visual?
                            .GetBinding(KnownNotificationBindings.ToastGeneric);
                        var hints = toastBinding?.Hints;
                        if (hints != null && hints.ContainsKey("scenario"))
                        {
                            var scenario = hints["scenario"]?.ToString();
                            isUrgent = scenario == "alarm" || scenario == "urgent"
                                || scenario == "incomingCall";
                        }
                    }
                    catch
                    {
                    }

                    if (isUrgent)
                    {
                        this.PluginEvents.RaiseEvent(EventUrgent);
                    }
                    else
                    {
                        this.PluginEvents.RaiseEvent(EventNotification);
                    }

                    _lastNotificationId = notification.Id;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"NotifyBuzz poll error: {ex.Message}");
            }
        }

        public override void Unload()
        {
            _pollTimer?.Dispose();
            _pollTimer = null;
        }
    }
}
