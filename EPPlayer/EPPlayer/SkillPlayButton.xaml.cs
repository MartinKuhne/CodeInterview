using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.System.Threading;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EPPlayer
{
    public sealed partial class SkillPlayButton : UserControl
    {
        const int RollResetDelay = 8;
        public SkillPlayButton()
        {
            this.InitializeComponent();
        }

        private void bRoll_Click(object sender, RoutedEventArgs e)
        {
            Skill Skill = (sender as Button).DataContext as Skill;
            int DiceRoll = Dice.SimpleSuccess();
            bool IsSucess = (DiceRoll <= Skill.cookedValue);
            bool IsCrit = (DiceRoll % 10 == DiceRoll / 10);
            int Margin = DiceRoll - DiceRoll % 10;

            txRoll.Text = DiceRoll.ToString();
            txSuccess.Text = IsCrit ? "CRIT" : (IsSucess ? "WIN" : "FAIL");
            txSuccess.Foreground = IsSucess ? new SolidColorBrush(Windows.UI.Colors.Chartreuse) : new SolidColorBrush(Windows.UI.Colors.Red);
            txMargin.Text = Margin.ToString();

            // schedule a timer that will then schedule an update on the UI thread...
            TimeSpan delay = TimeSpan.FromSeconds(RollResetDelay);
            ThreadPoolTimer Timer = ThreadPoolTimer.CreateTimer(TimerExpired, delay);

            this.bRoll.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.grRollResult.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        public void TimerExpired(ThreadPoolTimer timer)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Reset();
            });
        }
        private void Reset()
        {
            this.grRollResult.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.bRoll.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
