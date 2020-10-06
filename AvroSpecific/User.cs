﻿using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text;
using Avro;
using Avro.Specific;


namespace AvroSpecific
{
	
	public partial class User : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""User"",""namespace"":""Confluent.Kafka.Examples.AvroSpecific"",""fields"":[{""name"":""name"",""type"":""string""},{""name"":""favorite_number"",""type"":[""int"",""null""]},{""name"":""favorite_color"",""type"":[""string"",""null""]},{""name"":""hourly_rate"",""type"":{""type"":""bytes"",""logicalType"":""decimal"",""precision"":4,""scale"":2}}]}");
		private string _name;
		private System.Nullable<System.Int32> _favorite_number;
		private string _favorite_color;
		private Avro.AvroDecimal _hourly_rate;
		public virtual Schema Schema
		{
			get
			{
				return User._SCHEMA;
			}
		}
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		public System.Nullable<System.Int32> favorite_number
		{
			get
			{
				return this._favorite_number;
			}
			set
			{
				this._favorite_number = value;
			}
		}
		public string favorite_color
		{
			get
			{
				return this._favorite_color;
			}
			set
			{
				this._favorite_color = value;
			}
		}
		public Avro.AvroDecimal hourly_rate
		{
			get
			{
				return this._hourly_rate;
			}
			set
			{
				this._hourly_rate = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
				case 0: return this.name;
				case 1: return this.favorite_number;
				case 2: return this.favorite_color;
				case 3: return this.hourly_rate;
				default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
				case 0: this.name = (System.String)fieldValue; break;
				case 1: this.favorite_number = (System.Nullable<System.Int32>)fieldValue; break;
				case 2: this.favorite_color = (System.String)fieldValue; break;
				case 3: this.hourly_rate = (Avro.AvroDecimal)fieldValue; break;
				default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
