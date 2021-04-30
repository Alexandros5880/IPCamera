using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IPCamera
{
    class Install_Requarements
    {
        public static bool first_time_runs = false;
        public static bool First_time_runs
        {
            get 
            {
                try
                {
                    Console.WriteLine("first_run.txt EXISTS.");
                    String file = $"{GetRootDir()}\\first_run.txt";
                    if (File.Exists(file))
                    {
                        Console.WriteLine("File EXISTS.");
                        String str = System.IO.File.ReadAllText(file);
                        //MessageBox.Show($"First Time Runs: {str}");
                        if (str.Contains('1'))
                        {
                            first_time_runs = true;
                        }
                        else
                        {
                            first_time_runs = false;
                        }
                        return first_time_runs;
                    }
                    else
                    {
                        Console.WriteLine("first_run.txt NOT EXISTS.");
                        first_time_runs = true;
                        return first_time_runs;
                    }
                } catch (System.IO.FileLoadException ex)
                {
                    Console.WriteLine($"\n\nFirst Run Can't Loaded...\n\n");
                    Thread.Sleep(5000);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                    Thread.Sleep(5000);
                    return false;
                }
                return first_time_runs;
            }
            set
            {
                try
                {
                    first_time_runs = value;
                    string file = $"{GetRootDir()}\\first_run.txt";
                    // Create the file, or overwrite if the file exists.
                    using (FileStream fs = File.Create(file))
                    {
                        byte[] info;
                        if (first_time_runs.Equals(true))
                        {
                            info = new UTF8Encoding(true).GetBytes("1");
                        }
                        else
                        {
                            info = new UTF8Encoding(true).GetBytes("0");
                        }
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nSource:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}\n\n");
                    Thread.Sleep(5000);
                }
            }
        }

        public static String GetRootDir()
        {
            String cur_dir = Environment.CurrentDirectory;
            //string root_dir = Path.GetFullPath(Path.Combine(cur_dir, @"..\..\"));
            return cur_dir;
        }

        public static void Install_Req()
        {
            String req_dir = $"{GetRootDir()}\\Requarements\\";
            Console.WriteLine($"[ DIRECTORY ]: {req_dir}");
            /*
            String[] exes =
                    Directory.GetFiles(req_dir, "*.EXE", SearchOption.AllDirectories)
                    .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                    .AsEnumerable()
                    .ToArray();
            */
            String[] exes = {
                "redist_dotnet_base_x64" , "redist_dotnet_base_x86",
                "redist_dotnet_ffmpeg_exe_x64" , "redist_dotnet_ffmpeg_exe_x86",
                "redist_dotnet_ffmpeg_x86" , "redist_dotnet_lav_x64",
                "redist_dotnet_lav_x86" , "redist_dotnet_mp4_x64",
                "redist_dotnet_mp4_x86" , "redist_dotnet_vlc_x64",
                "redist_dotnet_vlc_x86" , "redist_dotnet_webm_x86",
                "redist_dotnet_webm_x86" , "redist_dotnet_xiph_x86",
            };
            // "NDP472-KB4054531-Web" , "NetFx64", "SQLServer2016-SSEI-Expr" , 
            foreach (String file in exes)
            {
                String exe = $"{req_dir}{file}.exe";
                Console.WriteLine(exe);
                try
                {
                    System.Diagnostics.Process.Start(exe);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
                Thread.Sleep(2000);
            }

        }
    }
}
