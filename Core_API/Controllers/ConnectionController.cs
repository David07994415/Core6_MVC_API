using Core_Common2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    }
}
