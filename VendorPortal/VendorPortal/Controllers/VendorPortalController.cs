using VendorPortal.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace VendorPortal.Controllers
{
    public class VendorPortalController : Controller
    {
        // GET: VendorPortal
        public ActionResult Index()
        {
            return View();
        }

        

        public string Login()
        {
            /*
            BL_DB DataAcesslayer = new BL_DB();
            int CompanyId = 122;
            string strErrorMessage = string.Empty;
            string strValue = "";
            strValue = $@"SELECT * FROM ICS_MasterMap";
            clsGeneric.writeLog("Query :" + strValue);
            DataAcesslayer.GetExecute(strValue, CompanyId, ref strErrorMessage);
            if (strErrorMessage != "")
            {
                clsGeneric.writeLog("strErrorMessage :" + strErrorMessage);
            }

            return null;
            */
            //return RedirectToAction("Home", "PendingOrder");
            return "999";
        }
    }
}