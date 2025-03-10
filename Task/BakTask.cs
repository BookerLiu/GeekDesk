using GeekDesk.Constant;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekDesk.Task
{
    internal class BakTask
    {

        public static void Start()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 60 * 1000 * 60 * 2, //60秒 * 60 * 2 2小时触发一次
                //Interval = 6000,
            };
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Constants.DATA_FILE_BAK_DIR_PATH);

            string todayBakName = DateTime.Now.ToString("yyyy-MM-dd") + ".bak";

            string bakDaysStr = ConfigurationManager.AppSettings["BakDays"];
            int bakDays = 7;
            if (bakDaysStr != null && !"".Equals(bakDaysStr.Trim()))
            {
                bakDays = Int32.Parse(bakDaysStr.Trim());
            }

            string bakFilePath = Constants.DATA_FILE_BAK_DIR_PATH + "\\" + todayBakName;
            if (dirInfo.Exists)
            {
                // 获取文件信息并按创建时间倒序排序
                FileInfo[] files = dirInfo.GetFiles()
                    .Where(f => f.Extension.Equals(".bak", StringComparison.OrdinalIgnoreCase)).ToArray();
                if (files.Length > 0)
                {
                    FileInfo[] sortedFiles = files.OrderByDescending(file => file.CreationTime).ToArray();
                    if (!sortedFiles[0].Name.Equals(todayBakName))
                    {
                        //今天未创建备份  开始创建今天的备份
                        createBakFile(bakFilePath);
                    }

                    //判断文件是否超过了7个  超过7个就删除
                    if (sortedFiles.Length > bakDays)
                    {
                        for (int i = bakDays; i < sortedFiles.Length; i++)
                        {
                            sortedFiles[i].Delete();
                        }
                    }

                } else
                {
                    //没有文件 直接创建今天的备份
                    createBakFile(bakFilePath);
                }
                
            }
            else
            {
                dirInfo.Create();
            }
        }

        //创建备份文件
        private static void createBakFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, MainWindow.appData);
            }
        }


    }
}
