using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExternalWebApiConsumingPortalOAuthTokenSample.App_Start;

namespace ExternalWebApiConsumingPortalOAuthTokenSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.AllowedPortals = DynamicsPortalBearerAuthenticationProvider.GetAllowedPortal();
            ViewBag.AllowedPortalsSigningKeyPlainText =
                DynamicsPortalBearerAuthenticationProvider.GetAllowedPortalSigningKeyPlainText();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "A Sample demonstrating how an external web api can be authorized against a Dynamics Portal's Implict Code Flow OAuth Token";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}