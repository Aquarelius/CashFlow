using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CashFlow.Storage;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace CashFlow.BusinessLogic.Providers
{
    public class GoogleSheetProvider : ICalendarProvider
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets, SheetsService.Scope.Drive, SheetsService.Scope.SpreadsheetsReadonly, SheetsService.Scope.DriveFile };
        private const string ApplicationName = "cashflow";

        private readonly SheetsService _sheetsService;
       
        private const string SpreadsheetId = "10VNQ_Rtgo8vxVRxE4gY5sn_aY_lLH2-WbvK3QIOYi2s";

        public GoogleSheetProvider(IDataStore tokenStore)
        {
            
            UserCredential credential;
            
            var sec = tokenStore.GetAsync<string>("client_secret.json").Result;
            var bytes = Encoding.UTF8.GetBytes(sec);
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes,0,bytes.Length);
                stream.Position = 0;
                var secrets = GoogleClientSecrets.Load(stream).Secrets;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    tokenStore).Result;
            }
          
            // Create Google Sheets API service.
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

        }

        public double GetAmountForDate(DateTime date)
        {
            double res = 0;
            var row = FindRowForDate(date);
            var val = _sheetsService.Spreadsheets.Values.Get(SpreadsheetId, $"Cashflow!E{row}").Execute();
            if (!val.Values.Any()) return res;
            var str = val.Values.First().First().ToString()
                .Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                .Replace("$", "");
            double.TryParse(str, out res);
            return res;
        }

        public void SetAmountToDate(DateTime date, double amount)
        {
            var row = FindRowForDate(date);
            var range = $"Cashflow!E{row}";

            var requestBody = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object>() {amount}
                }

            };

            var request = _sheetsService.Spreadsheets.Values.Update(requestBody, SpreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var resp = request.Execute();
            if (resp.UpdatedRows > 0)
            {
                //Mark by color
                var aa = _sheetsService.Spreadsheets.Get(SpreadsheetId).Execute();
                var sheet = aa.Sheets.FirstOrDefault(z => z.Properties.Title == "CashFlow");

                _sheetsService.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
                    {
                        new Request()
                        {
                            RepeatCell = new RepeatCellRequest
                            {
                                Range = new GridRange()
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 4,
                                    EndColumnIndex = 5,
                                    StartRowIndex = row -1,
                                    EndRowIndex = row
                                },
                                Fields = "userEnteredFormat.BackgroundColor",
                                Cell = new CellData
                                {
                                    UserEnteredFormat = new CellFormat
                                    {
                                        BackgroundColor = new Color
                                        {
                                            Green = 0.9f,
                                            Red = 0.9f,
                                            Blue = 0.9f,
                                            Alpha = 0.2f
                                        }
                                    }
                                }
                            }
                        }
                    }
                }, SpreadsheetId).Execute();
            }
        }

        private int FindRowForDate(DateTime date)
        {
            var res = -1;
            var dateString = date.ToString("dd.MM.yyyy");
            var vals = _sheetsService.Spreadsheets.Values.Get(SpreadsheetId, "CashFlow!A21:A1999").Execute();
            foreach (var val in vals.Values)
            {
                if (val.First().ToString() == dateString)
                {
                    res = 21 + vals.Values.IndexOf(val);
                    break;
                }
            }
            return res;
        }
    }
}
