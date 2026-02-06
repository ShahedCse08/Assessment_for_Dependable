using PharmacyManagementSystem.Helpers;
using PMS.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PharmacyManagementSystem.Pages
{
    public partial class Login : Page
    {
        private IUserService _userService;
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            _userService = Global.GetService<IUserService>();
            var user = _userService.GetByUsername(txtUsername.Text.Trim());
         
            if (user == null)
            {
                lblMessage.Text = "Invalid Username or Password";
                return;
            }
            var verifyUser = BCrypt.Net.BCrypt.Verify(txtPassword.Text.Trim(), user.Password);

            if (verifyUser)
            {
                Session["User"] = user.FullName;
                Session["UserId"] = user.Id;
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                lblMessage.Text = "Invalid Username or Password";
            }
        }

     
    }
}