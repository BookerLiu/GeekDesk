using System;

namespace GeekDesk.Constant
{
    internal class RunTimeStatus
    {

        /// <summary>
        /// 查询框是否在工作
        /// </summary>
        public static volatile bool SEARCH_BOX_SHOW = false;

        /// <summary>
        /// 查询框是否已经关闭了300毫秒   防止点击右侧区域关闭查询框时误打开列表
        /// </summary>
        public static volatile bool SEARCH_BOX_HIDED_300 = true;

        /// <summary>
        /// 贴边隐藏后  以非鼠标经过方式触发显示
        /// </summary>
        public static volatile bool MARGIN_HIDE_AND_OTHER_SHOW = false;


        /// <summary>
        /// 是否锁定主面板 锁定后 不执行隐藏动作
        /// </summary>
        public static volatile bool LOCK_APP_PANEL = false;


        /// <summary>
        /// 是否弹出了菜单密码框
        /// </summary>
        public static volatile bool SHOW_MENU_PASSWORDBOX = false;

        /// <summary>
        /// 是否弹出了右键菜单
        /// </summary>
        public static volatile bool SHOW_RIGHT_BTN_MENU = false;

        /// <summary>
        /// 是否点击了面板功能按钮
        /// </summary>
        public static volatile bool APP_BTN_IS_DOWN = false;

        /// <summary>
        /// 是否正在编辑菜单
        /// </summary>
        public static volatile bool IS_MENU_EDIT = false;


        /// <summary>
        /// 图标card 鼠标滚轮是否正在工作  
        /// 用来控制popup的显示 否则低性能机器会造成卡顿
        /// </summary>
        public static volatile bool ICONLIST_MOUSE_WHEEL = false;
        /// <summary>
        /// 控制多少毫秒后 关闭(ICONLIST_MOUSE_WHEEL)鼠标滚轮运行状态
        /// </summary>
        public static volatile int MOUSE_WHEEL_WAIT_MS = 100;
        /// <summary>
        /// 与关闭popup 配合使用, 避免线程结束后不显示popup
        /// </summary>
        public static volatile bool MOUSE_ENTER_ICON = false;
        /// <summary>
        /// 控制每次刷新搜索结果 鼠标移动后显示popup
        /// </summary>
        public static volatile int MOUSE_MOVE_COUNT = 0;


        /// <summary>
        /// everything 新的键入搜索
        /// </summary>
        public static volatile bool EVERYTHING_NEW_SEARCH = false;

        /// <summary>
        /// 键入多少毫秒后  没有新的键入开启搜索
        /// </summary>
        public static volatile int EVERYTHING_SEARCH_DELAY_TIME = 300;

        /// <summary>
        /// 控制主界面热键按下规定时间内只执行一次show hide
        /// </summary>
        public static volatile bool MAIN_HOT_KEY_DOWN = false;
        /// <summary>
        /// 控制主界面热键按下规定时间内只执行一次show hide
        /// </summary>
        public static volatile int MAIN_HOT_KEY_TIME = 300;

    }
}
