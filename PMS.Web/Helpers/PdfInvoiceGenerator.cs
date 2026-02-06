using iTextSharp.text;
using iTextSharp.text.pdf;
using PMS.Application.DTOs;
using System.IO;
using System.Linq;
using System.Web;

namespace PharmacyManagementSystem.Helpers
{
    public static class PdfInvoiceGenerator
    {
        public static void GenerateInvoicePdf(SalesMasterDto invoice)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 30f, 30f, 30f, 30f);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph("INVOICE",
                    new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 15f
                });

                PdfPTable infoTable = new PdfPTable(4)
                {
                    WidthPercentage = 100
                };
                infoTable.SetWidths(new float[] { 2f, 2f, 2f, 2f }); 

               
                infoTable.AddCell(new PdfPCell(new Phrase($"Invoice No: {invoice.InvoiceNumber}",
                    new Font(Font.FontFamily.HELVETICA, 10)))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5f,
                    Colspan = 2 
                });

                
                infoTable.AddCell(new PdfPCell(new Phrase($"Date: {invoice.Date}",
                    new Font(Font.FontFamily.HELVETICA, 10)))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5f,
                    Colspan = 2
                });

               
                infoTable.AddCell(new PdfPCell(new Phrase($"Customer: {invoice.CustomerName}",
                    new Font(Font.FontFamily.HELVETICA, 10)))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5f,
                    Colspan = 2
                });

               
                infoTable.AddCell(new PdfPCell(new Phrase($"Contact: {invoice.Contact}",
                    new Font(Font.FontFamily.HELVETICA, 10)))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5f,
                    Colspan = 2
                });

                doc.Add(infoTable);
                doc.Add(new Paragraph(" ") { SpacingAfter = 20f });

            
                PdfPTable table = new PdfPTable(7)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f
                };
                table.SetWidths(new float[] { 0.5f, 2f, 1.5f, 1.2f, 0.8f, 0.8f, 1f });

                Font headerFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE);
                Font dataFont = new Font(Font.FontFamily.HELVETICA, 9);
                BaseColor headerBgColor = new BaseColor(51, 122, 183);

           
                table.AddCell(CreateHeaderCell("SL", headerFont, headerBgColor, Element.ALIGN_CENTER));
                table.AddCell(CreateHeaderCell("Medicine", headerFont, headerBgColor, Element.ALIGN_LEFT));
                table.AddCell(CreateHeaderCell("Batch", headerFont, headerBgColor, Element.ALIGN_LEFT));
                table.AddCell(CreateHeaderCell("Expiry", headerFont, headerBgColor, Element.ALIGN_CENTER));
                table.AddCell(CreateHeaderCell("Qty", headerFont, headerBgColor, Element.ALIGN_CENTER));
                table.AddCell(CreateHeaderCell("Price", headerFont, headerBgColor, Element.ALIGN_RIGHT));
                table.AddCell(CreateHeaderCell("Total", headerFont, headerBgColor, Element.ALIGN_RIGHT));

            
                if (invoice.Details == null || !invoice.Details.Any())
                {
                    for (int i = 0; i < 7; i++)
                    {
                        table.AddCell(CreateDataCell("", dataFont, Element.ALIGN_CENTER));
                    }
                }
                else
                {
                    int sl = 1;
                    foreach (var d in invoice.Details)
                    {
                        table.AddCell(CreateDataCell(sl++.ToString(), dataFont, Element.ALIGN_CENTER));
                        table.AddCell(CreateDataCell(d.MedicineName, dataFont, Element.ALIGN_LEFT));
                        table.AddCell(CreateDataCell(d.BatchNumber, dataFont, Element.ALIGN_LEFT));
                        table.AddCell(CreateDataCell(d.ExpiryDate, dataFont, Element.ALIGN_CENTER));
                        table.AddCell(CreateDataCell(d.Quantity.ToString(), dataFont, Element.ALIGN_CENTER));
                        table.AddCell(CreateDataCell(d.UnitPrice.ToString("0.00"), dataFont, Element.ALIGN_RIGHT));
                        table.AddCell(CreateDataCell(d.LineTotal.ToString("0.00"), dataFont, Element.ALIGN_RIGHT));
                    }
                }

                doc.Add(table);

                doc.Add(new Paragraph(" ") { SpacingBefore = 20f });
                PdfPTable totalTable = new PdfPTable(2)
                {
                    WidthPercentage = 40, 
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 10f
                };
                totalTable.SetWidths(new float[] { 2f, 1f }); 

               
                totalTable.AddCell(CreateTotalLabelCell("Sub Total:", false));
                totalTable.AddCell(CreateTotalValueCell(invoice.SubTotal.ToString("0.00"), false));

              
                decimal discountAmount = invoice.SubTotal * invoice.Discount / 100;
                totalTable.AddCell(CreateTotalLabelCell($"Discount ({invoice.Discount}%):", false));
                totalTable.AddCell(CreateTotalValueCell(discountAmount.ToString("0.00"), false));

              
                totalTable.AddCell(CreateTotalLabelCell("Grand Total:", true));
                totalTable.AddCell(CreateTotalValueCell(invoice.GrandTotal.ToString("0.00"), true));

                doc.Add(totalTable);
                doc.Close();

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AddHeader(
                    "content-disposition",
                    $"attachment;filename=Invoice_{invoice.InvoiceNumber}.pdf"
                );
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                HttpContext.Current.Response.End();
            }
        }

        private static PdfPCell CreateHeaderCell(string text, Font font, BaseColor bgColor, int alignment)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = bgColor,
                HorizontalAlignment = alignment,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 8f,
                BorderWidth = 0.5f,
                NoWrap = true
            };
        }

        private static PdfPCell CreateDataCell(string text, Font font, int alignment)
        {
            return new PdfPCell(new Phrase(text ?? "", font))
            {
                HorizontalAlignment = alignment,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 6f,
                BorderWidth = 0.5f,
                NoWrap = false,
                MinimumHeight = 25f,
                PaddingLeft = alignment == Element.ALIGN_LEFT ? 6f : 4f,
                PaddingRight = alignment == Element.ALIGN_RIGHT ? 6f : 4f
            };
        }

        private static PdfPCell CreateTotalLabelCell(string text, bool isBold)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10, isBold ? Font.BOLD : Font.NORMAL);
            return new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = Rectangle.NO_BORDER,
                Padding = 5f,
                PaddingRight = 10f
            };
        }

        private static PdfPCell CreateTotalValueCell(string value, bool isBold)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10, isBold ? Font.BOLD : Font.NORMAL);
            return new PdfPCell(new Phrase(value, font))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = Rectangle.NO_BORDER,
                Padding = 5f
            };
        }
    }
}