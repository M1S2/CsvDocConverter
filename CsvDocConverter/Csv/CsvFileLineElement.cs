using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDocConverter.Csv
{
    /// <summary>
    /// Class that describes a single element in a line in a .csv file
    /// </summary>
    public class CsvFileLineElement
    {
        /// <summary>
        /// the value in the specific cell
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// the text of the column header of the current data element
        /// </summary>
        public string CorrespondingHeader { get; set; }

        /// <summary>
        /// Constructor of the CsvFileLineElement class
        /// </summary>
        /// <param name="value">the value in the specific cell</param>
        /// <param name="correspondingHeader">the text of the column header of the current data element</param>
        public CsvFileLineElement(string value, string correspondingHeader)
        {
            Value = value;
            CorrespondingHeader = correspondingHeader;
        }
    }
}
