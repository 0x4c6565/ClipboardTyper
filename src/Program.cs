using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClipboardTyper.ConsoleHotKey;
using CommandLine;
using CommandLine.Text;

namespace ClipboardTyper
{
    class Program
    {
        private static int KEY_INTERVAL_MS = 50;
        private static int KEY_DELAY_MS = 500;
        private static char[] SPECIAL_CHARS = { '{', '}', '(', ')', '+', '^', '%', '~' };

        public class Options
        {
            [Option("interval", Required = false, HelpText = "Sets interval between key presses")]
            public int Interval { get; set; }

            [Option("delay", Required = false, HelpText = "Sets delay before inital key press")]
            public int Delay { get; set; }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.Interval > 0)
                    {
                        KEY_INTERVAL_MS = o.Interval;
                    }
                    if (o.Delay > 0)
                    {
                        KEY_DELAY_MS = o.Delay;
                    }
                });

            Console.WriteLine("[+] Starting ClipboardTyper with interval [{0}ms] and delay [{1}ms]", KEY_INTERVAL_MS, KEY_DELAY_MS);

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

            if (KEY_DELAY_MS > 0)
            {
                Console.WriteLine("[*] Delaying typing for [{0}]ms", KEY_DELAY_MS);
                Thread.Sleep(KEY_DELAY_MS);
            }

            Console.WriteLine("[+] Typing text");
            string chars = Clipboard.GetText();
            for (int i = 0; i < chars.Count(); i++)
            {
                if (i > 0)
                {
                    Thread.Sleep(KEY_INTERVAL_MS);
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
