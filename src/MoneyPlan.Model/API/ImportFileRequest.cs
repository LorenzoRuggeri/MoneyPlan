using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyPlan.Model.API
{
    public class ImportFileRequest
    {
        /// <summary>
        /// Name of the Importer to be used
        /// </summary>
        public string Importer { get; set; }

        public byte[] Content { get; set; }

        public int Account { get; set; }
    }
}
