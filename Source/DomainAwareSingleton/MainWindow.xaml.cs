using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
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

namespace DomainAwareSingleton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LifetimeServices.LeaseTime = TimeSpan.FromSeconds(30);
            LifetimeServices.RenewOnCallTime = TimeSpan.FromSeconds(30);
            InitializeComponent();
        }

        private AppDomain _otherDomain1;

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (_otherDomain1 == null)
                _otherDomain1 = AppDomain.CreateDomain("otherDomain1");
            _otherDomain1.DoCallBack(Test1);
            Test1();
        }

        private static void Test1()
        {
            var value = Singleton<TestType1>.Instance;
            MessageBox.Show("Retrieved instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            value.Test();
        }

        private AppDomain _otherDomain2;

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (_otherDomain2 == null)
                _otherDomain2 = AppDomain.CreateDomain("otherDomain2");
            Test2();
            _otherDomain2.DoCallBack(Test2);
        }

        private static void Test2()
        {
            var value = Singleton<TestType2>.Instance;
            MessageBox.Show("Retrieved instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            value.Test();
        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var domain in AppDomainHelper.EnumerateLoadedAppDomains())
                MessageBox.Show(domain.FriendlyName + (domain == AppDomain.CurrentDomain ? " (current)" : " (not current)"));
        }
    }
}
