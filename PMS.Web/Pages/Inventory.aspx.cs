using iTextSharp.text;
using iTextSharp.text.pdf;
using PharmacyManagementSystem.Helpers;
using PMS.Application.DTOs;
using PMS.Application.Services;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Font = iTextSharp.text.Font;

namespace PharmacyManagementSystem.Pages
{
    public partial class Inventory : Page
    {
        private IMedicineService _medicineService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _medicineService = Global.GetService<IMedicineService>();
            if (!IsPostBack)
            {
                if (Session["User"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                var request = new MedicineQueryRequestDto
                {
                    SearchKeyword = string.Empty,           
                    PageIndex = 0,              
                    PageSize = gvMedicines.PageSize
                };
                BindMedicinesGrid(request);
            }
        }
        private void BindMedicinesGrid(MedicineQueryRequestDto request)
        {
                var result = _medicineService.GetMedicinesWithPagination(request);
                gvMedicines.VirtualItemCount = result.TotalCount;
                gvMedicines.DataSource = result.Medicines;
                gvMedicines.DataBind();
        }
        [WebMethod]
        public static object AddMedicine(MedicineDto medicine)
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
            var medicineService = Global.GetService<IMedicineService>();
            return medicineService.CreateMedicines(medicine, userId);
        }
        [WebMethod]
        public static MedicineDto GetMedicineById(int medicineId)
        {
            var medicineService = Global.GetService<IMedicineService>();
            return medicineService.GetMedicineById(medicineId);
        }
        [WebMethod]
        public static object UpdateMedicine(MedicineDto medicine)
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
            var medicineService = Global.GetService<IMedicineService>();
            return medicineService.UpdateMedicine(medicine, userId);
             
        }
        [WebMethod]
        public static object DeleteMedicine(int medicineId)
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
            var medicineService = Global.GetService<IMedicineService>();
            return medicineService.DeleteMedicine(medicineId);
         
        }
        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            var medicines = _medicineService.GetAllMedicines();

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 40f, 20f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                doc.Add(new Paragraph("MEDICINE INVENTORY REPORT",
                    new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                });
                doc.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}",
                    new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC))
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 15f
                });
                PdfPTable table = new PdfPTable(6)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 0.5f, 2.5f, 1.5f, 1.5f, 1f, 1.5f });
                Font headerFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE);
                Font dataFont = new Font(Font.FontFamily.HELVETICA, 9);
                BaseColor headerBgColor = new BaseColor(51, 122, 183);
                table.AddCell("ID".ToHeaderCell(headerFont, headerBgColor));
                table.AddCell("Medicine Name".ToHeaderCell(headerFont, headerBgColor));
                table.AddCell("Batch No".ToHeaderCell(headerFont, headerBgColor));
                table.AddCell("Expiry Date".ToHeaderCell(headerFont, headerBgColor));
                table.AddCell("Stock".ToHeaderCell(headerFont, headerBgColor));
                table.AddCell("Unit Price".ToHeaderCell(headerFont, headerBgColor));
                foreach (var med in medicines)
                {
                    table.AddCell(med.Id.ToString().ToDataCell(dataFont));
                    table.AddCell(med.Name.ToDataCell(dataFont));
                    table.AddCell(med.BatchNumber.ToDataCell(dataFont));
                    table.AddCell(med.ExpiryDate.ToDataCell(dataFont));
                    table.AddCell(med.Stock.ToString().ToDataCell(dataFont));
                    table.AddCell(med.UnitPrice.ToString("N2").ToDataCell(dataFont));
                }
                if (medicines.Any())
                {
                    table.AddCell(
                        $"Total Medicines: {medicines.Count()}".ToFooterCell(
                            new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD),
                            6
                        )
                    );
                }
                doc.Add(table);
                doc.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition",
                    $"attachment;filename=Medicines_Report_{DateTime.Now:yyyyMMdd}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var request = new MedicineQueryRequestDto
            {
                SearchKeyword = txtSearch.Text.Trim(),
                PageIndex = 0,
                PageSize = gvMedicines.PageSize
            };
            BindMedicinesGrid(request);
        }
        protected void gvMedicines_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMedicines.PageIndex = e.NewPageIndex;

            var request = new MedicineQueryRequestDto
            {
                SearchKeyword = txtSearch.Text.Trim(),
                PageIndex = e.NewPageIndex,
                PageSize = gvMedicines.PageSize
            };
            BindMedicinesGrid(request);
        }



    }
}