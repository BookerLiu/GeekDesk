using GeekDesk.Constant;
using GeekDesk.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GeekDesk.Util
{
    public class BGSettingUtil
    {
        private static readonly AppConfig appConfig = MainWindow.appData.AppConfig;
        public static void BGSetting()
        {
            if (appConfig.BGStyle == BGStyle.ImgBac || appConfig.BGStyle == 0)
            {
                Image image = new Image
                {
                    Effect = new BlurEffect()
                    {
                        Radius = appConfig.BlurValue
                    },
                    Margin = new Thickness(-30),
                    Source = appConfig.BitmapImage,
                    Opacity = (double)(Math.Round((decimal)(appConfig.BgOpacity / 100.00), 2))
                };


                //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //image.SetBinding(Image.OpacityProperty, binding);

                Grid grid = new Grid
                {
                    ClipToBounds = true
                };
                grid.Children.Add(image);

                VisualBrush vb = new VisualBrush
                {
                    Visual = grid
                };
                MainWindow.mainWindow.BGBorder.Background = vb;
            }
            else
            {
                LinearGradientBrush lgb = new LinearGradientBrush();
                GradientStop gs = new GradientStop
                {
                    Color = (Color)ColorConverter.ConvertFromString(appConfig.GradientBGParam.Color1),
                    Offset = 0
                };

                lgb.GradientStops.Add(gs);

                GradientStop gs2 = new GradientStop
                {
                    Color = (Color)ColorConverter.ConvertFromString(appConfig.GradientBGParam.Color2),
                    Offset = 1
                };
                lgb.GradientStops.Add(gs2);
                lgb.Opacity = (double)(Math.Round((decimal)(appConfig.BgOpacity / 100.00), 2));
                MainWindow.mainWindow.BGBorder.Background = lgb;
            }

        }

    }
}
