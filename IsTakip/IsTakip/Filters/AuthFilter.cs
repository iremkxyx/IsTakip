using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakip.Filters
{
    public class AuthFilter : FilterAttribute, IAuthorizationFilter
    {
        //bizim yetki tür bilgisini alıcak ve bu bilgiye göre
        //devam edip etmiyceğine bakcıak
        //filterCıntext ile session bilgisine erişebileceğiz
        protected int yetkiTur;
        public AuthFilter(int yetkiTur)
        {
            this.yetkiTur = yetkiTur;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            int yetkiTurId = Convert.ToInt32(filterContext.HttpContext.Session["PersonelYetkiTurId"]);

            if(this.yetkiTur != yetkiTurId)
            {                                            //controller, action
                filterContext.Result = new RedirectResult("/Login/Index");

            }
        }
    }
}