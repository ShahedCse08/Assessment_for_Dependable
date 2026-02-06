<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="PharmacyManagementSystem.Pages.Billing" %>

<%@ Register Src="~/Controls/Navbar.ascx" TagPrefix="uc1" TagName="Navbar" %>
<%@ Register Src="~/Controls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Billing Management - Pharmacy</title>
    <link href="../Styles/billing.css" rel="stylesheet" />
    <link href="../Styles/shared.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <script src="../Scripts/jquery-3.7.0.min.js"></script>
</head>

<body>
    <form id="form1" runat="server">
        <uc1:Navbar runat="server" ID="Navbar" />
        <div class="main-container">
            <div class="page-header">
                <h1 class="page-title">Invoice Management</h1>

                <div class="header-actions">
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="form-control"
                        Placeholder="Search by Invoice No, Customer Name, Contact"
                        Style="width: 350px;" />

                    <asp:Button ID="btnSearch" runat="server"
                        Text="Search"
                        CssClass="btn btn-secondary"
                        OnClick="btnSearch_Click" />

                    <button type="button" class="btn btn-primary" onclick="openCreateModal()">
                        Create Invoice
                    </button>
                </div>
            </div>

            <div class="section-card">
                <div class="grid-container">
                    <asp:GridView ID="gvInvoices" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="invoices-grid"
                        Width="100%"
                        AllowPaging="true"
                        AllowCustomPaging="true"
                        PagerSettings-Position="Bottom"
                        PagerSettings-Visible="true"
                        PageSize="10"
                        OnPageIndexChanging="gvInvoices_PageIndexChanging"
                        PagerStyle-CssClass="pager"
                        PagerSettings-Mode="NumericFirstLast"
                        PagerSettings-FirstPageText="«"
                        PagerSettings-LastPageText="»" 
                        EmptyDataText="No Data Found"
                        EmptyDataRowStyle-CssClass="empty-grid">
                        <Columns>
                            <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" />
                            <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
                            <asp:BoundField DataField="Contact" HeaderText="Contact" />
                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                            <asp:BoundField DataField="GrandTotal" HeaderText="Total Amount" DataFormatString="{0:N2}" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick='<%# "openEditModal(\"" + Eval("Id") + "\"); return false;" %>'
                                        CssClass="btn-action btn-edit" ToolTip="Edit">
<i class="fas fa-edit"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnDelete" runat="server"
                                        CssClass="btn-action btn-delete"
                                        OnClientClick='<%# "deleteInvoice(\"" + Eval("Id") + "\"); return false;" %>' ToolTip="Delete">
<i class="fas fa-trash"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnPrint" runat="server"
                                        CssClass="btn-action btn-print"
                                        OnClick="btnPrintInvoice_Click"
                                        CommandArgument='<%# Eval("Id") %>'
                                        ToolTip="Print Invoice">
<i class="fas fa-print"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

        </div>
        <uc1:Footer runat="server" ID="Footer" />

        <div id="createInvoiceModal" class="modal">

            <div class="modal-content">
                <div class="modal-header">
                    <h2 id="modalTitle">Create New Invoice</h2>
                    <button type="button" class="close-modal" onclick="closeModal()">&times;</button>
                </div>

                <div class="modal-body">
                    <div class="section-card">
                        <input type="hidden" id="modalMode" value="create" />
                        <input type="hidden" id="currentInvoiceId" value="" />
                        <div class="section-header">
                            <h3 class="section-title">Customer Information
                            </h3>

                            <span class="invoice-no">Invoice No: <strong id="modalInvoiceNo"></strong>
                            </span>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">
                                    Customer Name *
                                </label>
                                <input type="text" id="modalCustomerName" class="form-control" placeholder="Enter customer name" />
                                <small class="error-message" id="nameError"></small>
                            </div>

                            <div class="form-group">
                                <label class="form-label">
                                    Contact
                                </label>
                                <input type="text" id="modalContact" class="form-control" placeholder="Phone number" />
                            </div>

                            <div class="form-group">
                                <label class="form-label">
                                    Date
                                </label>
                                <input type="date" id="invoiceDate" class="form-control" />
                            </div>
                        </div>
                    </div>

                    <div class="section-card">
                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                            <h3 style="color: #005792;">Medicine Items
                            </h3>
                            <button type="button" class="btn btn-add" onclick="addMedicineRow()">
                                Add Medicine
                            </button>
                        </div>

                        <table id="modalBillingTable" class="modal-table">
                            <thead>
                                <tr>
                                    <th>Medicine</th>
                                    <th>Batch No</th>
                                    <th>Expiry Date</th>
                                    <th>Quantity</th>
                                    <th>Unit Price</th>
                                    <th>Line Total</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody id="modalBillingTableBody">
                            </tbody>
                        </table>

                        <small class="error-message" id="itemError"></small>
                    </div>

                    <div class="section-card">
                        <h3 style="color: #005792; margin-bottom: 20px;">Invoice Summary
                        </h3>

                        <div class="summary-grid">
                            <div class="summary-item">
                                <div class="summary-label">Subtotal</div>
                                <div class="summary-value"><span id="modalSubTotal">0.00</span></div>
                            </div>

                            <div class="summary-item">
                                <div class="summary-label">Discount (%)</div>
                                <input type="number" id="modalDiscount" class="discount-input" value="0" min="0" max="100"
                                    onchange="calculateModalTotal()" />
                            </div>

                            <div class="summary-item">
                                <div class="summary-label">Discount Amount</div>
                                <div class="summary-value"><span id="modalDiscountAmount">0.00</span></div>
                            </div>

                            <div class="summary-item">
                                <div class="summary-label">Grand Total</div>
                                <div class="summary-value" style="color: #4CAF50; font-size: 22px;">
                                    <span id="modalGrandTotal">0.00</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-cancel" onclick="closeModal()">
                        Cancel
                    </button>
                    <button id="btnSaveText" type="button" class="btn btn-success" onclick="saveOrUpdateInvoice()">
                        Save Invoice
                    </button>
                </div>
            </div>
        </div>

        <script src="../Scripts/Billing/billing.js"></script>

    </form>
</body>
</html>
