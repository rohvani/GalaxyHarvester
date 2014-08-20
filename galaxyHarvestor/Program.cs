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

            if (!File.Exists("config.cfg"))
            {
                Console.WriteLine("Couldn't find 'config.cfg' in local directory, is it missing?");
            }
            else if (!File.Exists("resource_manager_spawns.lua"))
            {
                Console.WriteLine("Couldn't find 'resource_manager_spawns.lua' in local directory, is it missing?");
            }
            else
            {
                string galaxyId = Config.getValue("galaxyId");
                string cookie = Config.getValue("cookie");

                Console.WriteLine("Loading resources from 'resource_manager_spawns.lua'");
                List<Resource> resources = SerializeResources();

                foreach (Resource resource in resources)
                {
                    using (var wb = new WebClient())
                    {
                        var data = new NameValueCollection();

                        data["gh_sid"] = "'%2Bsid%2B'";
                        data["galaxy"] = galaxyId;
                        data["planet"] = resource.planet.ToString();
                        data["resName"] = resource.name;
                        data["resType"] = resource.type;
                        data["sourceRow"] = "0";
                        data["CR"] = resource.res_cold_resist == 0 ? "" : resource.res_cold_resist.ToString();
                        data["CD"] = resource.res_conductivity == 0 ? "" : resource.res_conductivity.ToString();
                        data["DR"] = resource.res_decay_resist == 0 ? "" : resource.res_decay_resist.ToString();
                        data["FL"] = resource.res_flavor == 0 ? "" : resource.res_flavor.ToString();
                        data["HR"] = resource.res_heat_resist == 0 ? "" : resource.res_heat_resist.ToString();
                        data["MA"] = resource.res_malleability == 0 ? "" : resource.res_malleability.ToString();
                        data["PE"] = resource.res_potential_energy == 0 ? "" : resource.res_potential_energy.ToString();
                        data["OQ"] = resource.res_quality == 0 ? "" : resource.res_quality.ToString();
                        data["SR"] = resource.res_shock_resistance == 0 ? "" : resource.res_shock_resistance.ToString();
                        data["UT"] = resource.res_toughness == 0 ? "" : resource.res_toughness.ToString();
						data["ER"] = resource.entangle_resistance == 0 ? "" : resource.entangle_resistance.ToString();

                        wb.Headers.Add(HttpRequestHeader.Cookie, cookie);
                       // var response = wb.UploadValues(new Uri("http://www.galaxyharvester.net/postResource.py"), "POST", data);
                    }
                    Console.SetCursorPosition(0, 1);
					Console.WriteLine(resource.res_quality);
                    Console.WriteLine("[{0}/{1}] Uploading resources to GalaxyHarvester", resources.IndexOf(resource), resources.Count);
                    Thread.Sleep(10);
                }

                Console.WriteLine("Completed uploading {0} resources to GalaxyHarvester!", resources.Count);
            }
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

								string fieldName = ((String)attributeValues[0]);
								int value = Convert.ToInt32(attributeValues[1]);

								res.GetType().GetField(fieldName).SetValue(res, value);
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
