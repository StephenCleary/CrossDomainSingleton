using System;
using System.Collections.Generic;
using System.Linq;
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
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var otherDomain = AppDomain.CreateDomain("otherDomain1");
            otherDomain.DoCallBack(Test1);
            Test1();
        }

        private static void Test1()
        {
            var value = Singleton<TestType1>.Instance;
            if (value != null)
                MessageBox.Show("Retrieved instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var otherDomain = AppDomain.CreateDomain("otherDomain2");
            Test2();
            otherDomain.DoCallBack(Test2);
        }

        private static void Test2()
        {
            var value = Singleton<TestType2>.Instance;
            if (value != null)
                MessageBox.Show("Retrieved instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var domain in AppDomainHelper.EnumerateLoadedAppDomains())
                MessageBox.Show(domain.FriendlyName + (domain == AppDomain.CurrentDomain ? " (current)" : " (not current)"));
        }
    }
}
