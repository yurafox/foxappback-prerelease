using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Providers
{
    public class EntityConfig
    {
        private string _sqlCommandSelect;
        private string _sqlCommandWhere;
        private string _keyField;
        private string _valueField;
        private string _preserializedJSONField;
        private string _serializerFunc;
        private string _baseTable;
        private string _sequence;
        private string _sqlCommandOrderBy;

        public string ConnString { get; set; }
        public string SqlCommandSelect {
            get 
            {
                return _sqlCommandSelect;
            }
        }
        public string SqlCommandWhere
        {
            get
            {
                return _sqlCommandWhere;
            }
        }

        public string SqlCommandOrderBy
        {
            get
            {
                return _sqlCommandOrderBy;
            }
        }

        public string KeyField
        {
            get
            {
                return _keyField;
            }
        }

        public string ValueField
        {
            get
            {
                return _valueField;
            }
        }
        public string PreserializedJSONField
        {
            get
            {
                return _preserializedJSONField;
            }
        }
        public string SerializerFunc
        {
            get
            {
                return _serializerFunc;
            }
        }

        public string BaseTable
        {
            get
            {
                return _baseTable;
            }
        }

        public string Sequence
        {
            get
            {
                return _sequence;
            }
        }


        public EntityConfig AddSqlCommandSelect(string sqlCommandSelect) {
            _sqlCommandSelect = sqlCommandSelect;
            return this;
        }
        public EntityConfig AddSqlCommandWhere(string sqlCommandWhere)
        {
            _sqlCommandWhere = sqlCommandWhere;
            return this;
        }

        public EntityConfig AddSqlCommandOrderBy(string sqlCommandOrderBy)
        {
            _sqlCommandOrderBy = sqlCommandOrderBy;
            return this;
        }


        public EntityConfig SetKeyField(string keyField)
        {
            _keyField = keyField;
            return this;
        }

        public EntityConfig SetValueField(string valueField)
        {
            _valueField = valueField;
            return this;
        }

        public EntityConfig SetPreserializedJSONField(string preserializedJSONField)
        {
            _preserializedJSONField = preserializedJSONField;
            return this;
        }

        public EntityConfig SetSerializerFunc(string serializerFunc)
        {
            _serializerFunc = serializerFunc;
            return this;
        }

        public EntityConfig SetBaseTable(string baseTable)
        {
            _baseTable = baseTable;
            return this;
        }

        public EntityConfig SetSequence(string sequence)
        {
            _sequence = sequence;
            return this;
        }


        public EntityConfig(string connString)
        {
            ConnString = connString;
        }
    }
}
