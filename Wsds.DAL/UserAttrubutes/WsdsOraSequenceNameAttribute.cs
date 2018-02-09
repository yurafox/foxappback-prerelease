using System;

namespace Wsds.DAL.UserAttrubutes
{
    public class WsdsOraSequenceNameAttribute: Attribute
    {
        public string Name { get; set; }
        public WsdsOraSequenceNameAttribute (string name)
        {
            this.Name = name;
        }
    }
}