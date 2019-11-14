using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;


namespace FacebookIDE
{
    public class WebAuthenticationBroker
    {
        public WebAuthenticationBroker(string appId, Uri redirectUri)
        {
            this.appId = appId;
            this.redirectUri = redirectUri;

            NativeMethods.SuppressCookiePersistence();

            browser = new WebBrowser();            
            window = new Window();
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.Width = 450;
            window.Height = 550;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            window.Content = browser;

            browser.Navigating += Browser_Navigating;
        }


        private void Browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri.AbsoluteUri.StartsWith(redirectUri.AbsoluteUri))
            {
                try
                {
                    string url = e.Uri.Fragment.Replace("#", "?");
                    var r = HttpUtility.ParseQueryString(url);


                    token = new AccessToken()
                    {
                        Token = r["access_token"],
                        ExpireIn = int.Parse(r["expires_in"]),
                        State = r["state"]
                    };

                    window.DialogResult = true;
                }
                catch
                {
                    window.DialogResult = false;
                }
            }
        }


        WebBrowser browser;
        Window window;
        Uri redirectUri;
        string appId;
        AccessToken token;



        public async Task<AccessToken> AuthenticateAsync()
        {
            browser.Navigate(new Uri($"https://www.facebook.com/v5.0/dialog/oauth?client_id={appId}&redirect_uri={redirectUri}&response_type=token&state={Guid.NewGuid()}&display=popup"));
            await Task.Delay(50);
            if(window.ShowDialog() == true)
            {
                return token;
            }
            else
            {
                throw new AuthenticationException();
            }
        }

    }

    public class AuthenticationException : Exception
    {

    }

    public class AccessToken
    {
        public string Token { get; set; }
        public int ExpireIn { get; set; }
        public string State { get; set; }
    }

    public static partial class NativeMethods
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private const int INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81;
        private const int INTERNET_SUPPRESS_COOKIE_PERSIST = 3;

        public static void SuppressCookiePersistence()
        {
            var lpBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            Marshal.StructureToPtr(INTERNET_SUPPRESS_COOKIE_PERSIST, lpBuffer, true);

            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SUPPRESS_BEHAVIOR, lpBuffer, sizeof(int));

            Marshal.FreeCoTaskMem(lpBuffer);
        }
    }
}
