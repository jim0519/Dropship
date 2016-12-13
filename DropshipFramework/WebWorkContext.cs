using DropshipBusiness.User;
using DropshipCommon.Models;
using System;
using System.Linq;
using System.Web;


namespace DropshipFramework
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string UserCookieName = "Dropship.user";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;

        private T_User _cachedUser;
        
        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            IUserService userService
            )
        {
            this._httpContext = httpContext;
            this._userService = userService;
        }

        #endregion

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected virtual void SetCustomerCookie(string email)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = email;

                int cookieExpires = 24 * 365; //TODO make configurable
                cookie.Expires = DateTime.Now.AddHours(cookieExpires);


                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual T_User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;


                var user = _userService.GetAuthenticatedUser();

                _cachedUser = user;

                return _cachedUser;
            }
            set
            {
                SetCustomerCookie(value.Email);
                _cachedUser = value;
            }
        }

        #endregion
    }
}
