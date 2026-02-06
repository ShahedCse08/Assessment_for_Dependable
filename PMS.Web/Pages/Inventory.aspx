<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inventory.aspx.cs" Inherits="PharmacyManagementSystem.Pages.Inventory" %>

<%@ Register TagPrefix="uc" TagName="Navbar" Src="~/Controls/Navbar.ascx" %>
<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inventory Management</title>
    <link href="../Styles/inventory.css" rel="stylesheet" />
    <link href="../Styles/shared.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">

        <uc:Navbar runat="server" ID="UserNavbar" />

        <main class="inventory-container">

            <div class="page-header" style="display: flex; align-items: center; justify-content: space-between;">
                <h1 class="page-title" style="margin: 0;">Medicine Inventory Management</h1>

                <div class="header-actions">

                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="form-control"
                        Placeholder="Search by Name, Batch"
                        Style="width: 250px;" />

                    <asp:Button ID="btnSearch" runat="server"
                        Text="Search"
                        CssClass="btn btn-secondary"
                        OnClick="btnSearch_Click" />

                    <asp:Button ID="btnExportPdf" runat="server"
                        Text="Export PDF"
                        CssClass="btn btn-pdf"
                        OnClick="btnExportPdf_Click" />

                    <button type="button"
                        class="btn btn-primary"
                        onclick="showAddMedicineModal()">
                        Add Medicine
                    </button>
                </div>
            </div>


            <div class="inventory-card">
                <asp:GridView ID="gvMedicines" runat="server" 
                    AutoGenerateColumns="False" 
                    DataKeyNames="Id" 
                    CssClass="inventory-grid" 
                    AllowPaging="true" 
                    AllowCustomPaging="true" 
                    PagerSettings-Position="Bottom" 
                    PagerSettings-Visible="true" 
                    PageSize="10" 
                    OnPageIndexChanging="gvMedicines_PageIndexChanging" 
                    PagerStyle-CssClass="pager" 
                    PagerSettings-Mode="NumericFirstLast" 
                    PagerSettings-FirstPageText="«" 
                    PagerSettings-LastPageText="»"
                    EmptyDataText="No Data Found"
                    EmptyDataRowStyle-CssClass="empty-grid">

                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Medicine Name" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="BatchNumber" HeaderText="Batch No" ItemStyle-Width="120px" />

                        <asp:TemplateField HeaderText="Expiry Date" ItemStyle-Width="140px">
                            <ItemTemplate>
                                <%# Eval("ExpiryDate", "{0:dd-MMM-yyyy}") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Stock" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <span class='<%# Convert.ToInt32(Eval("Stock")) < 50 ? "stock-low" : "stock-ok" %>'>
                                    <%# Eval("Stock") %>
                    </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Unit Price" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <%# string.Format("{0:N2}", Eval("UnitPrice")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:LinkButton runat="server"
                                    CssClass="btn-action btn-edit"
                                    OnClientClick='<%# "editMedicine(\"" + Eval("Id") + "\"); return false;" %>'
                                    ToolTip="Edit">
                        <i class="fas fa-edit"></i>
                    </asp:LinkButton>

                                <asp:LinkButton runat="server"
                                    CssClass="btn-action btn-delete"
                                    OnClientClick='<%# "deleteMedicine(\"" + Eval("Id") + "\"); return false;" %>'
                                    ToolTip="Delete">
                        <i class="fas fa-trash"></i>
                    </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </main>

        <uc:Footer runat="server" ID="UserFooter" />

        <div id="addMedicineModal" class="modal">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 id="modalTitle">Add New Medicine</h3>
                    <button type="button" class="close-btn" onclick="closeModal()">&times;</button>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="modalMode" value="create" />
                    <input type="hidden" id="currentMedicineId" value="" />
                    <div class="form-container">
                        <div class="form-group">
                            <label for="txtMedName">Medicine Name *</label>
                            <input type="text" id="txtMedName" class="form-control" placeholder="Enter medicine name" />
                            <small class="error-message" id="nameError"></small>
                        </div>
                        <div class="form-row">
                            <div class="form-group half">
                                <label for="txtBatch">Batch Number *</label>
                                <input type="text" id="txtBatch" class="form-control" placeholder="Batch-001" />
                                <small class="error-message" id="batchError"></small>
                            </div>
                            <div class="form-group half">
                                <label for="txtExpiry">Expiry Date *</label>
                                <input type="date" id="txtExpiry" class="form-control" />
                                <small class="error-message" id="expiryError"></small>
                            </div>
                        </div>
                        <div class="form-row">

                            <div class="form-group half">
                                <label for="txtStock">Stock Quantity *</label>
                                <input type="number" id="txtStock" class="form-control" min="0" value="0" />
                                  <small class="error-message" id="stockError"></small>
                            </div>
                            <div class="form-group half">
                                <label for="txtPrice">Unit Price *</label>
                                <input type="number" id="txtPrice" class="form-control" step="0.01" min="0" value="0.00" />
                                <small class="error-message" id="priceError"></small>
                            </div>
                        </div>


                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Cancel</button>
                    <button type="button" id="btnSaveUpdate" class="btn btn-primary" onclick="saveOrUpdateMedicines()">
                        <i class="fas fa-save"></i>Save Medicine
                   
                    </button>
                </div>
            </div>
        </div>

        <script src="../Scripts/Inventory/inventory.js"></script>

    </form>
</body>
</html>
