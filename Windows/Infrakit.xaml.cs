using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Newtonsoft.Json.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Linq;
using Autodesk.Civil.ApplicationServices;
using System.Diagnostics;
using Autodesk.AutoCAD.EditorInput;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;




namespace Infrakit.Windows
{


    public class ListViewItemModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public ObservableCollection<ProfileModel> Profiles { get; set; } = new ObservableCollection<ProfileModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProfileModel : INotifyPropertyChanged
    {


        private bool _isSubItemSelected;

        public string Name { get; set; }

        public bool IsSubItemSelected
        {
            get => _isSubItemSelected;
            set
            {
                if (_isSubItemSelected != value)
                {
                    _isSubItemSelected = value;
                    OnPropertyChanged(nameof(IsSubItemSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SelectedAlignmentProfile
    {
        public string AlignmentName { get; set; }
        public List<ProfileModel> ProfileNames { get; set; } // List to store profile models for the alignment
        public bool IsAlignmentSelected { get; set; } // Indicates if the alignment is selected
        public bool IsProfileSelected { get; set; } // Indicates if any profile is selected

        public SelectedAlignmentProfile()
        {
            ProfileNames = new List<ProfileModel>();
            IsAlignmentSelected = false;
        }
    }



    public partial class Infrakit : Window
    {
        private string accessToken;
        private string decryptedParool;
        private string responseText;
        private string ProjectUUid;
        private Boolean DecryptedPassword;
        private Dictionary<string, string> SurfaceFolderNameUUidPairs;
        private Dictionary<string, string> AlignmentFolderNameUUidPairs;
        private Dictionary<string, string> ProjectNameUUidPairs;
        private List<string> allSurfaceNames; // Store all surface names during initialization
        private Dictionary<string, SelectedAlignmentProfile> allAlignmentData = new Dictionary<string, SelectedAlignmentProfile>();


        private List<string> SelectedPinnad;
        public ObservableCollection<ListViewItemModel> AlignmentItems { get; set; }


        public ObservableCollection<ListViewItemModel> SurfaceItems { get; set; } = new ObservableCollection<ListViewItemModel>();
        string Year = Commands.year;

        [CommandMethod("Upload")]
        public void Upload()
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Stopwatch stopwatch = Stopwatch.StartNew();  // Start the stopwatch
            TimeSpan elapsedTime;
            try
            {
                if (string.IsNullOrEmpty(Projektid.Text) || string.IsNullOrEmpty(PindadeKaustad.Text) || string.IsNullOrEmpty(TelgedeKaustad.Text))
                {                    
                    ShowErrorForEmptyFields("Upload");
                }
                else
                {
                    string exportFilePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\AutomaticLandXML.xml";                  
                    ExecuteLandXmlOutCommand(ed, exportFilePath);
                    ProcessSurfaces(exportFilePath, db);
                    ProcessAlignments(exportFilePath, db);
                    elapsedTime = stopwatch.Elapsed;  // Get the elapsed time as a TimeSpan                 
                    MessageBox.Show($"Upload complete. It took {elapsedTime.ToString(@"mm\:ss")} to complete.");
                }
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        [CommandMethod("UploadSurfaces")]
        public void UploadSurfaces()
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Stopwatch stopwatch = Stopwatch.StartNew();  // Start the stopwatch
            TimeSpan elapsedTime;
            try
            {
                if (string.IsNullOrEmpty(Projektid.Text) || string.IsNullOrEmpty(PindadeKaustad.Text) || string.IsNullOrEmpty(TelgedeKaustad.Text))
                {
                    ShowErrorForEmptyFields("SurfaceUpload");
                }
                else
                {
                    string exportFilePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\AutomaticLandXML.xml";
                    ExecuteLandXmlOutCommand(ed, exportFilePath);
                    ProcessSurfaces(exportFilePath, db);
                    elapsedTime = stopwatch.Elapsed;  // Get the elapsed time as a TimeSpan                 
                    MessageBox.Show($"Upload complete. It took {elapsedTime.ToString(@"mm\:ss")} to complete.");
                }
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        [CommandMethod("UploadAlignments")]
        public void UploadAlignments()
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Stopwatch stopwatch = Stopwatch.StartNew();  // Start the stopwatch
            TimeSpan elapsedTime;
            try
            {
                if (string.IsNullOrEmpty(Projektid.Text) || string.IsNullOrEmpty(PindadeKaustad.Text) || string.IsNullOrEmpty(TelgedeKaustad.Text))
                {
                    ShowErrorForEmptyFields("AlignmentUpload");
                }
                else
                {
                    string exportFilePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\AutomaticLandXML.xml";
                    ExecuteLandXmlOutCommand(ed, exportFilePath);                    
                    ProcessAlignments(exportFilePath, db);
                    elapsedTime = stopwatch.Elapsed;  // Get the elapsed time as a TimeSpan                 
                    MessageBox.Show($"Upload complete. It took {elapsedTime.ToString(@"mm\:ss")} to complete.");
                }
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        public Infrakit()
        {
            try
            {
                InitializeComponent();
                Document doc = AcAp.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                allSurfaceNames = new List<string>();
                SelectedPinnad = new List<string>();
                AddSurfaces();
                SurfaceTreeView.ItemsSource = SurfaceItems;
                //allAlignmentNames = new List<string>();
                AlignmentItems = new ObservableCollection<ListViewItemModel>();
                //SelectedTeljed = new List<string>();
                AlignmentTreeView.ItemsSource = AlignmentItems;
                AddAlignmentAndProfiles();

                using (Transaction transaction = doc.TransactionManager.StartTransaction())// Start a transaction to read from the drawing database
                {
                    Database database = doc.Database; // Get the current drawing's database
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Infrakit");// Open the Application Registry for read
                    if (regKey != null)
                    {

                        string kasutajanimi = (string)regKey.GetValue("kasutajanimi");// Retrieve the data from the registry

                        if (!string.IsNullOrEmpty(kasutajanimi))
                        {
                            Kasutajanimi.Text = kasutajanimi;
                        }
                        // Retrieve the data from the registry
                        string parool = (string)regKey.GetValue("parool");

                        if (!string.IsNullOrEmpty(parool))
                        {
                            //// Decrypt the data
                            decryptedParool = StringCipher.Decrypt(parool, "TEST");
                            ParoolPasswordBox.Password = decryptedParool;
                            DecryptedPassword = true;
                        }

                    }
                    ProjectNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here
                    SurfaceFolderNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here
                    AlignmentFolderNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here                    
                    DBDictionary nod = transaction.GetObject(database.NamedObjectsDictionaryId, OpenMode.ForRead) as DBDictionary;// Open the NOD (Named Object Dictionary) for read
                    if (nod.Contains("Infrakit"))                     // Check if our custom dictionary exists
                    {
                        DBDictionary customDictionary = transaction.GetObject(nod.GetAt("Infrakit"), OpenMode.ForRead) as DBDictionary;
                        Projektid.SelectionChanged -= Projektid_SelectionChanged;
                        FillComboBoxFromDictionary(transaction, customDictionary, "Projekt", "ProjektUUid", ProjectNameUUidPairs, Projektid);
                        Projektid.SelectionChanged += Projektid_SelectionChanged;
                        PindadeKaustad.SelectionChanged -= PindadeKaustad_SelectionChanged;
                        FillComboBoxFromDictionary(transaction, customDictionary, "PinnadKaust", "PinnadKaustUUid", SurfaceFolderNameUUidPairs, PindadeKaustad);
                        PindadeKaustad.SelectionChanged += PindadeKaustad_SelectionChanged;
                        TelgedeKaustad.SelectionChanged -= TelgedeKaustad_SelectionChanged;
                        FillComboBoxFromDictionary(transaction, customDictionary, "TeljedKaust", "TeljedKaustUUid", AlignmentFolderNameUUidPairs, TelgedeKaustad);
                        TelgedeKaustad.SelectionChanged += TelgedeKaustad_SelectionChanged;
                        //Pinnad.SelectionChanged -= Pinnad_SelectionChanged;                     
                        //FillListBoxValueFromDictionary(customDictionary, transaction, "Pinnad", Pinnad, SelectedPinnad);
                        //Pinnad.SelectionChanged += Pinnad_SelectionChanged;

                        //FillSurfaceTreeViewValueFromDictionary(customDictionary, transaction, "SelectedSurfaces", SurfaceTreeView);
                        FillSurfaceTreeViewValueFromDictionary(customDictionary, transaction, "SelectedSurfaces", SurfaceTreeView, SelectedPinnad);

                        //Teljed.SelectionChanged -= Teljed_SelectionChanged;                        
                        //FillListBoxValueFromDictionary(customDictionary, transaction, "Teljed", Teljed, SelectedTeljed);
                        //Teljed.SelectionChanged += Teljed_SelectionChanged;

                        FillAlignmentTreeViewValueFromDictionary(customDictionary, transaction, "SelectedAlignments", "SelectedProfiles", AlignmentTreeView);

                        PinnaKaust.TextChanged -= PinnaKaust_TextChanged;
                        FillTextBoxValueFromDictionary(transaction, customDictionary, "PinnaKaust", PinnaKaust);
                        PinnaKaust.TextChanged += PinnaKaust_TextChanged;
                        TeljeKaust.TextChanged -= TeljeKaust_TextChanged;
                        FillTextBoxValueFromDictionary(transaction, customDictionary, "TeljeKaust", TeljeKaust);
                        TeljeKaust.TextChanged += TeljeKaust_TextChanged;
                    }
                }
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }

        }

        private void AddSurfaces()
        {
            using (var db = HostApplicationServices.WorkingDatabase)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId entId in btr)
                    {
                        if (entId.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(TinSurface))))
                        {
                            TinSurface surface = tr.GetObject(entId, OpenMode.ForRead) as TinSurface;
                            if (surface != null)
                            {
                                // Create a new instance of ListViewItemModel for the surface name
                                ListViewItemModel surfaceItem = new ListViewItemModel
                                {
                                    Name = surface.Name
                                };

                                // Add the surface item to the SurfaceItems collection
                                SurfaceItems.Add(surfaceItem);
                                allSurfaceNames.Add(surface.Name);
                            }
                        }
                    }
                    tr.Commit();
                }
            }
        }

        private void AddAlignmentAndProfiles()
        {
            using (var db = HostApplicationServices.WorkingDatabase)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId entId in btr)
                    {
                        if (entId.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Alignment))))
                        {
                            Alignment alignment = tr.GetObject(entId, OpenMode.ForRead) as Alignment;
                            if (alignment != null)
                            {
                                if (alignment.FingerPrint!=0)
                                {
                                    var alignmentItem = new ListViewItemModel
                                    {
                                        Name = alignment.Name,
                                        Profiles = new ObservableCollection<ProfileModel>()
                                    };

                                    AlignmentItems.Add(alignmentItem);
                                    allAlignmentData[alignment.Name] = new SelectedAlignmentProfile
                                    {
                                        AlignmentName = alignment.Name,
                                        ProfileNames = new List<ProfileModel>(), // Initialize the list of profiles
                                        IsAlignmentSelected = false // Assuming alignment starts unselected
                                    };
                                    ObjectIdCollection profileIds = alignment.GetProfileIds();
                                    foreach (ObjectId profileId in profileIds)
                                    {
                                        Autodesk.Civil.DatabaseServices.Profile profile = tr.GetObject(profileId, OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.Profile;
                                        if (profile != null)
                                        {
                                            alignmentItem.Profiles.Add(new ProfileModel { Name = profile.Name });
                                            allAlignmentData[alignment.Name].ProfileNames.Add(new ProfileModel { Name = profile.Name });
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                    tr.Commit();
                }
            }
        }



        private void SurfaceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SelectedPinnad = new List<string>();
            AddOrUpdateSurfaceRegistryEntryFromTreeView(SurfaceTreeView, "SelectedSurfaces",  SelectedPinnad);
        }

        private void SurfaceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectedPinnad = new List<string>();
            AddOrUpdateSurfaceRegistryEntryFromTreeView(SurfaceTreeView, "SelectedSurfaces", SelectedPinnad);
        }


        private void AlignmentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            ListViewItemModel selectedAlignment = checkBox.DataContext as ListViewItemModel;

            if (selectedAlignment != null)
            {
                selectedAlignment.IsSelected = true;
            }
            AddOrUpdateRegistryEntriesFromAlignmentTreeView(AlignmentTreeView, "SelectedAlignments", "SelectedProfiles");
        }

        private void AlignmentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            ListViewItemModel selectedAlignment = checkBox.DataContext as ListViewItemModel;

            if (selectedAlignment != null)
            {
                selectedAlignment.IsSelected = false;
                // Disable all profile checkboxes under this alignment
                foreach (var profile in selectedAlignment.Profiles)
                {
                    profile.IsSubItemSelected = false; // Deselect all profiles
                }
            }
            AddOrUpdateRegistryEntriesFromAlignmentTreeView(AlignmentTreeView, "SelectedAlignments", "SelectedProfiles");
        }

        private void SubItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            ProfileModel selectedProfile = checkBox.DataContext as ProfileModel;
            ListViewItemModel parentAlignment = AlignmentItems.FirstOrDefault(alignment => alignment.Profiles.Contains(selectedProfile));

            parentAlignment.IsSelected = true;

            foreach (var profile in parentAlignment.Profiles)
            {
                profile.IsSubItemSelected = profile == selectedProfile;
            }
            AddOrUpdateRegistryEntriesFromAlignmentTreeView(AlignmentTreeView, "SelectedAlignments", "SelectedProfiles");
        }

        private void SubItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            ProfileModel selectedProfile = checkBox.DataContext as ProfileModel;
            ListViewItemModel parentAlignment = AlignmentItems.FirstOrDefault(alignment => alignment.Profiles.Contains(selectedProfile));
            if (parentAlignment?.IsSelected == true)
            {
                selectedProfile.IsSubItemSelected = false;
            }
            AddOrUpdateRegistryEntriesFromAlignmentTreeView(AlignmentTreeView, "SelectedAlignments", "SelectedProfiles");
        }

        void FillComboBoxFromDictionary(Transaction transaction, DBDictionary customDictionary, string nameKey, string uuidKey, Dictionary<string, string> NameUuidPairs, ComboBox comboBox)
        {
            if (customDictionary.Contains(nameKey) && customDictionary.Contains(uuidKey))
            {
                Xrecord nameXrec = transaction.GetObject(customDictionary.GetAt(nameKey), OpenMode.ForRead) as Xrecord;
                Xrecord uuidXrec = transaction.GetObject(customDictionary.GetAt(uuidKey), OpenMode.ForRead) as Xrecord;

                TypedValue[] nameData = nameXrec.Data.AsArray();
                TypedValue[] uuidData = uuidXrec.Data.AsArray();

                if (nameData != null && nameData.Length > 0 && nameData[0].TypeCode == (int)DxfCode.Text &&
                    uuidData != null && uuidData.Length > 0 && uuidData[0].TypeCode == (int)DxfCode.Text)
                {
                    string itemName = nameData[0].Value.ToString();
                    string itemUuid = uuidData[0].Value.ToString();
                    NameUuidPairs.Add(itemName, itemUuid);
                    comboBox.Items.Add(itemName);
                    comboBox.SelectedIndex = 0;
                }
            }
        }


        void FillTextBoxValueFromDictionary(Transaction transaction, DBDictionary customDictionary, string key, TextBox textBox)
        {
            if (customDictionary.Contains(key))
            {
                Xrecord xrec = transaction.GetObject(customDictionary.GetAt(key), OpenMode.ForRead) as Xrecord;                // Get the Xrecord associated with the key                
                TypedValue[] data = xrec.Data.AsArray(); // Get the value from the Xrecord
                if (data != null && data.Length > 0 && data[0].TypeCode == (int)DxfCode.Text)
                {
                    textBox.Text = data[0].Value.ToString();// Set the TextBox text to the value from the dictionary
                    if (!Directory.Exists(textBox.Text)) // Directory doesn't exist, set text color to red
                    {
                        textBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }
            }
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnValiPinnadKaust(object sender, RoutedEventArgs e)
        {
            string selectedFolder = ShowFolderBrowserDialog("Select folder");
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                PinnaKaust.Text = selectedFolder;
                PinnaKaust.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void BtnValiTeljeKaust(object sender, RoutedEventArgs e)
        {
            string selectedFolder = ShowFolderBrowserDialog("Select folder");
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                TeljeKaust.Text = selectedFolder;
                TeljeKaust.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private string ShowFolderBrowserDialog(string description)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = description
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                MessageBox.Show("Folder was not selected!");
                return null;
            }
        }

        private void BtnLaePinnadUlesseUus(object sender, RoutedEventArgs e)
        {
            UploadSurfaces();
        }


        private void ProcessSurfaces(string exportFilePath, Database db)
        {
            List<string> surfacesToKeep = new List<string>();
            foreach (ListViewItemModel surfaceItem in SurfaceItems)
            {
                if (surfaceItem.IsSelected)
                {
                    surfacesToKeep.Add(surfaceItem.Name);
                    //surfacesToKeep.Add(surfaceItem.Name.Replace(' ', '_'));
                }
            }
            SplitLandXMLSurfaces(exportFilePath, surfacesToKeep);
            foreach (ListViewItemModel surfaceItem in SurfaceItems)
            {
                if (surfaceItem.IsSelected)
                {
                    string filePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\" + surfaceItem.Name + ".xml";
                    if (PinnaKaust.Text != "")
                    {
                        filePath = PinnaKaust.Text + "\\" + surfaceItem.Name + ".xml";
                    }

                    string accessToken = GetAccessToken();
                    UploadFile(filePath, surfaceItem.Name, SurfaceFolderNameUUidPairs[PindadeKaustad.SelectedValue.ToString()]);
                }
            }
        }

        private void BtnLaeTeljedUlesseUus(object sender, RoutedEventArgs e)

        {
            UploadAlignments();
        }


        private void ProcessAlignments(string exportFilePath, Database db)
        {
            List<string> alignmentsToKeep = new List<string>();
            List<string> profilesToKeep = new List<string>();

            foreach (ListViewItemModel alignmentItem in AlignmentItems)
            {
                if (alignmentItem.IsSelected)
                {
                    ProfileModel selectedProfile = alignmentItem.Profiles.FirstOrDefault(p => p.IsSubItemSelected);    // Assuming there's only one profile selected for each alignment                           
                    string profileName = selectedProfile == null ? "" : selectedProfile.Name;
                    alignmentsToKeep.Add(alignmentItem.Name);
                    profilesToKeep.Add(selectedProfile == null ? "" : selectedProfile.Name);
                }
            }

            SplitLandXMLAlignments(exportFilePath, alignmentsToKeep, profilesToKeep);

            foreach (ListViewItemModel alignmentItem in AlignmentItems)
            {
                if (alignmentItem.IsSelected)
                {
                    ProfileModel selectedProfile = alignmentItem.Profiles.FirstOrDefault(p => p.IsSubItemSelected);    // Assuming there's only one profile selected for each alignment                           
                    string profileName = selectedProfile == null ? "" : selectedProfile.Name;
                    string filePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\" + alignmentItem.Name + ".xml";
                    if (TeljeKaust.Text != "")
                    {
                        filePath = TeljeKaust.Text + "\\" + alignmentItem.Name + ".xml";
                    }

                    string accessToken = GetAccessToken();
                    UploadFile(filePath, alignmentItem.Name, AlignmentFolderNameUUidPairs[TelgedeKaustad.SelectedValue.ToString()]);
                }
            }
        }

        private void ShowErrorForEmptyFields(string uploadType)
        {
            if (string.IsNullOrEmpty(Projektid.Text))
            {
                MessageBox.Show("Project is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (string.IsNullOrEmpty(PindadeKaustad.Text) && (uploadType == "SurfaceUpload" || uploadType == "Upload"))
            {
                MessageBox.Show("Surface folder is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (string.IsNullOrEmpty(TelgedeKaustad.Text) && (uploadType == "AlignmentUpload" || uploadType == "Upload"))
            {
                MessageBox.Show("Alignment folder is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteLandXmlOutCommand(Editor ed, string exportFilePath)
        {
            if (Year == "2025")
            {
                ed.Command("-LANDXMLOUT", "1.2", exportFilePath);
            }
            else
            {
                ed.Command("-LANDXMLOUT", exportFilePath);
            }
        }

        //private void BtnLaePinnadUlesse(object sender, RoutedEventArgs e)
        //{
        //    Document doc = AcAp.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    CivilDocument civDoc = CivilApplication.ActiveDocument;
        //    Stopwatch stopwatch = Stopwatch.StartNew();  // Start the stopwatch
        //    TimeSpan elapsedTime;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Projektid.Text) || string.IsNullOrEmpty(PindadeKaustad.Text))
        //        {
        //            if (string.IsNullOrEmpty(Projektid.Text))  // Show error message for empty Kasutajanimi.Text
        //            {
        //                MessageBox.Show("Project is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //            if (string.IsNullOrEmpty(PindadeKaustad.Text)) // Show error message for empty Parool.Text
        //            {
        //                MessageBox.Show("Surface folder is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //        else
        //        {
        //            foreach (ListViewItemModel surfaceItem in SurfaceItems)
        //            {
        //                if (surfaceItem.IsSelected)
        //                {
        //                    accessToken = GetAccessToken();

        //                    // Find the surface with the given name
        //                    ObjectId surfaceId = ObjectId.Null;
        //                    using (Transaction tr = db.TransactionManager.StartTransaction())
        //                    {
        //                        TinSurface surface = tr.GetObject(GetSurfaceIdByName(civDoc, surfaceItem.Name), OpenMode.ForRead) as TinSurface;
        //                        if (surface != null)
        //                        {
        //                            surfaceId = surface.Id;
        //                        }
        //                        tr.Commit();
        //                    }

        //                    using (Transaction tr = db.TransactionManager.StartTransaction())
        //                    {
        //                        TinSurface surface = tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

        //                        if (surface == null)
        //                        {
        //                            AcAp.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Failed to open surface!");
        //                            return;
        //                        }
        //                        string filePath = CreateAndSaveSurfaceLandXMLToFile(surface, surfaceItem.Name);
        //                        if (filePath != null)
        //                        {
        //                            UploadFile(filePath, surfaceItem.Name, SurfaceFolderNameUUidPairs[PindadeKaustad.SelectedValue.ToString()]);
        //                        }
        //                        tr.Commit();
        //                    }
        //                }
        //            }
        //            elapsedTime = stopwatch.Elapsed;  // Get the elapsed time as a TimeSpan                 
        //            MessageBox.Show($"Upload complete. It took {elapsedTime.ToString(@"mm\:ss")} to complete.");
        //        }
        //    }

        //    catch (WebException webEx)
        //    {
        //        HandleWebException(webEx);
        //    }
        //    catch (Autodesk.AutoCAD.Runtime.Exception ex)
        //    {
        //        MessageBox.Show("Exception: " + ex.Message);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        MessageBox.Show("Exception: " + ex.Message);
        //    }
        //}
        private ObjectId GetSurfaceIdByName(CivilDocument civDoc, string surfaceName)
        {
            foreach (ObjectId surfaceId in civDoc.GetSurfaceIds())
            {
                TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;
                if (surface != null && surface.Name == surfaceName)
                {
                    return surfaceId;
                }
            }
            return ObjectId.Null;
        }

        private string CreateAndSaveSurfaceLandXMLToFile(TinSurface surface, string filename)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            StringBuilder landXMLContent = new StringBuilder();
            landXMLContent.AppendLine(@"<?xml version=""1.0""?>");
            landXMLContent.AppendLine(@"<LandXML xmlns=""http://www.landxml.org/schema/LandXML-1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.landxml.org/schema/LandXML-1.2 http://www.landxml.org/schema/LandXML-1.2/LandXML-1.2.xsd"" date=""" + DateTime.Now.ToString("yyyy-MM-dd") + "\" time=\"" + DateTime.Now.ToString("HH:mm:ss") + "\" version=\"1.2\" language=\"English\" readOnly=\"false\">");
            landXMLContent.AppendLine(@"  <Units>");
            landXMLContent.AppendLine(@"    <Metric areaUnit=""squareMeter"" linearUnit=""meter"" volumeUnit=""cubicMeter"" temperatureUnit=""celsius"" pressureUnit=""milliBars"" diameterUnit=""millimeter"" angularUnit=""decimal degrees"" directionUnit=""decimal degrees""></Metric>");
            landXMLContent.AppendLine(@"  </Units>");
            landXMLContent.AppendLine(@"  <Project name=""" + doc.Name + @"""></Project>");
            landXMLContent.AppendLine(@"  <Application name=""Autodesk Civil 3D Infrakit"" desc=""Civil 3D Infrakit"" manufacturer=""Autodesk, Inc. Infrakit"" version=""" + Year + @""" manufacturerURL=""www.autodesk.com/civil https://infrakit.com/"" timeStamp=""" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"""></Application>");
            landXMLContent.AppendLine(@"  <Surfaces>");
            landXMLContent.AppendLine(@"    <Surface name=""" + surface.Name + @""" desc=""" + surface.Description + @""">");
            landXMLContent.AppendLine(@"      <SourceData></SourceData>");
            landXMLContent.AppendLine(@"      <Definition surfType=""TIN"" elevMax=""" + surface.GeometricExtents.MaxPoint.Z + @""" elevMin=""" + surface.GeometricExtents.MinPoint.Z + @""">");
            //area2DSurf = ""XXX""
            //area3DSurf = ""XXX""
            landXMLContent.AppendLine(@"        <Pnts>");

            foreach (TinSurfaceVertex vertex in surface.Vertices)
            {
                landXMLContent.AppendLine($@"          <P id=""{vertex.GetHashCode() + 1}"">{vertex.Location.Y} {vertex.Location.X} {vertex.Location.Z}</P>");
            }

            landXMLContent.AppendLine(@"        </Pnts>");
            landXMLContent.AppendLine(@"        <Faces>");

            foreach (TinSurfaceTriangle triangle in surface.Triangles)
            {
                landXMLContent.AppendLine($@"          <F >{triangle.Vertex1.GetHashCode() + 1} {triangle.Vertex2.GetHashCode() + 1} {triangle.Vertex3.GetHashCode() + 1}</F>");
            }

            landXMLContent.AppendLine(@"        </Faces>");
            landXMLContent.AppendLine(@"      </Definition>");
            landXMLContent.AppendLine(@"    </Surface>");
            landXMLContent.AppendLine(@"  </Surfaces>");
            landXMLContent.AppendLine(@"</LandXML>");
            string filePath = System.IO.Path.GetDirectoryName(db.Filename) + "\\" + filename + ".xml";
            if (PinnaKaust.Text != "")
            {
                filePath = PinnaKaust.Text + "\\" + filename + ".xml";
            }
            System.IO.File.WriteAllText(filePath, landXMLContent.ToString());
            return filePath;
        }

        //private void BtnLaeUlesse(object sender, RoutedEventArgs e)
        //{
        //    UploadMix();
        //}

        private void BtnLaeUlesseUus(object sender, RoutedEventArgs e)
        {
            Upload();
        }



        private string GetAccessToken()
        {

            using (WebClient client = new WebClient())
            {
                String Salasona;
                client.Encoding = Encoding.UTF8;
                string url = "https://iam.infrakit.com/auth/token";
                Salasona = DecryptedPassword ? ParoolPasswordBox.Password : Parool.Text; // Assign the value based on the condition
                //string postData = "grant_type=password&username=" + Kasutajanimi.Text + "&password=" + ParoolPasswordBox.Password;
                string postData = "grant_type=password&username=" + Kasutajanimi.Text + "&password=" + Salasona;
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                responseText = client.UploadString(url, postData);
                JObject json = JObject.Parse(responseText);
                accessToken = (string)json["accessToken"];
            }
            return accessToken;
        }
        private void BtnLogiSisse(object sender, RoutedEventArgs e)
        {
            try
            {
                accessToken = GetAccessToken();
                GetProjects();
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }


        private void Parool_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ParoolPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DecryptedPassword == true)
            {
                ParoolPasswordBox.Password = "";
                DecryptedPassword = false;
            }
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DecryptedPassword == true)
            {
                MessageBox.Show($"Can not show decrypted password");
                ShowPasswordCheckBox.Unchecked -= ShowPasswordCheckBox_Unchecked;
                ShowPasswordCheckBox.IsChecked = false;
                ShowPasswordCheckBox.Unchecked += ShowPasswordCheckBox_Unchecked;
            }
            else
            {
                Parool.Text = ParoolPasswordBox.Password;
                Parool.Visibility = System.Windows.Visibility.Visible;
                ParoolPasswordBox.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ParoolPasswordBox.Password = Parool.Text;
            Parool.Visibility = System.Windows.Visibility.Collapsed;
            ParoolPasswordBox.Visibility = System.Windows.Visibility.Visible;
        }


        private void GetProjects()
        {
            try
            {          
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string url = "https://app.infrakit.com/kuura/v1/projects";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                responseText = client.DownloadString(url);
                JArray jsonArray = JArray.Parse(responseText);
                Projektid.SelectionChanged -= Projektid_SelectionChanged;
                Projektid.Items.Clear();
                ProjectNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here          
                foreach (var item in jsonArray)
                {
                    //Projektid.Items.Add((string)item["name"] + "|" + (string)item["uuid"]);
                    string name = (string)item["name"];
                    string uuid = (string)item["uuid"];
                    ProjectNameUUidPairs.Add(name, uuid);
                    Projektid.Items.Add(name);
                }
                Projektid.SelectedIndex = 0;
                Projektid.SelectionChanged += Projektid_SelectionChanged;
                AddOrUpdateDictionaryEntriesFromTextBox("Projekt", Projektid.Text);
                AddOrUpdateDictionaryEntriesFromTextBox("ProjektUUid", ProjectNameUUidPairs[Projektid.Text]);
                ProjectUUid = ProjectNameUUidPairs[Projektid.Text];
                GetFolders(ProjectUUid);
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
                
        private void GetFolders(string ProjectUUid)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string url = $"https://app.infrakit.com/kuura/v1/project/{ProjectUUid}/folders?depth=-1";
                client.Headers[HttpRequestHeader.Authorization] = $"Bearer {accessToken}";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string responseText = client.DownloadString(url);
                JObject data = JObject.Parse(responseText);
                JArray folders = (JArray)data["folders"];
                // Temporarily detach the event handler
                PindadeKaustad.SelectionChanged -= PindadeKaustad_SelectionChanged;
                PindadeKaustad.Items.Clear();
                TelgedeKaustad.SelectionChanged -= TelgedeKaustad_SelectionChanged;
                TelgedeKaustad.Items.Clear();
                SurfaceFolderNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here
                AlignmentFolderNameUUidPairs = new Dictionary<string, string>(); // Initialize the dictionary here

                // Dictionary to store folder information by UUID
                var folderDict = folders.ToDictionary(
                    f => (string)f["uuid"],
                    f => new
                    {
                        Uuid = (string)f["uuid"],
                        Name = (string)f["name"],
                        ParentFolderUuid = (string)f["parentFolderUuid"],
                        Depth = (int)f["depth"]
                    });

                // Function to get folder path recursively
                Func<string, string> GetFolderPath = null;
                GetFolderPath = (uuid) =>
                {
                    var folder = folderDict[uuid];
                    if (folder.ParentFolderUuid != null && folder.ParentFolderUuid != "root")
                    {
                        string parentPath = GetFolderPath(folder.ParentFolderUuid);
                        return (parentPath != "root" ? parentPath + "\\" : "") + folder.Name;
                    }
                    return folder.Name;
                };

                Action<string, int> buildFolderStructure = null;
                buildFolderStructure = (uuid, level) =>
                {
                    var folder = folderDict[uuid];

                    if (folder.Name != "root")
                    {
                        string folderName = (level > 0) ? GetFolderPath(uuid) : folder.Name;

                        SurfaceFolderNameUUidPairs.Add(folderName, uuid);
                        PindadeKaustad.Items.Add(folderName);
                        AlignmentFolderNameUUidPairs.Add(folderName, uuid);
                        TelgedeKaustad.Items.Add(folderName);
                    }

                    foreach (var subfolder in folders)
                    {
                        if ((string)subfolder["parentFolderUuid"] == uuid)
                        {
                            buildFolderStructure((string)subfolder["uuid"], level + 1);
                        }
                    }
                };

                foreach (var folder in folderDict.Values.Where(f => f.Depth == 0))
                {
                    buildFolderStructure(folder.Uuid, 0);
                }

                PindadeKaustad.SelectedIndex = 0;
                PindadeKaustad.SelectionChanged += PindadeKaustad_SelectionChanged;
                AddOrUpdateDictionaryEntriesFromTextBox("PinnadKaust", PindadeKaustad.Text);
                AddOrUpdateDictionaryEntriesFromTextBox("PinnadKaustUUid", SurfaceFolderNameUUidPairs[PindadeKaustad.Text]);

                TelgedeKaustad.SelectedIndex = 0;
                TelgedeKaustad.SelectionChanged += TelgedeKaustad_SelectionChanged;
                AddOrUpdateDictionaryEntriesFromTextBox("TeljedKaust", TelgedeKaustad.Text);
                AddOrUpdateDictionaryEntriesFromTextBox("TeljedKaustUUid", AlignmentFolderNameUUidPairs[TelgedeKaustad.Text]);
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void Projektid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            try
            {

                AddOrUpdateDictionaryEntriesFromTextBox("Projekt", Projektid.SelectedValue.ToString());
                AddOrUpdateDictionaryEntriesFromTextBox("ProjektUUid", ProjectNameUUidPairs[Projektid.SelectedValue.ToString()]);
                ProjectUUid = ProjectNameUUidPairs[Projektid.SelectedValue.ToString()];
                accessToken = GetAccessToken();
                GetFolders(ProjectUUid);
            }
            catch (WebException webEx)
            {
                HandleWebException(webEx);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("Projektid_SelectionChanged AutoCAD exception: " + ex.Message);
                doc.Editor.WriteMessage("\nProjektid_SelectionChanged AutoCAD exception: " + ex.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Projektid_SelectionChanged System exception: " + ex.Message);
                doc.Editor.WriteMessage("\nProjektid_SelectionChanged System exception: " + ex.ToString());

            }

        }

        private void PindadeKaustad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddOrUpdateDictionaryEntriesFromTextBox("PinnadKaust", PindadeKaustad.SelectedValue.ToString());
            AddOrUpdateDictionaryEntriesFromTextBox("PinnadKaustUUid", SurfaceFolderNameUUidPairs[PindadeKaustad.SelectedValue.ToString()]);
        }
        private void TelgedeKaustad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddOrUpdateDictionaryEntriesFromTextBox("TeljedKaust", TelgedeKaustad.SelectedValue.ToString());
            AddOrUpdateDictionaryEntriesFromTextBox("TeljedKaustUUid", AlignmentFolderNameUUidPairs[TelgedeKaustad.SelectedValue.ToString()]);
        }

        private void UploadFile(string filePath, string fileName, string folderUuid)
        {
            string contentType = "text/xml";
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                byte[] fileContents = File.ReadAllBytes(filePath);
                string payload = $"{{\"filename\":\"{fileName}.xml\",\"folderUuid\":\"{folderUuid}\",\"contentType\":\"{contentType}\",\"size\":\"{fileContents.Length}\"}}";
                string url = "https://app.infrakit.com/kuura/v1/model/async-upload";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string responseText = client.UploadString(url, payload);
                JObject json = JObject.Parse(responseText);
                string uploadUrl = (string)json["uploadUrl"];
                using (WebClient uploadClient = new WebClient())
                {
                    uploadClient.Headers[HttpRequestHeader.ContentType] = contentType;
                    uploadClient.UploadData(uploadUrl, "PUT", fileContents);
                }
            }
        }

        private void HandleWebException(WebException webEx)
        {
            if (webEx.Response is HttpWebResponse httpResponse)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Unauthorized: Please check your username and password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"HTTP error: {httpResponse.StatusCode} - {httpResponse.StatusDescription}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to connect to server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Kasutajanimi_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnSalvestaSisselogimineAndmed(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Kasutajanimi.Text) || string.IsNullOrEmpty(ParoolPasswordBox.Password))
            {
                if (string.IsNullOrEmpty(Kasutajanimi.Text))  // Show error message for empty Kasutajanimi.Text
                {
                    MessageBox.Show("Username is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (string.IsNullOrEmpty(ParoolPasswordBox.Password)) // Show error message for empty Parool.Text
                {
                    MessageBox.Show("Password is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Infrakit", true); //Open the Application Registry for write                
                if (regKey == null) // Check if the key exists, if not create it
                {
                    regKey = Registry.CurrentUser.CreateSubKey("Infrakit");
                }
                regKey.SetValue("kasutajanimi", Kasutajanimi.Text); // Write the data to the registry               
                string encryptedData = StringCipher.Encrypt(ParoolPasswordBox.Password, "TEST");  // Encrypt the data
                regKey.SetValue("parool", encryptedData);
                AcAp.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Login information is saved.\r\n");
            }
        }

        private void AddOrUpdateDictionaryEntriesFromTextBox(string variableName, string variableValue)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;

            try
            {

                using (Transaction transaction = doc.TransactionManager.StartTransaction())
                {
                    Database database = doc.Database;
                    DBDictionary nod = transaction.GetObject(database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                    if (!nod.Contains("Infrakit"))
                    {
                        DBDictionary customDict = new DBDictionary();
                        nod.SetAt("Infrakit", customDict);
                        transaction.AddNewlyCreatedDBObject(customDict, true);
                    }
                    DBDictionary customDictionary = transaction.GetObject(nod.GetAt("Infrakit"), OpenMode.ForWrite) as DBDictionary;
                    if (customDictionary.Contains(variableName))
                    {
                        Xrecord xrec = transaction.GetObject(customDictionary.GetAt(variableName), OpenMode.ForWrite) as Xrecord;
                        xrec.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, variableValue));

                    }
                    else
                    {
                        Xrecord xrec = new Xrecord();
                        xrec.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, variableValue));
                        customDictionary.SetAt(variableName, xrec);
                        transaction.AddNewlyCreatedDBObject(xrec, true);
                    }
                    transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                MessageBox.Show("AddOrUpdateDictionaryEntriesFromTextBox AutoCAD exception: " + ex.Message);
                doc.Editor.WriteMessage("\nAddOrUpdateDictionaryEntriesFromTextBox AutoCAD exception: " + ex.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("AddOrUpdateDictionaryEntriesFromTextBox System exception: " + ex.Message);
                doc.Editor.WriteMessage("\nAddOrUpdateDictionaryEntriesFromTextBox AutoCAD exception: " + ex.ToString());
            }



        }

        private void AddOrUpdateSurfaceRegistryEntryFromTreeView(TreeView treeView, string surfaceKeyName, List<string> selectedList)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            using (Transaction transaction = doc.TransactionManager.StartTransaction())
            {
                Database database = doc.Database;
                DBDictionary nod = transaction.GetObject(database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;

                if (!nod.Contains("Infrakit"))
                {
                    DBDictionary customDict = new DBDictionary();
                    nod.SetAt("Infrakit", customDict);
                    transaction.AddNewlyCreatedDBObject(customDict, true);
                }

                DBDictionary customDictionary = transaction.GetObject(nod.GetAt("Infrakit"), OpenMode.ForWrite) as DBDictionary;
                ResultBuffer rb = new ResultBuffer();

                // Iterate over the items in the tree view
                foreach (ListViewItemModel surface in treeView.Items)
                {
                    if (surface.IsSelected)
                    {
                        // Add the surface name to the result buffer
                        rb.Add(new TypedValue((int)DxfCode.Text, surface.Name));
                        selectedList.Add(surface.Name);
                    }
                }

                // Create or update the registry entry with the result buffer
                Xrecord xrec = new Xrecord
                {
                    Data = rb
                };
                customDictionary.SetAt(surfaceKeyName, xrec);
                transaction.AddNewlyCreatedDBObject(xrec, true);
                transaction.Commit();
            }
        }

        private void FillSurfaceTreeViewValueFromDictionary(DBDictionary customDictionary, Transaction transaction, string surfaceXrecordKey, TreeView treeView, List<string> selectedList)
        {
            if (customDictionary.Contains(surfaceXrecordKey))
            {
                Xrecord surfaceXrec = transaction.GetObject(customDictionary.GetAt(surfaceXrecordKey), OpenMode.ForRead) as Xrecord; // Get the surface Xrecord with the specified key                
                ResultBuffer surfaceRb = surfaceXrec.Data; // Get the ResultBuffer from the surface Xrecord
                if (surfaceRb == null)
                {
                    return; // Exit the method early
                }
                TypedValue[] surfaceValues = surfaceRb.AsArray();  // Extract the surface values from the ResultBuffer                
                foreach (TypedValue surfaceValue in surfaceValues) // Loop through the surface values
                {
                    string surfaceName = surfaceValue.Value.ToString();
                    ListViewItemModel surfaceItem = treeView.Items.Cast<ListViewItemModel>().FirstOrDefault(item => item.Name == surfaceName);  // Find the surface in the tree view
                    if (surfaceItem != null)
                    {
                        surfaceItem.IsSelected = true; // Select the surface
                        selectedList.Add(surfaceValue.Value.ToString());
                    }
                }
            }
        }

        private void AddOrUpdateRegistryEntriesFromAlignmentTreeView(TreeView treeView, string AlignmentKeyName, string ProfileKeyName)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            using (Transaction transaction = doc.TransactionManager.StartTransaction())
            {
                Database database = doc.Database;
                DBDictionary nod = transaction.GetObject(database.NamedObjectsDictionaryId, OpenMode.ForWrite) as DBDictionary;
                if (!nod.Contains("Infrakit"))
                {
                    DBDictionary customDict = new DBDictionary();
                    nod.SetAt("Infrakit", customDict);
                    transaction.AddNewlyCreatedDBObject(customDict, true);
                }
                DBDictionary customDictionary = transaction.GetObject(nod.GetAt("Infrakit"), OpenMode.ForWrite) as DBDictionary;
                ResultBuffer alignmentRb = new ResultBuffer();   // Create result buffers for alignment and profile entries
                ResultBuffer profileRb = new ResultBuffer();

                foreach (ListViewItemModel alignment in treeView.Items) // Iterate over the items in the tree view
                {
                    if (alignment.IsSelected)
                    {
                        alignmentRb.Add(new TypedValue((int)DxfCode.Text, alignment.Name)); // Add the alignment name to the alignment result buffer                        
                        allAlignmentData[alignment.Name].IsAlignmentSelected = true;
                    }
                    else
                    {
                        allAlignmentData[alignment.Name].IsAlignmentSelected = false;
                    }
                    allAlignmentData[alignment.Name].IsProfileSelected = false;
                    foreach (ProfileModel profile in alignment.Profiles) // Loop through all profiles in the alignment
                    {
                        var profileData = allAlignmentData[alignment.Name].ProfileNames.FirstOrDefault(p => p.Name == profile.Name);

                        if (profile.IsSubItemSelected)
                        {                                
                            profile.IsSubItemSelected = true; // Select the profile
                            allAlignmentData[alignment.Name].IsProfileSelected = true;
                            profileData.IsSubItemSelected = true;
                            profileRb.Add(new TypedValue((int)DxfCode.Text, profile.Name)); // Add the alignment-profile pair to the profile result buffer   
                        }
                        else
                        {
                            profile.IsSubItemSelected = false; // Update the IsSubItemSelected property                               
                            profileData.IsSubItemSelected = false;
                        }
                    }
                    if  (allAlignmentData[alignment.Name].IsProfileSelected == false)
                    {
                        profileRb.Add(new TypedValue((int)DxfCode.Text, "Empty Profile")); // Add an empty profile entry for the alignment                              
                    }                   
                    
                }

                Xrecord alignmentXrec = new Xrecord // Create or update the registry entry for alignments
                {
                    Data = alignmentRb
                };
                customDictionary.SetAt(AlignmentKeyName, alignmentXrec);
                transaction.AddNewlyCreatedDBObject(alignmentXrec, true);
                Xrecord profileXrec = new Xrecord // Create or update the registry entry for profiles
                {
                    Data = profileRb
                };
                customDictionary.SetAt(ProfileKeyName, profileXrec);
                transaction.AddNewlyCreatedDBObject(profileXrec, true);

                transaction.Commit();
            }
        }




        private void FillAlignmentTreeViewValueFromDictionary(DBDictionary customDictionary, Transaction transaction, string alignmentXrecordKey, string profileXrecordKey, TreeView treeView)
        {
            if (customDictionary.Contains(alignmentXrecordKey) && customDictionary.Contains(profileXrecordKey))
            {
                Xrecord alignmentXrec = transaction.GetObject(customDictionary.GetAt(alignmentXrecordKey), OpenMode.ForRead) as Xrecord; // Get the alignment Xrecord with the specified key                
                ResultBuffer alignmentRb = alignmentXrec.Data; // Get the ResultBuffer from the alignment Xrecord
                if (alignmentRb == null)
                {
                    return; // Exit the method early
                }
                TypedValue[] alignmentValues = alignmentRb.AsArray();  // Extract the alignment values from the ResultBuffer                
                Xrecord profileXrec = transaction.GetObject(customDictionary.GetAt(profileXrecordKey), OpenMode.ForRead) as Xrecord; // Get the profile Xrecord with the specified key                
                ResultBuffer profileRb = profileXrec.Data; // Get the ResultBuffer from the profile Xrecord                
                TypedValue[] profileValues = profileRb.AsArray(); // Extract the profile values from the ResultBuffer
                string[] profileNames = profileValues.Select(tv => tv.Value.ToString()).ToArray();
                int minCount = Math.Min(alignmentValues.Length, profileValues.Length); // Ensure alignment and profile arrays have same length                
                for (int i = 0; i < minCount; i++) // Loop through alignment and profile values simultaneously
                {
                    string alignmentName = alignmentValues[i].Value.ToString();
                    string profileName = profileNames[i];
                    ListViewItemModel alignmentItem = treeView.Items.Cast<ListViewItemModel>().FirstOrDefault(item => item.Name == alignmentName);     // Find the alignment in the tree view
                    if (alignmentItem != null)
                    {
                        alignmentItem.IsSelected = true; // Select the alignment  
                        allAlignmentData[alignmentName].IsAlignmentSelected = true;

                        ProfileModel profile = alignmentItem.Profiles.FirstOrDefault(p => p.Name == profileName); // Find the profile in the alignment's profiles
                        if (profile != null)
                        {
                            profile.IsSubItemSelected = true; // Select the profile
                            allAlignmentData[alignmentName].IsProfileSelected = true;    
                            // Find and update the profile in allAlignmentData
                            var profileData = allAlignmentData[alignmentName].ProfileNames.FirstOrDefault(p => p.Name == profileName);
                            if (profileData != null)
                            {
                                profileData.IsSubItemSelected = true; // Update the IsSubItemSelected property
                            }

                        }

                    }
                }
            }
        }
        private void PinnaKaust_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddOrUpdateDictionaryEntriesFromTextBox("Pinnakaust", PinnaKaust.Text);
        }

        private void TeljeKaust_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddOrUpdateDictionaryEntriesFromTextBox("Teljekaust", TeljeKaust.Text);
        }

        private void ValiPinnad_Click(object sender, RoutedEventArgs e)
        {
            ValitudPinnadNähtavad.IsChecked = false;
            SelectAndAddToTreeView<Autodesk.Civil.DatabaseServices.TinSurface>("AECC_TIN_SURFACE", SurfaceTreeView);
        }

        private void ValiTeljed_Click(object sender, RoutedEventArgs e)
        {            
            ValitudTeljedNähtavad.IsChecked = false;
            SelectAndAddToTreeView<Autodesk.Civil.DatabaseServices.TinSurface>("AECC_ALIGNMENT", AlignmentTreeView);
        }

        private void SelectAndAddToTreeView<T>(string dxfName, TreeView treeView) where T : Autodesk.AutoCAD.DatabaseServices.Entity
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            var editor = doc.Editor;

            // Determine the correct message based on the DXF name
            string message = "";
            if (dxfName == "AECC_TIN_SURFACE")
            {
                message = "\nSelect tin surface: ";
            }
            else if (dxfName == "AECC_ALIGNMENT")
            {
                message = "\nSelect alignment: ";
            }
            else
            {
                message = $"\nSelect {typeof(T).Name.ToLower()}s: ";
            }

            // Prompt the user to select objects
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = message,
                AllowDuplicates = false
            };

            // Filter to only allow selection of the specified object type
            var filter = new SelectionFilter(new[]
            {
                new TypedValue((int)DxfCode.Start, dxfName)
            });

            var psr = editor.GetSelection(pso, filter);

            if (psr.Status == PromptStatus.OK)
            {
                using (var transaction = doc.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject selectedObj in psr.Value)
                    {
                        if (selectedObj != null)
                        {
                            Autodesk.Civil.DatabaseServices.Alignment alignmentEntity = null;
                            Autodesk.Civil.DatabaseServices.TinSurface tinSurfaceEntity = null;
                            //var entity = transaction.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as T;
                            if (dxfName == "AECC_TIN_SURFACE")
                            {
                                tinSurfaceEntity = transaction.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.TinSurface;
                            }
                            if (dxfName == "AECC_ALIGNMENT")
                            {
                                alignmentEntity = transaction.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.Alignment;
                            }

                            if (tinSurfaceEntity != null)
                            {
                                // Get the name of the entity
                                string entityName = tinSurfaceEntity.Name;

                                // Find the corresponding TreeViewItem in the TreeView
                                ListViewItemModel surfaceItem = treeView.Items.Cast<ListViewItemModel>().FirstOrDefault(item => item.Name == entityName);
                                if (surfaceItem != null)
                                {
                                    surfaceItem.IsSelected = true; // Select the surface
                                }
                            }
                            if (alignmentEntity != null)
                            {
                                // Get the name of the entity
                                string entityName = alignmentEntity.Name;

                                // Find the corresponding TreeViewItem in the TreeView
                                ListViewItemModel alignmentItem = treeView.Items.Cast<ListViewItemModel>().FirstOrDefault(item => item.Name == entityName);
                                if (alignmentItem != null)
                                {
                                    alignmentItem.IsSelected = true; // Select the alignment
                                }
                            }

                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private void ValitudPinnadNähtavad_Checked(object sender, RoutedEventArgs e)
        {
            HandleCheckedTreeView(SurfaceTreeView,false);
        }
        private void ValitudTeljedNähtavad_Checked(object sender, RoutedEventArgs e)
        {

            HandleCheckedTreeView(AlignmentTreeView,true);
        }

        private void HandleCheckedTreeView(TreeView treeView, bool handleProfiles)
        {
            if (treeView.ItemsSource is ObservableCollection<ListViewItemModel> itemsSource)
            {
                for (int i = itemsSource.Count - 1; i >= 0; i--)
                {
                    if (!itemsSource[i].IsSelected)
                    {
                        itemsSource.RemoveAt(i);
                    }
                    else if (handleProfiles)
                    {
                        HandleCheckedProfiles(itemsSource[i].Profiles);
                    }
                }
            }
        }

        private void HandleCheckedProfiles(ObservableCollection<ProfileModel> profiles)
        {
            if (profiles == null) return;

            for (int j = profiles.Count - 1; j >= 0; j--)
            {
                if (!profiles[j].IsSubItemSelected)
                {
                    profiles.RemoveAt(j);
                }
            }
        }

        private void ValitudPinnadNähtavad_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleUncheckedTreeView(SurfaceTreeView, allSurfaceNames);
        }

        private void ValitudTeljedNähtavad_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleUncheckedAlignmentTreeView(AlignmentTreeView, allAlignmentData);
        }


        private void HandleUncheckedTreeView(TreeView treeView, List<string> allItems)
        {
            if (treeView.ItemsSource is ObservableCollection<ListViewItemModel> itemsSource)
            {
                foreach (string name in allItems)
                {
                    // Check if the alignment already exists in itemsSource
                    bool Exists = itemsSource.Any(item => item.Name == name);

                    // Check if the item is not already in the collection
                    if (!Exists)
                    {
                        // Create a new ListViewItemModel with the name
                        var newItem = new ListViewItemModel { Name = name, IsSelected = false };

                        // Add the new item to the collection
                        itemsSource.Add(newItem);
                    }
                }
            }
        }
        private void HandleUncheckedAlignmentTreeView(TreeView treeView, Dictionary<string, SelectedAlignmentProfile> allAlignmentData)
        {
            if (treeView.ItemsSource is ObservableCollection<ListViewItemModel> itemsSource)
            {
                foreach (var alignmentName in allAlignmentData)
                {
                    // Check if the alignment already exists in itemsSource
                    bool Exists = itemsSource.Any(item => item.Name == alignmentName.Value.AlignmentName);

                    if (!Exists)
                    {
                        // Create a new alignment item if not found
                        var alignmentItem = new ListViewItemModel { Name = alignmentName.Value.AlignmentName, IsSelected = false, Profiles = new ObservableCollection<ProfileModel>() };

                        // Add profiles associated with the alignment
                        //foreach (string profileName in alignmentName.Value.ProfileNames)
                        foreach (var profileModel in alignmentName.Value.ProfileNames)
                        {
                            // Add profile only if it doesn't already exist
                            if (!alignmentItem.Profiles.Any(profile => profile.Name == profileModel.Name))
                            {
                                alignmentItem.Profiles.Add(new ProfileModel { Name = profileModel.Name});
                            }
                        }
                        // Add the new alignment item to the collection
                        itemsSource.Add(alignmentItem);
                    }
                    else
                    {
                        // Get the existing alignment item
                        var existingAlignmentItem = itemsSource.First(item => item.Name == alignmentName.Value.AlignmentName);

                        // Add missing profiles to the existing alignment item
                        //foreach (string profileName in alignmentName.Value.ProfileNames)
                        foreach (var profileModel in alignmentName.Value.ProfileNames)
                        {
                            if (!existingAlignmentItem.Profiles.Any(profile => profile.Name == profileModel.Name))
                            {
                                existingAlignmentItem.Profiles.Add(new ProfileModel { Name = profileModel.Name });
                            }
                        }
                    }
                }
            }
        }

        private void OtsiPinnad_TextChanged(object sender, EventArgs e)
        {
            ValitudPinnadNähtavad.IsChecked = false;
            FilterTreeView(SurfaceTreeView, OtsiPinnad, SelectedPinnad, allSurfaceNames);
        }

        private void OtsiTeljed_TextChanged(object sender, EventArgs e)
        {
            ValitudTeljedNähtavad.IsChecked = false;
            FilterAlignmentTreeView(AlignmentTreeView, OtsiTeljed, allAlignmentData);
        }

      
        private void FilterTreeView(TreeView treeView, TextBox textBox, List<string> selectedList, List<string> allItems)
        {
            if (allItems == null) return;

            string userInput = textBox.Text.ToLower(); // Convert user input to lower case for case-insensitive comparison

            // Assuming the original ItemsSource is an ObservableCollection<ListViewItemModel>
            if (!(treeView.ItemsSource is ObservableCollection<ListViewItemModel> itemsSource))
            {
                // If ItemsSource is not already set as ObservableCollection, handle this case appropriately
                return;
            }

            // Clear existing items (if any) from the ObservableCollection
            itemsSource.Clear();

            foreach (string item in allItems)
            {
                if (item.ToLower().Contains(userInput)) // If the item contains the user input substring
                {
                    var newItem = new ListViewItemModel { Name = item, IsSelected = false }; // Create a new ListViewItemModel
                    itemsSource.Add(newItem); // Add the new item to the ObservableCollection

                    if (selectedList != null)
                    {
                        // Check if the item is selected
                        foreach (string selectedItem in selectedList)
                        {
                            if (selectedItem.ToLower() == item.ToLower())
                            {
                                newItem.IsSelected = true; // Select the item
                                break; // Once item is found, no need to continue searching
                            }
                        }
                    }
                }
            }
        }
        private void FilterAlignmentTreeView(TreeView treeView, TextBox textBox, Dictionary<string, SelectedAlignmentProfile> allAlignmentData)
        {
            if (allAlignmentData == null) return;

            string userInput = textBox.Text.ToLower(); // Convert user input to lower case for case-insensitive comparison

            // Assuming the original ItemsSource is an ObservableCollection<ListViewItemModel>
            if (!(treeView.ItemsSource is ObservableCollection<ListViewItemModel> itemsSource))
            {
                // If ItemsSource is not already set as ObservableCollection, handle this case appropriately
                return;
            }

            // Clear existing items (if any) from the ObservableCollection
            itemsSource.Clear();

            foreach (var alignmentData in allAlignmentData)
            {
                if (alignmentData.Key.ToLower().Contains(userInput)) // If the alignment name contains the user input substring
                {
                    var newAlignmentItem = new ListViewItemModel
                    {
                        Name = alignmentData.Key,
                        IsSelected = alignmentData.Value.IsAlignmentSelected, // Use the stored selected state from allAlignmentData
                        Profiles = new ObservableCollection<ProfileModel>()
                    }; // Create a new ListViewItemModel for alignment

                    // Add profiles associated with the alignment
                    //foreach (string profileName in alignmentData.Value.ProfileNames)
                    foreach (var profileModel in alignmentData.Value.ProfileNames)
                    {
                        // Add profile only if it doesn't already exist
                        if (!newAlignmentItem.Profiles.Any(profile => profile.Name.ToLower() == profileModel.Name.ToLower()))
                        {
                            newAlignmentItem.Profiles.Add(new ProfileModel
                            {
                                Name = profileModel.Name,
                                IsSubItemSelected = profileModel.IsSubItemSelected // Set IsSelected based on allAlignmentData
                            });
                        }
                    }
                    // Add the new alignment item to the ObservableCollection
                    itemsSource.Add(newAlignmentItem);
                }
            }
        }

        public void SplitLandXMLSurfaces(string xmlfilename, List<string> surfacesToKeep)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            XmlDocument xmldoc = new XmlDocument();
            using (FileStream fs = new FileStream(xmlfilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xmldoc.Load(fs);
            }
            SplitNodes(xmldoc, xmlfilename, "Surface", surfacesToKeep, new List<string>(), ed);
        }

        public void SplitLandXMLAlignments(string xmlfilename, List<string> alignmentsToKeep, List<string> profilesToKeep)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            XmlDocument xmldoc = new XmlDocument();
            using (FileStream fs = new FileStream(xmlfilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xmldoc.Load(fs);
            }
            SplitNodes(xmldoc, xmlfilename, "Alignment", alignmentsToKeep, profilesToKeep, ed);
        }

        private void SplitNodes(XmlDocument xmldoc, string xmlfilename, string tagName, List<string> AlignmentsOrSurfacesToKeep, List<string> profilesToKeep, Editor ed)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
            nsmgr.AddNamespace("LandXML", "http://www.landxml.org/schema/LandXML-1.2");

            if (tagName == "Alignment")
            {
                XmlNode surfacesNode = xmldoc.SelectSingleNode("//LandXML:Surfaces", nsmgr);
                surfacesNode?.ParentNode?.RemoveChild(surfacesNode);
            }

            if (tagName == "Surface")
            {
                XmlNode alignmentsNode = xmldoc.SelectSingleNode("//LandXML:Alignments", nsmgr);
                alignmentsNode?.ParentNode?.RemoveChild(alignmentsNode);
            }

            XmlNodeList nodes = xmldoc.GetElementsByTagName(tagName);

            if (nodes == null || nodes.Count == 0)
                return;

            List<string> namelist = new List<string>();
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes != null && node.Attributes["name"] != null)
                {
                    namelist.Add(node.Attributes["name"].Value);
                }
            }

            string path = Path.GetDirectoryName(xmlfilename) + @"\";

            foreach (string nodeName in namelist)
            {
                string currentNodeName = nodeName;
                int index = AlignmentsOrSurfacesToKeep.IndexOf(currentNodeName);

                //if (index == -1)
                //    continue;
                if (index == -1)
                {
                    currentNodeName = nodeName.Replace('_', ' ');
                    index = AlignmentsOrSurfacesToKeep.IndexOf(currentNodeName);
                    if (index == -1)                    
                        continue;                 
                }



                string newfilename = path + currentNodeName + ".xml";
                if (tagName == "Alignment")
                {
                    if (TeljeKaust.Text != "")
                    {
                        newfilename = TeljeKaust.Text + "\\" + currentNodeName + ".xml";
                    }
                }

                if (tagName == "Surface")
                {
                    if (PinnaKaust.Text != "")
                    {
                        newfilename = PinnaKaust.Text + "\\" + currentNodeName + ".xml";
                    }
                }



                XmlDocument newdoc = (XmlDocument)xmldoc.Clone(); // Clone the original xmldoc to create a new document for each iteration
                nodes = newdoc.GetElementsByTagName(tagName);
                List<XmlNode> nodestoremove = new List<XmlNode>();
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["name"].Value != currentNodeName)
                    {
                        nodestoremove.Add(node);
                    }
                    else
                    {
                        if (tagName == "Alignment")
                        {
                            XmlNode alignmentNode = newdoc.SelectSingleNode($"//LandXML:Alignments/LandXML:Alignment[@name='{currentNodeName}']", nsmgr);
                            if (alignmentNode != null)
                            {
                                XmlNodeList profileNodes = alignmentNode.SelectNodes("LandXML:Profile", nsmgr);
                                if (profileNodes != null)
                                {
                                    foreach (XmlNode profileNode in profileNodes)
                                    {
                                        foreach (XmlNode childNode in profileNode.ChildNodes)
                                        {
                                            //if (childNode.Attributes["name"].Value != profileToKeep)
                                            if (childNode.Attributes["name"].Value != profilesToKeep[index])
                                            {
                                                nodestoremove.Add(childNode);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (XmlNode node in nodestoremove)
                {
                    node.ParentNode.RemoveChild(node);
                }

                newdoc.Save(newfilename);
            }
        }

        public static class StringCipher
        {
            private const int Keysize = 128;
            private const int DerivationIterations = 1000;

            public static string Encrypt(string plainText, string passPhrase)
            {
                byte[] saltStringBytes = Generate128BitsOfRandomEntropy();
                byte[] ivStringBytes = Generate128BitsOfRandomEntropy();
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = password.GetBytes(Keysize / 8);

                    using (var symmetricKey = new AesCryptoServiceProvider())
                    {
                        symmetricKey.BlockSize = 128;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;

                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                byte[] cipherTextBytes = memoryStream.ToArray();
                                byte[] resultBytes = new byte[saltStringBytes.Length + ivStringBytes.Length + cipherTextBytes.Length];

                                Buffer.BlockCopy(saltStringBytes, 0, resultBytes, 0, saltStringBytes.Length);
                                Buffer.BlockCopy(ivStringBytes, 0, resultBytes, saltStringBytes.Length, ivStringBytes.Length);
                                Buffer.BlockCopy(cipherTextBytes, 0, resultBytes, saltStringBytes.Length + ivStringBytes.Length, cipherTextBytes.Length);

                                return Convert.ToBase64String(resultBytes);
                            }
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = password.GetBytes(Keysize / 8);

                    using (var symmetricKey = new AesCryptoServiceProvider())
                    {
                        symmetricKey.BlockSize = 128;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;

                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            private static byte[] Generate128BitsOfRandomEntropy()
            {
                byte[] randomBytes = new byte[16];
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }


    }
}
