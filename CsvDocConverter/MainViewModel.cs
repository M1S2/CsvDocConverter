using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CsvDocConverter.Csv;
using Xceed.Words.NET;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Text.RegularExpressions;

namespace CsvDocConverter
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changed</param>
        /// see: https://docs.microsoft.com/de-de/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //##############################################################################################################################################################################################

        #region Commands

        private ICommand _convertCommand;
        public ICommand ConvertCommand
        {
            get
            {
                if (_convertCommand == null)
                {
                    _convertCommand = new RelayCommand(param => { ConvertCsvToDoc(); });
                }
                return _convertCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _selectTemplatePathCommand;
        public ICommand SelectTemplatePathCommand
        {
            get
            {
                if (_selectTemplatePathCommand == null)
                {
                    _selectTemplatePathCommand = new RelayCommand(param =>
                    {
                        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                        openFileDialog.FileName = TemplatePath;
                        openFileDialog.Filter = "Word File (*.docx)|*.docx";        // Only allow new *.docx format
                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            TemplatePath = openFileDialog.FileName;
                        }
                    });
                }
                return _selectTemplatePathCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _selectCsvPathCommand;
        public ICommand SelectCsvPathCommand
        {
            get
            {
                if (_selectCsvPathCommand == null)
                {
                    _selectCsvPathCommand = new RelayCommand(param =>
                    {
                        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                        openFileDialog.FileName = "\"" + string.Join("\" \"", CsvFilePaths) + "\"";
                        openFileDialog.Filter = "CSV File (*.csv)|*.csv|Text File (*.txt)|*.txt";
                        openFileDialog.Multiselect = true;
                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            CsvFilePaths = openFileDialog.FileNames.ToList();
                        }
                    });
                }
                return _selectCsvPathCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _selectMappingCsvPathCommand;
        public ICommand SelectMappingCsvPathCommand
        {
            get
            {
                if (_selectMappingCsvPathCommand == null)
                {
                    _selectMappingCsvPathCommand = new RelayCommand(param =>
                    {
                        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                        openFileDialog.FileName = MappingCsvFilePath;
                        openFileDialog.Filter = "CSV File (*.csv)|*.csv|Text File (*.txt)|*.txt";
                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            MappingCsvFilePath = openFileDialog.FileName;
                        }
                    });
                }
                return _selectMappingCsvPathCommand;
            }
        }

        #endregion

        //##############################################################################################################################################################################################

        #region Properties

        private string _templatePath;
        /// <summary>
        /// Path to the template
        /// </summary>
        public string TemplatePath
        {
            get { return _templatePath; }
            set { _templatePath = value; OnPropertyChanged(); }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private List<string> _csvFilePaths;
        /// <summary>
        /// Paths to the input .csv file(s).
        /// </summary>
        public List<string> CsvFilePaths
        {
            get { return (_csvFilePaths == null ? new List<string>() : _csvFilePaths); }
            set
            {
                _csvFilePaths = value;
                OnPropertyChanged();
                if (AutoConvertOnCsvFilePathChange) { ConvertCsvToDoc(); }
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _mappingCsvFilePath;
        /// <summary>
        /// Path to the mapping input .csv file
        /// </summary>
        public string MappingCsvFilePath
        {
            get { return _mappingCsvFilePath; }
            set
            {
                _mappingCsvFilePath = value;
                OnPropertyChanged();
                MappingFile = new CsvMappingFile(MappingCsvFilePath, DelimiterCharacter);
            }
        }

        private CsvMappingFile _mappingFile;
        /// <summary>
        /// Mapping file object
        /// </summary>
        public CsvMappingFile MappingFile
        {
            get { return _mappingFile; }
            set { _mappingFile = value; OnPropertyChanged(); }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private bool _autoConvertOnCsvFilePathChange;
        /// <summary>
        /// Automatically start conversion if .csv file path changes
        /// </summary>
        public bool AutoConvertOnCsvFilePathChange
        {
            get { return _autoConvertOnCsvFilePathChange; }
            set { _autoConvertOnCsvFilePathChange = value; OnPropertyChanged(); }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private bool _isDelimiterComma;
        /// <summary>
        /// true if the delimiter is set to comma
        /// </summary>
        public bool IsDelimiterComma
        {
            get { return _isDelimiterComma; }
            set { _isDelimiterComma = value; OnPropertyChanged(); }
        }

        private bool _isDelimiterSemicolon;
        /// <summary>
        /// true if the delimiter is set to semicolon
        /// </summary>
        public bool IsDelimiterSemicolon
        {
            get { return _isDelimiterSemicolon; }
            set { _isDelimiterSemicolon = value; OnPropertyChanged(); }
        }

        public char DelimiterCharacter => (IsDelimiterComma ? ',' : ';');

        #endregion

        private IDialogCoordinator _dialogCoordinator;

        //##############################################################################################################################################################################################

        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            // Load settings
            TemplatePath = Properties.Settings.Default.TemplatePath;
            IsDelimiterComma = Properties.Settings.Default.IsDelimiterComma;
            MappingCsvFilePath = Properties.Settings.Default.MappingCsvFilePath;
            AutoConvertOnCsvFilePathChange = Properties.Settings.Default.AutoConvertOnCsvFilePathChange;
        }

        //##############################################################################################################################################################################################

        /// <summary>
        /// Function that is called when the window is closing. Save all settings.
        /// </summary>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Store settings
            Properties.Settings.Default.TemplatePath = TemplatePath;
            Properties.Settings.Default.MappingCsvFilePath = MappingCsvFilePath;
            Properties.Settings.Default.IsDelimiterComma = IsDelimiterComma;
            Properties.Settings.Default.AutoConvertOnCsvFilePathChange = AutoConvertOnCsvFilePathChange;
            Properties.Settings.Default.Save();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Use the filepaths of the dropped files. It is possible to drop template, mapping and csv files the same time
        /// </summary>
        public void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> fileNames = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                string templateFileName = fileNames.Where(f => System.IO.Path.GetExtension(f) == ".docx").FirstOrDefault();
                string mappingFileName = fileNames.Where(f => (System.IO.Path.GetExtension(f) == ".csv" || System.IO.Path.GetExtension(f) == ".txt") && System.IO.Path.GetFileName(f).ToLower().StartsWith("mapping")).FirstOrDefault();
                List<string> csvFileNames = fileNames.Where(f => (System.IO.Path.GetExtension(f) == ".csv" || System.IO.Path.GetExtension(f) == ".txt") && !System.IO.Path.GetFileName(f).ToLower().StartsWith("mapping")).ToList();

                if (!string.IsNullOrEmpty(templateFileName)) { TemplatePath = templateFileName; }
                if (!string.IsNullOrEmpty(mappingFileName)) { MappingCsvFilePath = mappingFileName; }
                if (csvFileNames != null && csvFileNames.Count > 0) { CsvFilePaths = csvFileNames; }
            }
        }

        //##############################################################################################################################################################################################

        /// <summary>
        /// Convert the .csv files to documents
        /// </summary>
        private async void ConvertCsvToDoc()
        {
            // Check if template file exists
            if (!System.IO.File.Exists(TemplatePath))
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Das Template wurde nicht gefunden: " + Environment.NewLine + TemplatePath);
                return;
            }
            if (System.IO.Path.GetExtension(TemplatePath).ToLower() != ".docx")
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Das Template hat das falsche Format (nur *.docx erlaubt): " + Environment.NewLine + TemplatePath);
                return;
            }
            if (!System.IO.File.Exists(MappingCsvFilePath))
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Die Mapping Datei wurde nicht gefunden: " + Environment.NewLine + TemplatePath);
                return;
            }
            if (MappingFile.Mappings == null)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Es gibt Fehler in der Mapping Datei.");
                return;
            }

            int convertedFilesCnt = 0;
            // Loop over all .csv files
            foreach (string csvFilePath in CsvFilePaths)
            {
                // Check if .csv file exists
                if (!System.IO.File.Exists(csvFilePath))
                {
                    await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Die CSV Datei wurde nicht gefunden: " + Environment.NewLine + csvFilePath);
                    continue;
                }

                CsvFile csv_file = new CsvFile(csvFilePath, DelimiterCharacter);
                using (DocX templateDoc = DocX.Load(TemplatePath))
                {
                    Xceed.Document.NET.Table templateDocTable = templateDoc.Tables.FirstOrDefault();
                    if (templateDocTable == null)
                    {
                        await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Im Template wurde keine Tabelle gefunden.");
                        return;
                    }
                    if (templateDocTable.RowCount <= 1)
                    {
                        await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Die Tabelle im Template muss 2 Zeilen haben.");
                        return;
                    }
                    Xceed.Document.NET.Row rowPattern = templateDocTable.Rows[1];       // Get the row pattern of the second row (the first row contains the headers).

                    // Add the general informations to the document. This are the rows above the real CSV table.
                    foreach (KeyValuePair<string, string> generalInfoMapping in MappingFile.Mappings.Where(m => m.Value.StartsWith(MappingFile.GeneralInfosMarkerPart)))
                    {
                        string replacementText = "";
                        string generalInfoIndexStr = generalInfoMapping.Value.Replace(MappingFile.GeneralInfosMarkerPart, "").Replace("%", "");
                        if (string.IsNullOrEmpty(generalInfoIndexStr))
                        {
                            replacementText = csv_file.AllCsvFileDescriptionRows;
                        }
                        else
                        {
                            int generalInfoIndex = -1;
                            if (!int.TryParse(generalInfoIndexStr, out generalInfoIndex))
                            {
                                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Der Index (\"" + generalInfoIndexStr + "\") für die Beschreibungszeilen kann nicht geparst werden.");
                                continue;
                            }
                            if (generalInfoIndex < 0 || generalInfoIndex >= csv_file.CsvFileDescriptionRows.Count)
                            {
                                await _dialogCoordinator.ShowMessageAsync(this, "Fehler", "Der Index (" + generalInfoIndex.ToString() + ") für die Beschreibungszeilen ist außerhalb des gültigen Bereiches.");
                                continue;
                            }

                            replacementText = csv_file.CsvFileDescriptionRows[generalInfoIndex];
                        }
                        templateDoc.ReplaceText(generalInfoMapping.Key, replacementText);
                    }

                    // Insert csv content to table
                    foreach (CsvFileLine fileLine in csv_file.CsvFileLines)
                    {
                        Xceed.Document.NET.Row newRow = templateDocTable.InsertRow(rowPattern, templateDocTable.RowCount - 1);      // Insert new row at the end of the table
                        foreach (KeyValuePair<string, string> mappingPair in MappingFile.Mappings)
                        {
                            if (!mappingPair.Value.StartsWith(MappingFile.GeneralInfosMarkerPart))
                            {
                                CsvFileLineElement lineElement = fileLine?.LineElements?.Where(element => element.CorrespondingHeader == mappingPair.Value)?.FirstOrDefault();
                                newRow.ReplaceText(mappingPair.Key, lineElement == null ? "" : lineElement.Value);
                            }
                        }
                    }

                    // Remove the pattern row.
                    rowPattern.Remove();

                    templateDoc.SaveAs(System.IO.Path.ChangeExtension(csv_file.FilePath, null) + System.IO.Path.GetExtension(TemplatePath));
                    convertedFilesCnt++;
                }
            }
            await _dialogCoordinator.ShowMessageAsync(this, "Fertig", convertedFilesCnt.ToString() + " von " + CsvFilePaths.Count.ToString() + " CSV Datei(en) wurde(n) erfolgreich in Dokument(e) umgewandelt.");
        }

    }
}
