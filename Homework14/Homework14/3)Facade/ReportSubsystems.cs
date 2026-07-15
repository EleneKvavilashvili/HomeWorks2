using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._3_Facade
{
    internal class Header
    {
        public string GetHtmlHeader()
        {
            return "<header> My Header </header>";
        }

        public string GetPdfHeader()
        {
            return "Header: I'm using Facade Pattern";
        }
    }

    internal class Body
    {
        public string GetHtmlBody()
        {
            return "<body>\n" +
                   "Video provides a powerful way to help you prove your point. When you click\n" +
                   "Online Video, you can paste in the embed code for the video you want to add.\n" +
                   "</body>";
        }

        public string GetPdfBody()
        {
            return "Body:\n" +
                   "Video provides a powerful way to help you prove your point. When you click\n" +
                   "Online Video, you can paste in the embed code for the video you want to add.\n" +
                   "You can also type a keyword to search online for the video that best fits your\n" +
                   "document. To make your document look professionally produced, Word provides";
        }
    }

    internal class Footer
    {
        public string GetHtmlFooter()
        {
            return "<footer> My Footer </footer>";
        }

        public string GetPdfFooter()
        {
            return "Footer: Page 1";
        }
    }
}
