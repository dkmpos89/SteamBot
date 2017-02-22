using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.IO;
using System.Threading;



namespace SteamChatBot

{
    class Program

    {

        static String user, pass;
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static SteamFriends steamFriends;
        static bool isRunning = false;
        static string authcode;
        static void Main(string[] args)

        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "Epiphic's Steam Tool";
            Console.WriteLine("Control + C = closes console");
            System.Threading.Thread.Sleep(500);
            Console.Clear();
            Console.WriteLine("Welcome to TheRealOne's Steam tool");
            Console.WriteLine("█████████████████████████████████████");
            Console.WriteLine("██████████████████████████████▀▀▀▀███");
            Console.WriteLine("████████████████████████████▀─▄▀▀▄─▀█");
            Console.WriteLine("██▀─▄▄─▀████████████████████─█────█─█");
            Console.WriteLine("█─▄▀──▀─▀███████████████████─▀▄──▄▀─█");
            Console.WriteLine("█─█───────▀█████████████████▄──▀▀───█");
            Console.WriteLine("█▄─▀▄▄▀─────▀███████████████▀─────▄██");
            Console.WriteLine("███▄▄▄▄█▄─────▀████████████▀─────▄███");
            Console.WriteLine("██████████▄─────▀█████████▀─────▄████");
            Console.WriteLine("████████████▄─────▀██████▀─────▄█████");
            Console.WriteLine("██████████████▄─────▀▀──▀─────▄██████");
            Console.WriteLine("████████████████▄──────▀▀▄───▄███████");
            Console.WriteLine("██████████████████▄───────█─▄████████");
            Console.WriteLine("██████████████████▄───────█─▄████████");
            Console.WriteLine("█████████████████████▄─▀▀─▄██████████");
            Console.WriteLine("█████████████████████████████████████");
            Console.WriteLine("█████████████████████████████████████");
            Console.WriteLine("█████████████████████████████████████");
            Console.WriteLine("█░░░░░█░░░░░░█░░░░░███░░░███░░████░░█");
            Console.WriteLine("█░░██████░░███░░██████░░░███░░░██░░░█");
            Console.WriteLine("█░░░░░███░░███░░░░░██░░█░░██░░░░░░░░█");
            Console.WriteLine("████░░███░░███░░█████░░░░░██░░█░░█░░█");
            Console.WriteLine("█░░░░░███░░███░░░░░█░░░█░░░█░░█░░█░░█");
            Console.WriteLine("█████████████████████████████████████");


            Console.Title = "SteamBot";
            Console.Write("Username: ");
            user = Console.ReadLine();
            Console.Write("Password: ");
            pass = Console.ReadLine();

            SteamLogin();

        }

        static void SteamLogin()

        {

            steamClient = new SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();

            new Callback<SteamClient.ConnectedCallback>(OnConnected, manager);
            new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, manager);
            new Callback<SteamClient.DisconnectedCallback>(OnDisconnected, manager);
            new Callback<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth, manager);
            //new Callback<SteamUser.AccountInfoCallback>(OnAccount, manager);
            new Callback<SteamFriends.FriendMsgCallback>(OnChatMessage, manager);

            isRunning = true;
            Console.WriteLine("Connecting to steam...");
            steamClient.Connect();
            while (isRunning)

            {

                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));

            }

            Console.ReadKey();

        }

        static void OnConnected(SteamClient.ConnectedCallback callback)

        {

            if (callback.Result != EResult.OK)

            {

                Console.WriteLine("Unable to connect with steam: {0}", callback.Result);

                isRunning = false;
                return;

            }

            Console.WriteLine("Connected to Steam  network \n Loggin in {0}.......", user);

            byte[] sentryHash = null;
            if (File.Exists("sentry.bin"))

            {

                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);

            }


            steamUser.LogOn(new SteamUser.LogOnDetails
            {

                Username = user,
                Password = pass,
                AuthCode = authcode,
                SentryFileHash = sentryHash,

            });

        }

        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)

        {
            if (callback.Result == EResult.AccountLogonDenied)

            {

                Console.WriteLine("Account is steam guard Protected.");
                Console.Write("please write auth code from email at {0}: ", callback.EmailDomain);
                authcode = Console.ReadLine();

                return;

            }

            if (callback.Result != EResult.OK)

            {

                Console.WriteLine("Unable to connect to Steam account: {0}", callback.Result);
                isRunning = false;
                return;

            }

            Console.WriteLine("Succesfully logged in: {0}", callback.Result);

        }

        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)

        {
            Console.WriteLine("updating sentry file...");

            byte[] sentryHash = CryptoHelper.SHAHash(callback.Data);
            File.WriteAllBytes("sentry.bin", callback.Data);
            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails

            {
                JobID = callback.JobID,
                FileName = callback.FileName,
                BytesWritten = callback.BytesToWrite,
                FileSize = callback.Data.Length,
                Offset = callback.Offset,
                Result = EResult.OK,
                LastError = 0,
                OneTimePassword = callback.OneTimePassword,
                SentryFileHash = sentryHash,

            });

            Console.WriteLine("Done. ");
        }

        static void OnDisconnected(SteamClient.DisconnectedCallback callback)

        {

            Console.WriteLine("\n{0} disconnected from Steam, reconnecting in 5... \n", user);
            Thread.Sleep(TimeSpan.FromSeconds(5));

            steamClient.Connect();

        }

        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)

        {

            steamFriends.SetPersonaState(EPersonaState.Online);

        }

        static void OnChatMessage(SteamFriends.FriendMsgCallback callback)

        {


            steamFriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Hello");

        }

    }


}