<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="PharmacyManagementSystem.Pages.Dashboard" %>

<%@ Register TagPrefix="uc" TagName="Navbar" Src="~/Controls/Navbar.ascx" %>
<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pharmacy Dashboard</title>
    <link href="../Styles/dashboard.css" rel="stylesheet" />
    <link href="../Styles/shared.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
</head>
<body>
    <uc:Navbar runat="server" ID="UserNavbar" />
    <main class="dashboard-container">
        <section class="welcome-section">
            <h2>Dashboard Overview</h2>
            <p>Welcome back to your pharmacy management dashboard. Here you can manage inventory, process billing, view reports, and monitor pharmacy operations.</p>
        </section>
        <div class="dashboard-grid">
            <div class="dashboard-card card-inventory">
                <h3 class="card-title">Inventory Management</h3>
                <p class="card-description">
                    In the Inventory section, you can keep track of medicines, 
                    monitor expiry dates, and manage available quantities so your stock is always up to date.
                </p>
                <div class="card-action">
                    <a href="Inventory.aspx" class="card-btn">
                        Access Inventory
                    </a>
                </div>
            </div>
            <div class="dashboard-card card-billing">
                <h3 class="card-title">Billing & Invoicing</h3>
                <p class="card-description">
                    The Billing section lets you create invoices with multiple medicines in a single transaction.
  You can enter customer details, add or remove items dynamically, 
  and the system will automatically calculate totals and discounts in real time. 
  Before saving, stock availability is checked to ensure accuracy and prevent overselling.
                </p>
                <div class="card-action">
                    <a href="Billing.aspx" class="card-btn">
                        Create Invoice
                    </a>
                </div>
            </div>

        </div>
    </main>
    <uc:Footer runat="server" ID="UserFooter" />
</body>
</html>
