using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yfer.ZedGraph.Extension.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OscView
    {

        [TestMethod]
        public void ScienceView()
        {
            var form = new OscViewForm();
            var result = form.ShowDialog();
            if(result!=DialogResult.Yes) 
                Assert.Fail("View doesn't match.");
        }
    }
}
