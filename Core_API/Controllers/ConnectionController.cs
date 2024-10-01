using Core_Common2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Core_API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {

        [Authorize] // 驗證 Token
        [HttpGet]
        // [Route("GetConObj")]
        public IActionResult GetConObj()
        {
            ConnectionApiViewModel connectionApiViewModel = new ConnectionApiViewModel();
            connectionApiViewModel.Id = 1;
            connectionApiViewModel.name = "David";
            connectionApiViewModel.ConnectionApiObj = new ConnectionApiObj();
            connectionApiViewModel.ConnectionApiObj.DateTime = DateTime.Now;
            connectionApiViewModel.ConnectionApiObj.boo = true;


            return Ok(connectionApiViewModel);
        }

        [Authorize] // 驗證 Token
        [HttpPost]
        public async Task<IActionResult> PostConObj([FromBody] MVC_ConnectionPost_ViewModel connectionData)
        {
            // 檢查是否接收到有效數據
            if (connectionData == null)
            {
                return BadRequest("接收到的數據無效。");
            }

            // 檢查模型狀態是否有效
            if (!ModelState.IsValid)
            {
                // 可以返回具體的錯誤信息
                return BadRequest(ModelState);
            }

            // 如果模型有效，進行後續處理
            // 例如：儲存資料到資料庫
            // await _dbContext.Connections.AddAsync(connectionData);
            // await _dbContext.SaveChangesAsync();

            return Ok(connectionData); // 返回成功的響應
        }

        [Authorize] // 驗證 Token
        [HttpGet]
        public async Task<IActionResult> SendFile()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "table_example.pdf");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // 如果檔案不存在，返回 404
            }

            // 取得檔案類型
            string contentType = "";
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out  contentType))
            {
                contentType = "application/octet-stream";  // 預設內容類型
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            FileTransferDto fileTransferDto = new FileTransferDto();
            fileTransferDto.FileName = "table_example.pdf";
            fileTransferDto.FileContent = contentType;
            fileTransferDto.FileUrl = Url.Action("DownloadSend", new { fileName = Path.GetFileName(filePath) });      // 用於下載檔案的 URL

            //return new FileStreamResult(stream, contentType)
            //{
            //    FileDownloadName = Path.GetFileName(filePath) // 可選，設置下載時的檔名
            //};

            return Ok(fileTransferDto);

        }

        // 另一個方法來處理下載
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadSend(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // 如果檔案不存在，返回 404
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = "application/octet-stream"; // 預設內容類型
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);


            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = Path.GetFileName(filePath) // 可選，設置下載時的檔名
            };
        }


    }
}
