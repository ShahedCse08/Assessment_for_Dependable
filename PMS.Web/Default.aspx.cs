using System;
using System.Web.UI;

namespace PharmacyManagementSystem
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null && Session["User"].ToString() != "")
                {
                    Response.Redirect("~/Pages/Dashboard.aspx");
                }
                else
                {
                    Response.Redirect("~/Pages/Login.aspx");
                }
            }
        }
    }
}