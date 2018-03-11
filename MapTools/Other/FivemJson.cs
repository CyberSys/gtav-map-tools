using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MapTools.Other
{
    public class FivemJson
    {
        public string filename { get; set; }

        public List<FivemArchetype> archetypes;
        public List<FivemEntity> entities;
        public Dictionary<uint, string> hashes;

        public FivemJson(string jsonstring,string name)
        {
            filename = name;
            archetypes = new List<FivemArchetype>();
            entities = new List<FivemEntity>();
            hashes = new Dictionary<uint, string>();

            JObject json = JObject.Parse(jsonstring);

            ReadArchetypes(json);
            ReadEntities(json);
        }

        public void ReadEntities(JObject json)
        {
            Dictionary<uint, int> unresolved = new Dictionary<uint, int>();

            if (json["entities"] != null && json["entities"].Any())
            {
                foreach (JToken e in json["entities"])
                {
                    FivemEntity entity = new FivemEntity();

                    var pos = e["position"];
                    if (pos != null)
                    {
                        entity.position = new Vector3(
                          float.Parse(pos[0].ToString()),
                          float.Parse(pos[1].ToString()),
                          float.Parse(pos[2].ToString()));
                    }

                    var rot = e["rotation"];
                    if (rot != null)
                    {
                        entity.rotation = new Quaternion(
                          float.Parse(rot[0].ToString()),
                          float.Parse(rot[1].ToString()),
                          float.Parse(rot[2].ToString()),
                          float.Parse(rot[3].ToString()));
                    }

                    var name = e["archetypeName"];
                    if (name != null)
                    {
                        string namestring = name.ToString();
                        
                        if (namestring.StartsWith("hash:"))
                        {
                            uint hash = uint.Parse(namestring.Remove(0,5));

                            string value;

                            bool success = hashes.TryGetValue(hash, out value);

                            if (success)
                                namestring = value;
                            else
                            {
                                namestring = "0x" + hash.ToString("X");

                                if (unresolved.ContainsKey(hash))
                                    unresolved[hash] = unresolved[hash] + 1;
                                else
                                    unresolved[hash] = 1;
                            }
                        }
                        entity.archetypeName = namestring;
                    }
                    entities.Add(entity);
                }

                foreach(uint h in unresolved.Keys)
                {
                    string hex = "0x" + h.ToString("X");
                    Console.WriteLine("Can't resolve archetypeName for entity {0} : {1} ({2} times)", h, hex, unresolved[h]);
                }
            }
        }

        public void ReadArchetypes(JObject json)
        {
            if (json["archetypes"] != null && json["archetypes"].Any())
            {
                foreach (JToken a in json["archetypes"])
                {
                    FivemArchetype archetype = new FivemArchetype();

                    var bbmin = a["aabbMin"];
                    if(bbmin != null)
                    {
                        archetype.aabbMin = new Vector3(
                          float.Parse(bbmin[0].ToString()),
                          float.Parse(bbmin[1].ToString()),
                          float.Parse(bbmin[2].ToString()));
                    }
                    
                    var bbmax = a["aabbMax"];
                    if (bbmax != null)
                    {
                        archetype.aabbMax = new Vector3(
                        float.Parse(bbmax[0].ToString()),
                        float.Parse(bbmax[1].ToString()),
                        float.Parse(bbmax[2].ToString()));
                    }

                    var rd = a["radius"];
                    if (rd != null)
                        archetype.radius = float.Parse(rd.ToString());

                    var center = a["centroid"];
                    if (center != null)
                    {
                        archetype.centroid = new Vector3(
                        float.Parse(center[0].ToString()),
                        float.Parse(center[1].ToString()),
                        float.Parse(center[2].ToString()));
                    }

                    var name = a["archetypeName"];
                    if(name != null)
                    {
                        string namestring = name.ToString();
                        archetype.archetypeName = namestring;

                        if (!namestring.StartsWith("hash:", StringComparison.Ordinal))
                            hashes[Jenkin.GenHash(namestring)] = namestring;
                    }

                    var txd = a["txdName"];
                    if (txd != null)
                    {
                        string txdstring = txd.ToString();
                        archetype.txdName = txdstring;

                        if (!txdstring.StartsWith("hash:", StringComparison.Ordinal))
                            hashes[Jenkin.GenHash(txdstring)] =  txdstring;
                    }

                    archetypes.Add(archetype);
                }
            }
        }
    }

    public class FivemArchetype
    {
        public Vector3 aabbMin { get; set; }
        public Vector3 aabbMax { get; set; }
        public float radius { get; set; }
        public float drawDistance { get; set; } //not required
        public Vector3 centroid { get; set; }
        public string archetypeName { get; set; }
        public string txdName { get; set; }
        public string lodDictName { get; set; } //not required
    }

    public class FivemEntity
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Guid guid { get; set; }
        public string archetypeName { get; set; }
        public float float1 { get; set; } //not required //should be lodDist
        public float float2 { get; set; } //not required
    }
}
