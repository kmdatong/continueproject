using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class FlieTypeModel
    {
        /// <summary>
        /// 文类型 1-图片，2-音频，3-视频，4-其他
        /// </summary>
        public int Type { get; set; }

        public string[] ExtList { get; set; }
    }
}
