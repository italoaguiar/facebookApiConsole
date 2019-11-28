using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public WebAuthenticationBroker(string appId, Uri redirectUri, string scope = null)
        {
            this.appId = appId;
            this.redirectUri = redirectUri;
            this.scope = scope;

            NativeMethods.SuppressCookiePersistence();

            browser = new ChromiumWebBrowser();            
            window = new Window();
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.Width = 600;
            window.Height = 650;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            window.Content = browser;


            browser.FrameLoadStart += this.Browser_FrameLoadStart;
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            
            

            //browser.Navigating += Browser_Navigating;
            //browser.Navigated += Browser_Navigated;
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            window.Dispatcher.Invoke(() =>
            {
                var k = browser.Address;
                System.Diagnostics.Debug.WriteLine(k);

                if (k.StartsWith(redirectUri.AbsoluteUri))
                {
                    try
                    {
                        string url = (new Uri(k)).Fragment.Replace("#", "?");
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
            });
        }

        private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            window.Dispatcher.Invoke(() =>
            {
                var k = browser.Address;
                System.Diagnostics.Debug.WriteLine(k);
            });
        }





        ChromiumWebBrowser browser;
        Window window;
        Uri redirectUri;
        string appId;
        string scope;
        AccessToken token;



        public async Task<AccessToken> AuthenticateAsync()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");

            string req_scope = "";
            if (!string.IsNullOrEmpty(scope))
            {
                req_scope = "&scope=" + scope;
            }
            var url = new Uri($"https://www.facebook.com/v5.0/dialog/oauth?client_id={appId}&redirect_uri={redirectUri}&response_type=token&state={Guid.NewGuid()}&display=popup{req_scope}");
            
            //browser.Navigate(url);
            await Task.Delay(50);

            browser.Address = url.AbsoluteUri;

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
