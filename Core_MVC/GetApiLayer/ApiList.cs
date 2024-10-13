using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core_Common2.ViewModels;

namespace Core_MVC.GetApiLayer
{
    public class ApiList
    {
        private readonly JwtHelper _jwtHelper;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // 注入 Jwt 和 http 實體
        public ApiList(JwtHelper jwtHelper, HttpClient httpClient,string apiUrl, IHttpContextAccessor httpContextAccessor)
        {
            _jwtHelper = jwtHelper;
            _httpClient = httpClient;
            _apiUrl = apiUrl;
            _httpContextAccessor = httpContextAccessor;

			//// 登入前後會都會顯示 token 是 null
			//var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
			//if (!string.IsNullOrEmpty(token))
			//{
			//    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromSession);
			//}
			try
			{
				var tokenFromSession = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
				if (!string.IsNullOrEmpty(tokenFromSession))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromSession);
				}
			}
			catch
			{

			}


		}

		private void SetAuthorizationHeader()
        {
            try
            {
                var tokenFromSession = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
                if (!string.IsNullOrEmpty(tokenFromSession))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromSession);
                }
            }
            catch 
            {

            }

        }


        public async Task<ConnectionApiViewModel?> GetApiConnectionData()
        {
            try
            {
                // SetAuthorizationHeader();

                var response = await _httpClient.GetAsync(_apiUrl+ "/api/Connection/GetConObj");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<ConnectionApiViewModel>();
                    return content;
                    // 處理成功的響應
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return null;
            }
        }



        public async Task<MVC_ConnectionPost_ViewModel?> PostApiConnectionData(MVC_ConnectionPost_ViewModel postData)
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync<MVC_ConnectionPost_ViewModel>(_apiUrl + "/api/Connection/PostConObj", postData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<MVC_ConnectionPost_ViewModel>();
                    // return content;
                    // 處理成功的響應
                    return content;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return null;
            }
        }

        public async Task<string> PostLoginApi(LoginViewModel postData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<LoginViewModel>(_apiUrl + "/api/Login/Login", postData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(); // Token

                    // 將 token 放入 Session 中
                    // _httpContextAccessor.HttpContext.Session.SetString("JwtToken", content);
                    return content;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return null;
            }
        }


        public async Task<FileTransferViewModel?> GetFileStreamData()
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync(_apiUrl + "/api/Connection/SendFile");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<FileTransferDto>();

                    if (content == null)
                    {
                        return null;
                    }
                    string GetFileUrl = content.FileContent;

                    var responseStream = await _httpClient.GetAsync(_apiUrl + content.FileUrl);
                    if (responseStream.IsSuccessStatusCode)
                    {
                        var contentStream = await responseStream.Content.ReadAsStreamAsync();
                        FileTransferViewModel fileTransferViewModel = new FileTransferViewModel();
                        fileTransferViewModel.FileName = content.FileName;
                        fileTransferViewModel.FileContent = content.FileContent;
                        fileTransferViewModel.FileStream = contentStream;

                        return fileTransferViewModel;
                    }

                    return null;


                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return null;
            }
        }



    }
}
