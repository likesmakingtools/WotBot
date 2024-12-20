using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WotBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string info = String.Empty;
        private string state = "not started";
        private int _waitMultiplier = 1;
        private bool _running = false;
        private bool _gamingMode = false;
        private bool _eat = false;
        private bool _sleep = false;
        private bool _work = false;
        private bool _chests = false;
        private bool _tacoTruck = false;
        private bool _auctions = false;
        private bool _prestige = false;
        private readonly object _lock = new object();
        private readonly int eatMinutes = 71;
        private readonly int sleepMinutes = 240;
        private readonly int workMinutes = 151;

        private DateTime lastEat = DateTime.Now;
        private DateTime lastSleep = DateTime.Now;
        private DateTime lastWork = DateTime.Now;
        private DateTime lastChests = DateTime.Now;
        private DateTime lastTacoTruck = DateTime.Now;
        private DateTime lastAuctions = DateTime.Now;
        private DateTime lastPrestige = DateTime.Now;

        public bool isRunning { get { return _running; } }
        public bool isGamingMode { get { return _gamingMode; } }
        public bool isEat { get { return _eat; } }
        public bool isSleep { get { return _sleep; } }
        public bool isWork { get { return _work; } }
        public bool isChests { get { return _chests; } }
        public bool isTacoTruck { get { return _tacoTruck; } }
        public bool isAuctions { get { return _auctions; } }
        public bool isPrestige { get { return _prestige; } }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateMultiplier_Click(object sender, RoutedEventArgs e)
        {
            string multiplier = tbMulti.Text;
            int newMultiplier;
            if (Int32.TryParse(multiplier, out newMultiplier))
            {
                _waitMultiplier = (newMultiplier >= 1) ? newMultiplier : _waitMultiplier;
            }

            Bot.UpdateMultiplier(_waitMultiplier);
            Update_Display();
        }

        private void ForceEat_Click(object sender, RoutedEventArgs e)
        {
            Bot.ForceEat();
            lastEat = DateTime.Now;
            Update_Display();
        }

        private void ForceSleep_Click(object sender, RoutedEventArgs e)
        {
            Bot.ForceSleep();
            lastSleep = DateTime.Now;
            Update_Display();
        }

        private void ForceWork_Click(object sender, RoutedEventArgs e)
        {
            Bot.ForceWork();
            lastWork = DateTime.Now;
            Update_Display();
        }

        private void ForcePrestige_Click(object sender, RoutedEventArgs e)
        {
            Bot.ForcePrestige();
            lastPrestige = DateTime.Now;
            Update_Display();
        }

        private void IdleWorkThread()
        {
            bool didEat = false, didSleep = false, didWork = false, didChests = false, didTacoTruck = false, didAuctions = false, didPrestige = false;
            state = "running";
            Dispatcher.Invoke(Update_Display);

            while (true)
            {
                if (!isRunning) break;

                if (isEat) didEat = Bot.CheckForFood();
                if (isSleep) didSleep = Bot.CheckForSleep();
                if (isWork) didWork = Bot.CheckForWork();
                if (isChests) didChests = Bot.CheckForChests();
                if (isTacoTruck) didTacoTruck = Bot.CheckForTacos(lastTacoTruck);
                if (isAuctions) didAuctions = Bot.CheckAuctions();
                if (isPrestige) didPrestige = Bot.CheckPrestige();

                if (didEat) lastEat = DateTime.Now;
                if (didSleep) lastSleep = DateTime.Now;
                if (didWork) lastWork = DateTime.Now;
                if (didChests) lastChests = DateTime.Now;
                if (didTacoTruck) lastTacoTruck = DateTime.Now;
                if (didAuctions) lastAuctions = DateTime.Now;
                if (didPrestige) lastPrestige = DateTime.Now;

                if (didEat || didSleep || didWork || didChests || didTacoTruck || didAuctions || didPrestige) Dispatcher.Invoke(Update_Display);

                if (!ShouldRun())
                {
                    lock(_lock)
                    {
                        _running = false;
                    }
                }
            }

            state = "done running";
            lock(_lock)
            {
                _running = false;
            }

            Dispatcher.Invoke(Update_Display);
        }

        private void WorkThread()
        {
            bool didEat = false, didSleep = false, didWork = false, didChests = false, didTacoTruck = false, didAuctions = false, didPrestige = false;
            state = "running";
            Dispatcher.Invoke(Update_Display);

            while (true)
            {
                if (!isRunning) break;

                if (isGamingMode)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan ts;
                    if (isEat)
                    {
                        ts = now - lastEat;
                        if (ts.TotalMinutes > eatMinutes)
                        {
                            Bot.ForceEat();
                            lastEat = now;
                            Dispatcher.Invoke(Update_Display);
                        }
                    }
                    if (isSleep)
                    {
                        ts = now - lastSleep;
                        if (ts.TotalMinutes > sleepMinutes)
                        {
                            Bot.ForceSleep();
                            lastSleep = now;
                            Dispatcher.Invoke(Update_Display);
                        }
                    }
                    if (isWork)
                    {
                        ts = now - lastWork;
                        if (ts.TotalMinutes > workMinutes)
                        {
                            Bot.ForceWork();
                            lastWork = now;
                            Dispatcher.Invoke(Update_Display);
                        }
                    }

                    if (isPrestige) didPrestige = Bot.CheckPrestige();
                    if (didPrestige) lastPrestige = DateTime.Now;
                }
                else
                {
                    if (isEat) didEat = Bot.CheckForFood();
                    if (isSleep) didSleep = Bot.CheckForSleep();
                    if (isWork) didWork = Bot.CheckForWork();
                    if (isChests) didChests = Bot.CheckForChests();
                    if (isTacoTruck) didTacoTruck = Bot.CheckForTacos(lastTacoTruck);
                    if (isAuctions) didAuctions = Bot.CheckAuctions();
                    if (isPrestige) didPrestige = Bot.CheckPrestige();

                    if (didEat) lastEat = DateTime.Now;
                    if (didSleep) lastSleep = DateTime.Now;
                    if (didWork) lastWork = DateTime.Now;
                    if (didChests) lastChests = DateTime.Now;
                    if (didTacoTruck) lastTacoTruck = DateTime.Now;
                    if (didAuctions) lastAuctions = DateTime.Now;
                    if (didPrestige) lastPrestige = DateTime.Now;
                }

                if (didEat || didSleep || didWork || didChests || didTacoTruck || didAuctions || didPrestige) Dispatcher.Invoke(Update_Display);

                if (!ShouldRun())
                {
                    lock (_lock)
                    {
                        _running = false;
                    }
                }
            }

            state = "done running";
            lock (_lock)
            {
                _running = false;
            }

            Dispatcher.Invoke(Update_Display);
        }

        private async void Update_Checked(object sender, RoutedEventArgs e)
        {
            _gamingMode = cbGamingMode.IsChecked.GetValueOrDefault();
            _eat = cbEat.IsChecked.GetValueOrDefault();
            _sleep = cbSleep.IsChecked.GetValueOrDefault();
            _work = cbWork.IsChecked.GetValueOrDefault();
            _chests = cbChests.IsChecked.GetValueOrDefault();
            _tacoTruck = cbTacoTruck.IsChecked.GetValueOrDefault();
            //_auctions = cbAuctionItems.IsChecked.GetValueOrDefault();
            _prestige = cbPrestige.IsChecked.GetValueOrDefault();

            if (!isRunning && ShouldRun())
            {
                lock(_lock)
                {
                    _running = true;
                }

                await Task.Run(WorkThread);
            }

            Update_Display();
        }

        private void Update_Display()
        {
            info = $"state: {state}\n" +
                $"last Eat: {lastEat.ToShortTimeString()}\n" +
                $"last Sleep: {lastSleep.ToShortTimeString()}\n" +
                $"last Work: {lastWork.ToShortTimeString()}\n" +
                $"last Chest: {lastChests.ToShortTimeString()}\n" +
                $"last Taco Truck: {lastTacoTruck.ToShortTimeString()}\n" +
                //$"last Auctions: {lastAuctions.ToShortTimeString()}\n" +
                $"last Prestige: {lastPrestige.ToShortTimeString()}\n" +
                $"wait multiplier: x{_waitMultiplier}";

            tbInfo.Text = info;
        }

        private bool ShouldRun()
        {
            bool shouldRun = (
                isEat ||
                isSleep ||
                isWork ||
                isChests ||
                isTacoTruck ||
                isAuctions ||
                isPrestige
                );

            return shouldRun;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            int value;
            int lt = (Int32.TryParse(tbL.Text, out value)) ? value : 0;
            int tp = (Int32.TryParse(tbT.Text, out value)) ? value : 0;
            int x = (Int32.TryParse(tbX.Text, out value)) ? value : 0;
            int y = (Int32.TryParse(tbY.Text, out value)) ? value : 0;
            System.Windows.Media.Imaging.BitmapImage img = new System.Windows.Media.Imaging.BitmapImage();
            Bot.Test(lt, tp, x, y, ref img);

            imgPhoto.Source = img;
        }
    }
}
