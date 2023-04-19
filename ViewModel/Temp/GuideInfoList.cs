using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class GuideInfoList
    {

        public static ObservableCollection<GuideInfo> mainWindowGuideList = new ObservableCollection<GuideInfo>();

        static GuideInfoList()
        {
            GuideInfo guideInfo = new GuideInfo
            {
                Title1 = "引导提示",
                Title2 = "图标列表区",
                GuideText = "右侧高亮区域是图标列表区, 将文件拖动到图标列表区将自动创建快捷方式, 或者鼠标右键单击添加系统项目"
            };
            mainWindowGuideList.Add(guideInfo);

            guideInfo = new GuideInfo
            {
                Title1 = "引导提示",
                Title2 = "菜单栏",
                GuideText = "左侧高亮区域是菜单栏, 右键单击左侧区域可以创建菜单, 右键单击菜单可以对菜单进行操作"
            };
            mainWindowGuideList.Add(guideInfo);

            guideInfo = new GuideInfo
            {
                Title1 = "引导提示",
                Title2 = "拖动区域",
                GuideText = "左键按住上部高亮区域可以拖动程序窗体"
            };
            mainWindowGuideList.Add(guideInfo);

            guideInfo = new GuideInfo
            {
                Title1 = "引导提示",
                Title2 = "设置和关闭",
                GuideText = "高亮区域的两个按钮分别是设置和关闭按钮, 你可以点击设置按钮重新打开引导提示, 设置窗口中可自定义开启或关闭众多功能, 赶紧探索使用吧"
            };

            mainWindowGuideList.Add(guideInfo);
        }


        public class GuideInfo
        {
            private string title1;
            private string title2;
            private string guideText;

            public string Title1 { get; set; }
            public string Title2 { get; set; }
            public string GuideText { get; set; }
        }

        public enum GuidePopOffect
        {
            TOP,
            INNER_TOP,
            LEFT,
            INNER_LEFT,
            CENTER,
            RIGHT,
            INNER_RIGHT,
            BOTTOM,
            INNER_BOTTOM
        }

       

    }
}
