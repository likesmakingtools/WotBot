using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WotBot
{
    internal class Bot
    {
        private static readonly UI.RECT workOffset = new UI.RECT() { Left = 474, Top = 841, Right = -644, Bottom = -26 };
        private static readonly UI.RECT foodOffset = new UI.RECT() { Left = 144, Top = 792, Right = -908, Bottom = -80 };
        private static readonly UI.RECT sleepOffset = new UI.RECT() { Left = 144, Top = 844, Right = -908, Bottom = -30 };
        private static readonly UI.RECT chestOffset = new UI.RECT() { Left = 150, Top = 240, Right = -100, Bottom = -120 };
        private static readonly UI.RECT tacoOffset = new UI.RECT() { Left = 150, Top = 400, Right = -100, Bottom = -300 };
        private static readonly UI.RECT soldOffset = new UI.RECT() { Left = 485, Top = 113, Right = -663, Bottom = -769 };
        private static readonly UI.RECT expireOffset = new UI.RECT() { Left = 516, Top = 113, Right = -631, Bottom = -769 };
        //private static readonly UI.RECT prestigeOffset = new UI.RECT() { Left = 76, Top = 46, Right = -1024, Bottom = -791 };
        private static readonly UI.RECT prestigeOffset = new UI.RECT() { Left = 76, Top = 82, Right = -1025, Bottom = -791 };
        private static readonly Point goToRoomOffset = new Point(70, 830);
        private static readonly Point goEatOffset = new Point(240, 80);
        private static readonly Point goSleepOffset = new Point(240, 135);
        private static readonly Point goToWorkOffset = new Point(850, 90);
        private static readonly Point goToGameOffset = new Point(580, 490);
        private static readonly Point goToAHOffset = new Point(510, 70);
        private static readonly Point goToAuctionsOffset = new Point(950, 200);
        private static readonly Point goToClaimAllOffset = new Point(940, 665);
        private static readonly Point goToSellOffset = new Point(730, 200);
        private static readonly Point goToListAllOffset = new Point(930, 665);
        private static readonly Point goToTownOffset = new Point(1110, 70);
        private static readonly Point goToLogoutOffset = new Point(1050, 850);
        private static readonly Point goToPrestigeOffset = new Point(990, 750);
        private static readonly Point goToLogonOffset = new Point(580, 730);
        private static readonly Point goToMapOffset = new Point(1110, 70);
        private static readonly Point goToNoobAreaOffset = new Point(250, 490);
        private static readonly string game = "WorldOfTalesworth";
        //private static readonly string filepath = @"c:\Development\WindowsApplications\WoTBot\WoTBot\bin\Debug\net6.0-windows\";
        private static readonly string filepath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly IntPtr HWND = UI.FindWindow(null, game);
        private static DateTime lastTaco = DateTime.Now;
        private static int waitMultiplier = 1;

        public static void Test(int left, int top, int x, int y, ref BitmapImage img)
        {
            UI.RECT rect = new UI.RECT();
            UI.RECT offset = new UI.RECT() { Left = left, Top = top, Right = x, Bottom = y };
            if (UI.GetWindowRect(HWND, ref rect))
            {
                //UI.SetCursorPos(rect.Left + x, rect.Top + y);
                //UI.RECT test = RectAddOffset(rect, offset);
                UI.RECT test = RectAddOffset(rect, prestigeOffset);
                img = MSToImage(TakeScreenshot(test, HWND));
            }
        }

        public static void UpdateMultiplier(int newMultiplier)
        {
            if (newMultiplier >= 1)
                waitMultiplier = newMultiplier;
        }

        public static bool CheckForFood()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT food = RectAddOffset(rect, foodOffset);

                MemoryStream ms = TakeScreenshot(food, HWND);

                bool canEat = CompareImages(ms, "WotFood10.png");
                if (canEat)
                {
                    GoEat(rect);
                    didThing = true;
                }
            }

            return didThing;
        }

        public static bool CheckForSleep()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT sleep = RectAddOffset(rect, sleepOffset);

                MemoryStream ms = TakeScreenshot(sleep, HWND);

                bool canSleep = CompareImages(ms, "WotSleep10.png");
                if (canSleep)
                {
                    GoSleep(rect);
                    didThing = true;
                }
            }

            return didThing;
        }

        public static bool CheckForWork()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT work = RectAddOffset(rect, workOffset);

                MemoryStream ms = TakeScreenshot(work, HWND);

                bool canWork = CompareImages(ms, "WotCanWork.png");
                if (canWork)
                {
                    GoWork(rect);
                    didThing |= true;
                }
            }

            return didThing;
        }

        public static bool CheckForChests()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT chest = RectAddOffset(rect, chestOffset);

                MemoryStream ms = TakeScreenshot(chest, HWND);
                string[,] image = ImageToArray(ms);
                int xIndex = 0, yIndex = 0;
                bool chestFound = CheckForPattern(image, Patterns.chestTop, Patterns.chestColors, ref xIndex, ref yIndex);
                if (!chestFound) chestFound = CheckForPattern(image, Patterns.chestLeft, Patterns.chestColors, ref xIndex, ref yIndex);
                if (chestFound)
                {
                    ClickChest(chest.Left + xIndex, chest.Top + yIndex);
                    didThing = true;
                }
            }

            return didThing;
        }

        public static bool CheckForTacos()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT taco = RectAddOffset(rect, tacoOffset);

                MemoryStream ms = TakeScreenshot(taco, HWND);
                string[,] image = ImageToArray(ms);
                int xIndex = 0, yIndex = 0;
                bool tacoFound = CheckForPattern(image, Patterns.tacoTruck, Patterns.tacoColors, ref xIndex, ref yIndex);
                if (tacoFound)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan ts = now - lastTaco;
                    if (ts.Seconds > 30)
                    {
                        lastTaco = now;
                        ClickTacoTruck(taco.Left + xIndex, taco.Top + yIndex);
                        didThing = true;
                    }
                }
            }

            return didThing;
        }

        public static bool CheckForTacos(DateTime lastTacoLocal)
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT taco = RectAddOffset(rect, tacoOffset);

                MemoryStream ms = TakeScreenshot(taco, HWND);
                string[,] image = ImageToArray(ms);
                int xIndex = 0, yIndex = 0;
                bool tacoFound = CheckForPattern(image, Patterns.tacoTruck, Patterns.tacoColors, ref xIndex, ref yIndex);
                if (tacoFound)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan ts = now - lastTacoLocal;
                    if (ts.TotalSeconds > 30)
                    {
                        lastTaco = now;
                        ClickTacoTruck(taco.Left + xIndex, taco.Top + yIndex);
                        didThing = true;
                    }
                }
            }

            return didThing;
        }

        public static bool CheckAuctions()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT sold = RectAddOffset(rect, soldOffset);
                UI.RECT expire = RectAddOffset(rect, expireOffset);

                MemoryStream msSold = TakeScreenshot(sold, HWND);
                MemoryStream msExpire = TakeScreenshot(expire, HWND);

                bool isSold = CompareImages(msSold, "WotAHSold.png");
                bool isExpire = CompareImages(msExpire, "WotAHExpire.png");

                if (isSold || isExpire)
                {
                    RefreshAuctions(rect.Left, rect.Top);
                    didThing = true;
                }
            }

            return didThing;
        }

        public static bool CheckPrestige()
        {
            bool didThing = false;

            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                UI.RECT prestige = RectAddOffset(rect, prestigeOffset);

                MemoryStream msPrestige = TakeScreenshot(prestige, HWND);

                bool isPrestige = CompareImages(msPrestige, "WotPrestige.png");

                if (isPrestige)
                {
                    GoPrestige(rect.Left, rect.Top);
                    didThing = true;
                }
            }

            return didThing;
        }

        public static void ForceEat()
        {
            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                GoEat(rect);
            }
        }

        public static void ForceSleep()
        {
            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                GoSleep(rect);
            }
        }

        public static void ForceWork()
        {
            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                GoWork(rect);
            }
        }

        public static void ForcePrestige()
        {
            UI.RECT rect = new UI.RECT();
            if (UI.GetWindowRect(HWND, ref rect))
            {
                GoPrestige(rect.Left, rect.Top);
            }
        }

        private static void GoEat(UI.RECT rect)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point room = new Point(rect.Left + goToRoomOffset.X, rect.Top + goToRoomOffset.Y);
            Point eat = new Point(rect.Left + goEatOffset.X, rect.Top + goEatOffset.Y);
            Point game = new Point(rect.Left + goToGameOffset.X, rect.Top + goToGameOffset.Y);

            UI.ClickPosition(room);
            Thread.Sleep(1500);
            UI.ClickPosition(eat);
            Thread.Sleep(1000 * waitMultiplier);
            UI.ClickPosition(game);
            Thread.Sleep(2000);

            UI.SetForegroundWindow(curWind);
        }

        private static void GoSleep(UI.RECT rect)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point room = new Point(rect.Left + goToRoomOffset.X, rect.Top + goToRoomOffset.Y);
            Point sleep = new Point(rect.Left + goSleepOffset.X, rect.Top + goSleepOffset.Y);
            Point game = new Point(rect.Left + goToGameOffset.X, rect.Top + goToGameOffset.Y);

            UI.ClickPosition(room);
            Thread.Sleep(1500);
            UI.ClickPosition(sleep);
            Thread.Sleep(1000 * waitMultiplier);
            UI.ClickPosition(game);
            Thread.Sleep(2000);

            UI.SetForegroundWindow(curWind);
        }

        private static void GoWork(UI.RECT rect)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point room = new Point(rect.Left + goToRoomOffset.X, rect.Top + goToRoomOffset.Y);
            Point work = new Point(rect.Left + goToWorkOffset.X, rect.Top + goToWorkOffset.Y);
            Point game = new Point(rect.Left + goToGameOffset.X, rect.Top + goToGameOffset.Y);

            UI.ClickPosition(room);
            Thread.Sleep(1500);
            UI.ClickPosition(work);
            Thread.Sleep(1000 * waitMultiplier);
            UI.ClickPosition(game);
            Thread.Sleep(2000);

            UI.SetForegroundWindow(curWind);
        }

        private static void ClickChest(int x, int y)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point chest = new Point(x, y);

            UI.ClickPosition(chest);

            Thread.Sleep(50);
            UI.SetForegroundWindow(curWind);
        }

        private static void ClickTacoTruck(int x, int y)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point taco = new Point(x, y);

            for (int i = 0; i < 10; i++)
            {
                UI.ClickPosition(taco);
                Thread.Sleep(150);
            }

            UI.SetForegroundWindow(curWind);
        }

        private static void RefreshAuctions(int x, int y)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point ah = new Point(x + goToAHOffset.X, y + goToAHOffset.Y);
            Point auctions = new Point(x + goToAuctionsOffset.X, y + goToAuctionsOffset.Y);
            Point claimAll = new Point(x + goToClaimAllOffset.X, y + goToClaimAllOffset.Y);
            Point sell = new Point(x + goToSellOffset.X, y + goToSellOffset.Y);
            Point listAll = new Point(x + goToListAllOffset.X, y + goToListAllOffset.Y);

            UI.ClickPosition(ah);
            Thread.Sleep(500);
            UI.ClickPosition(auctions);
            Thread.Sleep(500);
            UI.ClickPosition(claimAll);
            Thread.Sleep(500);
            UI.ClickPosition(sell);
            Thread.Sleep(500);
            UI.ClickPosition(listAll);
            Thread.Sleep(500);
            UI.ClickPosition(ah);

            Thread.Sleep(50);
            UI.SetForegroundWindow(curWind);
        }

        private static void GoPrestige(int x, int y)
        {
            IntPtr curWind = UI.GetForegroundWindow();
            UI.SetForegroundWindow(HWND);
            Thread.Sleep(50);

            Point town = new Point(x + goToTownOffset.X, y + goToTownOffset.Y);
            Point logout = new Point(x + goToLogoutOffset.X, y + goToLogoutOffset.Y);
            Point prestige = new Point(x + goToPrestigeOffset.X, y + goToPrestigeOffset.Y);
            Point logon = new Point(x + goToLogonOffset.X, y + goToLogonOffset.Y);
            Point map = new Point(x + goToMapOffset.X, y + goToMapOffset.Y);
            Point noob = new Point(x + goToNoobAreaOffset.X, y + goToNoobAreaOffset.Y);

            UI.ClickPosition(town);
            Thread.Sleep(1000 * waitMultiplier);
            UI.ClickPosition(logout);
            Thread.Sleep(500);
            UI.ClickPosition(prestige);
            Thread.Sleep(500);
            UI.ClickPosition(logon);
            Thread.Sleep(500);
            UI.ClickPosition(map);
            Thread.Sleep(500);
            UI.ClickPosition(noob);

            Thread.Sleep(50);
            UI.SetForegroundWindow(curWind);
        }

        private static UI.RECT RectAddOffset(UI.RECT source, UI.RECT offset)
        {
            UI.RECT newRect = new UI.RECT();
            newRect.Left = source.Left + offset.Left;
            newRect.Top = source.Top + offset.Top;
            newRect.Right = source.Right + offset.Right;
            newRect.Bottom = source.Bottom + offset.Bottom;

            return newRect;
        }

        private static MemoryStream TakeScreenshot(UI.RECT rect, IntPtr hWnd)
        {
            MemoryStream ms = new MemoryStream();

            Size size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (Graphics wg = Graphics.FromHwnd(hWnd))
            {
                using (Bitmap b = new Bitmap(size.Width, size.Height, wg))
                {
                    using (Graphics ig = Graphics.FromImage(b))
                    {
                        int x = rect.Left;
                        int y = rect.Top;
                        ig.CopyFromScreen(x, y, 0, 0, size);
                        b.Save(ms, ImageFormat.Png);
                        if (!File.Exists("test.png")) b.Save("test.png", ImageFormat.Png);
                    }
                }
            }

            return ms;
        }

        private static bool CompareImages(MemoryStream ms, string file)
        {
            bool isSame = false;
            byte[] pixels1 = ImageToBytes(ms);
            byte[] pixels2 = ImageToBytes(filepath + file);

            if (pixels1.Length == pixels2.Length)
            {
                int sameCount = 0;
                for (int i = 0; i < pixels1.Length; i++)
                {
                    if (pixels1[i] == pixels2[i])
                        sameCount++;
                }

                isSame = sameCount == pixels1.Length;
                if (file == "WotSleep10.png")
                {
                    isSame = sameCount > (pixels1.Length - 200);
                }
            }

            return isSame;
        }

        private static byte[] ImageToBytes(MemoryStream ms)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();

            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        private static byte[] ImageToBytes(string file)
        {
            BitmapImage img = new BitmapImage(new Uri(file));
            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        private static BitmapImage MSToImage(MemoryStream ms)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption |= BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();

            return img;
        }

        private static string[,] ImageToArray(MemoryStream ms)
        {

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();

            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            string[,] image = new string[img.PixelHeight, img.PixelWidth];

            for (int y = 0; y < img.PixelHeight; y++)
            {
                for (int x = 0; x < img.PixelWidth; x++)
                {
                    int index = y * stride + 4 * x;
                    string b = pixels[index].ToString("X2");
                    string g = pixels[index + 1].ToString("X2");
                    string r = pixels[index + 2].ToString("X2");
                    string a = pixels[index + 3].ToString("X2");
                    image[y, x] = $"{r}{g}{b}{a}";
                }
            }

            return image;
        }

        private static bool CheckForPattern(string[,] image, bool[,] pattern, List<string> colors, ref int xIndex, ref int yIndex)
        {
            bool patternFound = false;
            bool keepGoing = false;
            int iHeight = image.GetLength(0);
            int iWidth = image.GetLength(1);
            int pHeight = pattern.GetLength(0);
            int pWidth = pattern.GetLength(1);

            for (int y = 0; y < iHeight - pHeight; y++)
            {
                for (int x = 0; x < iWidth - pWidth; x++)
                {
                    if (colors.Contains(image[y, x]))
                    {
                        string color = image[y, x];
                        int py = 0;
                        int px = 0;

                        for (int j = y; j < y + pHeight; j++)
                        {
                            for (int i = x; i < x + pWidth; i++)
                            {
                                keepGoing = ((image[j, i] == color) == pattern[py, px]);
                                if (!keepGoing) { j = y + pHeight; i = x + pWidth; }
                                px++;
                            }
                            py++;
                            px = 0;
                        }

                        if (keepGoing)
                        {
                            patternFound = true;
                            yIndex = y;
                            xIndex = x;
                            y = iHeight;
                            x = iWidth;
                        }
                    }
                }
            }

            return patternFound;
        }
    }
}
