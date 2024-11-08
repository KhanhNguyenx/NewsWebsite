using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsWebsite.Controllers
{
    public abstract class MyBaseController<T> : Controller where T : MyBaseController<T>
    {
        private IMapper mapper;
        private IHttpContextAccessor httpContextAccessor;
        private string accessToken = string.Empty;
        private string accountId = string.Empty;
        private int statusCode = 0;
        protected IMapper _mapper => mapper ?? (mapper = HttpContext?.RequestServices.GetService<IMapper>());
        protected IHttpContextAccessor _httpContextAccessor => httpContextAccessor ?? (httpContextAccessor = HttpContext?.RequestServices.GetService<IHttpContextAccessor>());

        protected string _accessToken => string.IsNullOrEmpty(accessToken) ? (accessToken = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value) : accessToken;

        protected string _accountId => string.IsNullOrEmpty(accountId) ? (accountId = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value) : accountId;

        //protected int _statusCode => statusCode = HttpContext.Session.GetInt32(mySetting.StatusCode) ?? 0 ;

        protected string _httpDomain => @"https://" + HttpContext.Request.Host.Value + @"/";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
