using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvDocConverter.Csv;

namespace CsvDocConverter
{
    public class CsvMappingFile : CsvFile
    {
        /// <summary>
        /// String that is the marker for general information blocks
        /// </summary>
        public const string GENERAL_INFOS_MARKER_PART = "%Infos";

        /// <summary>
        /// Dictionary containing mapping informations in the format: %Placeholder in Template%, %Column heading in CSV%
        /// </summary>
        public Dictionary<string, string> Mappings { get; private set; }

        //###############################################################################################################################################################################################

        /// <summary>
        /// Constructor of the CsvMappingFile class that loads the given file and creates a dictionary with mappings
        /// </summary>
        /// <param name="filepath">fully qualified filepath of the .csv file</param>
        /// <param name="delimiter">sign that is used in the .csv file to separate each element in a line</param>
        public CsvMappingFile(string filepath, char delimiter) : base(filepath, delimiter)
        {
            parseMappings();
        }

        /// <summary>
        /// Add all .csv data file header names as placeholders to the mapping dictionary
        /// </summary>
        /// <param name="csvDataFile">CSV data file</param>
        public void AddMappingsForDataCsvHeader(CsvFile csvDataFile)
        {
            if (!Mappings.ContainsKey("%Infos%"))
            {
                Mappings.Add("%Infos%", "%Infos%");
            }

            foreach (CsvFileLineElement headerElement in csvDataFile?.HeaderLine?.LineElements)
            {
                string headerPlaceholder = "%" + headerElement.Value + "%";
                if (!Mappings.ContainsKey(headerPlaceholder))
                {
                    Mappings.Add(headerPlaceholder, headerElement.Value);
                }
            }
        }

        //###############################################################################################################################################################################################

        /// <summary>
        /// Parse the mappings from the CSV file
        /// </summary>
        private void parseMappings()
        {
            Mappings = new Dictionary<string, string>();
            foreach (CsvFileLine mappingLine in this.CsvFileLines)
            {
                if (mappingLine.LineElements.Count < 2)     // Fehler: Eine Zeile in der mapping CSV Datei hat zu wenig Elemente.
                {
                    Mappings = null;
                    return;
                }
                if (!Mappings.ContainsKey(mappingLine.LineElements[0].Value.Trim()))
                {
                    Mappings.Add(mappingLine.LineElements[0].Value.Trim(), mappingLine.LineElements[1].Value.Trim());
                }
            }
        }
    }
}
