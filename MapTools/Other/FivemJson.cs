﻿using Newtonsoft.Json.Linq;
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

        public HashSet<uint> unresolved = new HashSet<uint>();

        public FivemJson(string jsonstring,string name)
        {
            filename = name;
            archetypes = new List<FivemArchetype>();
            entities = new List<FivemEntity>();
            hashes = new Dictionary<uint, string>();

            JObject json = JObject.Parse(jsonstring);

            ReadArchetypes(json);
            ReadEntities(json);
            ResolveHashes();
        }

        public string TryResolveHash(string s)
        {
            //if (s.StartsWith("hash:"))
            {
                uint hash = uint.Parse(s.Remove(0, 5));

                string value;
                bool success = hashes.TryGetValue(hash, out value);

                if (success)
                    s = value;
                else
                {
                    s = "0x" + hash.ToString("X");

                    if (!unresolved.Contains(hash))
                        unresolved.Add(hash);
                }
            }
            return s;
        }

        public void ResolveHashes()
        {

            for (int i = 0; i < archetypes.Count; i++)
            {
                string archetypeName = archetypes[i].archetypeName;
                if (archetypeName.StartsWith("hash:"))
                    archetypes[i].archetypeName = TryResolveHash(archetypeName);

                string txdName = archetypes[i].txdName;
                if (txdName.StartsWith("hash:"))
                    archetypes[i].txdName = TryResolveHash(txdName);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                string archetypeName = entities[i].archetypeName;
                if (archetypeName.StartsWith("hash:"))
                    entities[i].archetypeName = TryResolveHash(archetypeName);
            }

            foreach (uint h in unresolved)
            {
                string hex = "0x" + h.ToString("X");
                Console.WriteLine("Can't resolve hash {0} : {1}", h, hex);
            }
        }

        public void ReadEntities(JObject json)
        {
            
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

                        if (!namestring.StartsWith("hash:", StringComparison.Ordinal))
                            hashes[Jenkin.GenHash(namestring)] = namestring;

                        entity.archetypeName = namestring;
                    }
                    entities.Add(entity);
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

                        if (!namestring.StartsWith("hash:", StringComparison.Ordinal))
                            hashes[Jenkin.GenHash(namestring)] = namestring;

                        archetype.archetypeName = namestring;

                    }

                    var txd = a["txdName"];
                    if (txd != null)
                    {
                        string txdstring = txd.ToString();
                       
                        if (!txdstring.StartsWith("hash:", StringComparison.Ordinal))
                            hashes[Jenkin.GenHash(txdstring)] =  txdstring;

                        archetype.txdName = txdstring;
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
