using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDocConverter.Csv
{
    /// <summary>
    /// Class that describes a single line of a .csv file with prepared information
    /// </summary>
    public class CsvFileLine
    {
        /// <summary>
        /// This list contains each element of the line with it's corresponding header and value
        /// </summary>
        public List<CsvFileLineElement> LineElements { get; set; }

        /// <summary>
        /// Indicating whether a line is a header line or not
        /// </summary>
        public bool IsHeaderLine { get; set; }

        /// <summary>
        /// Standard Constructor of the CsvFileLine class
        /// </summary>
        public CsvFileLine()
        {
            LineElements = new List<CsvFileLineElement>();
            IsHeaderLine = false;
        }
    }
}
