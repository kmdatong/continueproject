using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3
{
    public static class FIleTypeServer
    {
        private static string[] imgExts = { ".bmp", ".dib", ".jpg", ".jpeg", ".jpe", ".jfif", ".png", ".gif", ".tif", ".tiff" };
        private static string[] vodieExt = { "avi", "mp4 ", "mov", "f4v", "mpg" };
        private static string[] musicExt = { ".wav", ".wav", ".mp3", ".mp3" };
        
        public static List<FlieTypeModel> GetFileTypeList()
        {
            List<FlieTypeModel> list = new List<FlieTypeModel>();
            list.Add(new FlieTypeModel { Type =1, ExtList = imgExts });
            list.Add(new FlieTypeModel { Type = 2, ExtList = musicExt });
            list.Add(new FlieTypeModel { Type = 3, ExtList = vodieExt });

            return list;
        }
    }
}
