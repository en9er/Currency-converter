using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace CurrencyConverter
{
    public class MainViewModel : BindableObject
    {
        public MainViewModel()
        {
            _valute = GetAllValutes();
            FSelected = GetCurrency("RUB");
            SSelected = GetCurrency("USD");
        }

        private double _first = 0;
        private double _second = 0;
        private Currency _selected;
        private Currency _target;
        private DataTable _valute;

        public static bool IsDoublesEqual(double first, double second, double epsilon)
        {
            if (Math.Abs(first - second) < epsilon)
            {
                return true;
            }
            return false;
        }

        public Currency FSelected
        {
            get
            {
                return _selected;
            }
            set
            { 
                _selected = value;
                if(_selected != null && _target != null)
                    First = _second * _selected.Value * _target.Value / _target.Nominal;
                OnPropertyChanged(nameof(FSelected));
            }
        }

        public Currency SSelected
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                if (_selected != null && _target != null)
                    Second = _first * _selected.Value / _target.Value / _selected.Nominal;
                OnPropertyChanged(nameof(SSelected));
            }
        }


        public double First
        {
            get
            {
                return _first;
            }
            set
            {
                if (value != null && _first != null && IsDoublesEqual(value, _first, 0.2))
                {
                    return;
                }
                _first = value; 
                Second = _first * _selected.Value / _target.Value / _target.Nominal;
                OnPropertyChanged(nameof(Second));
            }
        }

        public double Second
        {
            get
            {
                return _second;
            }
            set
            {
                if (value != null && _second != null && IsDoublesEqual(value, _second, 0.2))
                {
                    return;
                }
                _second = value;
                First = _second * _target.Value / _selected.Value / _target.Nominal;
                OnPropertyChanged(nameof(First));
            }
        }

        public string FSelectedChanged
        {
            get
            {
                return _selected.CharCode;
            }
            set
            {
                if(value != null)
                {
                    _selected = GetCurrency(value);
                    FSelected = _selected;
                }
                OnPropertyChanged(nameof(FSelectedChanged));
                OnPropertyChanged(nameof(First));
            }
        }

        public string SSelectedChanged
        {
            get
            {
                return _target.CharCode;
            }
            set
            {
                if(value != null)
                {
                    _target = GetCurrency(value);
                    SSelected = _target;
                }
                OnPropertyChanged(nameof(SSelectedChanged));
                OnPropertyChanged(nameof(Second));
            }
        }



        public Currency GetCurrency(string currencyName)
        {
            CultureInfo provider = new CultureInfo("fr-FR");
            if (currencyName == "RUB")
            {
                return new Currency()
                {
                    CharCode = "RUB",
                    Nominal = 1,
                    Name = "Российский рубль",
                    Value = 1
                };
            }
            foreach (DataRow row in _valute.Rows)
            {
                if (row["CharCode"].ToString() == currencyName)
                {
                    return new Currency()
                    {
                        ID = row["ID"].ToString(),
                        NumCode = row["NumCode"].ToString(),
                        CharCode = row["CharCode"].ToString(),
                        Nominal = int.Parse(row["Nominal"].ToString()),
                        Name = row["Name"].ToString(),
                        Value = double.Parse(row["Value"].ToString(), provider)
                    };
                }
            }
            return null;
        }


        public DataTable GetAllValutes()
        {
            string url = "https://www.cbr-xml-daily.ru/daily_utf8.xml";
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(url);
            }
            catch(WebException e)
            {
                _valute = GetAllValutes();
            }
            DataTable currency = ds.Tables["Valute"];
            return currency;
        }


        //public Root GetCurrencies()
        //{
        //    WebRequest req = WebRequest.Create("https://www.cbr-xml-daily.ru/daily_json.js"); 
        //    req.Method = "GET";
        //    req.ContentType = "application/x-www-urlencoded";

        //    string str = ""; 
        //    Root curentCurrency;

        //    WebResponse response = req.GetResponse();
        //    using (Stream s = response.GetResponseStream()) 
        //    {
        //        using (StreamReader r = new StreamReader(s)) 
        //        {
        //            str = r.ReadToEnd(); 
        //        }
        //    }
        //    response.Close(); 


        //    curentCurrency = JsonConvert.DeserializeObject<Root>(str);
        //    return curentCurrency;
        //}

    };
    public class Currency
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }; 
}
