using Microsoft.EntityFrameworkCore;
using Tallypath.Data;
using Tallypath.Models;

namespace Tallypath.Services
{
    public interface IUserDeviceService
    {
        Task RegisterOrUpdateAsync(Guid userId, RegisterDeviceRequest request);
        Task DeactivateAsync(Guid userId, DeactivateDeviceRequest request);
        Task DeactivateByTokenAsync(string fcmToken); // internal use
    }

    public class UserDeviceService : IUserDeviceService
    {
        private readonly AppDbContext _db;

        public UserDeviceService(AppDbContext db)
        {
            _db = db;
        }

        public async Task RegisterOrUpdateAsync(
            Guid userId,
            RegisterDeviceRequest request
        )
        {
            // 1️⃣ Deactivate same token on other users/devices (token rotation safety)
            var duplicates = await _db.UserDevices
                .Where(d => d.FcmToken == request.FcmToken &&
                            (d.UserId != userId || d.DeviceId != request.DeviceId))
                .ToListAsync();

            foreach (var d in duplicates)
                d.IsActive = false;

            // 2️⃣ Find existing device
            var device = await _db.UserDevices
                .FirstOrDefaultAsync(d =>
                    d.UserId == userId &&
                    d.DeviceId == request.DeviceId
                );

            if (device == null)
            {
                device = new UserDevice
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DeviceId = request.DeviceId,
                    Platform = request.Platform,
                    FcmToken = request.FcmToken,
                    IsActive = true,
                    LastSeenAt = DateTime.UtcNow
                };

                _db.UserDevices.Add(device);
            }
            else
            {
                device.Platform = request.Platform;
                device.FcmToken = request.FcmToken;
                device.IsActive = true;
                device.LastSeenAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Guid userId, DeactivateDeviceRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceId) &&
                string.IsNullOrEmpty(request.FcmToken))
            {
                throw new ArgumentException(
                    "Either DeviceId or FcmToken must be provided."
                );
            }

            var query = _db.UserDevices
                .Where(d => d.UserId == userId && d.IsActive);

            if (!string.IsNullOrEmpty(request.DeviceId))
                query = query.Where(d => d.DeviceId == request.DeviceId);

            if (!string.IsNullOrEmpty(request.FcmToken))
                query = query.Where(d => d.FcmToken == request.FcmToken);

            var devices = await query.ToListAsync();

            foreach (var device in devices)
            {
                device.IsActive = false;
                device.LastSeenAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        // 🔒 Internal use: called when FCM reports invalid token
        public async Task DeactivateByTokenAsync(string fcmToken)
        {
            var devices = await _db.UserDevices
                .Where(d => d.FcmToken == fcmToken && d.IsActive)
                .ToListAsync();

            foreach (var device in devices)
                device.IsActive = false;

            await _db.SaveChangesAsync();
        }
    }

}