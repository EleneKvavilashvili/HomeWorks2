using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._3_Facade
{
    internal class ReportFacade
    {
        private readonly Header _header;
        private readonly Body _body;
        private readonly Footer _footer;

        public ReportFacade()
        {
            _header = new Header();
            _body = new Body();
            _footer = new Footer();
        }

        public void GenerateHtmlReport()
        {
            Console.WriteLine(_header.GetHtmlHeader());
            Console.WriteLine(_body.GetHtmlBody());
            Console.WriteLine(_footer.GetHtmlFooter());
        }

        public void GeneratePdfReport()
        {
            Console.WriteLine(_header.GetPdfHeader());
            Console.WriteLine(_body.GetPdfBody());
            Console.WriteLine();
            Console.WriteLine(_footer.GetPdfFooter());
        }
    }
}
