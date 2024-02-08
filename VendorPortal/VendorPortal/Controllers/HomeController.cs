using VendorPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Xml.Linq;
using System.Xml;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Net;

/*
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
*/

namespace VendorPortal.Controllers
{
    public class HomeController : Controller
    {
        BL_DB focus_db = new BL_DB();
        public static string connection = "";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult PendingOrder()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.userID = Session["UserID"];

            return View();
        }
        public ActionResult GRNEntry()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult MRNEntry()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Login(string user_name, string password)
        {
            //return RedirectToAction("Home", "PendingOrder");
            /*
            string strErrorMessage = "";
            string strQuery = "select * from ICS_ProductMap where destination IN ('sName', 'sCode', 'iDefaultBaseUnit__Id', 'ParentId')";
            DataSet ds = focus_db.GetData(strQuery, CompanyId, ref strErrorMessage);

            if (ds.Tables[0].Rows.Count < 4)
            {
                //string error_msg = "Mandatory Fields Not Match!";
                //return Json(new { status = false, data = new { message = error_msg } });
            }
            */


            //string path = Server.MapPath("~/App_Data/somedata.xml");
            string path = Server.MapPath("~/bin/XMLFiles/DBConfig.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNodeList bookNodes = xmlDoc.SelectNodes("//DatabaseConfig/Database/Data_Source");



            //var xml = XmlDocument.Load(@path);

            //Session["UserID"] = path;
            //Session["UserID"] = xml.ToString();

            //XmlNode node = xmlDoc.SelectSingleNode("xmlRootNode/levelOneChildNode");
            XmlNode node = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source");
            //string text = node.InnerText;
            string serverName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source").InnerText;
            string databaseName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Initial_Catalog").InnerText;
            string userId = xmlDoc.SelectSingleNode("DatabaseConfig/Database/User_Id").InnerText;
            string Password = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Password").InnerText;

            Session["API_UserName"] = xmlDoc.SelectSingleNode("DatabaseConfig/Database/API_UserName").InnerText;
            Session["API_Password"] = xmlDoc.SelectSingleNode("DatabaseConfig/Database/API_Password").InnerText;
            Session["API_CompanyCode"] = databaseName.Substring(databaseName.Length - 3);


            //Session["UserID"] = serverName + databaseName + userId + password;
            //Session["UserID"] = "session is set";
            //Session["UserID"] = bookNodes.Item(1).Value.ToString();



            var con = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User Id=" + userId + ";Password=" + Password + ";Integrated Security=True;Trusted_Connection=false;";
            connection = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User Id=" + userId + ";Password=" + Password + ";Integrated Security=True;Trusted_Connection=false;";
            string returnData = "";
            bool status = false;
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                string oString = "Select * from ICS_VendorPortal_Login where user_name=@user_name AND password=@password";
                //string oString = "Select * from ICS_VendorPortal_Login where user_name='"+ user_name + "' AND password='"+ password + "'";

                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                oCmd.Parameters.AddWithValue("@user_name", user_name);
                oCmd.Parameters.AddWithValue("@password", password);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    if (oReader.Read())
                    {
                        Session["UserID"] = oReader["party_master_id"];
                        Session["UserName"] = oReader["user_name"];
                        returnData = "Successfully Login";
                        status = true;
                    }
                    else
                    {
                        returnData = "Failed Login";
                    }
                    myConnection.Close();
                }
            }


            return Json(new { status = status, data = new { message = returnData } });
            //return returnData;
        }

        public static string ConvertIntoJson(DataTable dt)
        {
            var jsonString = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonString.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                        jsonString.Append("\"" + dt.Columns[j].ColumnName + "\":\""
                            + dt.Rows[i][j].ToString().Replace('"', '\"') + (j < dt.Columns.Count - 1 ? "\"," : "\""));

                    jsonString.Append(i < dt.Rows.Count - 1 ? "}," : "}");
                }
                return jsonString.Append("]").ToString();
            }
            else
            {
                return "[]";
            }
        }

        [HttpPost]
        public ActionResult FillGRN()
        {
            string json_return = "";
            string sql = "select ICS_VendorPortal_PendPo.*, mCore_Product.sName as item_name, mCore_Department.sName as branch_name, cast(tCore_Indta_0.mRate as decimal(18,3)) as PORate from ICS_VendorPortal_PendPo inner join mCore_Product on mCore_Product.iMasterId = ICS_VendorPortal_PendPo.Item_Id inner join mCore_Department on mCore_Department.iMasterId = ICS_VendorPortal_PendPo.Branch_Id inner join tCore_Indta_0 on ICS_VendorPortal_PendPo.iBodyId=tCore_Indta_0.iBodyId where ICS_VendorPortal_PendPo.User_Id = " + Session["UserID"];
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                SqlCommand oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                json_return = ConvertIntoJson(dataTable);
                myConnection.Close();
            }

            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult FillBranch()
        {
            string json_return = "";
            string sql = "select iMasterId, sName from mCore_Department";
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                SqlCommand oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                json_return = ConvertIntoJson(dataTable);
                myConnection.Close();
            }
            //return JObject.Parse(json_return);
            //return Json(new { status = true, data = new { message = json_return } });


            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpPost]
        public ActionResult FillAccounts()
        {
            string path = Server.MapPath("~/bin/XMLFiles/DBConfig.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
           
            XmlNode node = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source");            
            string serverName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source").InnerText;
            string databaseName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Initial_Catalog").InnerText;
            string userId = xmlDoc.SelectSingleNode("DatabaseConfig/Database/User_Id").InnerText;
            string Password = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Password").InnerText;
            
            var con = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User Id=" + userId + ";Password=" + Password + ";Integrated Security=True;Trusted_Connection=false;";

            string json_return = "";
            string sql = "SELECT iMasterId as Id, sName as Name FROM mCore_Account where iMasterId NOT IN (118)";
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                SqlCommand oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                json_return = ConvertIntoJson(dataTable);
                myConnection.Close();
            }
           
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult Register_User(string UserName, string Password, int PartyId)
        {
            string json_return = "Data Inserted Successfully";

            string path = Server.MapPath("~/bin/XMLFiles/DBConfig.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNode node = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source");
            string serverName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Data_Source").InnerText;
            string databaseName = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Initial_Catalog").InnerText;
            string userId = xmlDoc.SelectSingleNode("DatabaseConfig/Database/User_Id").InnerText;
            string dbPassword = xmlDoc.SelectSingleNode("DatabaseConfig/Database/Password").InnerText;

            var con = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";User Id=" + userId + ";Password=" + dbPassword + ";Integrated Security=True;Trusted_Connection=false;";


            using (SqlConnection myConnection = new SqlConnection(con))
            {                
                SqlCommand oCmd = new SqlCommand("SELECT * FROM ICS_VendorPortal_Login WHERE user_name='"+ UserName + "' AND party_master_id="+ PartyId + "  ", myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                if (dataTable.Rows.Count > 1)
                {
                    json_return = "User Already Register";
                }
                else
                {
                    string sql = "INSERT INTO ICS_VendorPortal_Login (user_name, password, party_master_id) VALUES (@user_name, @password, @party_master_id)";
                    using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                    {
                        cmd.Parameters.AddWithValue("@user_name", UserName);
                        cmd.Parameters.AddWithValue("@password", Password);
                        cmd.Parameters.AddWithValue("@party_master_id", PartyId);

                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        json_return = "User Register Successfully";
                    }
                }
                
                myConnection.Close();
            }            

            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;           
        }

        [HttpPost]
        public ActionResult GetPO()
        {
            string json_return = "";
            string sql = "select vIcs_PndngLstNew.*, mCore_Department.sName[BranchName] from vIcs_PndngLstNew inner join mCore_Department on vIcs_PndngLstNew.Branch = mCore_Department.iMasterId where Status In('Pending', 'Partial Consumed') and Account = " + Session["UserID"];
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                SqlCommand oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                json_return = ConvertIntoJson(dataTable);
                myConnection.Close();
            }
            //return JObject.Parse(json_return);
            //return Json(new { status = true, data = new { message = json_return } });


            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult DeletePO()
        {
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();

                string sql = "delete from ICS_VendorPortal_PendPo where User_Id=@User_Id";

                using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                {
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                myConnection.Close();
            }

            return Json(new { status = true, data = new { message = "Delete Successfully" } });
        }


        [HttpPost]
        public ActionResult ValidatePO()
        {
            string return_msg = "";
            bool status = true;
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                string validate = "select Vtype,DocNo  from ICS_VendorPortal_PendPo Group By Vtype,DocNo";
                SqlCommand oCmd = new SqlCommand(validate, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                if (dataTable.Rows.Count > 1)
                {
                    return_msg = "Muliple PO Not Allowed";
                    status = false;
                }

                myConnection.Close();
            }



            return Json(new { status = status, data = new { message = return_msg } });
        }
        

        [HttpPost]
        public ActionResult SavePendPo(string Vtype, string DocNo, int Branch_Id, int Item_Id, decimal Pend_Qty, int IbodyId)
        {

            string json_return = "Data Inserted Successfully";

            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();


                string sql = "INSERT INTO ICS_VendorPortal_PendPo (iBodyId, Vtype ,DocNo ,Branch_Id ,Item_Id ,Pend_Qty ,User_Id ,Session_Id) VALUES (@iBodyId, @Vtype ,@DocNo ,@Branch_Id ,@Item_Id ,@Pend_Qty ,@User_Id ,@Session_Id)";
                using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                {
                    cmd.Parameters.AddWithValue("@iBodyId", IbodyId);
                    cmd.Parameters.AddWithValue("@Vtype", Vtype);
                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
                    cmd.Parameters.AddWithValue("@Branch_Id", Branch_Id);
                    cmd.Parameters.AddWithValue("@Item_Id", Item_Id);
                    cmd.Parameters.AddWithValue("@Pend_Qty", Pend_Qty);
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@Session_Id", Session.SessionID);

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }
            //return JObject.Parse(json_return);
            //return Json(new { status = true, data = new { message = json_return } });


            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            /*var jsonResult = Json(DocNo, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;*/
        }


        [HttpPost]
        public ActionResult SaveGRN(string DCNo, string DCDate, string BillNo, string BillDate, int IBody, decimal Pend_Qty, decimal PORate, decimal ChallanQty, decimal ChallanRate)
        {

            string json_return = "Data Inserted Successfully";

            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();


                string sql = "INSERT INTO ICS_VendorPortal_GRN (DCNo, DCDate, BillNo, BillDate, iBodyid, POQty, ChallanQty, PORate, ChallanRate, User_Id, Session_Id) VALUES (@DCNo, dbo.DateToInt(@DCDate), @BillNo, dbo.DateToInt(@BillDate), @iBodyid, @POQty, @ChallanQty, @PORate, @ChallanRate, @User_Id, @Session_Id)";
                using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                {
                    cmd.Parameters.AddWithValue("@DCNo", DCNo);
                    cmd.Parameters.AddWithValue("@DCDate", DCDate);
                    cmd.Parameters.AddWithValue("@BillNo", BillNo);
                    cmd.Parameters.AddWithValue("@BillDate", BillDate);
                    cmd.Parameters.AddWithValue("@iBodyid", IBody);
                    cmd.Parameters.AddWithValue("@POQty", Pend_Qty);
                    cmd.Parameters.AddWithValue("@ChallanQty", ChallanQty);
                    cmd.Parameters.AddWithValue("@PORate", PORate);
                    cmd.Parameters.AddWithValue("@ChallanRate", ChallanRate);
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@Session_Id", Session.SessionID);

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }
            //return JObject.Parse(json_return);
            //return Json(new { status = true, data = new { message = json_return } });


            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            /*var jsonResult = Json(DocNo, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;*/
        }

        public string GetSession(String ServerAddress, string UserName, string Password, string CompanyID)
        {
            HashData objHashRequest = new HashData();
            Hashtable objLogin = new Hashtable();
            string session = "";
            objLogin.Add("UserName", UserName);
            objLogin.Add("Password", Password);
            objLogin.Add("CompanyCode", CompanyID);
            List<Hashtable> lstHash = new List<Hashtable>();
            lstHash.Add(objLogin);
            objHashRequest.data = lstHash;
            string sContent = JsonConvert.SerializeObject(objHashRequest);
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                string sUrl = "http://" + ServerAddress + "/Focus8API/Login";
                string strResponse = client.UploadString(sUrl, sContent);

                var responseDataCENT = JsonConvert.DeserializeObject<APIResponse.PostResponse>(strResponse);
                if (responseDataCENT.result == -1)
                {
                    //return Json(new { status = false, data = new { message = responseDataCENT.message } });
                }
                else
                {
                    if (responseDataCENT.data.Count != 0)
                    {
                        session = responseDataCENT.data[0]["fSessionId"].ToString();
                    }
                }
            }



            return session;
        }


        [HttpPost]
        public ActionResult SaveMRN(string DCNo, string DCDate, string BillNo, string BillDate, int IBody, decimal Pend_Qty, decimal PORate, decimal ChallanQty, decimal ChallanRate)
        {

            string json_return = "Data Inserted Successfully";

            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();


                string sql = "INSERT INTO ICS_VendorPortal_MRN (DCNo, DCDate, BillNo, BillDate, iBodyid, POQty, ChallanQty, PORate, ChallanRate, User_Id, Session_Id) VALUES (@DCNo, dbo.DateToInt(@DCDate), @BillNo, dbo.DateToInt(@BillDate), @iBodyid, @POQty, @ChallanQty, @PORate, @ChallanRate, @User_Id, @Session_Id)";
                using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                {
                    cmd.Parameters.AddWithValue("@DCNo", DCNo);
                    cmd.Parameters.AddWithValue("@DCDate", DCDate);
                    cmd.Parameters.AddWithValue("@BillNo", BillNo);
                    cmd.Parameters.AddWithValue("@BillDate", BillDate);
                    cmd.Parameters.AddWithValue("@iBodyid", IBody);
                    cmd.Parameters.AddWithValue("@POQty", Pend_Qty);
                    cmd.Parameters.AddWithValue("@ChallanQty", ChallanQty);
                    cmd.Parameters.AddWithValue("@PORate", PORate);
                    cmd.Parameters.AddWithValue("@ChallanRate", ChallanRate);
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@Session_Id", Session.SessionID);

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }
            //return JObject.Parse(json_return);
            //return Json(new { status = true, data = new { message = json_return } });


            //var jsonResult = Json(JObject.Parse(json_return), "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            /*var jsonResult = Json(DocNo, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;*/
        }

        [HttpPost]
        public ActionResult PostMRN()
        {
            string json_return = "Data Inserted Successfully";

            var dataTable = new DataTable();
            string sql = "select ICS_VendorPortal_MRN.* , tCore_Data_0.iBookNo, tCore_Data_0.iFaTag, tCore_Data_0.iInvTag,"
                        + " tCore_Indta_0.iProduct, tCore_Indta_0.iUnit "
                        +" from" 
                        +" ICS_VendorPortal_MRN" 
                        +" inner join tCore_Data_0 on ICS_VendorPortal_MRN.iBodyId = tCore_Data_0.iBodyId" 
                        + " inner join tCore_Indta_0 on ICS_VendorPortal_MRN.iBodyId = tCore_Indta_0.iBodyId";
            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                SqlCommand oCmd = new SqlCommand(sql, myConnection);
                myConnection.Open();

                var dataReader = oCmd.ExecuteReader();
                
                dataTable.Load(dataReader);

                json_return = ConvertIntoJson(dataTable);
                myConnection.Close();
            }


          
            foreach (DataRow row in dataTable.Rows)
            {
                Hashtable headerData = new Hashtable();
                List<System.Collections.Hashtable> bodyDataList = new List<System.Collections.Hashtable>();
                Hashtable bodyData = new Hashtable();

                headerData.Add("DocNo", "");
                headerData.Add("VendorAC__Id", row["iBookNo"].ToString());
                headerData.Add("Branch__Id", row["iFaTag"].ToString());

                headerData.Add("PurchaseAC__Id", 7);

                headerData.Add("Warehouse__Id", row["iInvTag"].ToString());
                //headerData.Add("Date", 132582417);
                //headerData.Add("sNarration", "123");



                bodyData.Add("Item__Id", row["iProduct"].ToString());
                bodyData.Add("Unit__Id", row["iUnit"].ToString());
                bodyData.Add("Quantity", row["ChallanQty"].ToString());
                bodyData.Add("Rate", row["ChallanRate"].ToString());
                //bodyData.Add("Gross", 6250.0000000000);

                string batchNo = row["iProduct"].ToString() + "/" + row["ChallanQty"].ToString();

                Hashtable bodyBatch = new Hashtable
                                    {
                                        //{"BatchId", batchID }
                                        //{"BatchNo",  Convert.ToString(extHeader["GRNBatchNo"]) }
                                        {"BatchNo",  batchNo }//,
                                        //{"BatchRate", batchRate}
                                    };
                bodyData.Add("Batch", bodyBatch);
               
                bodyDataList.Add(bodyData);

                System.Collections.Hashtable objHash = new System.Collections.Hashtable();
                objHash.Add("Body", bodyDataList);
                objHash.Add("Header", headerData);

                List<System.Collections.Hashtable> lstHash = new List<System.Collections.Hashtable>();
                lstHash.Add(objHash);
                HashData objHashRequest = new HashData();
                objHashRequest.data = lstHash;
                string sContentCBROD = JsonConvert.SerializeObject(objHashRequest);
                clsGeneric.writeLog("Content CBROD: " + sContentCBROD);
                using (var clientCBROD = new WebClient())
                {
                    clientCBROD.Encoding = Encoding.UTF8;
                    clientCBROD.Headers.Add("fSessionId", GetSession("localhost", Session["API_UserName"].ToString(), Session["API_Password"].ToString(), Session["API_CompanyCode"].ToString()));
                    clientCBROD.Headers.Add("Content-Type", "application/json");
                    var responseCBROD = clientCBROD.UploadString("http://localhost/Focus8API/Transactions/Vouchers/1280", sContentCBROD);
                    clsGeneric.writeLog("response CBROD: " + responseCBROD);
                    if (responseCBROD != null)
                    {
                        var responseDataCBROD = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCBROD);

                    }
                }                
            }

            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();

                string delete = "delete from ICS_VendorPortal_MRN where User_Id=@User_Id";

                using (SqlCommand cmd = new SqlCommand(delete, myConnection))
                {
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                myConnection.Close();
            }

            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpPost]
        public ActionResult PostGRN()
        {
            string json_return = "Data Inserted Successfully";




            Hashtable headerData = new Hashtable();
            List<System.Collections.Hashtable> bodyDataList = new List<System.Collections.Hashtable>();
            Hashtable bodyData = new Hashtable();


            headerData.Add("PurchaseAC__Id", 80);

            headerData.Add("DocNo", "");
            headerData.Add("VendorAC__Id", 1160);
            headerData.Add("Branch__Id", 12);
            headerData.Add("Warehouse__Id", 188);
            headerData.Add("Date", 132582417);
            headerData.Add("sNarration", "123");



            bodyData.Add("Item__Id", 9497);
            bodyData.Add("Unit__Id", 3);
            bodyData.Add("Quantity", 25.0000000000);
            bodyData.Add("Rate", 250.0000000000);
            bodyData.Add("Gross", 6250.0000000000);
            //bodyData.Add("Gross", 9497);


            Hashtable bodyBatch = new Hashtable
                                    {
                                        //{"BatchId", batchID }
                                        //{"BatchNo",  Convert.ToString(extHeader["GRNBatchNo"]) }
                                        {"BatchNo",  "123" }//,
                                        //{"BatchRate", batchRate}
                                    };
            bodyData.Add("Batch", bodyBatch);


            Hashtable bom = new Hashtable
                                    {                                       
                                        {"Input",  0.00742300 },
                                        {"FieldName",  "BOM Qty" },
                                        {"Value",  0.00742300 },
                                        {"ColMap",  0 }
                                    };
            bodyData.Add("BOM Qty", bom);



            bodyDataList.Add(bodyData);




            System.Collections.Hashtable objHash = new System.Collections.Hashtable();
            objHash.Add("Body", bodyDataList);
            objHash.Add("Header", headerData);

            List<System.Collections.Hashtable> lstHash = new List<System.Collections.Hashtable>();
            lstHash.Add(objHash);
            HashData objHashRequest = new HashData();
            objHashRequest.data = lstHash;
            string sContentCBROD = JsonConvert.SerializeObject(objHashRequest);
            clsGeneric.writeLog("Content CBROD: " + sContentCBROD);            
            using (var clientCBROD = new WebClient())
            {
                clientCBROD.Encoding = Encoding.UTF8;
                clientCBROD.Headers.Add("fSessionId", "171220231616571686841");
                clientCBROD.Headers.Add("Content-Type", "application/json");
                var responseCBROD = clientCBROD.UploadString("http://localhost/Focus8API/Transactions/Vouchers/1280", sContentCBROD);
                clsGeneric.writeLog("response CBROD: " + responseCBROD);
                if (responseCBROD != null)
                {
                    var responseDataCBROD = JsonConvert.DeserializeObject<APIResponse.PostResponse>(responseCBROD);
                    
                }

            }

            using (SqlConnection myConnection = new SqlConnection(connection))
            {
                myConnection.Open();


                string sql = "INSERT INTO ICS_VendorPortal_GRN (DCNo, DCDate, BillNo, BillDate, iBodyid, POQty, ChallanQty, PORate, ChallanRate, User_Id, Session_iD) VALUES (@DCNo, dbo.DateToInt(@DCDate), @BillNo, dbo.DateToInt(@BillDate), @iBodyid, @POQty, @ChallanQty, @PORate, @ChallanRate, @User_Id, @Session_iD)";
                using (SqlCommand cmd = new SqlCommand(sql, myConnection))
                {
                   /* cmd.Parameters.AddWithValue("@DCNo", DCNo);
                    cmd.Parameters.AddWithValue("@DCDate", DCDate);
                    cmd.Parameters.AddWithValue("@BillNo", BillNo);
                    cmd.Parameters.AddWithValue("@BillDate", BillDate);
                    cmd.Parameters.AddWithValue("@iBodyid", IBody);
                    cmd.Parameters.AddWithValue("@POQty", Pend_Qty);
                    cmd.Parameters.AddWithValue("@ChallanQty", ChallanQty);
                    cmd.Parameters.AddWithValue("@PORate", PORate);
                    cmd.Parameters.AddWithValue("@ChallanRate", ChallanRate);
                    cmd.Parameters.AddWithValue("@User_Id", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@Session_Id", Session.SessionID);*/

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                myConnection.Close();
            }
            
            var jsonResult = Json(json_return, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


    }
}