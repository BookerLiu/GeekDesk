using GeekDesk.Constant;
using GeekDesk.Task;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Controls;
using Quartz;
using System;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// BacklogNotificatin.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalMsgNotification
    {

        public Notification ntf;
        public GlobalMsgNotification(DialogMsg msg)
        {
            InitializeComponent();
            this.DataContext = msg;
        }


        public class DialogMsg
        {
            public string msg;
            public string title;

            public string Msg
            {
                get
                {
                    return msg;
                }
                set
                {
                    msg = value;
                }
            }
            public string Title
            {
                get
                {
                    return title;
                }
                set
                {
                    title = value;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ntf.Close();
        }
    }
}
