using System;
using RedDev.Kernel.DB;
using SimpleXML;

namespace Game.DB {
    [MetaModel("DB/Vehicles/Basis")]
    public class VehiclesDBMeta : BaseMetaDB, IMetaXML {
        public bool enabled;
        public bool defaultLocked;

        public string title;
        public string iconName;
        public string prefabName;

        public int cost;
        public int sort;

        public TuningCollection engines = new TuningCollection();
        public TuningCollection transmissions = new TuningCollection();
        public TuningCollection steering = new TuningCollection();
        public TuningCollection brakes = new TuningCollection();
        public TuningCollection axles = new TuningCollection();
        public TuningCollection wheels = new TuningCollection();

        public void Parse(string source) {
            var doc = new XMLDoc(source, false);
            _id = doc.GetAttribDef("id", -1);
            _identifier = doc.GetAttribDef("identifier", "none");
            title = doc.GetAttribute("title");
            enabled = doc.GetAttribDef("enabled", false);
            defaultLocked = doc.GetAttribDef("defaultLocked", true);
            cost = doc.GetAttribDef("cost", 0);
            sort = doc.GetAttribDef("sort", 0);

            iconName = doc["icon"]?.nodeValue;
            prefabName = doc["prefab"]?.nodeValue;

            var enginesNode = doc["engines"];
            if (enginesNode != null)
                engines = new TuningCollection(enginesNode);

            var trNode = doc["transmissions"];
            if (trNode != null)
                transmissions = new TuningCollection(trNode);

            var steeringNode = doc["steering"];
            if (steeringNode != null)
                steering = new TuningCollection(steeringNode);

            var brakesNode = doc["brakes"];
            if (brakesNode != null)
                brakes = new TuningCollection(brakesNode);

            var axlesNode = doc["axles"];
            if (axlesNode != null)
                axles = new TuningCollection(axlesNode);

            var wheelsNode = doc["wheels"];
            if (wheelsNode != null)
                wheels = new TuningCollection(wheelsNode);
        }
    }

    [Serializable]
    public class TuningCollection {
        public string defaultValue;
        public string[] values;

        public string this[int index] => values[index];
        public int count => values.Length;

        public TuningCollection() { }

        public TuningCollection(XMLDoc node) {
            this.values = node.childs.ConvertAll(x => x.nodeValue).ToArray();
            this.defaultValue = node.GetAttribDef("default", values.Length > 0 ? values[0] : "");
        }
    }
}