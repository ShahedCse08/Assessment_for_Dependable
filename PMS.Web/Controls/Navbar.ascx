<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navbar.ascx.cs" Inherits="PharmacyManagementSystem.Controls.Navbar" %>

<header class="dashboard-header">
<div class="header-content">
        <div class="logo-section">
            <i class="pharmacy-icon fas fa-prescription-bottle-alt"></i>
            <h1>Pharmacy Management System</h1>
        </div>
        <div class="nav-links">

            <a href="Dashboard.aspx" class="nav-link">Dashboard</a>

            <a href="Inventory.aspx" class="nav-link">Inventory</a>

            <a href="Billing.aspx" class="nav-link">Billing</a>

        </div>
        <div class="user-section">
            <div class="user-info">
                <div class="user-name"><%= Session["User"].ToString() %></div>
            </div>
            <a href="Logout.aspx" class="logout-btn">
                <i class="fas fa-sign-out-alt"></i>Logout
            </a>
        </div>
    </div>
</header>

<div id="globalToaster" class="toaster">
    <span id="toasterMessage"></span>
</div>

<script type="text/javascript">
    window.showToast = function (message, isError = false) {
        const toaster = document.getElementById('globalToaster');
        const msgSpan = document.getElementById('toasterMessage');
        if (!toaster || !msgSpan) {
            console.error("Toaster elements not found in DOM");
            return;
        }
        msgSpan.innerText = message;
        toaster.style.backgroundColor = isError ? "#d32f2f" : "#4BB543";
        toaster.classList.add('show');
        setTimeout(() => {
            toaster.classList.remove('show');
        }, 3000);
    };

    document.addEventListener("DOMContentLoaded", function () {
        const pendingMsg = localStorage.getItem("toastMessage");
        const pendingType = localStorage.getItem("toastType");
        if (pendingMsg) {
            showToast(pendingMsg, pendingType === "error");
            localStorage.removeItem("toastMessage");
            localStorage.removeItem("toastType");
        }
    });

</script>
