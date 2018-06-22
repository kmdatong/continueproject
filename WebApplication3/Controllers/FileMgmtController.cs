using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult UploadFile(List<IFormFile> files)
        {
            string name = Guid.NewGuid().ToString("n");
            string filePath = _hostingEnvironment.WebRootPath+@"/Files/";

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
                        entity.Path = @"/Files/"+ name + ext;
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

            return RedirectToAction("List");
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
    }
}