using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;

namespace Tallypath.Services
{
    public class FirebaseService
    {
        public static void Initialize()
        {
            if (FirebaseApp.DefaultInstance != null) return;

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("tallypath-996f1-firebase-adminsdk-fbsvc-9996057ef9.json")
            });
        }
    }

}