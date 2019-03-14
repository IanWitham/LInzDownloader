using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinzDownloader.AddressReferences
{
    [DeserializeAs(Name = "FeatureCollection")]
    public class FeatureCollection
    {
        public long hits { get; set; }
        public List<Feature> features { get; set; }
    }

    public class Feature
    {
        public string id { get; set; }

        public Properties properties { get; set; }
    }

    public class Properties
    {
        public string address_reference_id { get; set; }
        public string address_id { get; set; }
        public string address_reference_object_type { get; set; }
        public string address_reference_object_value { get; set; }

        // only used for changesets to indicate CREATE, UPDATE or DELETE
        public string __change__ { get; set; }
    }
}