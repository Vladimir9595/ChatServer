using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace ProjForms
{
    public class MessageReseau : ISerializable
    {
        public string Expediteur { get; set; }
        public string Contenu { get; set; }

        public MessageReseau() { }

        public MessageReseau(SerializationInfo info, StreamingContext context)
        {
            Expediteur = info.GetString("expediteur");
            Contenu = info.GetString("contenu");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("expediteur", Expediteur);
            info.AddValue("contenu", Contenu);
        }
    }
}
