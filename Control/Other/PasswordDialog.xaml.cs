using GeekDesk.Constant;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GeekDesk.Control.Other
{
    /// <summary>
    /// TextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordDialog
    {
        private AppData appData = MainWindow.appData;
        
        public PasswordType type;
        public MenuInfo menuInfo;
        public int count = 0;
        private string tempPassword = null;
        private PasswordType tempType;
        public PasswordDialog()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            if (!string.IsNullOrEmpty(pb.Password))
            {
                char c = pb.Password.ToCharArray()[0];
                if (c > '9' || c < '0')
                {
                    pb.Password = "";
                    return;
                }
            }
            string tag = pb.Tag.ToString();
            switch (tag)
            {
                case "P1":
                    if (!string.IsNullOrEmpty(pb.Password))
                    {
                        P2.Focus();
                    }
                    break;
                case "P2":
                    if (!string.IsNullOrEmpty(pb.Password))
                    {
                        P3.Focus();
                    }
                    break;
                case "P3":
                    if (!string.IsNullOrEmpty(pb.Password))
                    {
                        P4.Focus();
                    }
                    break;
                case "P4":
                    if (string.IsNullOrEmpty(pb.Password))
                    {
                        P3.Focus();
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(P1.Password)
                    && !string.IsNullOrEmpty(P2.Password)
                    && !string.IsNullOrEmpty(P3.Password)
                    && !string.IsNullOrEmpty(P4.Password))
            {
                string pw = P1.Password
                    + P2.Password
                    + P3.Password
                    + P4.Password;
                pw = MD5Util.CreateMD5(pw);
                if (type == PasswordType.INPUT || type == PasswordType.CANCEL)
                {
                    if (pw.Equals(appData.AppConfig.MenuPassword))
                    {
                        //隐藏弹框
                        MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Collapsed;
                        //赋值
                        MainWindow.appData.AppConfig.SelectedMenuIcons
                            = appData.MenuList[
                                MainWindow.mainWindow.LeftCard.MenuListBox.SelectedIndex
                                ].IconList;
                        //显示数据托盘
                        MainWindow.mainWindow.RightCard.WrapUFG.Visibility = Visibility.Visible;
                        //取消加密操作
                        if (type == PasswordType.CANCEL)
                        {
                            menuInfo.IsEncrypt = false;
                        }
                    } else
                    {
                        //密码比对不一致
                        ErrorMsg.Text = "密码输入错误";
                        ErrorMsg.Visibility = Visibility.Visible;
                        if (!string.IsNullOrEmpty(appData.AppConfig.PasswordHint))
                        {
                            //显示提示信息
                            HintMsg.Text = "提示: " + appData.AppConfig.PasswordHint;
                            HintMsg.Visibility = Visibility.Visible;
                        }
                    }
                } else if (type == PasswordType.CREATE)
                {
                    //创建密码
                    if (count == 0)
                    {
                        count++;
                        tempPassword = pw;
                        Title.Text = "再次输入密码";
                        ClearVal();
                        SetFocus(0);
                    } else
                    {
                        if (tempPassword.Equals(pw))
                        {
                            //两次密码设置一致  显示提示输入框
                            Title.Text = "填写密码提示";
                            PasswordGrid.Visibility = Visibility.Collapsed;
                            HintGrid.Visibility = Visibility.Visible;
                            HintBox.Focus();
                        } else
                        {
                            ErrorMsg.Text = "两次密码输入不一致";
                            ErrorMsg.Visibility = Visibility.Visible;
                        }
                    }
                } else if (type == PasswordType.ALTER)
                {
                    //修改密码
                    if (appData.AppConfig.MenuPassword.Equals(pw))
                    {
                        tempType = type;
                        type = PasswordType.CREATE;
                        Title.Text = "设置新密码";
                        ClearVal();
                        SetFocus(0);
                    } else
                    {
                        //密码比对不一致
                        ErrorMsg.Text = "密码输入错误";
                        ErrorMsg.Visibility = Visibility.Visible;
                        HintMsg.Text = MainWindow.appData.AppConfig.PasswordHint;
                        HintMsg.Visibility = Visibility.Visible;
                    }
                }
            } else
            {
                //密码未输入完全  隐藏错误信息
                if (ErrorMsg.IsVisible)
                {
                    ErrorMsg.Visibility = Visibility.Hidden;
                    HintMsg.Visibility = Visibility.Hidden;
                    HintMsg.Visibility = Visibility.Hidden;
                }
            }
        }

        public void SetFocus(int time = 100)
        {
            new Thread(() =>
            {
                Thread.Sleep(time);
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(P1.Password))
                            {
                                P1.Focus();
                                return;
                            }
                            if (string.IsNullOrEmpty(P2.Password))
                            {
                                P2.Focus();
                                return;
                            }
                            if (string.IsNullOrEmpty(P3.Password))
                            {
                                P3.Focus();
                                return;
                            }
                            P4.Focus();
                        }
                        catch (Exception ex) { }
                    });
                }
                catch (Exception e2) { }
            }).Start();
        }

        public void ClearVal()
        {
            P1.Clear();
            P2.Clear();
            P3.Clear();
            P4.Clear();
        }

        /// <summary>
        /// 跳过设置密码提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            appData.AppConfig.PasswordHint = "";
            DonePassword();
        }

        private void DoneTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string hint = HintBox.Text.Trim();
            appData.AppConfig.PasswordHint = hint;
            DonePassword();
        }

        private void DonePassword()
        {
            appData.AppConfig.MenuPassword = tempPassword;
            CommonCode.SavePassword(tempPassword);
            MainWindow.mainWindow.RightCard.PDDialog.Visibility = Visibility.Collapsed;
            PasswordGrid.Visibility = Visibility.Visible;
            HintGrid.Visibility = Visibility.Collapsed;
            if (tempType == PasswordType.ALTER)
            {
                HandyControl.Controls.Growl.Success("密码修改成功!", "MainWindowGrowl");
            } else
            {
                menuInfo.IsEncrypt = true;
                HandyControl.Controls.Growl.Success(menuInfo.MenuName + " 已加密!", "MainWindowGrowl");
            }

        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (P2.IsKeyboardFocused)
                {
                    if (string.IsNullOrEmpty(P2.Password))
                    {
                        P1.Password = "";
                    } else
                    {
                        P2.Password = "";
                    }
                }

                if (P3.IsKeyboardFocused)
                {
                    if (string.IsNullOrEmpty(P3.Password))
                    {
                        P2.Password = "";
                    }
                    else
                    {
                        P3.Password = "";
                    }
                }

                if (P4.IsKeyboardFocused)
                {
                    if (string.IsNullOrEmpty(P4.Password))
                    {
                        P3.Password = "";
                    }
                    else
                    {
                        P4.Password = "";
                    }
                }
            }
            SetFocus(0);
        }
    }
}
