using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RPS_Challenge {
    public partial class About : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Path.EndsWith("About.aspx", StringComparison.OrdinalIgnoreCase)) {
                this.Response.Redirect("~/About", true);
            }
        }
    }
}
