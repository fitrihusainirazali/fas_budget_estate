using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ClassBudget
{
    public static class Reporting
    {
        public enum PageOrientation
        {
            Portrait,
            Landscape
        }

        public static FileContentResult ShowReport(LocalReport lr, string format = "PDF", PageOrientation orientation = PageOrientation.Portrait, string filename = "Report")
        {
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
            "  <OutputFormat>" + format + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11.69in</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.25in</MarginLeft>" +
            "  <MarginRight>0.25in</MarginRight>" +
            "  <MarginBottom>0.25in</MarginBottom>" +
            "</DeviceInfo>";

            if (orientation == PageOrientation.Landscape)
                deviceInfo = "<DeviceInfo>" +
                "  <OutputFormat>" + format + "</OutputFormat>" +
                "  <PageWidth>11.69in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.25in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                renderedBytes = lr.Render(
                    format,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                return (new FileContentResult(renderedBytes, mimeType));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static FileContentResult DownloadReport(LocalReport lr, string format = "PDF", PageOrientation orientation = PageOrientation.Portrait, string filename = "Report", bool isLegal = false) //fitri
        {
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo>" +
            "  <OutputFormat>" + format + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11.69in</PageHeight>" +
            "  <MarginTop>0.25in</MarginTop>" +
            "  <MarginLeft>0.25in</MarginLeft>" +
            "  <MarginRight>0.25in</MarginRight>" +
            "  <MarginBottom>0.25in</MarginBottom>" +
            "</DeviceInfo>";

            if (orientation == PageOrientation.Landscape)
                deviceInfo = "<DeviceInfo>" +
                "  <OutputFormat>" + format + "</OutputFormat>" +
                //"  <PageWidth>11.69in</PageWidth>" + //fitri
                $"  <PageWidth>{(isLegal ? "14in" : "11.69in")}</PageWidth>" + //fitri
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                //"  <MarginLeft>0.25in</MarginLeft>" + //fitri
                //"  <MarginRight>0.25in</MarginRight>" + //fitri
                $"  <MarginLeft>{(isLegal ? "0" : "0.25in")}</MarginLeft>" + //fitri
                $"  <MarginRight>{(isLegal ? "0" : "0.25in")}</MarginRight>" + //fitri
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            try
            {
                renderedBytes = lr.Render(
                    format,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                HttpResponse response = HttpContext.Current.Response;
                string ext = GetFileExtensionFromMimeType(mimeType);
                response.AddHeader("Content-Disposition", "attachment; filename=" + filename + "." + ext);

                return (new FileContentResult(renderedBytes, mimeType));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static string GetFileExtensionFromMimeType(string mimeType)
        {
            Dictionary<string, string> mimeTypeToExtensionMap = new Dictionary<string, string>()
            {
                { "application/msword", "doc" },
                { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx" },
                { "application/vnd.ms-excel", "xls" },
                { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx" },
                { "application/pdf", "pdf" },
                // Add more MIME types and their corresponding file extensions here
            };

            string extension;
            if (mimeTypeToExtensionMap.TryGetValue(mimeType, out extension))
            {
                return extension;
            }

            return null; // MIME type not found in the dictionary
        }
    }
}