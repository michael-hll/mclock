using System;
using System.Collections.Generic;
using System.Globalization;
using mClock.Services;
using mClock.Themes;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mClock.Views
{
    public partial class MClockPage : ContentPage
    {
        const double DAY_OF_WEEK_OPACITY_SHOW = 1;
        const double DAY_OF_WEEK_OPACITY_HIDE = 0.2;
        const int SLEEPTIME_START = 23;
        const int SLEEPTIME_END = 6;
        ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        ResourceDictionary CurrentTheme = new DarkTheme();
        IBrightnessService brightnessService = DependencyService.Get<IBrightnessService>();


        public static readonly string[] DateFormats = {
            "MMM dd, yyyy",
            "MMM dd",
            "MMMM dd, yyyy",
            "MMMM dd",
            "yyyy-MM-dd",
            "MM-dd",
            "MM/dd/yyyy",
            "MM/dd",
            "dd/MM/yyyy",
            "dd/MM"
        };

        public static readonly string[] Themes = {
            ThemeKeys.DarkTheme,
            ThemeKeys.SkyTheme,
            ThemeKeys.GrassTheme,
            ThemeKeys.PinkTheme,
            ThemeKeys.LightTheme,
        };

        public static readonly string[] WeekDaysNormalCap = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
        public static readonly string[] WeekDaysNormal = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        public static readonly string[] WeekDaysNormalShort = { "M", "T", "W", "T", "F", "S", "S" };
        public static readonly string[] WeekDaysChinese = { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };
        public static readonly string[] WeekDaysChinese2 = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
        public static readonly string[] WeekDaysChineseShort = { "一", "二", "三", "四", "五", "六", "日" };

        public static ISettings AppSettings => CrossSettings.Current;
        public static MClockPage MainInstance = null;
        public static MTimerPage TimerPageInstance = null;

        public bool Is12Hour
        {
            get => AppSettings.GetValueOrDefault(nameof(Is12Hour), true);
            set => AppSettings.AddOrUpdateValue(nameof(Is12Hour), value);
        }

        public bool IsShowSeconds
        {
            get => AppSettings.GetValueOrDefault(nameof(IsShowSeconds), true);
            set => AppSettings.AddOrUpdateValue(nameof(IsShowSeconds), value);
        }

        public bool IsSundayFirstDay
        {
            get => AppSettings.GetValueOrDefault(nameof(IsSundayFirstDay), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsSundayFirstDay), value);
        }

        public int DateFormatIndex
        {
            get => AppSettings.GetValueOrDefault(nameof(DateFormatIndex), 0);
            set => AppSettings.AddOrUpdateValue(nameof(DateFormatIndex), value);
        }

        public int DayOfWeekFormatIndex
        {
            get => AppSettings.GetValueOrDefault(nameof(DayOfWeekFormatIndex), 0);
            set => AppSettings.AddOrUpdateValue(nameof(DayOfWeekFormatIndex), value);
        }

        public int MTimerDefaultMins
        {
            get => AppSettings.GetValueOrDefault(nameof(MTimerDefaultMins), 25);
            set => AppSettings.AddOrUpdateValue(nameof(MTimerDefaultMins), value);
        }

        public int ThemeIndex
        {
            get => AppSettings.GetValueOrDefault(nameof(ThemeIndex), 0);
            set => AppSettings.AddOrUpdateValue(nameof(ThemeIndex), value);
        }

        public float ScreenBrightness
        {
            get => AppSettings.GetValueOrDefault(nameof(ScreenBrightness), 0.6f);
            set => AppSettings.AddOrUpdateValue(nameof(ScreenBrightness), value);
        }

        readonly Dictionary<int, string[]> DayOfWeekFormatsDict = new Dictionary<int, string[]>()
        {
            { 0, WeekDaysNormalCap },
            { 1, WeekDaysNormal },
            { 2, WeekDaysNormalShort },
            { 3, WeekDaysChinese },
            { 4, WeekDaysChinese2 },
            { 5, WeekDaysChineseShort }
        };

        bool IsInSleepTime { get; set; }

        double DayOfWeekFontSize
        {
            get
            {
                if (DayOfWeekFormatIndex >= 3)
                {
                    if (Application.Current.MainPage.Width < 600)
                        return 13;
                    else if (Application.Current.MainPage.Width < 800)
                        return 15;
                    else
                        return 20;
                }
                else return 20;
            }
        }

        Thickness DayOfWeekFramePadding
        {
            get
            {
                if (DayOfWeekFormatIndex >= 3) return new Thickness(0, 0, 0, 0);
                else return new Thickness(0, 0, 0, 0);
            }
        }

        string DayOfWeekFrameFontFamily
        {
            get
            {
                if (DayOfWeekFormatIndex >= 3) return "Weibei SC";
                else return "Courier";
            }
        }

        public MClockPage()
        {
            InitializeComponent();
            MainInstance = this;

            // loading themes
            UpdateMergedDictionaries();
        }

        bool HasDayOfWeekTextChanged { get; set; }

        void UpdateMergedDictionaries()
        {
            if (mergedDictionaries != null) mergedDictionaries.Clear();
            CurrentTheme = GetCurrentTheme();
            mergedDictionaries.Add(CurrentTheme);
        }

        void InitiUIControls()
        {
            double width = Application.Current.MainPage.Width;

            double hourFontSize = width * 0.3;
            double minuteFontSize = width * 0.3;
            double secondFontSize = width * 0.10;

            lableHour.FontSize = hourFontSize;
            lableMinute.FontSize = minuteFontSize;
            lableSecond.FontSize = secondFontSize;

            this.relativeLayoutRight.Children.Add(this.lableSecond,
                Constraint.Constant(0),
                Constraint.Constant(secondFontSize * 1.45));

            if (width > 600)
            {
                lablePartOfDay.Margin = new Thickness(0, 15, 20, 0);
            }

            DeviceDisplay.KeepScreenOn = true;
            HasDayOfWeekTextChanged = true;

            if (DateTime.Now.Hour >= SLEEPTIME_START && DateTime.Now.Hour <= SLEEPTIME_END)
            {
                IsInSleepTime = true;
            }
            else
            {
                IsInSleepTime = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitiUIControls();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                DateTime now = DateTime.Now;

                // make the screen brightness low
                if (now.Hour >= SLEEPTIME_START && now.Hour <= SLEEPTIME_END)
                {
                    if (IsInSleepTime == false)
                    {
                        ScreenBrightness = brightnessService.GetBrightness();
                        brightnessService.SetBrightness(0f);
                        IsInSleepTime = true;
                    }
                }
                else
                {
                    if (IsInSleepTime == true)
                    {
                        brightnessService.SetBrightness(ScreenBrightness);
                        IsInSleepTime = false;
                    }
                }

                // Update datetime
                Device.InvokeOnMainThreadAsync(() =>
                {
                    if (Is12Hour) lableHour.Text = now.ToString("hh");
                    else lableHour.Text = now.ToString("HH");

                    lableMinute.Text = now.Minute.ToString("00");
                    lableSecond.Text = now.Second.ToString("00");
                    lablePartOfDay.Text = now.ToString("tt");

                    UpdateDayOfWeekText();
                    UpdateDayOfWeekVisibility(now);
                    UpdateDateLabelText();

                    // show hide controls
                    lablePartOfDay.IsVisible = Is12Hour;
                    lableSecond.IsVisible = IsShowSeconds;
                });

                return true;
            });

        }

        void OnHourDoubleTapped(System.Object sender, System.EventArgs e)
        {
            Is12Hour = !Is12Hour;
            lablePartOfDay.IsVisible = Is12Hour;
        }

        void OnMinuteDoubleTapped(System.Object sender, System.EventArgs e)
        {
            IsShowSeconds = !IsShowSeconds;
            lableSecond.IsVisible = IsShowSeconds;
        }

        void OnDateDoubleTapped(System.Object sender, System.EventArgs e)
        {
            // https://gist.github.com/hilen/b425e65d019d9e7fe90fcbced0e3c4fd
            // https://github.com/rex11458/iOS-Private-URL-Scheme
            Launcher.OpenAsync("calshow:");
        }

        void OnDateSwiped(System.Object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    DateFormatIndex--;
                    if (DateFormatIndex == -1) DateFormatIndex = DateFormats.Length - 1;
                    UpdateDateLabelText();
                    break;
                case SwipeDirection.Right:
                    DateFormatIndex++;
                    if (DateFormatIndex == DateFormats.Length) DateFormatIndex = 0;
                    UpdateDateLabelText();
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    break;
            }
        }
        void OnDayOfWeeksSwiped(System.Object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    DayOfWeekFormatIndex--;
                    if (DayOfWeekFormatIndex == -1) DayOfWeekFormatIndex = DayOfWeekFormatsDict.Keys.Count - 1;
                    HasDayOfWeekTextChanged = true;
                    UpdateDayOfWeekText();
                    break;
                case SwipeDirection.Right:
                    DayOfWeekFormatIndex++;
                    if (DayOfWeekFormatIndex == DayOfWeekFormatsDict.Keys.Count) DayOfWeekFormatIndex = 0;
                    HasDayOfWeekTextChanged = true;
                    UpdateDayOfWeekText();
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    break;
            }
        }

        void OnHourSwiped(System.Object sender, SwipedEventArgs e)
        {
            var brightnessService = DependencyService.Get<IBrightnessService>();
            float current = brightnessService.GetBrightness();
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    ThemeIndex--;
                    if (ThemeIndex < 0) ThemeIndex = Themes.Length - 1;
                    UpdateMergedDictionaries();
                    UpdateSpecialDayOfWeekColors();
                    break;
                case SwipeDirection.Right:
                    ThemeIndex++;
                    if (ThemeIndex == Themes.Length) ThemeIndex = 0;
                    UpdateMergedDictionaries();
                    UpdateSpecialDayOfWeekColors();
                    break;
                case SwipeDirection.Up:
                    if (current <= 0.8)
                        brightnessService.SetBrightness(current + 0.2f);
                    break;
                case SwipeDirection.Down:
                    if (current >= 0.0)
                        brightnessService.SetBrightness(current - 0.2f);
                    break;
            }
        }

        async void OnMinuteSwiped(System.Object sender, SwipedEventArgs e)
        {
            float current = brightnessService.GetBrightness();
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    if (TimerPageInstance == null)
                        TimerPageInstance = new MTimerPage();
                    await Navigation.PushAsync(TimerPageInstance, true);
                    break;
                case SwipeDirection.Right:
                    break;
                case SwipeDirection.Up:
                    if (current <= 0.8)
                        brightnessService.SetBrightness(current + 0.2f);
                    break;
                case SwipeDirection.Down:
                    if (current >= 0.2)
                        brightnessService.SetBrightness(current - 0.2f);
                    break;
            }
        }

        string GetDayOfWeekText(int index)
        {
            int i = GetMappingDayOfWeekIndex(index);
            return DayOfWeekFormatsDict[DayOfWeekFormatIndex][i];
        }

        void UpdateDayOfWeekText()
        {
            if (HasDayOfWeekTextChanged)
            {
                lableDayOfWeek1.Text = GetDayOfWeekText(1);
                lableDayOfWeek2.Text = GetDayOfWeekText(2);
                lableDayOfWeek3.Text = GetDayOfWeekText(3);
                lableDayOfWeek4.Text = GetDayOfWeekText(4);
                lableDayOfWeek5.Text = GetDayOfWeekText(5);
                lableDayOfWeek6.Text = GetDayOfWeekText(6);
                lableDayOfWeek7.Text = GetDayOfWeekText(7);

                lableDayOfWeek1.FontSize = DayOfWeekFontSize;
                lableDayOfWeek2.FontSize = DayOfWeekFontSize;
                lableDayOfWeek3.FontSize = DayOfWeekFontSize;
                lableDayOfWeek4.FontSize = DayOfWeekFontSize;
                lableDayOfWeek5.FontSize = DayOfWeekFontSize;
                lableDayOfWeek6.FontSize = DayOfWeekFontSize;
                lableDayOfWeek7.FontSize = DayOfWeekFontSize;

                lableDayOfWeek1.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek2.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek3.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek4.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek5.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek6.FontFamily = DayOfWeekFrameFontFamily;
                lableDayOfWeek7.FontFamily = DayOfWeekFrameFontFamily;

                frameDayOfWeek1.Padding = DayOfWeekFramePadding;
                frameDayOfWeek2.Padding = DayOfWeekFramePadding;
                frameDayOfWeek3.Padding = DayOfWeekFramePadding;
                frameDayOfWeek4.Padding = DayOfWeekFramePadding;
                frameDayOfWeek5.Padding = DayOfWeekFramePadding;
                frameDayOfWeek6.Padding = DayOfWeekFramePadding;
                frameDayOfWeek7.Padding = DayOfWeekFramePadding;

                UpdateSpecialDayOfWeekColors();

                HasDayOfWeekTextChanged = false;
            }
        }

        void UpdateDayOfWeekVisibility(DateTime dateTime)
        {
            int dayOfWeek = GetDayOfWeekIndex(dateTime);
            frameDayOfWeek1.Opacity = (GetMappingDayOfWeekIndex(1) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek2.Opacity = (GetMappingDayOfWeekIndex(2) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek3.Opacity = (GetMappingDayOfWeekIndex(3) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek4.Opacity = (GetMappingDayOfWeekIndex(4) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek5.Opacity = (GetMappingDayOfWeekIndex(5) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek6.Opacity = (GetMappingDayOfWeekIndex(6) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
            frameDayOfWeek7.Opacity = (GetMappingDayOfWeekIndex(7) == dayOfWeek) ? DAY_OF_WEEK_OPACITY_SHOW : DAY_OF_WEEK_OPACITY_HIDE;
        }

        void UpdateSpecialDayOfWeekColors()
        {
            try
            {
                if (IsSundayFirstDay)
                {
                    frameDayOfWeek1.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week6to7BackgroundColor];
                    frameDayOfWeek6.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week1to5BackgroundColor];
                    frameDayOfWeek7.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week6to7BackgroundColor];
                    lableDayOfWeek1.TextColor = (Color)CurrentTheme[ThemeKeys.Week6to7TextColor];
                    lableDayOfWeek6.TextColor = (Color)CurrentTheme[ThemeKeys.Week1to5TextColor];
                    lableDayOfWeek7.TextColor = (Color)CurrentTheme[ThemeKeys.Week6to7TextColor];
                }
                else
                {
                    frameDayOfWeek1.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week1to5BackgroundColor];
                    frameDayOfWeek6.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week6to7BackgroundColor];
                    frameDayOfWeek7.BackgroundColor = (Color)CurrentTheme[ThemeKeys.Week6to7BackgroundColor];
                    lableDayOfWeek1.TextColor = (Color)CurrentTheme[ThemeKeys.Week1to5TextColor];
                    lableDayOfWeek6.TextColor = (Color)CurrentTheme[ThemeKeys.Week6to7TextColor];
                    lableDayOfWeek7.TextColor = (Color)CurrentTheme[ThemeKeys.Week6to7TextColor];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in method UpdateDayOfWeekText:" + ex.Message);
            }
        }

        void UpdateDateLabelText()
        {
            lableDate.Text = DateTime.Now.ToString(DateFormats[DateFormatIndex], CultureInfo.InvariantCulture);
        }

        int GetMappingDayOfWeekIndex(int index)
        {
            int i = index - 1;
            if (IsSundayFirstDay)
            {
                i = index - 2;
                if (i < 0) i = 6;
            }
            return i;
        }

        int GetDayOfWeekIndex(DateTime dateTime)
        {
            if (dateTime == null) return -1;
            for (int i = 0; i < WeekDaysNormalCap.Length; i++)
            {
                if (WeekDaysNormalCap[i] == dateTime.DayOfWeek.ToString().Substring(0, 3).ToUpper())
                    return i;
            }
            return -1;
        }

        void OnDayOfWeek1DoubleTapped(System.Object sender, System.EventArgs e)
        {
            if (IsSundayFirstDay)
            {
                OnSundayClicked();
            }
            else
            {
                OnMondayClicked();
            }
        }

        void OnDayOfWeek2DoubleTapped(System.Object sender, System.EventArgs e)
        {
            if (IsSundayFirstDay)
            {
                OnMondayClicked();
            }
        }


        void OnDayOfWeek3DoubleTapped(System.Object sender, System.EventArgs e)
        {

        }

        void OnDayOfWeek4DoubleTapped(System.Object sender, System.EventArgs e)
        {

        }

        void OnDayOfWeek5DoubleTapped(System.Object sender, System.EventArgs e)
        {

        }

        void OnDayOfWeek6DoubleTapped(System.Object sender, System.EventArgs e)
        {

        }

        void OnDayOfWeek7DoubleTapped(System.Object sender, System.EventArgs e)
        {
            if (!IsSundayFirstDay)
            {
                OnSundayClicked();
            }
        }

        void OnMondayClicked()
        {

        }

        void OnSundayClicked()
        {
            IsSundayFirstDay = !IsSundayFirstDay;
            HasDayOfWeekTextChanged = true;
        }

        public string GetAppVersion()
        {
            // app version
            return DependencyService.Get<IAppVersionAndBuild>().GetAppVersion();
        }

        ResourceDictionary GetCurrentTheme()
        {
            switch (Themes[ThemeIndex])
            {
                case ThemeKeys.DarkTheme:
                    return new DarkTheme();
                case ThemeKeys.LightTheme:
                    return new LightTheme();
                case ThemeKeys.SkyTheme:
                    return new SkyTheme();
                case ThemeKeys.GrassTheme:
                    return new GrassTheme();
                case ThemeKeys.PinkTheme:
                    return new PinkTheme();
                default:
                    break;
            }
            return null;
        }

    }
}
