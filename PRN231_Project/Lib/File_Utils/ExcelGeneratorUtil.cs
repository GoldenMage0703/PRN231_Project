using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.File_Utils
{
    public class ExcelGeneratorUtil
    {
        public static byte[] GenerateExcelFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Questions");
            worksheet.Columns[1].Width = 600;
            
            // Define headers
            worksheet.Cells["A1"].Value = "Question";
            worksheet.Cells["B1"].Value = "Answer";
            worksheet.Cells["C1"].Value = "Correct";
         

            // Sample data in the required structure
            var data = new[]
            {
                new { Question = "Test question", Answer = "True ans", Correct = "TRUE" },
                new { Question = "Test question", Answer = "False ans", Correct = "FALSE" },
                new { Question = "Test question", Answer = "False ans", Correct = "FALSE" },
                new { Question = "Test question", Answer = "False ans", Correct = "FALSE" }
            };

            // Fill worksheet with data
            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = item.Question;
                worksheet.Cells[row, 2].Value = item.Answer;
                worksheet.Cells[row, 3].Value = item.Correct;
               
                row++;
            }

            // Merge cells A2 and A3
            worksheet.Cells["A2:A5"].Merge = true;
            worksheet.Cells["A2:A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A2:A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Format header
            using (var range = worksheet.Cells["A1:C1"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Adjust column widths
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Convert package to byte array
            return package.GetAsByteArray();
        }

    }
}
