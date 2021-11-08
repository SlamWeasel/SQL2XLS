using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using System.Net.Mail;
using System.Net.Mime;

namespace AbrechnungGewichtGS
{
    class Program
    {
        static List<TableRow> table = new List<TableRow>();
        const string connectionString = @"secret";
        static string startTime = "";

        static void Main(string[] args)
        {
            try
            {
                startTime = DateTime.Now.ToString("HH:mm:ss,ff");

                SqlCommand sqlCommand = new SqlCommand(File.ReadAllText(Environment.CurrentDirectory + "\\command.sql"));

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    sqlCommand.Connection = cn;
                    sqlCommand.CommandTimeout = 600;

                    SqlDataReader read = sqlCommand.ExecuteReader();

                    while (read.Read())
                    {
                        TableRow row = new TableRow()
                        {
                            TourNr = read.GetInt32(0),
                            LPosNr = read.GetString(1),
                            RechNr = read.GetInt32(2),
                            Rechnungsdatum = read.GetDateTime(3),
                            FakNr = read.GetInt32(4),
                            FFNr = read.GetInt32(5),
                            FF = read.GetString(6),
                            FZNr = read.GetInt32(7),
                            FZ = read.GetString(8),
                            Tonnenpreis = read.GetString(9),
                            Gutschrift = read.GetSqlMoney(10).ToDouble(),
                            GS_Frachtpflichtigem_Gew = (float)read.GetDouble(11),
                            GS_Differenz = (float)read.GetDouble(12),
                            Gewicht_errechnet_aus_GS = (float)read.GetDouble(13),
                            Frachtpflichtigesgewicht = read.GetSqlMoney(14).ToDouble(),
                            Differenz_in_KG = (float)read.GetDouble(15)
                        };

                        table.Add(row);

                        Console.WriteLine(row.ToString());
                    }

                    cn.Close();
                }

                string result = "Keine Funde";

                if (table.Count != 0)
                {
                    Application xlApp = new Application();
                    xlApp.DisplayAlerts = false;

                    if (xlApp == null)
                    {
                        Console.WriteLine("Excel is not properly installed!!");
                        return;
                    }
                    xlApp.DisplayAlerts = false;
                    Workbook xlWorkBook;
                    Worksheet xlWorkSheet;

                    xlWorkBook = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets[1];

                    xlWorkSheet.Cells[1, 1] = "TourNr";
                    xlWorkSheet.Cells[1, 2] = "LPosNr";
                    xlWorkSheet.Cells[1, 3] = "RechNr";
                    xlWorkSheet.Cells[1, 4] = "Rechnungsdatum";
                    xlWorkSheet.Cells[1, 5] = "FakNr";
                    xlWorkSheet.Cells[1, 6] = "FFNr";
                    xlWorkSheet.Cells[1, 7] = "FF";
                    xlWorkSheet.Cells[1, 8] = "FZNr";
                    xlWorkSheet.Cells[1, 9] = "FZ";
                    xlWorkSheet.Cells[1, 10] = "Tonnenpreis";
                    xlWorkSheet.Cells[1, 11] = "Gutschrift";
                    xlWorkSheet.Cells[1, 12] = "GS_Frachtpflichtigem_Gew";
                    xlWorkSheet.Cells[1, 13] = "GS_Differenz";
                    xlWorkSheet.Cells[1, 14] = "Gewicht_errechnet_aus_GS";
                    xlWorkSheet.Cells[1, 15] = "Frachtpflichtigesgewicht";
                    xlWorkSheet.Cells[1, 16] = "Differenz_in_KG";

                    Range formatRange;
                    formatRange = xlWorkSheet.get_Range("1:1");
                    formatRange.EntireRow.Font.Bold = true;
                    formatRange.EntireRow.BorderAround2();

                    int cursor = 2;
                    foreach (TableRow tr in table)
                    {
                        xlWorkSheet.Cells[cursor, 1] = tr.TourNr;
                        xlWorkSheet.Cells[cursor, 2] = tr.LPosNr;
                        xlWorkSheet.Cells[cursor, 3] = tr.RechNr;
                        xlWorkSheet.Cells[cursor, 4] = tr.Rechnungsdatum.ToString("yyyy-MM-dd");
                        xlWorkSheet.Cells[cursor, 5] = tr.FakNr;
                        xlWorkSheet.Cells[cursor, 6] = tr.FFNr;
                        xlWorkSheet.Cells[cursor, 7] = tr.FF;
                        xlWorkSheet.Cells[cursor, 8] = tr.FZNr;
                        xlWorkSheet.Cells[cursor, 9] = tr.FZ;
                        xlWorkSheet.Cells[cursor, 10] = tr.Tonnenpreis;
                        xlWorkSheet.Cells[cursor, 11] = tr.Gutschrift;
                        xlWorkSheet.Cells[cursor, 12] = tr.GS_Frachtpflichtigem_Gew;
                        xlWorkSheet.Cells[cursor, 13] = Math.Round((decimal)tr.GS_Differenz, 2);
                        xlWorkSheet.Cells[cursor, 13].Interior.Color = tr.GS_Differenz < 0 ? XlRgbColor.rgbOrangeRed : XlRgbColor.rgbLightGreen;
                        xlWorkSheet.Cells[cursor, 13].NumberFormat = "0,00";
                        xlWorkSheet.Cells[cursor, 14] = tr.Gewicht_errechnet_aus_GS;
                        xlWorkSheet.Cells[cursor, 15] = tr.Frachtpflichtigesgewicht;
                        xlWorkSheet.Cells[cursor, 16] = tr.Differenz_in_KG;

                        cursor++;
                    }
                    xlWorkSheet.Columns.AutoFit();

                    xlWorkBook.SaveAs("C:\\Temp\\Bericht.xls", XlFileFormat.xlWorkbookNormal, null, null, null, null, XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
                    xlWorkBook.Close(false, null, null);
                    xlApp.Quit();
                    while (xlApp.Quitting) { }

                    using (SmtpClient smtp = new SmtpClient("255.255.255.255"))
                    {
                        smtp.Port = 25;
                        //smtp.Credentials = new NetworkCredential("user", "pwd");

                        MailMessage mail = new MailMessage
                        {
                            From = new MailAddress("Automation@fixemer.com"),
                            Subject = "Abrechnung Gewicht Gutschrift",
                            Body = "Automatische Abfrage wurde um " + startTime + " gestartet und beendet um " + DateTime.Now.ToString("HH:mm:ss,ff")
                        }; mail.To.Add("info@fixemer.com"); ;
                        mail.Attachments.Add(new Attachment("C:\\Temp\\Bericht.xls", MediaTypeNames.Application.Octet));

                        smtp.Send(mail);
                        mail.Dispose();
                        smtp.Dispose();
                    }

                    result = table.Count == 1 ? table.Count + " Fund. Excel-Datei erstellt!" : table.Count + " Funde. Excel-Datei erstellt!";
                }
                else
                {
                    using (SmtpClient smtp = new SmtpClient("255.255.255.255"))
                    {
                        smtp.Port = 25;
                        //smtp.Credentials = new NetworkCredential("user", "pwd");

                        MailMessage mail = new MailMessage
                        {
                            From = new MailAddress("Automation@fixemer.com"),
                            Subject = "Keine problematische Gutschrift gefunden!",
                            Body = "Automatische Abfrage wurde um " + startTime + " gestartet und beendet um " + DateTime.Now.ToString("HH:mm:ss,ff")
                        }; mail.To.Add("info@fixemer.com");

                        smtp.Send(mail);
                        mail.Dispose();
                        smtp.Dispose();
                    }
                }

                int ind = 0;
                string contents = DateTime.Now.ToString("u") + " -> " + result + "\n";
                foreach (string line in File.ReadAllLines(Environment.CurrentDirectory + "\\log.txt"))
                {
                    if (ind == 50) break;
                    contents += line + "\n";
                    ind++;
                }
                File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", contents);
            }
            catch(Exception ex)
            {
                int ind = 0;
                string contents = DateTime.Now.ToString("u") + " -> Ein kritischer Fehler ist aufgetreten!\n" + ex.Message;
                foreach (string line in File.ReadAllLines(Environment.CurrentDirectory + "\\log.txt"))
                {
                    if (ind == 50) break;
                    contents += line + "\n";
                    ind++;
                }

                try
                {
                    File.WriteAllText(Environment.CurrentDirectory + "\\log.txt", contents);
                }
                catch(Exception)
                {
                    Environment.Exit(2);
                }

                Environment.Exit(13816);
            }
        }
    }

    class TableRow
    {
        public int? TourNr { get; set; }
        public string LPosNr { get; set; }
        public int? RechNr { get; set; }
        public DateTime Rechnungsdatum { get; set; }
        public int? FakNr { get; set; }
        public int? FFNr { get; set; }
        public string FF { get; set; }
        public int? FZNr { get; set; }
        public string FZ { get; set; }
        public string Tonnenpreis { get; set; }
        public double? Gutschrift { get; set; }
        public float? GS_Frachtpflichtigem_Gew { get; set; }
        public float? GS_Differenz { get; set; }
        public float? Gewicht_errechnet_aus_GS { get; set; }
        public double? Frachtpflichtigesgewicht { get; set; }
        public float? Differenz_in_KG { get; set; }

        public TableRow()
        {
        }

        public override string ToString()
            => "(" +
                TourNr.ToString() + " | " +
                LPosNr + " | " +
                RechNr.ToString() + " | " +
                Rechnungsdatum.ToString("dd.MM.yyyy") + " | " +
                FakNr.ToString() + " | " +
                FFNr.ToString() + " | " +
                FF + " | " +
                FZNr.ToString() + " | " +
                FZ + " | " +
                Tonnenpreis + " Euro | " +
                Gutschrift.ToString() + " Euro | " +
                GS_Frachtpflichtigem_Gew.ToString() + " kg | " +
                GS_Differenz.ToString() + @" Euro | " +
                Gewicht_errechnet_aus_GS.ToString() + " kg | " +
                Frachtpflichtigesgewicht.ToString() + " kg | " +
                Differenz_in_KG.ToString() + " kg)";
    }
}
