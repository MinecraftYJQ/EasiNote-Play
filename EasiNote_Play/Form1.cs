using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasiNote_Play
{
    public partial class Form1 : Form
    {
        // 导入 Win32 API 函数
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern bool SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        // THREAD_SUSPEND_RESUME 常量
        const uint SUSPEND_RESUME = 0x0002;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            bool isRunning = Process.GetProcessesByName("EasiNote").Length > 0;
            if (isRunning)
            {
                Console.WriteLine("程序正在运行。");
                if (MessageBox.Show("希沃白板正在运行，请问是否结束进程？", "希沃白板 - 结束进程", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    foreach(Process processToKill in Process.GetProcessesByName("EasiNote"))
                    {
                        // 结束进程
                        processToKill.Kill();

                        // 等待进程结束
                        processToKill.WaitForExit();

                        Console.WriteLine("进程已结束.");
                    }
                }
            }
            Directory.CreateDirectory("EasiNote");
            if (!File.Exists("EasiNote\\EasiNote_Path.txt"))
            {
                while (true)
                {
                    // 创建并配置OpenFileDialog对象
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "选择希沃白板主程序";
                    openFileDialog.Filter = "希沃白板主程序|EasiNote.exe"; // 设置文件过滤器
                    openFileDialog.Multiselect = false; // 允许选择多个文件

                    // 显示文件对话框
                    DialogResult result = openFileDialog.ShowDialog();

                    // 如果用户选择了文件并点击了“打开”按钮
                    if (result == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        // 此处可以对选定的文件进行操作，例如加载文件内容
                        Console.WriteLine($"选中的文件路径：{filePath}");
                        File.WriteAllText("EasiNote\\EasiNote_Path.txt", filePath);
                        break;
                    }
                }
            }
            label1.Text = $"希沃白板软件路径：{File.ReadAllText("EasiNote\\EasiNote_Path.txt")}";
        }

        private void Starting_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("注意：\n此程序仅用来演示！\n产生的一切纠纷和责任作者概不负责！", "宇宙免责声明", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                Start(checkBox1.Checked);
            }
        }

        private void Start(bool explorer)
        {
            if(explorer)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        bool isRunning = Process.GetProcessesByName("explorer").Length > 0;
                        if (isRunning)
                        {
                            foreach (Process processToKill in Process.GetProcessesByName("explorer"))
                            {
                                // 结束进程
                                processToKill.Kill();

                                // 等待进程结束
                                processToKill.WaitForExit();

                                Console.WriteLine("进程已结束.");
                            }
                        }
                    }
                });
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (checkBox1.Checked)
            {
                Process[] processes = Process.GetProcesses();//获取所有进程信息
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].ProcessName.ToLower() == "explorer")
                    {
                        try
                        {
                            processes[i].Kill(); //停止进程
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("进程访问失败！");
                        }
                    }
                }
                Process.Start("explorer.exe");//再启动进程
            }
        }
    }
}
