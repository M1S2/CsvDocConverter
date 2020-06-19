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
        /// Dictionary containing mapping informations in the format: %Placeholder in Template%, %Column heading in CSV%
        /// </summary>
        public Dictionary<string, string> Mappings { get; private set; }

        /// <summary>
        /// String that is the marker for general information blocks
        /// </summary>
        public string GeneralInfosMarkerPart { get; private set; }

        //###############################################################################################################################################################################################

        /// <summary>
        /// Constructor of the CsvMappingFile class that loads the given file and creates a dictionary with mappings
        /// </summary>
        /// <param name="filepath">fully qualified filepath of the .csv file</param>
        /// <param name="delimiter">sign that is used in the .csv file to separate each element in a line</param>
        public CsvMappingFile(string filepath, char delimiter) : base(filepath, delimiter, false)
        {
            parseMappings();
        }

        //###############################################################################################################################################################################################

        /// <summary>
        /// Parse the mappings from the CSV file
        /// </summary>
        private void parseMappings()
        {
            Mappings = new Dictionary<string, string>();
            GeneralInfosMarkerPart = (this.CsvFileDescriptionRows.Count == 0 ? "" : this.CsvFileDescriptionRows[0].Trim());
            foreach (CsvFileLine mappingLine in this.CsvFileLines)
            {
                if (mappingLine.LineElements.Count < 2)     // Fehler: Eine Zeile in der mapping CSV Datei hat zu wenig Elemente.
                {
                    Mappings = null;
                    return;
                }
                Mappings.Add(mappingLine.LineElements[0].Value.Trim(), mappingLine.LineElements[1].Value.Trim());
            }
        }
    }
}
