using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Text.RegularExpressions;

//[assembly: CommandClass(typeof(Infrakit.Commands))]

namespace Infrakit
{
    public class Commands : IExtensionApplication
    {
        public static bool isAutoCADWithSurface = false;
        public static string productId;
        public static string year;
        public void Initialize()
        {
            //https://forums.autodesk.com/t5/net/which-autocad-vertical-am-i-using/td-p/6859901
            var productKey = HostApplicationServices.Current.UserRegistryProductRootKey;
            var groups = Regex.Match(productKey, @"ACAD-([0-9A-F])\d(\d{2}):([0-9A-F]{3})").Groups;
            switch (groups[2].Value)
            {
                case "00": productId = "Autodesk Civil 3D"; break;
                case "01": productId = "AutoCAD"; break;
                case "0A": productId = "AutoCAD OEM"; break;
                case "02": productId = "AutoCAD Map"; break;
                case "04": productId = "AutoCAD Architecture"; break;
                case "05": productId = "AutoCAD Mechanical"; break;
                case "06": productId = "AutoCAD MEP"; break;
                case "07": productId = "AutoCAD Electrical"; break;
                case "16": productId = "AutoCAD P & ID"; break;
                case "17": productId = "AutoCAD Plant 3D"; break;
                case "29": productId = "AutoCAD ecscad"; break;
                case "30": productId = "AutoCAD Structural Detailing"; break;
                default: productId = "unknown"; break;
            }
            if (productId == "Autodesk Civil 3D")
            {
                isAutoCADWithSurface = true;
            }
            else
            {
                //AcAp.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKasutate " + productId + " seega program töötab vaid Civil 3D-ga");
                AcAp.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nYou are using " + productId + " because of that program will not work, this program only works with Civil 3D");
            }

            var version = (string)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("ACADVER");
            switch (version.Substring(0, 4))

            {
                case "25.0":
                    year = "2025";
                    break;
                case "24.1":
                    year = "2024";
                    break;
                case "24.0":
                    year = "2023";
                    break;
                case "23.1":
                    year = "2022";
                    break;
                case "23.0":
                    year = "2021";
                    break;
                case "22.1":
                    year = "2020";
                    break;
                case "22.0":
                    year = "2019";
                    break;
                case "21.1":
                    year = "2018";
                    break;
                case "21.0":
                    year = "2017";
                    break;
                case "20.1":
                    year = "2016";
                    break;
                case "20.0":
                    year = "2015";
                    break;
                case "19.1":
                    year = "2014";
                    break;
                case "19.0":
                    year = "2013";
                    break;
                case "18.2":
                    year = "2012";
                    break;
                case "18.0":
                    year = "2011";
                    break;
                case "17.2":
                    year = "2010";
                    break;
                case "17.0":
                    year = "2009";
                    break;
                default:
                    year = "Unknown";
                    break;
            }
        }
        public void Terminate()
        {

        }
        [CommandMethod("Infrakit")]
        public static void Infrakit()
        {
            if (isAutoCADWithSurface == true)
                Infrakit_Surface();
            else
            {
                //AcAp.ShowAlertDialog("\nKasutate " + productId + " seega programm töötab vaid vaid Civil 3D-ga");
                AcAp.ShowAlertDialog("\nYou are using " + productId + " because of that program will not work, this program only works with Civil 3D");

            }
        }
        public static void Infrakit_Surface()
        {
            Windows.Infrakit win = new Windows.Infrakit();
            AcAp.ShowModalWindow(AcAp.MainWindow.Handle, win);
            //AcAp.ShowModelessWindow(AcAp.MainWindow.Handle, win);
        }      


    }
}




       

