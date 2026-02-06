using PharmacyManagementSystem.Helpers;
using PMS.Application.DTOs;
using PMS.Application.Services;
using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PharmacyManagementSystem.Pages
{
    public partial class Billing : Page
    {
        private ISalesService _salesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _salesService = Global.GetService<ISalesService>();

            if (!IsPostBack)
            {
                if (Session["User"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                var salesRequest = new SalesQueryRequestDto
                {
                    SearchKeyword = "",
                    PageIndex = 0,
                    PageSize = gvInvoices.PageSize,
                   
                };
                BindInvoicesGrid(salesRequest);

            }
        }

        [WebMethod]
        public static string GenerateInvoiceNumber()
        {
            var salesService = Global.GetService<ISalesService>();
            var invoiceNumber = salesService.GenerateInvoiceNumber();
            return invoiceNumber;
        }

        [WebMethod]
        public static object GetMedicines()
        {
            var salesService = Global.GetService<ISalesService>();
            var list = salesService.GetAllMedicines();
            return list;
        }

        [WebMethod]
        public static object GetInvoiceData(int invoiceId)
        {
            var salesService = Global.GetService<ISalesService>();
            var invoice = salesService.GetById(invoiceId);
            return invoice;
        }
        private void BindInvoicesGrid(SalesQueryRequestDto request)
        {
                var result = _salesService.GetSalesWithPagination(request);
                gvInvoices.VirtualItemCount = result.TotalCount;
                gvInvoices.DataSource = result.Sales; 
                gvInvoices.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var request = new SalesQueryRequestDto
            {
                SearchKeyword = txtSearch.Text?.Trim(),
                PageIndex = 0, 
                PageSize = gvInvoices.PageSize
               
            };

            BindInvoicesGrid(request);
        }
        protected void gvInvoices_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInvoices.PageIndex = e.NewPageIndex;

            var request = new SalesQueryRequestDto
            {
                SearchKeyword = "",//txtInvoiceSearch.Text?.Trim()
                PageIndex = e.NewPageIndex, // Note: 0-based for DTO
                PageSize = gvInvoices.PageSize,
               
            };

            BindInvoicesGrid(request);
        }
        protected void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int invoiceId = Convert.ToInt32(btn.CommandArgument);
            var salesService = Global.GetService<ISalesService>();
            var invoice = salesService.GetById(invoiceId);
            PdfInvoiceGenerator.GenerateInvoicePdf(invoice);
        }

        [WebMethod]
        public static OperationResultDto CreateInvoice(SalesMasterDto dto)
        {
            var context = HttpContext.Current;
            if (context.Session["UserId"] == null) {

                return new OperationResultDto
                {
                    Success = false,
                    Message = "Session Expired"
                };

            }
            int userId = (int)context.Session["UserId"];
            var salesService = Global.GetService<ISalesService>();
            return salesService.CreateInvoice(dto, userId);
        }

        [WebMethod]
        public static OperationResultDto UpdateInvoice(SalesMasterDto dto)
        {
            var context = HttpContext.Current;
            if (context.Session["UserId"] == null)
            {

                return new OperationResultDto
                {
                    Success = false,
                    Message = "Session Expired"
                };

            }
            int userId = (int)context.Session["UserId"];
            var salesService = Global.GetService<ISalesService>();
            return salesService.UpdateInvoice(dto , userId);
        }

        [WebMethod]
        public static OperationResultDto DeleteInvoice(int invoiceId)
        {
            var context = HttpContext.Current;
            if (context.Session["UserId"] == null)
            {

                return new OperationResultDto
                {
                    Success = false,
                    Message = "Session Expired"
                };

            }
            var salesService = Global.GetService<ISalesService>();
            return salesService.DeleteInvoice(invoiceId);
        }

    }
}