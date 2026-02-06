<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PharmacyManagementSystem.Pages.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pharmacy Login</title>
    <link href="../Styles/login.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <script src="../Scripts/jquery-3.7.0.min.js"></script>
</head>
<body>
    <div class="login-container">
        <div class="login-header">
            <i class="pharmacy-icon fas fa-prescription-bottle-alt"></i>
            <h2>Pharmacy Management System</h2>
        </div>

        <form id="form1" runat="server">
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Style="display: none;"></asp:Label>
  
            <div class="form-group">
                <label class="input-label" for="txtUsername">Username</label>
                <div class="input-container">
                    <i class="input-icon fas fa-user"></i>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-input" placeholder="Enter your username"></asp:TextBox>
                </div>
            </div>

            <div class="form-group">
                <label class="input-label" for="txtPassword">Password</label>
                <div class="input-container">
                    <i class="input-icon fas fa-lock"></i>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-input" placeholder="Enter your password"></asp:TextBox>
                    <button type="button" class="password-toggle" id="togglePassword">
                        <i class="fas fa-eye"></i>
                    </button>
                </div>
            </div>

            <div class="btn-container">
                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
            </div>
        </form>
    </div>
    <script src="../Scripts/Login/login.js"></script>
</body>
</html>
