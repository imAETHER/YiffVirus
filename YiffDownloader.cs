using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace YiffVirus
{
    class YiffDownloader
    {
        private readonly string FUNNY_API = "http://e621.net/posts.json?rating:e&limit=251";
        private Random rand = new Random();
        private int lastPostIndex = 0;
        private dynamic jsonResponse;

        public string downloadYiff(string folderPath, bool convertToIcon)
        {
            try
            {
                // Download all posts once
                if(jsonResponse == null) { 
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FUNNY_API);
                    request.UserAgent = "Yiff Virus (by Imf44#6363)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                    using StreamReader reader = new StreamReader(response.GetResponseStream());

                    jsonResponse = JsonConvert.DeserializeObject(reader.ReadToEnd());
                }

                for (int postIndex = lastPostIndex + 1; postIndex < jsonResponse["posts"].Count; postIndex++)
                {
                    if (!jsonResponse["posts"][postIndex]["rating"].ToString().Equals("e"))
                    {
                        // Skipping SFW posts bc we only want the **VERY HALAL** ONES
                        continue;
                    }

                    string sampleUrl = jsonResponse["posts"][lastPostIndex = postIndex]["sample"]["url"].ToString();
                    int postId = jsonResponse["posts"][postIndex]["id"];

                    string yiffFilePath = (!convertToIcon && folderPath != null) ? folderPath + @"\" + postId + ".jpg" : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + postId + ".jpg";

                    using WebClient client = new WebClient();
                    client.DownloadFile(new Uri(sampleUrl), yiffFilePath);

                    if (convertToIcon)
                    {
                        IcoFromFile(yiffFilePath, folderPath, postId);
                        return $"owo_{postId}";
                    } 
                    else
                    {
                        return sampleUrl;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "owo";
        }

        public void IcoFromFile(string filePath, string folderPath, int postId)
        {
            using Image imageSex = Image.FromFile(filePath);
            using Bitmap bitmap = new Bitmap(imageSex, new Size(256, 256));
            SaveAsIcon(bitmap, folderPath + $"\\owo_{postId}.ico");
        }

        // Taken From: https://stackoverflow.com/a/11448060/368354
        // & Fix from: https://stackoverflow.com/a/14157197
        public static void SaveAsIcon(Bitmap SourceBitmap, string FilePath)
        {
            using FileStream FS = new FileStream(FilePath, FileMode.Create);
            // ICO header
            FS.WriteByte(0); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);

            // Image size
            // Set to 0 for 256 px width/height
            FS.WriteByte(0);
            FS.WriteByte(0);
            // Palette
            FS.WriteByte(0);
            // Reserved
            FS.WriteByte(0);
            // Number of color planes
            FS.WriteByte(1); FS.WriteByte(0);
            // Bits per pixel
            FS.WriteByte(32); FS.WriteByte(0);

            // Data size, will be written after the data
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Offset to image data, fixed at 22
            FS.WriteByte(22);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Writing actual data
            SourceBitmap.Save(FS, System.Drawing.Imaging.ImageFormat.Png);

            // Getting data length (file length minus header)
            long Len = FS.Length - 22;

            // Write it in the correct place
            FS.Seek(14, SeekOrigin.Begin);
            FS.WriteByte((byte)Len);
            FS.WriteByte((byte)(Len >> 8));
            FS.WriteByte((byte)(Len >> 16));
            FS.WriteByte((byte)(Len >> 24));

            FS.Close();
        }
    }
}
