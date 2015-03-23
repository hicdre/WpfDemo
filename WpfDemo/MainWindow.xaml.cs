using Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<WinService> ServiceToDisplay { get; set; }
        public MainWindow()
        {
            ServiceToDisplay = new ObservableCollection<WinService>();
            InitializeComponent();


            IObservable<WinService> currentList = WinService.CurrentList().ToObservable();
            currentList.Subscribe(service => ServiceToDisplay.Add(service));
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            base.WindowState = WindowState.Minimized;
        }

        private void Restore(object sender, RoutedEventArgs e)
        {
            base.WindowState = (base.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            this.WindowResized(base.WindowState == WindowState.Maximized);
        }

        private void WindowResized(bool maximized)
        {
            this.MaximizeButton.Visibility = maximized ? Visibility.Hidden : Visibility.Visible;
            this.RestoreButton.Visibility = maximized ? Visibility.Visible : Visibility.Hidden;
            //this.frameGrid.IsHitTestVisible = !maximized;
            //this.UpdateDwmBorder();
        }
    }
}
