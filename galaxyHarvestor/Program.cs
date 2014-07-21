using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Seefo;

namespace galaxyHarvestor
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "GalaxyHarvester Uploader";

			if(!File.Exists("config.cfg"))
			{
				Console.WriteLine("Couldn't find 'config.cfg' in local directory, is it missing?");
				Console.Read();
			}
			else if (!File.Exists("resource_manager_spawns.lua"))
			{
				Console.WriteLine("Couldn't find 'resource_manager_spawns.lua' in local directory, is it missing?");
				Console.Read();
			}			

			string galaxyId = Config.getValue("galaxyId");
			string cookie = Config.getValue("cookie");

			Console.WriteLine("Loading resources from 'resource_manager_spawns.lua'");
			List<Resource> resources = SerializeResources();

			foreach (Resource res in resources)
			{
				using (var wb = new WebClient())
				{
					var data = new NameValueCollection();

					data["gh_sid"] = "'%2Bsid%2B'";
					data["galaxy"] = galaxyId;
					data["planet"] = res.planet.ToString();
					data["resName"] = res.name;
					data["resType"] = res.type;
					data["sourceRow"] = "0";
					data["CR"] = res.cold_resist == 0 ? "" : res.cold_resist.ToString();
					data["CD"] = res.conductivity == 0 ? "" : res.conductivity.ToString();
					data["DR"] = res.decay_resist == 0 ? "" : res.decay_resist.ToString();
					data["FL"] = res.flavor == 0 ? "" : res.flavor.ToString();
					data["HR"] = res.heat_resist == 0 ? "" : res.heat_resist.ToString();
					data["MA"] = res.malleability == 0 ? "" : res.malleability.ToString();
					data["PE"] = res.potential_energy == 0 ? "" : res.potential_energy.ToString();
					data["OQ"] = res.overal_quality == 0 ? "" : res.overal_quality.ToString();
					data["SR"] = res.shock_resistance == 0 ? "" : res.shock_resistance.ToString();
					data["UT"] = res.unit_toughness == 0 ? "" : res.unit_toughness.ToString();
					data["ER"] = "";

					wb.Headers.Add(HttpRequestHeader.Cookie, cookie);
					var response = wb.UploadValues(new Uri("http://www.galaxyharvester.net/postResource.py"), "POST", data);
				}
				Console.SetCursorPosition(0, 1);
				Console.WriteLine("[{0}/{1}] Uploading resources to GalaxyHarvester", resources.IndexOf(res), resources.Count);
				Thread.Sleep(10);
			}

			Console.WriteLine("Completed uploading {0} resources to GalaxyHarvester!", resources.Count);
			Console.Read();
		}

		static List<Resource> SerializeResources()
		{
			List<Resource> resources = new List<Resource>();

			Lua lua = new Lua();
			var result = lua.DoFile("resource_manager_spawns.lua");

			foreach (DictionaryEntry resource in lua.GetTable("resources"))
			{
				Resource res = new Resource();
				foreach (DictionaryEntry stat in lua.GetTableDict((LuaTable) resource.Value))
				{
					switch((String) stat.Key)
					{
						case "name": 
							res.name = (String) stat.Value;
							break;

						case "type":
							res.type = (String)stat.Value;
							break;

						case "attributes":
							foreach (DictionaryEntry attribute in lua.GetTableDict((LuaTable)stat.Value))
							{
								object[] attributeValues = new object[2];
								((LuaTable)attribute.Value).Values.CopyTo(attributeValues, 0);

								switch ((String) attributeValues[0])
								{
									case "res_cold_resist":
										res.cold_resist = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_conductivity":
										res.conductivity = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_decay_resist":
										res.decay_resist = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_heat_resist":
										res.heat_resist = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_malleability":
										res.malleability = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_quality":
										res.overal_quality = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_shock_resistance":
										res.shock_resistance = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_toughness":
										res.unit_toughness = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_flavor":
										res.flavor = (int) Convert.ToInt32(attributeValues[1]);
										break;

									case "res_potential_energy":
										res.potential_energy = (int) Convert.ToInt32(attributeValues[1]);
										break;
								}
							}
							break;

						case "zoneRestriction":
							switch((String) stat.Value)
							{
								case "corellia":
									res.planet = 1;
									break;

								case "dantooine":
									res.planet = 2;
									break;

								case "dathomir":
									res.planet = 3;
									break;

								case "endor":
									res.planet = 4;
									break;

								case "lok":
									res.planet = 5;
									break;

								case "naboo":
									res.planet = 6;
									break;

								case "rori":
									res.planet = 7;
									break;

								case "talus":
									res.planet = 8;
									break;

								case "tatooine":
									res.planet = 9;
									break;

								case "yavin4":
									res.planet = 10;
									break;

								default:
									res.planet = 0;
									break;
							}
							break;

						default:
							break;
					}
				}
				resources.Add(res);
			}
			return resources;
		}
	}
}
