using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DomainAwareSingleton
{
    public sealed class TestType1 : MarshalByRefObject
    {
        public TestType1()
        {
            MessageBox.Show("Creating instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }
    }

    public sealed class TestType2 : MarshalByRefObject
    {
        public TestType2()
        {
            MessageBox.Show("Creating instance in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
