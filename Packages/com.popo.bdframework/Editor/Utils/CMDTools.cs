﻿using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

namespace BDFramework.Editor.Tools
{
    static public class CMDTools
    {
        private static string CmdPath = @"C:\Windows\System32\cmd.exe";

        /// <summary>
        /// 执行cmd命令 返回cmd窗口显示的信息
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// Windows only
        /// </summary>
        /// <param name="cmd">执行的命令</param>
        public static void RunCmd(string[] cmds)
        {
            //执行
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;

                p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("gb2312");
                p.StartInfo.StandardErrorEncoding = Encoding.GetEncoding("gb2312");

                //日志
                p.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.Log(e.Data);
                    }
                };
                p.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.LogError(e.Data);
                    }
                };

                //提前输入参数，Unity中用这个 不会有首字符乱码问题
                // p.StartInfo.Arguments = cmd;

                p.Start(); //启动程序
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                //向cmd窗口写入命令
                foreach (string cmd in cmds)
                {
                    p.StandardInput.WriteLine(cmd); //输入CMD命令
                }

                p.StandardInput.WriteLine("exit"); //结束执行，很重要的
                // p.StandardInput.WriteLine(cmd);
                // p.StandardInput.AutoFlush = true;


                //获取cmd窗口的输出信息
                // string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(); //等待程序执行完退出进程
                p.Close();

                // Debug.LogWarning(output);
                //return output;
            }
        }
        
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="args">执行的脚本名</param>
        static public void RunCmdFile(string shellpath, string args = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = shellpath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            //日志
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.Log("[Cmd]" + e.Data);
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.Log("[Error]" + e.Data);
                }
            };


            //执行
            Debug.Log("执行:\n" + args);
            process.StartInfo.Arguments = args;
            //开始
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            //
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
    }
}
