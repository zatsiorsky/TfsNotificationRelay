using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DevCore.TfsNotificationRelay
{
    public class UserMap
    {
        private string SlackAPIToken = "xoxp-4780571987-5169200945-6585715173-2dc98e";

        public static readonly Dictionary<string, string> map = new Dictionary<string, string>()
        {
            { "Anoop.Pv" ,"U04PCHWQ1"},
            { "Ayush.Sahay", "U04NE7J4S"},
            { "christopher.dudak", "U050BPP7M"},
            { "Daniel.Spector", "U04NYGU01"},
            { "E.Gomez-Echeverry", "U04P0C123"},
            { "Fariya.Wani", "U04P8LFFK"},
            { "fergal_sweeney", "U04PH0LB1"},
            { "George.Vlatas", "U04NB2RBL"},
            { "James.Zatsiorsky", "U054Z5WTT"},
            { "Jay.Marciano", "U0508GWGE"},
            { "Kevin.Brown", "U0508E8EG"},
            { "Luke.Burnham" ,"U04NYT63K"},
            { "Michael.Cavanaugh", "U06GK78RG"},
            { "Michael.Viar", "U04NWD57S"},
            { "Rakesh.Mohan", "U04P8UMKP"},
            { "Ruchi.Karambelkar", "U04P8JM13"},
            { "Shilpa.Saware", "U04PACNHY"},
            { "v-Vladimir.Veevnik", "U04NZ1ZJX"}  
        };

        // Takes a TFS username, and returns the corresponding Slack username if it exists, 
        // else return the TFS username
        public static string TfsToSlack(string tfsname)
        {
            if (map.ContainsKey(tfsname)) {
                return "<@" + map[tfsname] + ">";
            }
            else {
                return tfsname;
            }
        }
    }
}
