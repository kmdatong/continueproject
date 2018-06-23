using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using WebApplication3.Domains;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class FileMgmtController : Controller
    {
        
        private readonly IRepstoriy<DtFile> _fileMgmt;
        private readonly IHostingEnvironment _hostingEnvironment;
        public FileMgmtController(
            IRepstoriy<DtFile> fileMgmt,
            IHostingEnvironment hostingEnvironment
            )
        {
            _fileMgmt = fileMgmt;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult List()
        {

            var list = _fileMgmt.Quyer().ToList();

            return View(list);
        }

        public ActionResult Add()
        {
            
            return View();
        }
        /// <summary>
        /// 使用formData 上传文件
        /// </summary>
        /// <returns></returns>
        public ActionResult FormDataTest()
        {

            return View();
        }

        [HttpPost]
        public JsonResult UploadFile(List<IFormFile> files)
        {
            string name = Guid.NewGuid().ToString("n");
            string filePath = _hostingEnvironment.WebRootPath+@"/UploadFiles/";

            foreach (var formFile in files)
            {
                string ext = System.IO.Path.GetExtension(formFile.FileName);
                System.IO.Directory.CreateDirectory(filePath);
                if (formFile.Length > 0)
                {
                    string path = filePath + name + ext;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        formFile.CopyToAsync(stream);

                        DtFile entity = new DtFile();
                        entity.FileExt = ext;
                        entity.Length =(Int32) formFile.Length;
                        entity.NewName = name;
                        entity.OldName = formFile.FileName;
                        entity.Path = @"/UploadFiles/" + name + ext;
                        entity.Type = 4;
                        var extlist = FIleTypeServer.GetFileTypeList();
                        foreach (var item in extlist)
                        {
                            if (item.ExtList.Contains(ext.ToLower()))
                            {
                                entity.Type = item.Type;
                                break;
                            }
                               
                        }
                        
                        _fileMgmt.Inserter(entity);
                        _fileMgmt.Save();
                    }
                }
            }

            return Json(new { flag=true});
        }

        public ActionResult Delete(int id)
        {
            var entity = _fileMgmt.GetById(id);
            if (entity == null || string.IsNullOrEmpty(entity.Path))
                return NotFound("文件不存在");

            _fileMgmt.Delete(entity);
            _fileMgmt.Save();

            string filePath = _hostingEnvironment.WebRootPath + @"/"+ entity.Path;

            if(!System.IO.File.Exists(filePath))
                return RedirectToAction("List");

            System.IO.File.Delete(filePath);

           

            return RedirectToAction("List");
        }

        public IActionResult DownFlie(int id)
        {
            var entity = _fileMgmt.GetById(id);
            if (entity == null || string.IsNullOrEmpty(entity.Path))
                return NotFound("文件不存在");

            var path = _hostingEnvironment.WebRootPath + entity.Path;

            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
             {

                 stream.CopyTo(memory);

            }
            memory.Position = 0;
            return File(memory, "application/octet-stream", Path.GetFileName(path));
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="skip"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public  JsonResult FragmentUpload(IFormFile file,int skip,string tempDirectory)
        {
           
            if (string.IsNullOrWhiteSpace(tempDirectory))
            {
                tempDirectory = Guid.NewGuid().ToString("n");
               
            }
            string directory = string.Format(@"{0}/UploadFiles/{1}/", _hostingEnvironment.WebRootPath, tempDirectory);

            if (file.Length > 0)
            {
               
                System.IO.Directory.CreateDirectory(directory);
                string path = directory + skip.ToString();

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

            }
            
            return Json(new { flag=true, tempDirectory = tempDirectory });
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public JsonResult MergeFiles(string oldName, string tempDirectory)
        {
            string ext = System.IO.Path.GetExtension(oldName);
            string filePath = string.Format(@"{0}/UploadFiles/{1}{2}", _hostingEnvironment.WebRootPath, Guid.NewGuid().ToString("n") ,ext);
            string directory = string.Format(@"{0}/UploadFiles/{1}/", _hostingEnvironment.WebRootPath, tempDirectory);
            var fileList = System.IO.Directory.GetFiles(directory)
                // 按照文件名(数字)排序
                .OrderBy(s => int.Parse(System.IO.Path.GetFileNameWithoutExtension(s)))
                .ToList();

            foreach (string itempath in fileList)
            {
                using (var stream = new FileStream(filePath, FileMode.Append,FileAccess.Write))
                {
                  //  System.Threading.Thread.Sleep(1000);
                   byte[] bytes = System.IO.File.ReadAllBytes(itempath);
                    stream.Write(bytes,0,bytes.Length);
                }
                
                    System.IO.File.Delete(itempath);
            }
            Directory.Delete(directory);

            FileInfo fielinfo = new System.IO.FileInfo(filePath);
            DtFile entity = new DtFile();
            entity.FileExt = ext;
            entity.Length = (int)fielinfo.Length;
            entity.NewName = fielinfo.Name;
            entity.OldName = oldName;
            entity.Path = @"/UploadFiles/" + fielinfo.Name;
            entity.Type = 4;
            var extlist = FIleTypeServer.GetFileTypeList();
            foreach (var item in extlist)
            {
                if (item.ExtList.Contains(ext.ToLower()))
                {
                    entity.Type = item.Type;
                    break;
                }

            }

            _fileMgmt.Inserter(entity);
            _fileMgmt.Save();

            return Json(new { flag=true});
        }

        /// <summary>
        /// 断点下载
        /// </summary>
        public void BreakpointDownload(int id)
        {
            var entity = _fileMgmt.GetById(id);
            int begin = 0, end = 0, blackSize = 1*1000*1000;
            string filePath = _hostingEnvironment.WebRootPath + @"/" + entity.Path;

            StringValues range = Response.Headers["Range"]; ;

            if (string.IsNullOrWhiteSpace(range))
            {
                Response.Headers.Add("Range",string.Format("bytes={0}-{1}/{2}",begin,blackSize,entity.Length));
            }
                int fileLength = entity.Length;
              
                Response.ContentType = "application/octet-stream";
                Response.Headers.Add("Content-Disposition", "attachment;filename=" + System.IO.Path.GetFileName(filePath));
                System.Web.http
        }

    }
}