using FirebaseAdmin.Messaging;

namespace Tallypath.Services
{
    public class PushNotificationService
    {
        public async Task SendToDevice(
            string fcmToken,
            string title,
            string body,
            Dictionary<string, string>? data = null
        )
        {
            var message = new Message
            {
                Token = fcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        public async Task SendToMultipleDevice(
            List<string> tokens,
            string title,
            string body,
            Dictionary<string, string>? data = null
        )
        {
            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
        }
    }

}
