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
        /// Character that is used in the .csv file to separate each element in a line
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// Header line of the csv file
        /// </summary>
        public CsvFileLine HeaderLine { get; set; }

        /// <summary>
        /// A list that contains the rows above the csv table (all rows with a length not equal to the last row in the csv file)
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
        public CsvFile(string filepath, char delimiter)
        {
            Delimiter = delimiter;
            FilePath = filepath;
        }
        //###############################################################################################################################################################################################

        /// <summary>
        /// Read the content of the given .csv file to a list of CSVFileLines that contains prepared information. The .csv file is structured as follows ([...] means optionally):
        /// 
        /// [DescriptionRow1]
        /// [DescriptionRow2]
        /// ...
        /// HeaderLine
        /// DataLine
        /// DataLine
        /// ...
        /// 
        /// DescriptionRows can be omitted if not needed. There can be more than 2 DescriptionRows.
        /// Exact one header line is neccessary. The header line is the last line (from the bottom, no need to be the first line) that has the same number of elements as the last line in the .csv file.
        /// There can be any number of DataLines.
        /// </summary>
        /// <returns>true on success; otherwise false</returns>
        private bool ReadCSVLines()
        {
            CsvFileLines = new List<CsvFileLine>();
            CsvFileDescriptionRows = new List<string>();
            HeaderLine = null;

            // ########## Check FilePath ##########
            if (!File.Exists(FilePath))
            {
                return false;
            }
            if (Path.GetExtension(FilePath).ToLower() != ".txt" && Path.GetExtension(FilePath).ToLower() != ".csv")
            {
                return false;
            }
            string[] csv_lines = System.IO.File.ReadAllLines(FilePath);                             //Read all lines of the .csv file

            // ########## Find Header Line ##########
            int headerLineIndex = -1;
            string regexPatternLineElements = Delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";       //Using quotes to allow the delimiter

            // The header line is the last line (from the bottom, no need to be the first line) that has the same number of elements as the last line in the .csv file
            int numLastLineElements = Regex.Split(csv_lines.Last(), regexPatternLineElements).Length;
            for (int lineNo = csv_lines.Length - 1; lineNo >= 0; lineNo--)
            {
                int numLineElements = Regex.Split(csv_lines[lineNo], regexPatternLineElements).Length;
                if(numLineElements != numLastLineElements)
                {
                    headerLineIndex = lineNo;
                    break;
                }
            }
            headerLineIndex++;                                                                      //Increment because the headerLineIndex points to the row before the header line
            
            for(int lineNo = 0; lineNo < csv_lines.Length; lineNo++)                                //loop through all lines
            {
                string[] line_split = Regex.Split(csv_lines[lineNo], regexPatternLineElements);     //split line (using quotes to allow the delimiter)
                if (line_split.Length == 0 || line_split.Count(s => s == "") == line_split.Length)  //if the line is empty (0 elements) or each line element is "", continue with the next line
                {
                    continue;
                }

                CsvFileLine temp_line = new CsvFileLine();

                // ########## Line is Description Row ##########
                if (lineNo < headerLineIndex)
                {
                    CsvFileDescriptionRows.Add(csv_lines[lineNo]);
                    continue;
                }
                // ########## Line is Header Row ##########
                else if (lineNo == headerLineIndex)
                {
                    temp_line.IsHeaderLine = true;

                    for (int i = 0; i < line_split.Length; i++)                                     //loop through all elements in the current line
                    {                        
                        string value = line_split[i].Trim(' ').Trim('"');
                        temp_line.LineElements.Add(new CsvFileLineElement(value, value));           //add a new line element to the line
                    }
                    HeaderLine = temp_line;
                }
                // ########## Line is Data Row ##########
                else
                {
                    temp_line.IsHeaderLine = false;

                    for (int i = 0; i < line_split.Length; i++)                                     //loop through all elements in the current line
                    {
                        string header_text = "";
                        if (i < HeaderLine.LineElements.Count)
                        {
                            header_text = HeaderLine.LineElements[i].Value;                         //get the header text for the current line element
                        }

                        string value = line_split[i].Trim(' ').Trim('"');
                        temp_line.LineElements.Add(new CsvFileLineElement(value, header_text));     //add a new line element to the line
                    }
                    CsvFileLines.Add(temp_line);                                                    //Add the temporary line to the list of lines
                }
            }
            return true;
        }
    }
}
