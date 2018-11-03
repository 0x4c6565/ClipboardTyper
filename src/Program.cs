using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClipboardTyper.ConsoleHotKey;

namespace ClipboardTyper
{
    class Program
    {
        private static int DEFAULT_KEY_DELAY_MS = 50;
        private static char[] SPECIAL_CHARS = { '{', '}', '(', ')', '+', '^', '%', '~' };


        [STAThread]
        public static void Main(string[] args)
        {
            if (args?.Length > 0 && int.TryParse(args[0], out int parsedDelay))
            {
                DEFAULT_KEY_DELAY_MS = parsedDelay;
            }

            Console.WriteLine("[+] Starting ClipboardTyper with delay [{0}ms]", DEFAULT_KEY_DELAY_MS);

            HotKeyManager.RegisterHotKey(Keys.P, KeyModifiers.Control | KeyModifiers.Alt | KeyModifiers.Shift);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);

            Console.ReadKey(true);
        }

        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(DoWork));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }

        static void DoWork()
        {
            if (!Clipboard.ContainsText())
            {
                Console.WriteLine("[-] Text not found on clipboard");
                return;
            }

            Console.WriteLine("[*] Sleeping for 500ms");
            Thread.Sleep(500);

            Console.WriteLine("[+] Typing text");
            string chars = Clipboard.GetText();
            for (int i = 0; i < chars.Count(); i++)
            {
                if (i > 0)
                {
                    Thread.Sleep(DEFAULT_KEY_DELAY_MS);
                }

                char c = chars[i];
                if (SPECIAL_CHARS.Contains(c))
                {
                    SendKeys.SendWait("{" + c.ToString() + "}");
                }
                else
                {
                    SendKeys.SendWait(c.ToString());
                }
            }
            Console.WriteLine("[*] Finished typing text");
        }
    }
}
