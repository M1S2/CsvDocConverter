using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvDocConverter.Csv
{
    public class CsvFile
    {
        private string _filePath;
        /// <summary>
        /// Fully qualified filepath of the CSV file
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; ReadCSVLines(); }
        }

        /// <summary>
        /// sign that is used in the .csv file to separate each element in a line
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// true -> save header lines to the list of lines; false -> ignore header lines
        /// </summary>
        public bool ListHeaderLines { get; set; }

        /// <summary>
        /// A list that contains the rows above the csv table (all rows with only one element)
        /// </summary>
        public List<string> CsvFileDescriptionRows { get; set; }

        /// <summary>
        /// String containing all CsvFileDescriptionRows seperated by a NewLine
        /// </summary>
        public string AllCsvFileDescriptionRows => string.Join(Environment.NewLine, CsvFileDescriptionRows);

        /// <summary>
        /// A list that contains each line of the .csv file with prepared information
        /// </summary>
        public List<CsvFileLine> CsvFileLines { get; set; }

        //###############################################################################################################################################################################################

        /// <summary>
        /// Constructor of the CsvFile class that loads the given file and creates a list of lines with prepared information
        /// </summary>
        /// <param name="filepath">fully qualified filepath of the .csv file</param>
        /// <param name="delimiter">sign that is used in the .csv file to separate each element in a line</param>
        /// <param name="listHeaderLines">true -> save header lines to the list of lines; false -> ignore header lines</param>
        public CsvFile(string filepath, char delimiter, bool listHeaderLines = false)
        {
            Delimiter = delimiter;
            ListHeaderLines = listHeaderLines;
            FilePath = filepath;
        }
        //###############################################################################################################################################################################################

        /// <summary>
        /// Read the content of the given .csv file to a list of CSVFileLines that contains prepared information
        /// </summary>
        /// <returns>true on success; otherwise false</returns>
        private bool ReadCSVLines()
        {
            CsvFileLines = new List<CsvFileLine>();
            CsvFileDescriptionRows = new List<string>();

            if (!File.Exists(FilePath))
            {
                return false;
            }
            if (Path.GetExtension(FilePath).ToLower() != ".txt" && Path.GetExtension(FilePath).ToLower() != ".csv")
            {
                return false;
            }
            string[] csv_lines = System.IO.File.ReadAllLines(FilePath);                 //Read all lines of the .csv file

            CsvFileLine headerLine = null;                          //This array holds the found header line
            bool currentLineIsHeaderLine = false;

            foreach (string line in csv_lines)                      //loop through all lines
            {
                CsvFileLine temp_line = new CsvFileLine();
                
                string[] line_split = Regex.Split(line, Delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");    //split line (using quotes to allow the delimiter)

                if (line_split.Length == 0 || line_split.Count(s => s == "") == line_split.Length)          //if the line is empty (0 elements) or each line element is "", continue with the next line
                {
                    continue;
                }

                if(line_split.Length == 1 && headerLine == null)                    //save rows with only one element as description rows in the csv file
                {
                    CsvFileDescriptionRows.Add(line);
                    continue;
                }
                else if(line_split.Length > 1 && headerLine == null)
                {
                    currentLineIsHeaderLine = true;
                }

                temp_line.IsHeaderLine = currentLineIsHeaderLine;

                for (int i = 0; i < line_split.Length; i++)                         //loop through all elements in the current line
                {
                    string header_text = "";

                    if (!currentLineIsHeaderLine && headerLine != null)
                    {
                        if (i < headerLine.LineElements.Count)
                        {
                            header_text = headerLine.LineElements[i].Value;         //get the header text for the current line element
                        }
                    }

                    string value = line_split[i].Trim('"');
                    temp_line.LineElements.Add(new CsvFileLineElement(value, header_text));        //add a new line element to the line
                }
                currentLineIsHeaderLine = false;

                if(temp_line.IsHeaderLine)
                {
                    headerLine = temp_line;
                }
                if (temp_line.IsHeaderLine && ListHeaderLines == false)             //Don't save the current line if it is a header line and the option ListHeaderLines == false
                {
                    continue;
                }

                CsvFileLines.Add(temp_line);                                        //Add the temporary line to the list of lines
            }
            return true;
        }

    }
}
