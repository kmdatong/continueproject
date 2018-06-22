using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Domains
{
    [Table("DtFile")]
    public class DtFile
    {
        public int Id { get; set; }

        /// <summary>
        /// 源文件名
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExt { get; set; }

        /// <summary>
        /// 文类型 1-图片，2-音频，3-视频，4-其他
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Path { get; set; }
    }
}
