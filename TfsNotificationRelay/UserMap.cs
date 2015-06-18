using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay
{
    public class UserMap
    {
        public static readonly Dictionary<string, string> map = new Dictionary<string, string>()
        {
            { "Anoop.Pv" ,"anooppv"},
            { "Ayush.Sahay", "ayush"},
            { "christopher.dudak", "chris"},
            { "Daniel.Spector", "dan"},
            { "E.Gomez-Echeverry", "eduardo"},
            { "Fariya.Wani", "fariya.wani"},
            { "fergal_sweeney", "fergal"},
            { "George.Vlatas", "george"},
            { "James.Zatsiorsky", "james.z"},
            { "Jay.Marciano", "jaymarciano"},
            { "Kevin.Brown", "kevinbrown"},
            { "Luke.Burnham" ,"luke"},
            { "Michael.Cavanaugh", "michael.cavanaugh"},
            { "Michael.Viar", "michaelviar"},
            { "Rakesh.Mohan", "rakesh.mohan"},
            { "Ruchi.Karambelkar", "ruchi"},
            { "Shilpa.Saware", "shilpa"},
            { "v-Vladimir.Veevnik", "vladimir"}  
        };

        // Takes a TFS username, and returns the corresponding Slack username if it exists, 
        // else return the TFS username
        public static string TfsToSlack(string tfsname)
        {
            if (map.ContainsKey(tfsname)) {
                return "@" + map[tfsname];
            }
            else {
                return tfsname;
            }
        }
    }
}
