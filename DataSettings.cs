using Raimo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raimo
{
    public static class DataSettings
    {
        public static List<string> materials = new List<string>() { "Metall", "Betoon", "Metall" };
        public static List<string> pipeHeight = new List<string>() { "Pealt", "Alt" };
        public static List<string> pipeShape = new List<string>() { "Ümar", "Ristkülikuline" };
        public static List<string> stClass = new List<string>() { "A", "B", "C" };
        public static List<LayerMap> lms = new List<LayerMap>();
        public static List<ManufModelInfo> mms = new List<ManufModelInfo>();
        public static List<Manufacturer> manufacturers = new List<Manufacturer>()
        {
            new Manufacturer ("Tootja1", new List<string> () {"Mudel-11", "Mudel-12", "Mudel-13" }) ,
            new Manufacturer ("Tootja2", new List<string> () { "Mudel-21", "Mudel-22", "Mudel-23" }) ,
            new Manufacturer ("Tootja3", new List<string> () { "Mudel-31", "Mudel-32", "Mudel-33" })

        };
        //static CultureInfo culture = new CultureInfo("en-US");
        public static List<LicenseNode> allowedIds = new List<LicenseNode>()
        {
            new LicenseNode (Convert.ToDateTime( "25/11/2021" ),Convert.ToDateTime( "02/12/2022" ) , "BFEBFBFF000306C3"), //Imran
            new LicenseNode (Convert.ToDateTime( "25/11/2021" ),Convert.ToDateTime( "26/11/2022" ) , "BFEBFBFF000906E9"), //Raimo
            new LicenseNode (Convert.ToDateTime( "25/11/2021" ),Convert.ToDateTime( "31/01/2022" ) , "BFEBFBFF000906EA"),  //Kalle
            new LicenseNode (Convert.ToDateTime( "07/12/2021" ),Convert.ToDateTime( "01/10/2022" ) , "BFEBFBFF000A0652")   //Helena Eikla <Helena.Eikla@nordecon.com>
        };

        public enum FillResult
        {
            filled = 256,
            PartiallyFilled = 3,
            NotFilled = 2
        }

        public static class checkFields 
            {
           public static  bool ID = false ;
            public static bool Material = true ;
            public static bool Manufacturer = true;
            public static bool Model = true;
            public static bool StClass = true;
            public static bool PipeHeight = true;
            public static bool PipeGap = false ;

            public static bool pipeShape = true;
            public static bool InnerDia = false ;
            public static bool OuterDia = false ;
            public static bool InnerWidth = false ;
            public static bool OuterWidth = false ;
            public static bool InnerHeight = false ;
            public static bool OuterHeight = false ;

            public static bool Contents = false ;
            public static bool date = true ;
        }
    }
}
