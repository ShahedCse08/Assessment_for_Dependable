using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PharmacyManagementSystem.Helpers
{
    public static class PdfCellExtensions
    {
        public static PdfPCell ToHeaderCell(this string text, Font font, BaseColor bgColor)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = bgColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 8f,
                BorderWidth = 0.5f
            };
        }

        public static PdfPCell ToDataCell(this string text, Font font)
        {
            return new PdfPCell(new Phrase(text ?? "", font))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 6f,
                BorderWidth = 0.5f,
                NoWrap = false,
                MinimumHeight = 25f
            };
        }

        public static PdfPCell ToFooterCell(this string text, Font font, int colspan)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding = 8f,
                BorderWidth = 0
            };
        }
    }
}