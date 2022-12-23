using MedContactCore.DataTransferObjects;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MedContactApp.Helpers
{
    public class FileValidation
    {
        private readonly IConfiguration _configuration;
        private int _maxFileSize = 5 * 1024 * 1024;
        private Dictionary<string, string> _extensions = new Dictionary<string, string>()
        { {"0",".jpg" },{"1",".png" },{"2",".tif" },{"3",".doc" }, 
          {"4",".docx" },{"5",".pdf" },{"6",".txt" },{"7",".rtf" }};
        public FileValidation(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Result FileSizeValidation(IFormFileCollection files)
        {
            bool result = int.TryParse(_configuration["FileValidation:MaxSizeMB"], out var fileSize);
            if (result) _maxFileSize = fileSize*1024*1024;

            if (files != null && files.Any(f => f.Length > _maxFileSize))
                    return new Result() { IntResult = 0, Name = $"<br/><b>Maximum allowed file size is {_maxFileSize / 1024 / 1024} MB.</b>" };

            return new Result() { IntResult = 1 };

        }

        public Result SingleFileSizeValidation(IFormFile file)
        {
            bool result = int.TryParse(_configuration["FileValidation:MaxSizeMB"], out var fileSize);
            if (result) _maxFileSize = fileSize * 1024 * 1024;

            if (file != null && file.Length > _maxFileSize)
                return new Result() { IntResult = 0, Name = $"<b>Maximum allowed file size is {_maxFileSize / 1024 / 1024} MB.</b>" };

            return new Result() { IntResult = 1 };

        }
        public Result FileExtValidation(IFormFileCollection files)
        {
            var extDic = _configuration.GetSection("FileExtensions")
                            .Get<IDictionary<string, string>>();

            if (extDic!=null && extDic.Any()) _extensions = (Dictionary<string, string>)extDic;

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.ContainsValue(extension.ToLower()))
                    {
                        return new Result() { IntResult = 0, Name = $"<br/><b>You can upload only  pdf, doc, txt, rtf, docx and image files.</b>" };
                    }
                }
            }

            return new Result() { IntResult = 1 };
        }

        public Result SingleFileExtValidation(IFormFile file)
        {
            var extDic = _configuration.GetSection("FileExtensions")
                            .Get<IDictionary<string, string>>();

            if (extDic != null && extDic.Any()) _extensions = (Dictionary<string, string>)extDic;

            if (file != null)
            {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.ContainsValue(extension.ToLower()))
                    {
                        return new Result() { IntResult = 0, Name = $"<b>You can upload only  pdf, doc, txt, rtf, docx and image files.</b>" };
                    }
                
            }

            return new Result() { IntResult = 1 };
        }
    }
}
