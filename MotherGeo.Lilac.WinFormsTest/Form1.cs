using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace MotherGeo.Lilac.WinFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            //Create a new instance in code or add via the designer
//Set the ChromiumWebBrowser.Address property to your Url if you use the designer.
            var browser = new ChromiumWebBrowser("www.google.com");
            //this.Fparent.Controls.Add(browser);
            
            this.components.Add(browser);
        }
    }
}