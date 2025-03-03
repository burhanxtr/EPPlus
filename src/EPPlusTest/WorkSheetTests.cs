/*******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * Required Notice: Copyright (C) EPPlus Software AB. 
 * https://epplussoftware.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "" as is "" with no warranty either express or implied. 
 * The author accepts no liability for any damage or loss of business that this product may cause.
 *
 * Code change notes:
 * 
  Date               Author                       Change
 *******************************************************************************
  01/27/2020         EPPlus Software AB       Initial release EPPlus 5
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Drawing;
using System.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Vml;
using OfficeOpenXml.Style;
using System.Data;
using OfficeOpenXml.Table.PivotTable;
using System.Reflection;
using OfficeOpenXml.Table;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EPPlusTest
{

    [TestClass]
    public class WorkSheetTest : TestBase
    {
        static ExcelPackage _pck;
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            InitBase();
            _pck = OpenPackage("Worksheet.xlsx", true);
        }   
        [ClassCleanup]
        public static void Cleanup()
        {
            var dirName = _pck.File.DirectoryName;
            var fileName = _pck.File.FullName;

            SaveAndCleanup(_pck);
            if (File.Exists(fileName))
            {
                File.Copy(fileName, dirName + "\\WorksheetRead.xlsx", true);
            }
        }
        [TestMethod]
        public void AutoFilter()
        {
            var ws = _pck.Workbook.Worksheets.Add("Autofilter");
            ws.Cells["A1"].Value = "A1";
            ws.Cells["B1"].Value = "B1";
            ws.Cells["C1"].Value = "C1";
            ws.Cells["D1"].Value = "D1";

            ws.Cells["A2"].Value = 1;
            ws.Cells["B2"].Value = 2;
            ws.Cells["C2"].Value = 3;
            ws.Cells["D2"].Value = 4;

            ws.Cells["A1:D2"].AutoFilter = true;
            ws.Cells["A1:D2"].AutoFilter = false;
            ws.Cells["A1:D2"].AutoFilter = true;
            ws.Cells["A1:D5"].AutoFilter = false;
            ws.Cells["A1:D2"].AutoFilter = true;
        }

        [TestMethod]
        public void AddChartSheet()
        {
            var chart = _pck.Workbook.Worksheets.AddChart("ChartSheet", eChartType.ColumnClustered);
            foreach (var _n in _pck.Workbook.Names)
            {

            }
            //Iterate all collection and make sure no exception is thrown.
            foreach (var worksheet in _pck.Workbook.Worksheets)
            {
                if (!(worksheet is ExcelChartsheet))
                {
                    foreach (var d in worksheet.Drawings)
                    {

                    }
                    foreach (var d in worksheet.Tables)
                    {

                    }
                    foreach (var d in worksheet.PivotTables)
                    {

                    }
                    foreach (var d in worksheet.Names)
                    {

                    }
                    foreach (var d in worksheet.Comments)
                    {

                    }
                    foreach (var d in worksheet.ConditionalFormatting)
                    {

                    }
                }
            }
        }
        [TestMethod]
        public void ReadWorkSheet()
        {
            using (ExcelPackage pck = OpenPackage("WorksheetRead.xlsx"))
            {
                var ws = TryGetWorksheet(pck, "Perf");
                Assert.AreEqual(ws.Cells["H6"].Formula, "B5+B6");

                ws = pck.Workbook.Worksheets["Comment"];
                var comment = ws.Cells["B2"].Comment;

                Assert.AreNotEqual(comment, null);
                Assert.AreEqual(comment.Author, "Jan Källman");
                ws = TryGetWorksheet(pck, "Hidden");
                ws = pck.Workbook.Worksheets["Hidden"];
                Assert.AreEqual<eWorkSheetHidden>(ws.Hidden, eWorkSheetHidden.Hidden);

                ws = TryGetWorksheet(pck, "VeryHidden");
                Assert.AreEqual<eWorkSheetHidden>(ws.Hidden, eWorkSheetHidden.VeryHidden);

                ws = TryGetWorksheet(pck, "RichText");
                Assert.AreEqual("Room 02 & 03", ws.Cells["G1"].RichText.Text);

                ws = TryGetWorksheet(pck, "HeaderImage");
                //Assert.AreEqual(ws.HeaderFooter.Pictures.Count, 3);

                ws = TryGetWorksheet(pck, "newsheet");
                Assert.AreEqual(ws.Cells["F2"].Style.Font.UnderLine, true);
                Assert.AreEqual(ws.Cells["F2"].Style.Font.UnderLineType, ExcelUnderLineType.Double);
                Assert.AreEqual(ws.Cells["F3"].Style.Font.UnderLineType, ExcelUnderLineType.SingleAccounting);
                Assert.AreEqual(ws.Cells["F5"].Style.Font.UnderLineType, ExcelUnderLineType.None);
                Assert.AreEqual(ws.Cells["F5"].Style.Font.UnderLine, false);

                Assert.AreEqual(ws.Cells["T20"].GetValue<string>().Substring(0, 15), 0.3961805555555.ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual(ws.Cells["T20"].GetValue<int>(), 0);
                Assert.AreEqual(ws.Cells["T20"].GetValue<int?>(), 0);
                Assert.AreEqual(Math.Round(ws.Cells["T20"].GetValue<double>(),15), 0.396180555555556d);
                Assert.AreEqual(Math.Round(ws.Cells["T20"].GetValue<double?>().Value,15), 0.396180555555556d);
                Assert.AreEqual(ws.Cells["T20"].GetValue<decimal>(), 0.396180555555556m);
                Assert.AreEqual(ws.Cells["T20"].GetValue<decimal?>(), 0.396180555555556m);
                Assert.AreEqual(ws.Cells["T20"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["T20"].GetValue<bool?>(), true);
                Assert.AreEqual(ws.Cells["T20"].GetValue<DateTime>(), new DateTime(1899, 12, 30, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T20"].GetValue<DateTime?>(), new DateTime(1899, 12, 30, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T20"].GetValue<TimeSpan>(), new TimeSpan(693593, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T20"].GetValue<TimeSpan?>(), new TimeSpan(693593, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T20"].Text, "09:30:30");

                Assert.AreEqual(ws.Cells["T24"].GetValue<string>().Substring(0, 15), 1.3961805555555d.ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual(ws.Cells["T24"].GetValue<int>(), 1);
                Assert.AreEqual(ws.Cells["T24"].GetValue<int?>(), 1);
                Assert.AreEqual(Math.Round(ws.Cells["T24"].GetValue<double>(), 14), 1.39618055555556d);
                Assert.AreEqual(Math.Round(ws.Cells["T24"].GetValue<double?>().Value, 14), 1.39618055555556d);
                Assert.AreEqual(ws.Cells["T24"].GetValue<decimal>(), 1.39618055555556m);
                Assert.AreEqual(ws.Cells["T24"].GetValue<decimal?>(), 1.39618055555556m);
                Assert.AreEqual(ws.Cells["T24"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["T24"].GetValue<bool?>(), true);
                Assert.AreEqual(ws.Cells["T24"].GetValue<DateTime>(), new DateTime(1899, 12, 31, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T24"].GetValue<DateTime?>(), new DateTime(1899, 12, 31, 9, 30, 30));
                Assert.AreEqual(ws.Cells["T24"].GetValue<TimeSpan>(), new TimeSpan(693593, 33, 30, 30));
                Assert.AreEqual(ws.Cells["T24"].GetValue<TimeSpan?>(), new TimeSpan(693593, 33, 30, 30));
                Assert.AreEqual(ws.Cells["T24"].Text, "09:30:30");

                Assert.AreEqual(ws.Cells["U20"].GetValue<string>(), "40179");
                Assert.AreEqual(ws.Cells["U20"].GetValue<int>(), 40179);
                Assert.AreEqual(ws.Cells["U20"].GetValue<int?>(), 40179);
                Assert.AreEqual(ws.Cells["U20"].GetValue<double>(), 40179d);
                Assert.AreEqual(ws.Cells["U20"].GetValue<double?>(), 40179d);
                Assert.AreEqual(ws.Cells["U20"].GetValue<decimal>(), 40179m);
                Assert.AreEqual(ws.Cells["U20"].GetValue<decimal?>(), 40179m);
                Assert.AreEqual(ws.Cells["U20"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["U20"].GetValue<bool?>(), true);
                Assert.AreEqual(ws.Cells["U20"].GetValue<DateTime>(), new DateTime(2010, 1, 1));
                Assert.AreEqual(ws.Cells["U20"].GetValue<DateTime?>(), new DateTime(2010, 1, 1));
                Assert.AreEqual(ws.Cells["U20"].Text, "2010-01-01");

                Assert.AreEqual(ws.Cells["V20"].GetValue<string>(), "102");
                Assert.AreEqual(ws.Cells["V20"].GetValue<int>(), 102);
                Assert.AreEqual(ws.Cells["V20"].GetValue<int?>(), 102);
                Assert.AreEqual(ws.Cells["V20"].GetValue<double>(), 102d);
                Assert.AreEqual(ws.Cells["V20"].GetValue<double?>(), 102d);
                Assert.AreEqual(ws.Cells["V20"].GetValue<decimal>(), 102m);
                Assert.AreEqual(ws.Cells["V20"].GetValue<decimal?>(), 102m);
                Assert.AreEqual(ws.Cells["V20"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["V20"].GetValue<bool?>(), true);
                Assert.AreEqual(ws.Cells["V20"].GetValue<DateTime>(), new DateTime(1900, 4, 11));
                Assert.AreEqual(ws.Cells["V20"].GetValue<DateTime?>(), new DateTime(1900, 4, 11));
                Assert.AreEqual(ws.Cells["V20"].Text,
                    $"$102{CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator}00");

                Assert.AreEqual(ws.Cells["W20"].GetValue<string>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<int>(), 0);
                Assert.AreEqual(ws.Cells["W20"].GetValue<int?>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<double>(), 0d);
                Assert.AreEqual(ws.Cells["W20"].GetValue<double?>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<decimal>(), 0m);
                Assert.AreEqual(ws.Cells["W20"].GetValue<decimal?>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<bool>(), false);
                Assert.AreEqual(ws.Cells["W20"].GetValue<bool?>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<DateTime>(), DateTime.MinValue);
                Assert.AreEqual(ws.Cells["W20"].GetValue<DateTime?>(), null);
                Assert.AreEqual(ws.Cells["W20"].GetValue<TimeSpan>(), TimeSpan.Zero);
                Assert.AreEqual(ws.Cells["W20"].GetValue<TimeSpan?>(), null);
                Assert.AreEqual(ws.Cells["W20"].Text, string.Empty);

                Assert.AreEqual(ws.Cells["Y20"].GetValue<string>(), "True");
                Assert.AreEqual(ws.Cells["Y20"].GetValue<int>(), 1);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<int?>(), 1);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<double>(), 1d);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<double?>(), 1d);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<decimal>(), 1m);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<decimal?>(), 1m);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["Y20"].GetValue<bool?>(), true);

                Assert.IsInstanceOfType(GetValueException<DateTime>(ws.Cells["Y20"]), typeof(InvalidCastException));
                Assert.IsInstanceOfType(GetValueException<DateTime?>(ws.Cells["Y20"]), typeof(InvalidCastException));
                Assert.IsInstanceOfType(GetValueException<TimeSpan>(ws.Cells["Y20"]), typeof(InvalidCastException));
                Assert.IsInstanceOfType(GetValueException<TimeSpan?>(ws.Cells["Y20"]), typeof(InvalidCastException));

                Assert.AreEqual(ws.Cells["Z20"].GetValue<string>(), "Text2");
                Assert.IsInstanceOfType(GetValueException<int>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<int?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<double>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<double?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<decimal>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<decimal?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<bool>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<bool?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<DateTime>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<DateTime?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<TimeSpan>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<TimeSpan?>(ws.Cells["Z20"]), typeof(FormatException));
                Assert.AreEqual(ws.Cells["Z20"].Text, "Text2");

                // even though value is set as integer, it is stored/exposed by EPPlust as double; feature?
                //Assert.IsInstanceOfType(ws.Cells["X19"].Value, typeof(int));
                Assert.AreEqual(ws.Cells["X19"].GetValue<string>(), "210");
                Assert.AreEqual(ws.Cells["X19"].GetValue<int>(), 210);
                Assert.AreEqual(ws.Cells["X19"].GetValue<int?>(), 210);
                Assert.AreEqual(ws.Cells["X19"].GetValue<double>(), 210d);
                Assert.AreEqual(ws.Cells["X19"].GetValue<double?>(), 210d);
                Assert.AreEqual(ws.Cells["X19"].GetValue<decimal>(), 210m);
                Assert.AreEqual(ws.Cells["X19"].GetValue<decimal?>(), 210m);
                Assert.AreEqual(ws.Cells["X19"].GetValue<bool>(), true);
                Assert.AreEqual(ws.Cells["X19"].GetValue<bool?>(), true);
                // double is converted to DateTime and TimeSpan, but value is meaningless
                Assert.IsNull(GetValueException<DateTime>(ws.Cells["X19"]));
                Assert.IsNull(GetValueException<DateTime?>(ws.Cells["X19"]));
                Assert.IsNull(GetValueException<TimeSpan>(ws.Cells["X19"]));
                Assert.IsNull(GetValueException<TimeSpan?>(ws.Cells["X19"]));
                Assert.AreEqual(ws.Cells["X19"].Text, "210");

                Assert.IsInstanceOfType(ws.Cells["AA19"].Value, typeof(string));
                Assert.AreEqual(ws.Cells["AA19"].GetValue<string>(), "210");
                Assert.AreEqual(ws.Cells["AA19"].GetValue<int>(), 210);
                Assert.AreEqual(ws.Cells["AA19"].GetValue<int?>(), 210);
                Assert.AreEqual(ws.Cells["AA19"].GetValue<double>(), 210d);
                Assert.AreEqual(ws.Cells["AA19"].GetValue<double?>(), 210d);
                Assert.AreEqual(ws.Cells["AA19"].GetValue<decimal>(), 210m);
                Assert.AreEqual(ws.Cells["AA19"].GetValue<decimal?>(), 210m);
                Assert.IsInstanceOfType(GetValueException<bool>(ws.Cells["AA19"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<bool?>(ws.Cells["AA19"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<DateTime>(ws.Cells["AA19"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<DateTime?>(ws.Cells["AA19"]), typeof(FormatException));
                Assert.AreEqual(TimeSpan.FromDays(210), ws.Cells["AA19"].GetValue<TimeSpan>());
                Assert.AreEqual(TimeSpan.FromDays(210), ws.Cells["AA19"].GetValue<TimeSpan?>());
                Assert.AreEqual(ws.Cells["AA19"].Text, "210");

                // blank string produces null for nullable primitive types and conversion to non-nullable types throws FormatException
                Assert.IsInstanceOfType(ws.Cells["AA18"].Value, typeof(string));
                Assert.IsInstanceOfType(GetValueException<int>(ws.Cells["AA18"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<double>(ws.Cells["AA18"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<decimal>(ws.Cells["AA18"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<bool>(ws.Cells["AA18"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<DateTime>(ws.Cells["AA18"]), typeof(FormatException));
                Assert.IsInstanceOfType(GetValueException<TimeSpan>(ws.Cells["AA18"]), typeof(FormatException));

                Assert.IsNull(GetValueException<int?>(ws.Cells["AA18"]));
                Assert.IsNull(GetValueException<double?>(ws.Cells["AA18"]));
                Assert.IsNull(GetValueException<decimal?>(ws.Cells["AA18"]));
                Assert.IsNull(GetValueException<bool?>(ws.Cells["AA18"]));
                Assert.IsNull(GetValueException<DateTime?>(ws.Cells["AA18"]));
                Assert.IsNull(GetValueException<TimeSpan?>(ws.Cells["AA18"]));
                Assert.AreEqual(ws.Cells["AA18"].Text, " ");
            }
        }

        [TestMethod]
        public void ReadStreamWithTemplateWorkSheet()
        {
            AssertIfNotExists("WorksheetRead.xlsx");
            using (FileStream instream = new FileStream(_worksheetPath + @"WorksheetRead.xlsx", FileMode.Open, FileAccess.Read))
            {
                MemoryStream stream = new MemoryStream();
                using (ExcelPackage pck = new ExcelPackage(stream, instream))
                {
                    var ws = TryGetWorksheet(pck, "Perf");
                    Assert.AreEqual(ws.Cells["H6"].Formula, "B5+B6");

                    ws = TryGetWorksheet(pck, "newsheet");
                    Assert.AreEqual(ws.GetValue<DateTime>(20, 21), new DateTime(2010, 1, 1));

                    ws = TryGetWorksheet(pck, "Loaded DataTable");
                    Assert.AreEqual(ws.GetValue<string>(2, 1), "Row1");
                    Assert.AreEqual(ws.GetValue<int>(2, 2), 1);
                    Assert.AreEqual(ws.GetValue<bool>(2, 3), true);
                    Assert.AreEqual(ws.GetValue<double>(2, 4), 1.5);

                    ws = TryGetWorksheet(pck, "RichText");

                    var r1 = ws.Cells["A1"].RichText[0];
                    Assert.AreEqual(r1.Text, "Test");
                    Assert.AreEqual(r1.Bold, true);

                    ws = TryGetWorksheet(pck, "Pic URL");
                    Assert.AreEqual(((ExcelPicture)ws.Drawings["Pic URI"]).Hyperlink, "http://epplus.codeplex.com");

                    Assert.AreEqual(pck.Workbook.Worksheets["Address"].GetValue<string>(40, 1), "\b\t");

                    pck.SaveAs(new FileInfo(@"Test\Worksheet2.xlsx"));
                }
                instream.Close();
            }
        }
        [TestMethod]
        public void ReadStreamSaveAsStream()
        {
            if (!File.Exists(_worksheetPath + @"WorksheetRead.xlsx"))
            {
                Assert.Inconclusive("WorksheetRead.xlsx does not exists");
            }
            using (FileStream instream = new FileStream(_worksheetPath + @"/WorksheetRead.xlsx", FileMode.Open, FileAccess.ReadWrite))
            {
                MemoryStream stream = new MemoryStream();
                using (ExcelPackage pck = new ExcelPackage(instream))
                {
                    var ws = TryGetWorksheet(pck, "Names");
                    Assert.AreEqual(ws.Names["FullCol"].Start.Row, 1);
                    Assert.AreEqual(ws.Names["FullCol"].End.Row, ExcelPackage.MaxRows);
                    pck.SaveAs(stream);
                }
                instream.Close();
            }
        }
        [TestMethod]
        public void LoadData()
        {
            ExcelWorksheet ws = _pck.Workbook.Worksheets.Add("newsheet");
            ws.Cells["T19"].Value = new TimeSpan(3, 30, 30);
            ws.Cells["T20"].Value = new TimeSpan(9, 30, 30);
            ws.Cells["T21"].Value = new TimeSpan(15, 30, 30);
            ws.Cells["T22"].Value = new TimeSpan(21, 30, 30);
            ws.Cells["T23"].Value = new TimeSpan(27, 30, 30);
            ws.Cells["T24"].Value = new TimeSpan(33, 30, 30);
            ws.Cells["T19:T24"].Style.Numberformat.Format = "hh:mm:ss";

            ws.Cells["U19"].Value = new DateTime(2009, 12, 31);
            ws.Cells["U20"].Value = new DateTime(2010, 1, 1);
            ws.Cells["U21"].Value = new DateTime(2010, 1, 2);
            ws.Cells["U22"].Value = new DateTime(2010, 1, 3);
            ws.Cells["U23"].Value = new DateTime(2010, 1, 4);
            ws.Cells["U24"].Value = new DateTime(2010, 1, 5);
            ws.Cells["U19:U24"].Style.Numberformat.Format = "yyyy-mm-dd";

            ws.Cells["V19"].Value = 100;
            ws.Cells["V20"].Value = 102;
            ws.Cells["V21"].Value = 101;
            ws.Cells["V22"].Value = 103;
            ws.Cells["V23"].Value = 105;
            ws.Cells["V24"].Value = 104;
            ws.Cells["v19:v24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells["v19:v24"].Style.Numberformat.Format = @"$#,##0.00_);($#,##0.00)";

            // int value is actually exposed as double; feature or bug??
            ws.Cells["X19"].Value = 210;
            ws.Cells["X20"].Value = 212;
            ws.Cells["X21"].Value = 221;
            ws.Cells["X22"].Value = 123;
            ws.Cells["X23"].Value = 135;
            ws.Cells["X24"].Value = 134;

            ws.Cells["AA18"].Value = " ";

            ws.Cells["AA19"].Value = "210";
            ws.Cells["AA20"].Value = "212";
            ws.Cells["AA21"].Value = "221";
            ws.Cells["AA22"].Value = "123";
            ws.Cells["AA23"].Value = "135";
            ws.Cells["AA24"].Value = "134";

            ws.Cells["Y19"].Value = true;
            ws.Cells["Y20"].Value = true;
            ws.Cells["Y21"].Value = true;
            ws.Cells["Y22"].Value = false;
            ws.Cells["Y23"].Value = false;
            ws.Cells["Y24"].Value = false;

            ws.Cells["Z19"].Value = "Text1";
            ws.Cells["Z20"].Value = "Text2";
            ws.Cells["Z21"].Value = "Text3";
            ws.Cells["Z22"].Value = "Text4";
            ws.Cells["Z23"].Value = "Text5";
            ws.Cells["Z24"].Value = "Text6";

            // add autofilter
            ws.Cells["U19:X24"].AutoFilter = true;
            ExcelPicture pic = ws.Drawings.AddPicture("Pic1", Properties.Resources.Test1);
            pic.SetPosition(150, 140);

            ws.Cells["A30"].Value = "Text orientation 45";
            ws.Cells["A30"].Style.TextRotation = 45;
            ws.Cells["B30"].Value = "Text orientation 90";
            ws.Cells["B30"].Style.TextRotation = 90;
            ws.Cells["C30"].Value = "Text orientation 180";
            ws.Cells["C30"].Style.TextRotation = 180;
            ws.Cells["D30"].Value = "Text orientation 38";
            ws.Cells["D30"].Style.TextRotation = 38;
            ws.Cells["D30"].Style.Font.Bold = true;
            ws.Cells["D30"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            ws.Workbook.Names.Add("TestName", ws.Cells["B30:E30"]);
            ws.Workbook.Names["TestName"].Style.Font.Color.SetColor(Color.Red);


            ws.Workbook.Names["TestName"].Offset(1, 0).Value = "Offset test 1";
            ws.Workbook.Names["TestName"].Offset(2, -1, 2, 2).Value = "Offset test 2";

            //Test vertical align
            ws.Cells["E19"].Value = "Subscript";
            ws.Cells["E19"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Subscript;
            ws.Cells["E20"].Value = "Subscript";
            ws.Cells["E20"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
            ws.Cells["E21"].Value = "Superscript";
            ws.Cells["E21"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
            ws.Cells["E21"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.None;


            ws.Cells["E22"].Value = "Indent 2";
            ws.Cells["E22"].Style.Indent = 2;
            ws.Cells["E23"].Value = "Shrink to fit";
            ws.Cells["E23"].Style.ShrinkToFit = true;

            ws.Cells["e24"].Value = "ReadingOrder LeftToRight";
            ws.Cells["e24"].Style.ReadingOrder = ExcelReadingOrder.LeftToRight;
            ws.Cells["e25"].Value = "ReadingOrder RightToLeft";
            ws.Cells["e25"].Style.ReadingOrder = ExcelReadingOrder.RightToLeft;
            ws.Cells["e26"].Value = "ReadingOrder Context";
            ws.Cells["e26"].Style.ReadingOrder = ExcelReadingOrder.ContextDependent;
            ws.Cells["e27"].Value = "Default Readingorder";

            //Underline

            ws.Cells["F1:F7"].Value = "Underlined";
            ws.Cells["F1"].Style.Font.UnderLineType = ExcelUnderLineType.Single;
            ws.Cells["F2"].Style.Font.UnderLineType = ExcelUnderLineType.Double;
            ws.Cells["F3"].Style.Font.UnderLineType = ExcelUnderLineType.SingleAccounting;
            ws.Cells["F4"].Style.Font.UnderLineType = ExcelUnderLineType.DoubleAccounting;
            ws.Cells["F5"].Style.Font.UnderLineType = ExcelUnderLineType.None;
            ws.Cells["F6:F7"].Style.Font.UnderLine = true;
            ws.Cells["F7"].Style.Font.UnderLine = false;

            ws.Cells["E24"].Value = 0;
            Assert.AreEqual(ws.Cells["E24"].Text, "0");
            ws.Cells["F7"].Style.Font.UnderLine = false;
            ws.Names.Add("SheetName", ws.Cells["A1:A2"]);
            ws.View.    FreezePanes(3, 5);

            foreach (ExcelRangeBase cell in ws.Cells["A1"])
            {
                Assert.Fail("A1 is not set");
            }

            foreach (ExcelRangeBase cell in ws.Cells[ws.Dimension.Address])
            {
                System.Diagnostics.Debug.WriteLine(cell.Address);
            }

            ////Linq test
            var res = from c in ws.Cells[ws.Dimension.Address] where c.Value != null && c.Value.ToString() == "Offset test 1" select c;

            foreach (ExcelRangeBase cell in res)
            {
                System.Diagnostics.Debug.WriteLine(cell.Address);
            }

            _pck.Workbook.Properties.Author = "Jan Källman";
            _pck.Workbook.Properties.Category = "Category";
            _pck.Workbook.Properties.Comments = "Comments";
            _pck.Workbook.Properties.Company = "Adventure works";
            _pck.Workbook.Properties.Keywords = "Keywords";
            _pck.Workbook.Properties.Title = "Title";
            _pck.Workbook.Properties.Subject = "Subject";
            _pck.Workbook.Properties.Status = "Status";
            _pck.Workbook.Properties.HyperlinkBase = new Uri("https://epplussoftware.com", UriKind.Absolute);
            _pck.Workbook.Properties.Manager = "Manager";
            _pck.Workbook.Properties.AppVersion = "5.0000";
            _pck.Workbook.Properties.LinksUpToDate = false;
            _pck.Workbook.Properties.HyperlinksChanged = false;
            _pck.Workbook.Properties.SharedDoc = false;
            _pck.Workbook.Properties.ScaleCrop = false;


            _pck.Workbook.Properties.SetCustomPropertyValue("DateTest", new DateTime(2008, 12, 31));
            Console.WriteLine(_pck.Workbook.Properties.GetCustomPropertyValue("DateTest").ToString());
            _pck.Workbook.Properties.SetCustomPropertyValue("Author", "Jan Källman");
            _pck.Workbook.Properties.SetCustomPropertyValue("Count", 1);
            _pck.Workbook.Properties.SetCustomPropertyValue("IsTested", false);
            _pck.Workbook.Properties.SetCustomPropertyValue("LargeNo", 123456789123);
            _pck.Workbook.Properties.SetCustomPropertyValue("Author", 3);
        }
        const int PERF_ROWS = 5000;
        [TestMethod]
        public void Performance()
        {
            ExcelWorksheet ws = _pck.Workbook.Worksheets.Add("Perf");
            Console.WriteLine("StartTime {0}", DateTime.Now);

            Random r = new Random();
            for (int i = 1; i <= PERF_ROWS; i++)
            {
                ws.Cells[i, 1].Value = string.Format("Row {0}\n.Test new row\"' öäåü", i);
                ws.Cells[i, 2].Value = i;
                ws.Cells[i, 2].Style.WrapText = true;
                ws.Cells[i, 3].Value = DateTime.Now;
                ws.Cells[i, 4].Value = r.NextDouble() * 100000;
            }
            ws.Cells[1, 2, PERF_ROWS, 2].Style.Numberformat.Format = "#,##0";
            ws.Cells[1, 3, PERF_ROWS, 3].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
            ws.Cells[1, 4, PERF_ROWS, 4].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[PERF_ROWS + 1, 2].Formula = "SUM(B1:B" + PERF_ROWS.ToString() + ")";
            ws.Column(1).Width = 12;
            ws.Column(2).Width = 8;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 14;

            ws.Cells["A1:C1"].Merge = true;
            ws.Cells["A2:A5"].Merge = true;
            ws.DeleteRow(1, 1);
            ws.InsertRow(1, 1);
            ws.InsertRow(3, 1);

            ws.DeleteRow(1000, 3, true);
            ws.DeleteRow(2000, 1, true);

            ws.InsertRow(2001, 4);

            ws.InsertRow(2010, 1, 2010);

            ws.InsertRow(20000, 2);

            ws.DeleteRow(20005, 4, false);

            //Single formula
            ws.Cells["H3"].Formula = "B2+B3";
            ws.DeleteRow(2, 1, true);

            //Shared formula
            ws.Cells["H5:H30"].Formula = "B4+B5";
            ws.Cells["H5:H30"].Style.Numberformat.Format = "_(\"$\"* # ##0.00_);_(\"$\"* (# ##0.00);_(\"$\"* \"-\"??_);_(@_)";
            ws.InsertRow(7, 3);
            ws.InsertRow(2, 1);
            ws.DeleteRow(30, 3, true);

            ws.DeleteRow(15, 2, true);
            ws.Cells["a1:B100"].Style.Locked = false;
            ws.Cells["a1:B12"].Style.Hidden = true;
            Console.WriteLine("EndTime {0}", DateTime.Now);
        }
        [TestMethod]
        public void InsertDeleteTestRows()
        {
            ExcelWorksheet ws = _pck.Workbook.Worksheets.Add("InsertDelete");
            //ws.Cells.Value = 0;
            ws.Cells["A1:C5"].Value = 1;
            Assert.AreEqual(((object[,])ws.Cells["A1:C5"].Value)[1, 1], 1);
            ws.Cells["A1:B3"].Merge = true;
            ws.Cells["D3"].Formula = "A2+C5";
            ws.InsertRow(2, 1);

            ws.Cells["A10:C15"].Value = 1;
            ws.Cells["A11:B13"].Merge = true;
            ws.DeleteRow(12, 1, true);

            ws.Cells["a1:B100"].Style.Locked = false;
            ws.Cells["a1:B12"].Style.Hidden = true;
            ws.Protection.IsProtected = true;
            ws.Protection.SetPassword("Password");


            var range = ws.Cells["B2:D100"];

            ws.PrinterSettings.PrintArea = null;
            ws.PrinterSettings.PrintArea = ws.Cells["B2:D99"];
            ws.PrinterSettings.PrintArea = null;
            ws.Row(15).PageBreak = true;
            ws.Column(3).PageBreak = true;
            ws.View.ShowHeaders = false;
            ws.View.PageBreakView = true;

            ws.Row(200).Height = 50;
            ws.Workbook.CalcMode = ExcelCalcMode.Automatic;

            Assert.AreEqual(range.Start.Column, 2);
            Assert.AreEqual(range.Start.Row, 2);
            Assert.AreEqual(range.Start.Address, "B2");

            Assert.AreEqual(range.End.Column, 4);
            Assert.AreEqual(range.End.Row, 100);
            Assert.AreEqual(range.End.Address, "D100");

            ExcelAddress addr = new ExcelAddress("B1:D3");

            Assert.AreEqual(addr.Start.Column, 2);
            Assert.AreEqual(addr.Start.Row, 1);
            Assert.AreEqual(addr.End.Column, 4);
            Assert.AreEqual(addr.End.Row, 3);
        }
        [TestMethod]
        public void InsertDeleteTestColumns()
        {
            ExcelWorksheet ws = _pck.Workbook.Worksheets.Add("InsertDeleteColumns");
            //ws.Cells.Value = 0;
            ws.Cells["A1:C1"].Value = 1;
            ws.Cells["A2:C2"].Value = 2;
            ws.Cells["A3:C3"].Value = 3;
            ws.Cells["A4:C4"].Value = 4;
            ws.Cells["A5:C5"].Value = 5;
            Assert.AreEqual(((object[,])ws.Cells["A1:C5"].Value)[1, 1], 2);
            ws.Cells["A1:B3"].Merge = true;
            ws.Cells["D3"].Formula = "A2+C5";
            ws.InsertColumn(1, 1);

            //ws.DeleteColumn(3, 2);
            ws.Cells["K10:M15"].Value = 1;
            ws.Cells["K11:L13"].Merge = true;
            ws.DeleteColumn(12, 1);

            ws.Cells["X1:Y100"].Style.Locked = false;
            ws.Cells["C1:Y12"].Style.Hidden = true;
            ws.Protection.IsProtected = true;
            ws.Protection.SetPassword("Password");


            var range = ws.Cells["X2:Z100"];

            ws.PrinterSettings.PrintArea = null;
            ws.PrinterSettings.PrintArea = ws.Cells["X2:Z99"];
            ws.PrinterSettings.PrintArea = null;
            ws.Row(15).PageBreak = true;
            ws.Column(3).PageBreak = true;
            ws.View.ShowHeaders = false;
            ws.View.PageBreakView = true;

            ws.Row(200).Height = 50;
            ws.Workbook.CalcMode = ExcelCalcMode.Automatic;

            //Assert.AreEqual(range.Start.Column, 2);
            //Assert.AreEqual(range.Start.Row, 2);
            //Assert.AreEqual(range.Start.Address, "B2");

            //Assert.AreEqual(range.End.Column, 4);
            //Assert.AreEqual(range.End.Row, 100);
            //Assert.AreEqual(range.End.Address, "D100");

            //ExcelAddress addr = new ExcelAddress("B1:D3");

            //Assert.AreEqual(addr.Start.Column, 2);
            //Assert.AreEqual(addr.Start.Row, 1);
            //Assert.AreEqual(addr.End.Column, 4);
            //Assert.AreEqual(addr.End.Row, 3);
        }
        [TestMethod]
        public void RichTextCells()
        {
            ExcelWorksheet ws = _pck.Workbook.Worksheets.Add("RichText");
            var rs = ws.Cells["A1"].RichText;

            var r1 = rs.Add("Test");
            r1.Bold = true;
            r1.Color = Color.Pink;

            var r2 = rs.Add(" of");
            r2.Size = 14;
            r2.Italic = true;

            var r3 = rs.Add(" rich");
            r3.FontName = "Arial";
            r3.Size = 18;
            r3.Italic = true;

            var r4 = rs.Add("text.");
            r4.Size = 8.25f;
            r4.Italic = true;
            r4.UnderLine = true;

            var rIns = rs.Insert(2, " inserted");
            rIns.Bold = true;
            rIns.Color = Color.Green;

            rs = ws.Cells["A3:A4"].RichText;

            var r5 = rs.Add("Double");
            r5.Color = Color.PeachPuff;
            r5.FontName = "times new roman";
            r5.Size = 16;

            var r6 = rs.Add(" cells");
            r6.Color = Color.Red;
            r6.UnderLine = true;


            rs = ws.Cells["C8"].RichText;
            r1 = rs.Add("Blue ");
            r1.Color = Color.Blue;

            r2 = rs.Add("Red");
            r2.Color = Color.Red;

            ws.Cells["G1"].RichText.Add("Room 02 & 03");
            ws.Cells["G2"].RichText.Text = "Room 02 & 03";

            ws = _pck.Workbook.Worksheets.Add("RichText2");
            ws.Cells["A1"].RichText.Text = "Room 02 & 03";
            ws.TabColor = Color.PowderBlue;

            r1 = ws.Cells["G3"].RichText.Add("Test");
            r1.Bold = true;
            ws.Cells["G3"].RichText.Add(" a new t");
            ws.Cells["G3"].RichText[1].Bold = false;

            ws.Cells["G3"].RichText.Add("");

            //Set printersettings
            ws.PrinterSettings.RepeatColumns = ws.Cells["A:B"];
            ws.PrinterSettings.RepeatRows = ws.Cells["1:11"];
            ws.PrinterSettings.TopMargin = 1M;
            ws.PrinterSettings.LeftMargin = 1M;
            ws.PrinterSettings.BottomMargin = 1M;
            ws.PrinterSettings.RightMargin = 1M;
            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            ws.PrinterSettings.PaperSize = ePaperSize.A4;
        }
        [TestMethod]
        public void TestComments()
        {
            var ws = _pck.Workbook.Worksheets.Add("Comment");
            var comment = ws.Comments.Add(ws.Cells["C3"], "Jan Källman\r\nAuthor\r\n", "JK");
            comment.RichText[0].Bold = true;
            comment.RichText[0].PreserveSpace = true;
            var rt = comment.RichText.Add("Test comment");
            comment.VerticalAlignment = eTextAlignVerticalVml.Center;
            comment = ws.Comments.Add(ws.Cells["A2"], "Jan Källman\r\nAuthor\r\n1", "JK");

            comment = ws.Comments.Add(ws.Cells["A1"], "Jan Källman\r\nAuthor\r\n2", "JK");
            comment.AlternativeText = "Test of AlternetiveText2";
            comment = ws.Comments.Add(ws.Cells["C2"], "Jan Källman\r\nAuthor\r\n3", "JK");
            comment = ws.Comments.Add(ws.Cells["C1"], "Jan Källman\r\nAuthor\r\n5", "JK");
            comment = ws.Comments.Add(ws.Cells["B1"], "Jan Källman\r\nAuthor\r\n7", "JK");

            ws.Comments.Remove(ws.Cells["A2"].Comment);
            //comment.HorizontalAlignment = eTextAlignHorizontalVml.Center;
            //comment.Visible = true;
            //comment.BackgroundColor = Color.Green;
            //comment.To.Row += 4;
            //comment.To.Column += 2;
            //comment.LineStyle = eLineStyleVml.LongDash;
            //comment.LineColor = Color.Red;
            //comment.LineWidth = (Single)2.5;
            //rt.Color = Color.Red;

            var rt2 = ws.Cells["B2"].AddComment("Range Added Comment test test test test test test test test test test testtesttesttesttesttesttesttesttesttesttest", "Jan Källman");
            ws.Cells["c3"].Comment.AutoFit = true;

        }
        [TestMethod]
            public void Address()
        {
            var ws = _pck.Workbook.Worksheets.Add("Address");
            ws.Cells["A1:A4,B5:B7"].Value = "AddressTest";
            ws.Cells["A1:A4,B5:B7"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["A2:A3,B4:B8"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightUp;
            ws.Cells["A2:A3,B4:B8"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            ws.Cells["2:2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["2:2"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            ws.Cells["B:B"].Style.Font.Name = "Times New Roman";

            ws.Cells["C4:G4,H8:H30,B15"].FormulaR1C1 = "RC[-1]+R1C[-1]";
            ws.Cells["C4:G4,H8:H30,B15"].Style.Numberformat.Format = "#,##0.000";
            ws.Cells["G1,G3"].Hyperlink = new ExcelHyperLink("Comment!$A$1", "Comment");
            ws.Cells["G1,G3"].Style.Font.Color.SetColor(Color.Blue);
            ws.Cells["G1,G3"].Style.Font.UnderLine = true;

            ws.Cells["A1:G5"].Copy(ws.Cells["A50"]);

            var ws2 = _pck.Workbook.Worksheets.Add("Copy Cells");
            ws.Cells["1:4"].Copy(ws2.Cells["1:1"]);

            ws.Cells["H1:J5"].Merge = true;
            ws.Cells["2:3"].Copy(ws.Cells["50:51"]);

            ws.Cells["A40"].Value = new string(new char[] { (char)8, (char)9 });

            ExcelRange styleRng = ws.Cells["A1"];
            ExcelStyle tempStyle = styleRng.Style;
            var namedStyle = _pck.Workbook.Styles.CreateNamedStyle("HyperLink", tempStyle);
            namedStyle.Style.Font.UnderLineType = ExcelUnderLineType.Single;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);
        }
        [TestMethod]
        public void Encoding()
        {
            var ws = _pck.Workbook.Worksheets.Add("Encoding");
            ws.Cells["A1"].Value = "_x0099_";
            ws.Cells["A2"].Value = " Test \b" + (char)1 + " end\"";
            ws.Cells["A3"].Value = "_x0097_ test_x001D_1234";
            ws.Cells["A4"].Value = "test" + (char)31;   //Bug issue 14689 //Fixed
        }
        [TestMethod]
        public void WorksheetCopy()
        {
            using (var pck = OpenPackage("WorksheetRead.xlsx"))
            {
                var ws = TryGetWorksheet(pck, "Address");
                var wsCopy = pck.Workbook.Worksheets.Add("Copied Address", ws);

                ws = TryGetWorksheet(pck, "Comment");
                ws = pck.Workbook.Worksheets.Add("Copied Comment", ws);

                ExcelPackage pck2 = new ExcelPackage();
                pck2.Workbook.Worksheets.Add("Copy From other pck", pck.Workbook.Worksheets["Address"]);
                pck2.SaveAs(new FileInfo(_worksheetPath + "copy.xlsx"));
                pck2.Dispose();
                pck2 = null;
                Assert.AreEqual(6, ws.Comments.Count);
            }
        }
        [TestMethod]
        public void LoadFromCollectionTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("LoadFromCollection");
            List<TestDTO> list = new List<TestDTO>();
            list.Add(new TestDTO() { Id = 1, Name = "Item1", Boolean = false, Date = new DateTime(2011, 1, 1), dto = null, NameVar = "Field 1" });
            list.Add(new TestDTO() { Id = 2, Name = "Item2", Boolean = true, Date = new DateTime(2011, 1, 15), dto = new TestDTO(), NameVar = "Field 2" });
            list.Add(new TestDTO() { Id = 3, Name = "Item3", Boolean = false, Date = new DateTime(2011, 2, 1), dto = null, NameVar = "Field 3" });
            list.Add(new TestDTO() { Id = 4, Name = "Item4", Boolean = true, Date = new DateTime(2011, 4, 19), dto = list[1], NameVar = "Field 4" });
            list.Add(new TestDTO() { Id = 5, Name = "Item5", Boolean = false, Date = new DateTime(2011, 5, 8), dto = null, NameVar = "Field 5" });
            list.Add(new TestDTO() { Id = 6, Name = "Item6", Boolean = true, Date = new DateTime(2010, 3, 27), dto = null, NameVar = "Field 6" });
            list.Add(new TestDTO() { Id = 7, Name = "Item7", Boolean = false, Date = new DateTime(2009, 1, 5), dto = list[3], NameVar = "Field 7" });
            list.Add(new TestDTO() { Id = 8, Name = "Item8", Boolean = true, Date = new DateTime(2018, 12, 31), dto = null, NameVar = "Field 8" });
            list.Add(new TestDTO() { Id = 9, Name = "Item9", Boolean = false, Date = new DateTime(2010, 2, 1), dto = null, NameVar = "Field 9" });

            ws.Cells["A1"].LoadFromCollection(list, true);
            ws.Cells["A30"].LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.Medium9, BindingFlags.Instance | BindingFlags.Instance, typeof(TestDTO).GetFields());

            ws.Cells["A45"].LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.Light1, BindingFlags.Instance | BindingFlags.Instance, new MemberInfo[] { typeof(TestDTO).GetMethod("GetNameID"), typeof(TestDTO).GetField("NameVar") });
            ws.Cells["J1"].LoadFromCollection(from l in list where l.Boolean orderby l.Date select new { Name = l.Name, Id = l.Id, Date = l.Date, NameVariable = l.NameVar }, true, OfficeOpenXml.Table.TableStyles.Dark4);

            var ints = new int[] { 1, 3, 4, 76, 2, 5 };
            ws.Cells["A15"].Value = ints;

            ws = _pck.Workbook.Worksheets.Add("LoadFromCollection_Inherited");
            List<InheritTestDTO> inhList = new List<InheritTestDTO>();
            inhList.Add(new InheritTestDTO() { Id = 1, Name = "Item1", Boolean = false, Date = new DateTime(2011, 1, 1), dto = null, NameVar = "Field 1", InheritedProp="Inherited 1" });
            inhList.Add(new InheritTestDTO() { Id = 2, Name = "Item2", Boolean = true, Date = new DateTime(2011, 1, 15), dto = new TestDTO(), NameVar = "Field 2", InheritedProp = "Inherited 2" });
            ws.Cells["A1"].LoadFromCollection(inhList, true);
            Assert.AreEqual("Inherited 2", ws.Cells[3, 1].Value);

            ws.Cells["A5"].LoadFromCollection(inhList, true, TableStyles.None, BindingFlags.Public | BindingFlags.Instance, new MemberInfo[]{typeof(InheritTestDTO).GetProperty("InheritedProp"), typeof(TestDTO).GetProperty("Name") });
            Assert.AreEqual("Inherited 2", ws.Cells[7, 1].Value);

        }
        [TestMethod]
        public void LoadFromEmptyCollectionTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("LoadFromEmptyCollection");
            List<TestDTO> listDTO = new List<TestDTO>(0);
            //List<int> list = new List<int>(0);

            ws.Cells["A1"].LoadFromCollection(listDTO, true);
            ws.Cells["A5"].LoadFromCollection(listDTO, true, OfficeOpenXml.Table.TableStyles.Medium9, BindingFlags.Instance | BindingFlags.Instance, typeof(TestDTO).GetFields());

            ws.Cells["A10"].LoadFromCollection(listDTO, true, OfficeOpenXml.Table.TableStyles.Light1, BindingFlags.Instance | BindingFlags.Instance, new MemberInfo[] { typeof(TestDTO).GetMethod("GetNameID"), typeof(TestDTO).GetField("NameVar") });
            ws.Cells["A15"].LoadFromCollection(from l in listDTO where l.Boolean orderby l.Date select new { Name = l.Name, Id = l.Id, Date = l.Date, NameVariable = l.NameVar }, true, OfficeOpenXml.Table.TableStyles.Dark4);

            ws.Cells["A20"].LoadFromCollection(listDTO, false);
        }
        [TestMethod]
        public void LoadFromOneCollectionTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("LoadFromOneCollection");
            List<TestDTO> listDTO = new List<TestDTO>(0) { new TestDTO() { Name = "Single" } };
            //List<int> list = new List<int>(0);

            var r = ws.Cells["A1"].LoadFromCollection(listDTO, true);
            Assert.AreEqual(2, r.Rows);
            var r2 = ws.Cells["A5"].LoadFromCollection(listDTO, false);
            Assert.AreEqual(1, r2.Rows);
        }
        static void Create(string file)
        {
            ExcelPackage pack = new ExcelPackage(new FileInfo(file));
            ExcelWorksheet w = pack.Workbook.Worksheets.Add("delete");
            w.Cells[1, 1].Value = "test";
            w.Cells[1, 2].Value = "test";
            w.Cells[2, 1].Value = "to delete";
            w.Cells[2, 2].Value = "to delete";
            w.Cells[3, 1].Value = "3Left";
            w.Cells[3, 2].Value = "3Left";
            w.Cells[4, 1].Formula = "B3+C3";
            w.Cells[4, 2].Value = "C3+D3";
            pack.Save();
        }
        [TestMethod]
        public void HideTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("Hidden");
            ws.Cells["A1"].Value = "This workbook is hidden";
            ws.Hidden = eWorkSheetHidden.Hidden;
        }

        [TestMethod]
        public void Hyperlink()
        {
            var ws = _pck.Workbook.Worksheets.Add("HyperLinks");
            var hl = new ExcelHyperLink("G1", "Till G1");
            hl.ToolTip = "Link to cell G1";
            ws.Cells["A1"].Hyperlink = hl;
            //ws.Cells["A2"].Hyperlink = new ExcelHyperLink("mailto:ecsomany@google:huszar", UriKind.Absolute); //Invalid URL will throw an Exception
        }
        [TestMethod]
        public void VeryHideTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("VeryHidden");
            ws.Cells["a1"].Value = "This workbook is hidden";
            ws.Hidden = eWorkSheetHidden.VeryHidden;
        }
        [TestMethod]
        public void PrinterSettings()
        {
            var ws = _pck.Workbook.Worksheets.Add("Sod/Hydroseed");

            ws.Cells[1, 1].Value = "1; 1";
            ws.Cells[2, 1].Value = "2; 1";
            ws.Cells[1, 2].Value = "1; 2";
            ws.Cells[2, 2].Value = "2; 2";
            ws.Cells[1, 1, 1, 2].AutoFilter = true;
            ws.PrinterSettings.BlackAndWhite = true;
            ws.PrinterSettings.ShowGridLines = true;
            ws.PrinterSettings.ShowHeaders = true;
            ws.PrinterSettings.PaperSize = ePaperSize.A4;

            ws.PrinterSettings.RepeatRows = new ExcelAddress("1:1");
            ws.PrinterSettings.RepeatColumns = new ExcelAddress("A:A");

            ws.PrinterSettings.Draft = true;
            var r = ws.Cells["A26"];
            r.Value = "X";
            r.Worksheet.Row(26).PageBreak = true;
            ws.PrinterSettings.PrintArea = ws.Cells["A1:B2"];
            ws.PrinterSettings.HorizontalCentered = true;
            ws.PrinterSettings.VerticalCentered = true;

            ws.Select(new ExcelAddress("3:4,E5:F6"));
        }
        [TestMethod]
        public void StyleNameTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("StyleNameTest");

            ws.Cells[1, 1].Value = "R1 C1";
            ws.Cells[1, 2].Value = "R1 C2";
            ws.Cells[1, 3].Value = "R1 C3";
            ws.Cells[2, 1].Value = "R2 C1";
            ws.Cells[2, 2].Value = "R2 C2";
            ws.Cells[2, 3].Value = "R2 C3";
            ws.Cells[3, 1].Value = double.PositiveInfinity;
            ws.Cells[3, 2].Value = double.NegativeInfinity;
            ws.Cells[4, 1].CreateArrayFormula("A1+B1");
            var ns = _pck.Workbook.Styles.CreateNamedStyle("TestStyle");
            ns.Style.Font.Bold = true;

            ws.Cells.Style.Locked = true;
            ws.Cells["A1:C1"].StyleName = "TestStyle";
            ws.DefaultRowHeight = 35;
            ws.Cells["A1:C4"].Style.Locked = false;
            ws.Protection.IsProtected = true;
        }
        [TestMethod]
        public void ValueError()
        {
            var ws = _pck.Workbook.Worksheets.Add("ValueError");

            ws.Cells[1, 1].Value = "Domestic Violence&#xB; and the Professional";
            var rt = ws.Cells[1, 2].RichText.Add("Domestic Violence&#xB; and the Professional 2");
            Console.WriteLine(rt.Bold.ToString());
            rt.Bold = true;
            Console.WriteLine(rt.Bold.ToString());
        }   
        [TestMethod]
        public void FormulaError()
        {
            var ws = _pck.Workbook.Worksheets.Add("FormulaError");

            ws.Cells["D5"].Formula = "COUNTIF(A1:A100,\"Miss\")";
            ws.Cells["A1:K3"].Formula = "A3+A4";
            ws.Cells["A4"].FormulaR1C1 = "+ROUNDUP(RC[1]/10,0)*10";

            ws = _pck.Workbook.Worksheets.Add("Sheet-RC1");
            ws.Cells["A4"].FormulaR1C1 = "+ROUNDUP('Sheet-RC1'!RC[1]/10,0)*10";

            //ws.Cells["B2:I2"].Formula = "";   //Error
        }
        [TestMethod]
        public void FormulaArray()
        {
            var ws = _pck.Workbook.Worksheets.Add("FormulaArray");

            ws.Cells["E2:E5"].CreateArrayFormula("FREQUENCY(B2:B18,C2:C5)");
        }
        [TestMethod]
        public void PivotTableTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("PivotTable");
            ws.Cells["A1"].LoadFromArrays(new object[][] { new[] { "A&B", "B\"", "C'", "<D>" } });
            ws.Cells["A2"].LoadFromArrays(new object[][]
            {
                new object [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                new object [] { 9, 8, 7 ,6, 5, 4, 3, 2, 1, 0 },
                new object [] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55}
            });
            var table = ws.Tables.Add(ws.Cells["A1:D4"], "PivotData");
            var pt=ws.PivotTables.Add(ws.Cells["G20"], ws.Cells["A1:D4"], "PivotTable1");
            pt.ColumnFields.Add(pt.Fields[1]);
            pt.DataFields.Add(pt.Fields[3]);
            Assert.AreEqual("PivotStyleMedium9", ws.PivotTables["PivotTable1"].StyleName);

            _pck.Workbook.Worksheets.AddChart("PivotChartWorksheet", eChartType.Line, pt);
        }
       [TestMethod]
        public void TableTest()
        {
            var ws = _pck.Workbook.Worksheets.Add("Table");
            ws.Cells["B1"].Value = 123;
            var tbl = ws.Tables.Add(ws.Cells["B1:P12"], "TestTable");
            tbl.TableStyle = OfficeOpenXml.Table.TableStyles.Custom;

            tbl.ShowFirstColumn = true;
            tbl.ShowTotal = true;
            tbl.ShowHeader = true;
            tbl.ShowLastColumn = true;
            tbl.ShowFilter = false;
            Assert.AreEqual(tbl.ShowFilter, false);
            ws.Cells["K2"].Value = 5;
            ws.Cells["J3"].Value = 4;

            tbl.Columns[8].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
            tbl.Columns[9].TotalsRowFormula = string.Format("SUM([{0}])", tbl.Columns[9].Name);
            tbl.Columns[14].CalculatedColumnFormula = "TestTable[[#This Row],[123]]+TestTable[[#This Row],[Column2]]";
            ws.Cells["B2"].Value = 1;
            ws.Cells["B3"].Value = 2;
            ws.Cells["B4"].Value = 3;
            ws.Cells["B5"].Value = 4;
            ws.Cells["B6"].Value = 5;
            ws.Cells["B7"].Value = 6;
            ws.Cells["B8"].Value = 7;
            ws.Cells["B9"].Value = 8;
            ws.Cells["B10"].Value = 9;
            ws.Cells["B11"].Value = 10;
            ws.Cells["B12"].Value = 11;
            ws.Cells["C7"].Value = "Table test";
            ws.Cells["C8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["C8"].Style.Fill.BackgroundColor.SetColor(Color.Red);

            tbl = ws.Tables.Add(ws.Cells["a12:a13"], "");

            tbl = ws.Tables.Add(ws.Cells["C16:Y35"], "");
            tbl.TableStyle = OfficeOpenXml.Table.TableStyles.Medium14;
            tbl.ShowFirstColumn = true;
            tbl.ShowLastColumn = true;
            tbl.ShowColumnStripes = true;
            Assert.AreEqual(tbl.ShowFilter, true);
            tbl.Columns[2].Name = "Test Column Name";

            ws.Cells["G50"].Value = "Timespan";
            ws.Cells["G51"].Value = new DateTime(new TimeSpan(1, 1, 10).Ticks); //new DateTime(1899, 12, 30, 1, 1, 10);
            ws.Cells["G52"].Value = new DateTime(1899, 12, 30, 2, 3, 10);
            ws.Cells["G53"].Value = new DateTime(1899, 12, 30, 3, 4, 10);
            ws.Cells["G54"].Value = new DateTime(1899, 12, 30, 4, 5, 10);

            ws.Cells["G51:G55"].Style.Numberformat.Format = "HH:MM:SS";
            tbl = ws.Tables.Add(ws.Cells["G50:G54"], "");
            tbl.ShowTotal = true;
            tbl.ShowFilter = false;
            tbl.Columns[0].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
        }
        [TestMethod]
        public void CopyTable()
        {
            using (var pck = OpenPackage("WorksheetRead.xlsx"))
            {
                TryGetWorksheet(pck, "File4");
                pck.Workbook.Worksheets.Copy("File4", "Copied table");
            }
        }
        [TestMethod]
        public void CopyRange()
        {
            var ws = _pck.Workbook.Worksheets.Add("CopyTest");

            ws.Cells["A1"].Value = "Single Cell";
            ws.Cells["A2"].Value = "Merged Cells";
            ws.Cells["A2:D30"].Merge = true;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["G4:H5"].Merge = true;
            ws.Cells["B3:C5"].Copy(ws.Cells["G4"]);
        }
        [TestMethod]
        public void CopyMergedRange()
        {
            var ws = _pck.Workbook.Worksheets.Add("CopyMergedRangeTest");

            ws.Cells["A11:C11"].Merge = true;
            ws.Cells["A12:C12"].Merge = true;

            var source = ws.Cells["A11:C12"];
            var target = ws.Cells["A21"];

            source.Copy(target);

            var a21 = ws.Cells[21, 1];
            var a22 = ws.Cells[22, 1];

            Assert.IsTrue(a21.Merge);
            Assert.IsTrue(a22.Merge);

            //Assert.AreNotEqual(a21.MergeId, a22.MergeId);
        }
        [TestMethod]
        public void CopyPivotTable()
        {
            using (var pck = OpenPackage("worksheetRead.xlsx"))
            {
                var ws = TryGetWorksheet(pck, "Pivot-Group Date");
                pck.Workbook.Worksheets.Copy("Pivot-Group Date", "Copied Pivottable 1");
                ws = TryGetWorksheet(pck, "Pivot-Group Number");
                pck.Workbook.Worksheets.Copy("Pivot-Group Number", "Copied Pivottable 2");

                SaveWorkbook("WorksheetCopyPivot.xlsx", pck);
            }
        }
        [TestMethod]
        public void FormulaOverwrite()
        {
            var ws = _pck.Workbook.Worksheets.Add("FormulaOverwrite");
            //Inside
            ws.Cells["A1:G12"].Formula = "B1+C1";
            ws.Cells["B2:C3"].Formula = "G2+E1";


            //Top bottom overwrite
            ws.Cells["A14:G26"].Formula = "B1+C1+D1";
            ws.Cells["B13:C28"].Formula = "G2+E1";

            //Top bottom overwrite
            ws.Cells["B30:E42"].Formula = "B1+C1+$D$1";
            ws.Cells["A32:H33"].Formula = "G2+E1";

            ws.Cells["A50:A59"].CreateArrayFormula("C50+D50");
            ws.Cells["A1"].Value = "test";
            ws.Cells["A15"].Value = "Värde";
            ws.Cells["C12"].AddComment("Test", "JJOD");
            ws.Cells["D12:I12"].Merge = true;
            ws.Cells["D12:I12"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            ws.Cells["D12:I12"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            ws.Cells["D12:I12"].Style.WrapText = true;

            ws.Cells["F1:F3"].Formula = "F2+F3";
            ws.Cells["J1:J3"].Formula = "F2+F3";
            ws.Cells["F1:F3"].Formula = "F5+F6";    //Overwrite same range
        }
        [TestMethod]
        public void DefinedName()
        {
            var ws = _pck.Workbook.Worksheets.Add("Names");
            ws.Names.Add("RefError", ws.Cells["#REF!"]);

            ws.Cells["A1"].Value = "Test";
            ws.Cells["A1"].Style.Font.Size = 8.5F;

            ws.Names.Add("Address", ws.Cells["A2:A3"]);
            ws.Cells["Address"].Value = 1;
            ws.Names.AddValue("Value", 5);
            ws.Names.Add("FullRow", ws.Cells["2:2"]);
            ws.Names.Add("FullCol", ws.Cells["A:A"]);

            ws.Names.AddFormula("Formula", "Names!A2+Names!A3+Names!Value");
        }
        [TestMethod]
        public void LoadDataReader()
        {
            if (_pck == null) _pck = new ExcelPackage();
            var ws = _pck.Workbook.Worksheets.Add("Loaded DataReader");
            ExcelRangeBase range;
            using (var dt = GetDataTable())
            {
                using (var reader = dt.CreateDataReader())
                {
                    range = ws.Cells["A1"].LoadFromDataReader(reader, true, "My_Table", TableStyles.Medium5);
                }
                Assert.AreEqual(1, range.Start.Column);
                Assert.AreEqual(4, range.End.Column);
                Assert.AreEqual(1, range.Start.Row);
                Assert.AreEqual(3, range.End.Row);

                using (var reader = dt.CreateDataReader())
                {
                    range = ws.Cells["A5"].LoadFromDataReader(reader, false, "My_Table2", TableStyles.Medium5);
                }
            }
            Assert.AreEqual(1, range.Start.Column);
            Assert.AreEqual(4, range.End.Column);
            Assert.AreEqual(5, range.Start.Row);
            Assert.AreEqual(6, range.End.Row);
        }
        [TestMethod]
        public async Task LoadDataReaderAsync()
        {
            if (_pck == null) _pck = new ExcelPackage();
            var ws = _pck.Workbook.Worksheets.Add("Loaded DataReader Async");
            ExcelRangeBase range;
            using (var dt = GetDataTable())
            {
                using (var reader = dt.CreateDataReader())
                {
                    range = await ws.Cells["A1"].LoadFromDataReaderAsync(reader, true, "My_Table_Async", TableStyles.Medium5).ConfigureAwait(false);
                }
                Assert.AreEqual(1, range.Start.Column);
                Assert.AreEqual(4, range.End.Column);
                Assert.AreEqual(1, range.Start.Row);
                Assert.AreEqual(3, range.End.Row);

                using (var reader = dt.CreateDataReader())
                {
                    range = await ws.Cells["A5"].LoadFromDataReaderAsync(reader, false, "My_Table_Async2", TableStyles.Medium5).ConfigureAwait(false);
                }
            }
            Assert.AreEqual(1, range.Start.Column);
            Assert.AreEqual(4, range.End.Column);
            Assert.AreEqual(5, range.Start.Row);
            Assert.AreEqual(6, range.End.Row);
        }
        private static DataTable GetDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("String", typeof(string));
            dt.Columns.Add("Int", typeof(int));
            dt.Columns.Add("Bool", typeof(bool));
            dt.Columns.Add("Double", typeof(double));

            var dr = dt.NewRow();
            dr[0] = "Row1";
            dr[1] = 1;
            dr[2] = true;
            dr[3] = 1.5;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "Row2";
            dr[1] = 2;
            dr[2] = false;
            dr[3] = 2.25;
            dt.Rows.Add(dr);
            return dt;
        }
    
        [TestMethod]
        public void LoadDataTable()
        {
            if (_pck == null) _pck = new ExcelPackage();
            _pck.Workbook.Properties.Title = $"from {DateTime.Today.AddDays(-7):D} to {DateTime.Today:D}";
            _pck.Workbook.Properties.Author = "Jan Källman";
            _pck.Workbook.Properties.Company = "EPPlus software";

            var ws = _pck.Workbook.Worksheets.Add("Loaded DataTable");
            var dt = new DataTable();
            dt.Columns.Add("String", typeof(string));
            dt.Columns.Add("Int", typeof(int));
            dt.Columns.Add("Bool", typeof(bool));
            dt.Columns.Add("Double", typeof(double));


            var dr = dt.NewRow();
            dr[0] = "Row1";
            dr[1] = 1;
            dr[2] = true;
            dr[3] = 1.5;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "Row2";
            dr[1] = 2;
            dr[2] = false;
            dr[3] = 2.25;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "Row3";
            dr[1] = 3;
            dr[2] = true;
            dr[3] = 3.125;
            dt.Rows.Add(dr);

            var range = ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Medium5);
            range.AutoFilter = true;
            ws.Tables[0].Columns[1].TotalsRowFunction = OfficeOpenXml.Table.RowFunctions.Sum;
            ws.Tables[0].ShowTotal = true;

            Assert.AreEqual("A1:D4", range.Address);
            Assert.AreEqual("String", ws.Cells["A1"].Value);
            Assert.AreEqual("Double", ws.Cells["D1"].Value);
            Assert.AreEqual("Row3", ws.Cells["A4"].Value);
            Assert.AreEqual(3.125, ws.Cells["D4"].Value);
        }
        [TestMethod]
        public void LoadEmptyDataTable()
        {
            if (_pck == null) _pck = new ExcelPackage();
            var ws = _pck.Workbook.Worksheets.Add("Loaded Empty DataTable");

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("col1"));
            dt.Columns.Add(new DataColumn("col2"));
            var range = ws.Cells["A1"].LoadFromDataTable(dt, true);
            Assert.AreEqual("A1:B1", range.Address);
            range = ws.Cells["D1"].LoadFromDataTable(dt, false);
            Assert.IsNull(range);
        }
        [TestMethod]
        public void LoadText_Bug15015()
        {
            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Loaded Text");
            ws.Cells["A1"].LoadFromText("\"text with eol,\r\n in a cell\",\"other value\"", new ExcelTextFormat { TextQualifier = '"', EOL = ",\r\n", Delimiter = ',' });
        }

        [TestMethod]
        public void LoadText_Bug15015_Negative()
        {
            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Loaded Text");
            bool exceptionThrown = false;
            try
            {
                ws.Cells["A1"].LoadFromText("\"text with eol,\r\n",
                                            new ExcelTextFormat { TextQualifier = '"', EOL = ",\r\n", Delimiter = ',' });
            }
            catch //(Exception e)
            {
                //Assert.AreEqual("Text delimiter is not closed in line : \"text with eol", e.Message, "Exception message");
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Exception thrown");
        }

        [TestMethod]
        public void LoadText()
        {
            var ws = _pck.Workbook.Worksheets.Add("Loaded Text");

            ws.Cells["A1"].LoadFromText("1.2");
            ws.Cells["A2"].LoadFromText("1,\"Test av data\",\"12,2\",\"\"Test\"\"");
            ws.Cells["A3"].LoadFromText("\"1,3\",\"Test av \"\"data\",\"12,2\",\"Test\"\"\"", new ExcelTextFormat() { TextQualifier = '"' });
            ws.Cells["A4"].LoadFromText("\"1,3\",\"\",\"12,2\",\"Test\"\"\"", new ExcelTextFormat() { TextQualifier = '"' });

            ws = _pck.Workbook.Worksheets.Add("File1");
            if(File.Exists(@"c:\temp\csv\et1c1004.csv"))
                ws.Cells["A1"].LoadFromText(new FileInfo(@"c:\temp\csv\et1c1004.csv"), new ExcelTextFormat() {SkipLinesBeginning=3,SkipLinesEnd=1, EOL="\n"});

            ws = _pck.Workbook.Worksheets.Add("File2");
            if (File.Exists(@"c:\temp\csv\etiv2812.csv"))
                ws.Cells["A1"].LoadFromText(new FileInfo(@"c:\temp\csv\etiv2812.csv"), new ExcelTextFormat() { SkipLinesBeginning = 3, SkipLinesEnd = 1, EOL = "\n" });

            ws = _pck.Workbook.Worksheets.Add("File3");
            if (File.Exists(@"c:\temp\csv\last_gics.txt"))
                ws.Cells["A1"].LoadFromText(new FileInfo(@"c:\temp\csv\last_gics.txt"), new ExcelTextFormat() { SkipLinesBeginning = 1, Delimiter='|'});

            ws = _pck.Workbook.Worksheets.Add("File4");

            ws.Cells["A1"].LoadFromText("1,\"Test\",\"\",\"\"\"\",3\r\n", new ExcelTextFormat() { TextQualifier = '\"' });

            var style = _pck.Workbook.Styles.CreateNamedStyle("RedStyle");
            style.Style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Style.Fill.BackgroundColor.SetColor(Color.Red);
        }
        [TestMethod]
        public void TestRepeatRowsAndColumnsTest()
        {
            var p = new ExcelPackage();

            var w = p.Workbook.Worksheets.Add("RepeatRowsAndColumnsTest");

            w.PrinterSettings.RepeatColumns = new ExcelAddress("A:A");
            w.PrinterSettings.RepeatRows = new ExcelAddress("1:1");

            Assert.IsNotNull(w.PrinterSettings.RepeatColumns);
            Assert.IsNotNull(w.PrinterSettings.RepeatRows); // Fails!
        }
        [TestMethod]
        public void Merge()
        {
            var ws = _pck.Workbook.Worksheets.Add("Merge");
            ws.Cells["A1:A4"].Merge = true;
            ws.Cells["C1:C4,C8:C12"].Merge = true;
            ws.Cells["D13:E18,G5,U32:U45"].Merge = true;
            ws.Cells["D13:E18,G5,U32:U45"].Style.WrapText = true;
            //ws.Cells["50:52"].Merge = true;
            ws.Cells["AA:AC"].Merge = true;
            ws.SetValue(13, 4, "Merged\r\nnew row");
        }
        [TestMethod]
        public void DefaultColWidth()
        {
            var ws = _pck.Workbook.Worksheets.Add("DefColWidth");
            ws.DefaultColWidth = 45;
        }
        [TestMethod]
        public void LoadArray()
        {
            var ws = _pck.Workbook.Worksheets.Add("Loaded Array");
            List<object[]> testArray = new List<object[]>() { new object[] { 3, 4, 5 }, new string[] { "Test1", "test", "5", "6" } };
            var range = ws.Cells["A1"].LoadFromArrays(testArray);
            Assert.AreEqual("A1:D2", range.Address);
        }
        [TestMethod]
        public void SetBackground()
        {
            var ws = _pck.Workbook.Worksheets.Add("backimg");

            ws.BackgroundImage.Image.SetImage(Properties.Resources.Test1JpgByteArray, ePictureType.Jpg);
            ws = _pck.Workbook.Worksheets.Add("backimg2");
            ws.BackgroundImage.Image.SetImage(new MemoryStream(Properties.Resources.CodeEmfByteArray), ePictureType.Emf);
        }
        [TestMethod]
        public void RemoveBackground()
        {
            var ws = _pck.Workbook.Worksheets.Add("Remove_BackImg");

            ws.BackgroundImage.Image.SetImage(new MemoryStream(Properties.Resources.Png2ByteArray), ePictureType.Png);
            Assert.AreEqual(ePictureType.Png, ws.BackgroundImage.Image.Type);
            Assert.IsNotNull(ws.BackgroundImage.Image.ImageBytes);
            
            ws.BackgroundImage.Remove();
            Assert.IsNull(ws.BackgroundImage.Image.Type);
            Assert.IsNull(ws.BackgroundImage.Image.ImageBytes);
        }
        [TestMethod]
        public void SetPngFromExcelImage()
        {
            var ws = _pck.Workbook.Worksheets.Add("BackImg_Png");
            var wsCopy = _pck.Workbook.Worksheets.Add("BackImg_Png_Copy");

            var image=ws.BackgroundImage.Image.SetImage(new MemoryStream(Properties.Resources.Png3ByteArray), ePictureType.Png);
            Assert.AreEqual(ePictureType.Png, ws.BackgroundImage.Image.Type);
            Assert.IsNotNull(ws.BackgroundImage.Image.ImageBytes);
            wsCopy.BackgroundImage.Image.SetImage(image);
            
            Assert.AreEqual(wsCopy.BackgroundImage.Image.Type, ws.BackgroundImage.Image.Type);
            Assert.AreEqual(wsCopy.BackgroundImage.Image.ImageBytes, ws.BackgroundImage.Image.ImageBytes);
        }
        [TestMethod]
        public void SetHeaderFooterImage()
        {
            var ws = _pck.Workbook.Worksheets.Add("HeaderImage");
            ws.HeaderFooter.OddHeader.CenteredText = "Before ";
            var img = ws.HeaderFooter.OddHeader.InsertPicture(Properties.Resources.Test1, PictureAlignment.Centered);
            img.Title = "Renamed Image";
            //img.GrayScale = true;
            //img.BiLevel = true;
            //img.Gain = .5;
            //img.Gamma = .35;

            Assert.AreEqual(img.Width, 426);
            img.Width /= 4;
            Assert.AreEqual(img.Height, 49.5);
            img.Height /= 4;
            Assert.AreEqual(img.Left, 0);
            Assert.AreEqual(img.Top, 0);
            ws.HeaderFooter.OddHeader.CenteredText += " After";


            //img = ws.HeaderFooter.EvenFooter.InsertPicture(new FileInfo(Path.Combine(_clipartPath,"Vector Drawing.wmf")), PictureAlignment.Left);
            //img.Title = "DiskFile";

            //img = ws.HeaderFooter.FirstHeader.InsertPicture(new FileInfo(Path.Combine(_clipartPath, "Vector Drawing2.WMF")), PictureAlignment.Right);
            //img.Title = "DiskFile2";
            ws.Cells["A1:A400"].Value = 1;

            _pck.Workbook.Worksheets.Copy(ws.Name, "Copied HeaderImage");
        }
        [TestMethod]
        public void NamedStyles()
        {
            var wsSheet = _pck.Workbook.Worksheets.Add("NamedStyles");

            var firstNamedStyle =
                _pck.Workbook.Styles.CreateNamedStyle("templateFirst");

            var s = firstNamedStyle.Style;

            s.Fill.PatternType = ExcelFillStyle.Solid;
            s.Fill.BackgroundColor.SetColor(Color.LightGreen);
            s.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            s.VerticalAlignment = ExcelVerticalAlignment.Center;

            var secondNamedStyle = _pck.Workbook.Styles.CreateNamedStyle("first", firstNamedStyle.Style).Style;
            secondNamedStyle.Font.Bold = true;
            secondNamedStyle.Font.SetFromFont("Arial Black", 8);
            secondNamedStyle.Border.Bottom.Style = ExcelBorderStyle.Medium;
            secondNamedStyle.Border.Left.Style = ExcelBorderStyle.Medium;

            wsSheet.Cells["B2"].Value = "Text Center";
            wsSheet.Cells["B2"].StyleName = "first";
            _pck.Workbook.Styles.NamedStyles[0].Style.Font.Name = "Arial";

            var rowStyle = _pck.Workbook.Styles.CreateNamedStyle("RowStyle", firstNamedStyle.Style).Style;
            rowStyle.Fill.BackgroundColor.SetColor(Color.Pink);
            wsSheet.Cells.StyleName = "templateFirst";
            wsSheet.Cells["C5:H15"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsSheet.Cells["C5:H15"].Style.Fill.BackgroundColor.SetColor(Color.OrangeRed);

            wsSheet.Cells["30:35"].StyleName = "RowStyle";
            var colStyle = _pck.Workbook.Styles.CreateNamedStyle("columnStyle", firstNamedStyle.Style).Style;
            colStyle.Fill.BackgroundColor.SetColor(Color.CadetBlue);

            wsSheet.Cells["D:E"].StyleName = "ColumnStyle";

            //Verify that cloned styles works.
            var defaultStyle = _pck.Workbook.Styles.CreateNamedStyle("Default");
            defaultStyle.Style.Font.Name = "Arial";
            defaultStyle.Style.Font.Size = 18;
            defaultStyle.Style.Font.UnderLine = true;
            var boldStyle = _pck.Workbook.Styles.CreateNamedStyle("Bold", defaultStyle.Style);
            boldStyle.Style.Font.Color.SetColor(Color.Red);

            Assert.AreEqual("Arial", defaultStyle.Style.Font.Name);
            Assert.AreEqual(18, defaultStyle.Style.Font.Size);

            Assert.AreEqual(boldStyle.Style.Font.Color.Rgb, "FFFF0000");

        }
        [TestMethod]
        public void StyleFill()
        {
            var ws = _pck.Workbook.Worksheets.Add("Fills");
            ws.Cells["A1:C3"].Style.Fill.Gradient.Type = ExcelFillGradientType.Linear;
            ws.Cells["A1:C3"].Style.Fill.Gradient.Color1.SetColor(Color.Red);
            ws.Cells["A1:C3"].Style.Fill.Gradient.Color2.SetColor(Color.Blue);

            ws.Cells["J20:J23"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["J20:J23"].Style.Fill.BackgroundColor.SetColor(0xFF,0x00,0XFF,0x00); //Green

            ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.MediumGray;
            ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);
            var r = ws.Cells["A2:A3"];
            r.Style.Fill.Gradient.Type = ExcelFillGradientType.Path;
            r.Style.Fill.Gradient.Left = 0.7;
            r.Style.Fill.Gradient.Right = 0.7;
            r.Style.Fill.Gradient.Top = 0.7;
            r.Style.Fill.Gradient.Bottom = 0.7;

            ws.Cells[4, 1, 4, 360].Style.Fill.Gradient.Type = ExcelFillGradientType.Path;

            for (double col = 1; col < 360; col++)
            {
                r = ws.Cells[4, Convert.ToInt32(col)];
                r.Style.Fill.Gradient.Degree = col;
                r.Style.Fill.Gradient.Left = col / 360;
                r.Style.Fill.Gradient.Right = col / 360;
                r.Style.Fill.Gradient.Top = col / 360;
                r.Style.Fill.Gradient.Bottom = col / 360;
            }
            r = ws.Cells["A5"];
            r.Style.Fill.Gradient.Left = .50;

            ws = _pck.Workbook.Worksheets.Add("FullFills");
            ws.Cells.Style.Fill.Gradient.Left = 0.25;
            ws.Cells["A1"].Value = "test";
            ws.Cells["A1"].RichText.Add("Test rt");
            ws.Cells.AutoFilter = true;
            Assert.AreNotEqual(ws.Cells["A1:D5"].Value, null);
        }
        [TestMethod, Ignore]
        public void BuildInStyles()
        {
            var ws = _pck.Workbook.Worksheets.Add("Default");
            ws.Cells.Style.Font.Name = "Arial";
            ws.Cells.Style.Font.Size = 15;
            ws.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.MediumDashed;
            var n = _pck.Workbook.Styles.NamedStyles[0];
            n.Style.Numberformat.Format = "yyyy";
            n.Style.Font.Name = "Arial";
            n.Style.Font.Size = 15;
            n.Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
            n.Style.Border.Bottom.Color.SetColor(Color.Red);
            n.Style.Fill.PatternType = ExcelFillStyle.Solid;
            n.Style.Fill.BackgroundColor.SetColor(Color.Blue);
            n.Style.Border.Bottom.Color.SetColor(Color.Red);
            n.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            n.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            n.Style.TextRotation = 90;
            ws.Cells["a1:c3"].StyleName = "Normal";
            //  n.CustomBuildin = true;
        }
        [TestMethod]
        public void AutoFitColumns()
        {
            var ws = _pck.Workbook.Worksheets.Add("Autofit");
            ws.Cells["A1:H1"].Value = "Auto fit column that is veeery long...";
            ws.Cells["A1:H1"].Style.Font.Name = "Arial";
            ws.Cells["B1"].Style.TextRotation = 30;
            ws.Cells["C1"].Style.TextRotation = 45;
            ws.Cells["D1"].Style.TextRotation = 75;
            ws.Cells["E1"].Style.TextRotation = 90;
            ws.Cells["F1"].Style.TextRotation = 120;
            ws.Cells["G1"].Style.TextRotation = 135;
            ws.Cells["H1"].Style.TextRotation = 180;
            ws.Cells["A1:H1"].AutoFitColumns(0);

            ws.Column(40).AutoFit();
        }
        [TestMethod]
        public void CopyOverwrite()
        {
            var ws = _pck.Workbook.Worksheets.Add("CopyOverwrite");

            for (int col = 1; col < 15; col++)
            {
                for (int row = 1; row < 30; row++)
                {
                    ws.SetValue(row, col, "cell " + ExcelAddressBase.GetAddress(row, col));
                }
            }
            ws.Cells["A1:P30"].Copy(ws.Cells["B1"]);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExcelWorksheetRenameWithStartApostropheThrowsException()
        {
            using (var package = new ExcelPackage())
            {
                var sheet1 = package.Workbook.Worksheets.Add("Sheet1");
                sheet1.Name = "'New Name";
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExcelWorksheetRenameWithEndApostropheThrowsException()
        {
            using (var package = new ExcelPackage())
            {
                var sheet1 = package.Workbook.Worksheets.Add("Sheet1");
                sheet1.Name = "New Name'";
                package.SaveAs(new FileInfo("c:\\epplustest\\ap.xlsx"));
            }
        }
        [TestMethod]
        public void CopyWorkSheetWithInsertInSharedFormula()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var sheet1 = package.Workbook.Worksheets.Add("Sheet1");
                // Creates a shared formula.
                sheet1.Cells["C1:C5"].Formula = "SUM(A1:B1)";
                var sheet2 = package.Workbook.Worksheets.Copy(sheet1.Name, "Sheet2");
                sheet1.InsertColumn(2, 1);
                // Inserting a column on sheet1 should modify the shared formula on sheet1, but not sheet2.
                Assert.AreEqual("SUM(A1:C1)", sheet1.Cells["D1"].Formula);
                Assert.AreEqual("SUM(A1:B1)", sheet2.Cells["C1"].Formula);
            }
        }
        [TestMethod]
        public void Sort()
        {            
            using (var package = OpenPackage("Sort.xlsx", true))
            {
                var ws = package.Workbook.Worksheets.Add("Sorting");
                AddSortingData(ws,1,1);
                ws.Cells["A:C"].Sort(2, true);
                ws.Cells["A:C"].Sort(new int[] { 2,1 }, new bool[] { true, false });

                AddSortingData(ws, 1, 5);
                ws.Cells["H1:H1000"].Formula = "E1*2";
                ws.Calculate();

                ws.Cells["E1:H1000"].Sort(3);

                ws.Cells["I5"].Value = 8;
                ws.Cells["I6"].Value = 1;
                ws.Cells["I7"].Value = "12";
                ws.Cells["I8"].Value = "Test";
                ws.Cells["I9"].Value = "aa";
                ws.Cells["I7"].AddComment("Comment", "Jan");


                ws.Cells["I:I"].Sort(0,true);
                package.Save();
            }
        }
        [TestMethod]
        public void ShowGridlines()
        {
            using (var p = OpenPackage("ShowGridlines.xlsx", true))
            {
                var ws = p.Workbook.Worksheets.Add("sort");
                Assert.IsTrue(ws.View.ShowGridLines);   //Default true
                ws.View.ShowGridLines = false;
                Assert.IsFalse(ws.View.ShowGridLines);
                ws.View.ShowGridLines = true;
                Assert.IsTrue(ws.View.ShowGridLines);
            }
        }
        [TestMethod]
        public void SheetProtection()
        {
            using (var p = OpenPackage("SheetProtection.xlsx", true))
            {
                var ws = p.Workbook.Worksheets.Add("Sheetprotection");

                ws.View.ShowHeaders = false;
                ws.Protection.AllowSelectLockedCells = false;
                ws.Protection.AllowInsertColumns = false;
                ws.Protection.AllowInsertRows = false;
                ws.Protection.AllowDeleteColumns = false;
                ws.Protection.AllowDeleteRows = false;
                ws.Protection.AllowSort = false;
                ws.Protection.AllowEditObject = false;
                ws.Protection.AllowFormatCells = false;
                ws.Protection.AllowInsertHyperlinks = false;
                ws.Protection.AllowFormatCells = false;
                ws.Protection.AllowFormatColumns = false;
                ws.Protection.AllowFormatRows = false;
                ws.Protection.AllowAutoFilter = false;
                ws.Protection.AllowPivotTables = false;
                ws.Cells["A3:D4"].AutoFitColumns();

                var drawing = ws.Drawings.AddShape("Note", OfficeOpenXml.Drawing.eShapeStyle.Rect);
                drawing.SetPosition(9, 0, 2, 0);
                drawing.SetSize(500, 200);
                drawing.Text = "This is a note";

                ws.Protection.SetPassword("A4A71A39-12DF-4F7A-BD4A-CCA4AABCC159");
                //ws.Protection.SetPassword("ddd");

                p.Save();
            }
        }
        [TestMethod]
        public void IgnoreErrors()
        {
            var ws = _pck.Workbook.Worksheets.Add("IgnoreError");
            ws.Cells["A1"].Value = "1";
            ws.Cells["C3"].Value = "5";
            ws.Cells["F6"].Value = "7";
            var ie = ws.IgnoredErrors.Add(ws.Cells["A1"]);
            var ie2 = ws.IgnoredErrors.Add(ws.Cells["A1:C3,D6:F7"]);
            ie.NumberStoredAsText = true;
            ie2.NumberStoredAsText = true;

            _pck.Save();
        }
        [TestMethod]
        public void RepeateRowColumns()
        {
            var ws = _pck.Workbook.Worksheets.Add("RepeatRowCols");
            ws.PrinterSettings.RepeatRows = new ExcelAddress("1:1");
            ws.PrinterSettings.RepeatColumns = new ExcelAddress("B:C");
            Assert.AreEqual("B:C", ws.PrinterSettings.RepeatColumns.Address);
            Assert.AreEqual("1:1", ws.PrinterSettings.RepeatRows.Address);
            Assert.AreEqual("B:C,1:1", ws.Names["_xlnm.Print_Titles"].Address);
        }
        private static void AddSortingData(ExcelWorksheet ws, int row, int col)
        {
            var rand = new Random();
            var addr = ExcelCellBase.GetAddress(row, col, row + 999, col);
            ws.Cells[addr].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[addr].Style.Font.Color.SetColor(Color.White);
            for (var r = row; r <= row+999; r++)
            {
                var v = rand.NextDouble() * row;
                ws.SetValue(r, col, v);
                ws.SetValue(r, col+1, $"Rad {r} v={v}");
                ws.SetValue(r, col+2, $"Rad {r} v={v}");
                ws.Cells[r, col].Style.Fill.BackgroundColor.SetColor(r % 3 == 0 ? Color.Red : r % 3 == 1 ? Color.Green : Color.Blue);
                if (r % 10 == 0)
                {
                    ws.Cells[r, col+1, r, col+2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[r, col+1].Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                    ws.Cells[r, col+2].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                }
            }
        }

        /// <summary>
        ///     Get exception from thrown from <see cref="ExcelRangeBase.GetValue{T}"/>
        /// </summary>
        /// <typeparam name="T">
        ///     Type of value to attempt getting from <paramref name="range"/>
        /// </typeparam>
        /// <param name="range">
        ///     Mandatory, cell range from which to attempt getting typed value
        /// </param>
        /// <returns>
        ///     null if none is thrown
        /// </returns>
        private static Exception GetValueException<T>(ExcelRangeBase range)
        {
            return GetException(() => range.GetValue<T>());
        }

        /// <summary>
        ///     Get exception thrown by <paramref name="action"/> when invoked.
        /// </summary>
        /// <param name="action">
        ///     Mandatory
        /// </param>
        /// <returns>
        ///     null if none is thrown
        /// </returns>
        private static Exception GetException(Action action)
        {
            try
            {
                action.Invoke();
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        [TestMethod]
        public void ClearFormulasOnEmptySheetTest()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                // Should not throw an exception
                worksheet.ClearFormulas();
            }
        }

        [TestMethod]
        public void ClearFormulasTest()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = 1;
                worksheet.Cells["A2"].Value = 2;
                worksheet.Cells["A3"].Formula = "SUM(A1:A2)";
                worksheet.Calculate();
                Assert.AreEqual(3d, worksheet.Cells["A3"].Value);
                Assert.AreEqual("SUM(A1:A2)", worksheet.Cells["A3"].Formula);
                worksheet.ClearFormulas();
                Assert.AreEqual(3d, worksheet.Cells["A3"].Value);
                Assert.AreEqual(string.Empty, worksheet.Cells["A3"].Formula);
            }
        }

        [TestMethod]
        public void ClearFormulaValuesTest()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = 1;
                worksheet.Cells["A2"].Value = 2;
                worksheet.Cells["A3"].Formula = "SUM(A1:A2)";
                worksheet.Calculate();
                Assert.AreEqual(3d, worksheet.Cells["A3"].Value);
                worksheet.ClearFormulaValues();
                Assert.IsNull(worksheet.Cells["A3"].Value);
            }
        }
        [TestMethod]
        public void RemoveFormulaFromFirstCellOfSharedFormulaWithGap()
        {
            using (var p = new ExcelPackage())
            {
                // Get the worksheet
                var ws = p.Workbook.Worksheets.Add("Sheet1");

                ws.Cells["G1:G6"].Formula = "SUM(A1:F1)";
                ws.Cells["G5"].Formula = null;

                // Check that G1:G6 have formulas, except for G5
                Assert.AreEqual("SUM(A1:F1)", ws.Cells["G1"].Formula);
                Assert.AreEqual("SUM(A2:F2)", ws.Cells["G2"].Formula);
                Assert.AreEqual("SUM(A3:F3)", ws.Cells["G3"].Formula);
                Assert.AreEqual("SUM(A4:F4)", ws.Cells["G4"].Formula);
                Assert.AreEqual("", ws.Cells["G5"].Formula);
                Assert.AreEqual("SUM(A6:F6)", ws.Cells["G6"].Formula);

                // Remove the formula from G1
                ws.Cells["G1"].Formula = null;

                // Check that the formula was removed, and the other formulas are unchanged
                Assert.AreEqual("", ws.Cells["G1"].Formula);
                Assert.AreEqual("SUM(A2:F2)", ws.Cells["G2"].Formula);
                Assert.AreEqual("SUM(A3:F3)", ws.Cells["G3"].Formula);
                Assert.AreEqual("SUM(A4:F4)", ws.Cells["G4"].Formula);
                Assert.AreEqual("", ws.Cells["G5"].Formula);
                Assert.AreEqual("SUM(A6:F6)", ws.Cells["G6"].Formula);
            }
        }
        [TestMethod]
        public void ClearBySettingValueToNull()
        {
            using (var package = OpenPackage("i568.xlsx", true))
            {
                var worksheet = package.Workbook.Worksheets.Add("ClearRangeA1B2");
                var range = worksheet.Cells["A1:B2"];
                range.FillNumber(1);
                range.Value = null;

                Assert.IsNull(worksheet.Cells["A1"].Value);
                Assert.IsNull(worksheet.Cells["A2"].Value);
                Assert.IsNull(worksheet.Cells["B1"].Value);
                Assert.IsNull(worksheet.Cells["B3"].Value);
            }
        }

    }
}
