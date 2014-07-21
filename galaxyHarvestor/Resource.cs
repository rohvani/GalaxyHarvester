using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace galaxyHarvestor
{
	public class Resource
	{
		// not using a struct cause i'm lazy :-D

		public int planet;
		public string name;
		public string type;
		public int cold_resist;
		public int conductivity;
		public int decay_resist;
		public int flavor;
		public int heat_resist;
		public int malleability;
		public int overal_quality;
		public int shock_resistance;
		public int unit_toughness;
		public int potential_energy;

		public Resource() 
		{
			planet = 0;
			name = "none";
			type = "none";
			cold_resist = 0;
			conductivity = 0;
			decay_resist = 0;
			flavor = 0;
			heat_resist = 0;
			malleability = 0;
			overal_quality = 0;
			shock_resistance = 0;
			unit_toughness = 0;
			potential_energy = 0;
		}
	}
}
