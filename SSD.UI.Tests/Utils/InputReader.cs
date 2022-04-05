using NPOI.SS.UserModel;
using NUnit.Framework;
using SSD.UI.Tests.Models;
using SSD.UI.Tests.Tests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static SSD.UI.Tests.Models.EnvironmentLandContamination;

namespace SSD.UI.Tests.Utils
{
    public partial class InputReader
    {
        private static IWorkbook workbook;
        private static DataFormatter dataFormatter;
        private static IFormulaEvaluator formulaEvaluator;
       
       
        private static EnvironmentLandContamination environmentLandContamination;





        // Initialize from a stream of Excel file
        private static ISheet GetSheet(string sheetName, string workbookName = "TestData")
        {
            var inputPath = System.AppDomain.CurrentDomain.BaseDirectory + $"Input\\{workbookName}.xlsx";
            /* private static ISheet GetSheet(string sheetName)
             {
                 var inputPath = System.AppDomain.CurrentDomain.BaseDirectory + "Input\\TestData.xlsx";*/
            workbook = WorkbookFactory.Create(new FileStream(inputPath, FileMode.Open, FileAccess.Read));
            if (workbook != null)
            {
                dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
                formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(workbook);
            }

            return workbook.GetSheet(sheetName);
        }

        private static void CloseWorkBook()
        {
            if (workbook != null)
            {
                workbook.Close();
            }
        }


        //Read the EN_ sheets and put the records in to respective model List
        public static List<EnvironmentLandContamination> GetLandContaminationData()
        {
           
                var sheet = GetSheet("EN-LandContamination", "Environment");
                List<EnvironmentLandContamination> LandContanminationList = new List<EnvironmentLandContamination>();
                foreach (IRow row in sheet)
                {
                    if (row.GetCell(0).RowIndex == 0)
                    {
                        continue;
                    }
                    var landContamination = new EnvironmentLandContamination();
                    //landContamination.RecNo = GetUnformattedValue(row.GetCell(0));
                    landContamination.OrgUnit = GetUnformattedValue(row.GetCell(0));
                    landContamination.Reference = GetUnformattedValue(row.GetCell(1));
                    landContamination.EnvironmentalAssessmentReference = GetUnformattedValue(row.GetCell(2));
                    landContamination.Description = GetUnformattedValue(row.GetCell(3));
                    landContamination.Location = GetUnformattedValue(row.GetCell(4));
                    landContamination.Locality = GetUnformattedValue(row.GetCell(5));
                    landContamination.SampleDate = GetUnformattedValue(row.GetCell(6));
                    landContamination.SelectfromPersonregister = GetUnformattedValue(row.GetCell(7));
                    landContamination.PersonReference = GetUnformattedValue(row.GetCell(8));
                    landContamination.Forenames = GetUnformattedValue(row.GetCell(9));
                    landContamination.Surname = GetUnformattedValue(row.GetCell(10));
                    landContamination.SelectEquipmentfromRegister = GetUnformattedValue(row.GetCell(11));
                    landContamination.EquipmentReference = GetUnformattedValue(row.GetCell(12));
                    landContamination.EquipmentName = GetUnformattedValue(row.GetCell(13));
                    landContamination.Delete = GetUnformattedValue(row.GetCell(14));

                    LandContanminationList.Add(landContamination);
                }
                CloseWorkBook();
            EnvironmentLandContaminationTest.recCount = LandContanminationList.Count();
                return LandContanminationList;
        }
            
        
      

        // Get formatted value as string from the specified cell
        //
        protected string GetFormattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {

                try
                {
                    // Get evaluated and formatted cell value
                    returnValue = dataFormatter.FormatCellValue(cell, formulaEvaluator);
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...

                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = dataFormatter.FormatRawCellContents(cell.NumericCellValue, 0, cell.CellStyle.GetDataFormatString());
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return (returnValue ?? string.Empty).Trim();
        }


        // Get unformatted value as string from the specified cell

        protected static string GetUnformattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    if (cell.Sheet.SheetName == "UDD-BusinessCalendar")
                    {
                        var t = cell.CellType;
                    }
                    // Get evaluated cell value
                    returnValue = (cell.CellType == CellType.Numeric ||
                    (cell.CellType == CellType.Formula &&
                    cell.CachedFormulaResultType == CellType.Numeric)) ?
                        formulaEvaluator.EvaluateInCell(cell).NumericCellValue.ToString() :
                        dataFormatter.FormatCellValue(cell, formulaEvaluator);
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }
    }
}

